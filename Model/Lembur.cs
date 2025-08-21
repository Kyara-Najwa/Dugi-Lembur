using System;

namespace Mobile.Model{

    //untuk mendefinisikan status lembur
    public enum LemburStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    //model untuk lembur
    public class Lembur
    {
        public int Id { get; set; } //pk
        public int EmployeeId { get; set; } //fk ke employee.id
        public DateTime TanggalLembur { get; set; } //tanggal lembur
        public decimal Durasi { get; set; } //durasi lembur
        public string Alasan { get; set; } //alasan atau keterangan lembur
        
        public LemburStatus Status { get; set; } //status pengajuan lembur
        public int? ApprovedBy { get; set; } //id yang menyetujui atau menolak
        public DateTime? ApprovedAt { get; set; } //waktu saat disetujui atau ditolak
        public string? RejectReason { get; set; } //alasan ditolak
        public DateTime DateCreated { get; set; } //tanggal dibuat
    }
}
