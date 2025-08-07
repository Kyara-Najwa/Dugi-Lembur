using Dapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mobile.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using MailKit.Security;

namespace Mobile.Services
{
    public class GeneralService
    {

        public IDbConnection Connection
        {
            get
            {
                var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables();
                var config = builder.Build();
                var connectionstring = config["connectionstring"];
                return new NpgsqlConnection(connectionstring);
            }
        }
        public string SecondDB
        {
            get
            {
                var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables();
                var config = builder.Build();
                var api = config["SecondDB"];
                return api;
            }
        }
        public string smtphost
        {
            get
            {
                var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables();
                var config = builder.Build();
                var api = config["smtphost"];
                return api;
            }
        }
        public string Emailfrom
        {
            get
            {
                var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables();
                var config = builder.Build();
                var api = config["Emailfrom"];
                return api;
            }
        }
        public string Username
        {
            get
            {
                var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables();
                var config = builder.Build();
                var api = config["EmailUsername"];
                return api;
            }
        }
        public string Password
        {
            get
            {
                var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables();
                var config = builder.Build();
                var api = config["Password"];
                return api;
            }
        }
        public string port
        {
            get
            {
                var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables();
                var config = builder.Build();
                var api = config["port"];
                return api;
            }
        }
        public string linkresetpassword
        {
            get
            {
                var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .AddEnvironmentVariables();
                var config = builder.Build();
                var api = config["linkresetpassword"];
                return api;
            }
        }
        public async Task sendmail(string tujuan, string messagebody, string username, string Subject)
        {
            string smtphost = this.smtphost;
            string Emailfrom = this.Emailfrom;
            string Username = this.Username;
            string Password = this.Password;
            int port = int.Parse(this.port);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Administrator WorkPress", Emailfrom));
            message.To.Add(new MailboxAddress(username, tujuan));
            message.Subject = Subject;
            message.Body = new TextPart("plain")
            {
                Text = messagebody
            };
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                try
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    //client.Connect(smtphost, port, true);
                    client.Connect(smtphost, port, SecureSocketOptions.StartTls);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(Username, Password);
                    client.Send(message);
                    client.Disconnect(true);
                }
                catch
                { throw; }

            }

        }
        private static Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789@#_-.,#!*ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#_-.,#!*";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public salt salt(string password)
        {
            try
            {
                salt salted = new salt();
                byte[] salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

                // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
                string hashed = hash(salt, password);
                Console.WriteLine($"Hashed: {hashed}");
                salted.garem = Convert.ToBase64String(salt);
                salted.hashed = hashed;
                return salted;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public string hash(byte[] salt, string password)
        {
            try
            {
                // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8));
                Console.WriteLine($"Hashed: {hashed}");

                return hashed;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public Employee GetEmployeeByToken(string user)
        {
            using (IDbConnection conn = Connection)
                try
                {
                    conn.Open();    
                    string query = @"select e.*, r.name as employeeRole , c.name as companyName, o.name as OfficeName, d.name as DivisionName
                from employee e left join employeeRole r on e.employeeroleid=r.id left join Company c on e.companyid=c.id
                left join Office o on e.Officeid=o.id left join Division d on e.Divisionid=d.id where e.email=@email and e.active=true and e.IsDeleted=false";
                    var datas = conn.Query<Employee>(query, new { email = user }).FirstOrDefault();                    
                    if (datas.Id > 0)
                    {
                        return datas;
                    }
                    conn.Close();
                    return null;
                }
                catch (Exception ex)
                {
                    return null;
                }
        }
        

    }
}
