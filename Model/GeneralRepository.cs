using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mobile.Services;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Mobile.Infrastructure;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Google.Apis.Drive.v3.Data;


namespace Mobile.Model
{
    public class GeneralRepository
    {        
        GeneralService general = new GeneralService();
        public async Task<GeneralResponse> Register(Register register)
        {
            using (IDbConnection conn = general.Connection)
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string query = @"select * from Company where Email=@CompanyEmail and name=@CompanyName and IsDeleted=false";
                        var data = conn.Query<Company>(query, register, transaction).FirstOrDefault();
                        if (data != null)
                        {
                             return new GeneralResponse { status = "Error", message = "Such Company already exists" };
                        }
                        query = @"select * from Employee where lower(email)=lower(@email)";
                        var data3 = conn.Query<ReqEmployee>(query, new { email = register.CompanyEmail }, transaction).FirstOrDefault();
                        if (data3 != null)
                        {
                            return new GeneralResponse { status = "Error", message = "Perusahaan dengan email tersebut sudah ada" };
                        }
                        query = @"insert into Company (name,Email,active,DateCreated,DateModified,city,address) values 
                            (@CompanyName,@CompanyEmail,true,current_timestamp,current_timestamp,@city,@address) RETURNING id";
                        register.CompanyId = conn.Query<int>(query, register, transaction).FirstOrDefault();

                        string password = general.RandomString(15);
                        register.Password = password;

                        salt pwd = general.salt(register.Password);
                        var emp = new ReqEmployee
                        {
                            CompanyId = register.CompanyId,
                            employeeroleid = 1,
                            Username = register.UserName,
                            Fullname = register.FullName,
                            Gender = "",
                            Officeid = 0,
                            Divisionid = 0,
                            Position = "Admin",
                            Phonenumber = register.PhoneNumber,
                            Address = "",
                            NIK = "",
                            Active = true,
                            Email = register.CompanyEmail,
                            salt = pwd.garem,
                            password = pwd.hashed,
                            Photo =""
                        };
                        query = @"insert into Employee (CompanyId,employeeroleid,Username,Fullname,Gender,Officeid,Divisionid,Position,Phonenumber ,Address,NIK,Active,Email,salt,password,Photo,DateCreated,DateModified) 
                        values (@CompanyId,@employeeroleid,@Username,@Fullname,@Gender,@Officeid,@Divisionid,@Position,@Phonenumber ,@Address,@NIK,@Active,@Email,@salt,@password,@Photo,current_timestamp,current_timestamp)";
                        conn.Execute(query, emp, transaction);

                        try
                        {
                            if (emp.Email != null)
                            {
                                var Message = string.Format("Dear {0},\n\nAnda telah mendaftar diaplikasi WorkPress.\nBerikut username Anda : {1}\n Pasword: {2}\n\n Anda dapat mengganti password Anda pada menu setting.\n\nEmail ini tergenerate automatis oleh system. Mohon tidak membalas ke alamat email ini.\n\nRegards,\nWorkPress",
                               emp.Fullname,
                               emp.Email,
                               password);
                                await general.sendmail(emp.Email, Message, emp.Email, "Welcome to WorkPress");
                            }
                        }
                        catch (Exception ex)
                        {
                             return new GeneralResponse { status = "Error", message = "Error send mail : " + ex.Message };
                        }
                        transaction.Commit();
                        return new GeneralResponse { status = "Success Register", message = "" };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return new GeneralResponse { status = "Error", message = ex.Message };

                    }
                }
            }
            
        }
        public async Task<ResultLogin> login(loginMobile login)
        {
            var result = new ResultLogin();
            string role = "";
            using (IDbConnection conn = general.Connection)
                try
                {
                    conn.Open();
                    string query = @"select e.*, r.name as employeeRole , c.name as companyName, o.name as OfficeName, d.name as DivisionName
                    from employee e left join employeeRole r on e.employeeroleid=r.id left join Company c on e.companyid=c.id
                    left join Office o on e.Officeid=o.id left join Division d on e.Divisionid=d.id where e.email=@email and e.IsDeleted=false";
                    var user = conn.Query<ReqEmployee>(query, login).FirstOrDefault();
                    
                    if (user == null) throw new Exception("Email anda belum terdaftar, silahkan melakukan registrasi"); // invalid login
                    
                    role = user.employeerole;
                    string pwd = general.hash(Convert.FromBase64String(user.salt), login.password);
                    
                    if (pwd != user.password) throw new Exception("Invalid Email / Password"); // invalid password
                    if (user != null && !user.Active) throw new Exception("User is blocked / inactive"); // user is blocked
                    
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name,login.email),
                        new Claim(ClaimTypes.Role, role)
                    };

                    var jwtResult = GenerateTokens(login.email, claims, DateTime.Now);
                    string refreshToken = jwtResult.RefreshToken.TokenString;

                    await conn.ExecuteAsync(@"update employee set ""refreshToken"" = @refreshtoken WHERE email = @email;", new { refreshToken, email = login.email });

                    result.userid = user.Id;
                    result.UserName = user.Username;
                    result.Role = role;
                    result.AccessToken = jwtResult.AccessToken;
                    result.RefreshToken = refreshToken;
                    
                    result.message = "Success";
                    result.Success = true;

                    return result;
                }
                catch (Exception ex)
                {
                   
                    result.message = ex.Message;
                    result.Success = false;
                    Console.WriteLine(ex.Message);
                    return result;
                }
        }
        public async Task<ResultLogin> Refreshtoken(refreshlogin data, string usr)
        {
            using (IDbConnection conn = general.Connection)
                try
                {
                    conn.Open();
                    var result = new ResultLogin();
                    var emp = general.GetEmployeeByToken(usr);
                    
                    var username = emp.Email;
                    
                    if (emp == null) throw new Exception("Email not found"); // invalid login
                    if (emp.refreshToken.ToString() != data.refreshtoken) throw new Exception("Invalid refresh token");

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name,username),
                        new Claim(ClaimTypes.Role, emp.employeerole)
                    };

                    var jwtResult = GenerateTokens(emp.Email, claims, DateTime.Now);
                    result.userid = emp.Id;
                    result.UserName = emp.Username;
                    result.Role = emp.employeerole;
                    result.AccessToken = jwtResult.AccessToken;
                    result.RefreshToken = jwtResult.RefreshToken.TokenString;

                    result.message = "Success";
                    result.Success = true;



                    var query = @"update employee set ""refreshToken"" = @refreshtoken WHERE email = @email;";
                    await conn.ExecuteAsync(query, new { refreshtoken =jwtResult.RefreshToken.TokenString,email=emp.Email});


                    return result;

                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                };
        }

        public JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now)
        {
            byte[] _secret = Encoding.ASCII.GetBytes("1234567890123456789");
            var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
            var jwtToken = new JwtSecurityToken(
                "Attendance User",
                shouldAddAudienceClaim ? "Attendance" : string.Empty,
                claims,
                expires: now.AddDays(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var refreshToken = new RefreshToken
            {
                UserName = username,
                TokenString = GenerateRefreshTokenString(),
                ExpireAt = now.AddDays(5)
            };
            //_usersRefreshTokens.AddOrUpdate(refreshToken.TokenString, refreshToken, (_, _) => refreshToken);

            return new JwtAuthResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        private static string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
      
       
    }
}
