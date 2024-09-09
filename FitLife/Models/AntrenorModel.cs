namespace FitLife.Models
{
    public class AntrenorModel
    {
        public string antrenorKey { get; set; }
        public string isim { get; set; }
        public string soyisim { get; set; }
        public int telefon { get; set; }
        public string email { get; set; }
        public string sifre { get; set; }
        public string cinsiyet { get; set; }
        public DateTime dogumtarihi { get; set; }
        public string deneyimler { get; set; }
        public bool kiloAldirma { get; set; }
        public bool kiloVerdirme { get; set; }
        public bool kiloKoruma { get; set; }
        public bool kasKazanma { get; set; }
        public bool hesapAktifMi { get; set; }
        public int kalanDanisanSayisi { get; set; }

    }
}
