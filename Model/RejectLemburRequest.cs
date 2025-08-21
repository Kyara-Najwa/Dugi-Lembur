using System.ComponentModel.DataAnnotations;

namespace Mobile.Model
{
    //model request unutk menolak lembur
    public class RejectLemburRequest
    {
        [Required]
        [StringLength(500, ErrorMessage = "Reject reason cannot be longer than 500 characters")]
        public string RejectReason { get; set; } //alasan penolakan lembur
    }
}