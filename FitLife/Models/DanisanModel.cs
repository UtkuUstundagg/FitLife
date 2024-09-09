using Microsoft.AspNetCore.Mvc;

namespace FitLife.Models
{
    public class DanisanModel
    {
        public string isim { get; set; }
        public string soyisim { get; set; }
        public int telefon { get; set; }
        public string email { get; set; }
        public string sifre { get; set; }
        public string cinsiyet { get; set; }
        public string dogumtarihi { get; set; }
        public int dogrulamakodu { get; set; }
        public string danisanKey { get; set; }
        public bool hesapAktifMi { get; set; }
        public bool kasKazanma { get; set; }
        public bool kiloKoruma { get; set; }
        public bool kiloVerme { get; set; }
        public bool kiloAlma { get; set; }
        public string antrenorKey { get; set; }

    }
}
