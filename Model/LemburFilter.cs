using System;
using System.ComponentModel.DataAnnotations;

namespace Mobile.Model
{
    //ini adalah request model untuk filter data lembur
    public class LemburFilterRequest
    {
        public string Keyword { get; set; } = "";
        
        public DateTime? Start { get; set; }
        
        public DateTime? End { get; set; }
        
        public int CompanyId { get; set; } = 0;
        
        public int EmployeeId { get; set; } = 0;
        
        public int Status { get; set; } = 0; 
        
        [Range(1, 200, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;
        
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;
    }

    //ini adalah response model untuk data lembur yang uda difilter
    public class LemburFilterResponse
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int OfficeId { get; set; }
        public int EmployeeId { get; set; }
        public string NIK { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Division { get; set; }
        public DateTime TanggalLembur { get; set; }
        public decimal Durasi { get; set; }
        public string Alasan { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public int TotalCount { get; set; }
        public string UserApproved { get; set; }
        public DateTime? DateApproved { get; set; }
        public string UserReject { get; set; }
        public DateTime? DateReject { get; set; }
        public string RejectNote { get; set; }
    }
}
