using System;
using System.ComponentModel.DataAnnotations;

namespace Mobile.Model
{
    //model request untuk membuat lembur
    //untuk validasi input dari client
    public class CreateLemburRequest
    {
        [Required]
        public DateTime TanggalLembur { get; set; } 
        
        [Required]
        [Range(0.01, 24.0, ErrorMessage = "Duration must be between 0.01 and 24 hours")]
        public decimal Durasi { get; set; } 
        
        [Required]
        [StringLength(500, ErrorMessage = "Reason cannot be longer than 500 characters")]
        public string Alasan { get; set; } 
    }
}