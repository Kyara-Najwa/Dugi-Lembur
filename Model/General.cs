using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Mobile.Model
{
    public class GeneralResponse
    {
        public string status { get; set; }
        public string message { get; set; }
    }
    public class salt
    {
        public string garem { get; set; }
        public string hashed { get; set; }
    }
    public class ChangePass
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    public class Register
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string PhoneNumber { get; set; }    
        public string Password { get; set; }
        public string city { get; set; }
        public string address { get; set; }
    }
    public class ResultLogin
    {
        public string message { get; set; }
        public bool Success { get; set; }
        public int userid { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class loginMobile
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class refreshlogin
    {
        public string refreshtoken { get; set; }
    }
}
