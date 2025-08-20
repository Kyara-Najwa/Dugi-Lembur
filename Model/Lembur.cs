using System;

namespace Mobile.Model{
    public class Lembur
    {
        public int Id { get; set; } //Primary key
        public int EmployeeId { get; set; } //Foreign key ke Employee.Id
        public DateTime TanggalLembur { get; set; } //Tanggal lembur
        public decimal Durasi { get; set; } //Durasi lembur
        public string Alasan { get; set; } //Alasan lembur
    }
}
