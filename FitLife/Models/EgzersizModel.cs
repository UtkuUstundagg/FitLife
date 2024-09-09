namespace FitLife.Models
{
    public class EgzersizModel
    {
        public string danisanKey { get; set; }
        public bool kasKazanma { get; set; }
        public bool kiloAlma { get; set; }
        public bool kiloKoruma { get; set; }
        public bool kiloVerme { get; set; }
        public int setSayisi { get; set; }
        public int tekrarSayisi { get; set; }
        public DateTime baslangicTarihi { get; set; }    
        public int programSuresi { get; set; }

        public string egzersizKey { get; set; }

    }
}
