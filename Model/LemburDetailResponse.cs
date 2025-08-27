using System;

namespace Mobile.Model
{
    public class LemburDetailResponse : Lembur //class turunan dari lembur
    { //nambahin properti detail employee
        public int CompanyId { get; set; }
        public int OfficeId { get; set; }
        public string Nik { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string PhoneNumber { get; set; }
        public string Division { get; set; }
    }
}
