using System;

namespace Mobile.Model{

    //untuk mendefinisikan status lembur
    public enum LemburStatus
    {
        All = 0,
        Pending = 1,
        Approved = 2,
        Rejected = 3
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
        public int? ApprovedBy { get; set; } //id yang menyetujui
        public DateTime? ApprovedAt { get; set; } //waktu saat disetujui
        public int? RejectedBy { get; set; } //id yang menolak
        public DateTime? RejectedAt { get; set; } //waktu saat ditolak
        public string? RejectReason { get; set; } //alasan ditolak
        public DateTime DateCreated { get; set; } //tanggal dibuat
    }
}
