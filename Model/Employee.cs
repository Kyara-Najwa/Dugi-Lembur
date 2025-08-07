using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mobile.Model
{
    public class ReqEmployee
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int employeeroleid { get; set; }
        public string employeerole { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Gender { get; set; }
        public int Officeid { get; set; }
        public int Divisionid { get; set; }
        public string Position { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }
        public string NIK { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public string salt { get; set; }
        public string password { get; set; }
        public string Photo { get; set; }
    }
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int employeeroleid { get; set; }
        public string employeerole { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Gender { get; set; }
        public int Officeid { get; set; }
        public string OfficeName { get; set; }
        public string OfficeContactPerson { get; set; }
        public string OfficeCity { get; set; }
        public int Divisionid { get; set; }
        public string DivisionName { get; set; }
        public string Position { get; set; }
        public string Phonenumber { get; set; }
        public string Address { get; set; }
        public string NIK { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int TotalCount { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
        public string refreshToken { get; set; }

    }
    public class EmployeeRole
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class SearchEmployee
    {
        public int CompanyId { get; set; }
        public int DivisionId { get; set; }
        public string keyword { get; set; }
        public string active { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
