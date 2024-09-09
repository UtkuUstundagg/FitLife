using FitLife.Models;
using Microsoft.AspNetCore.Mvc;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FitLife.Controllers
{
    public class RegisterController : Controller
    {

        IFirebaseConfig config = new FirebaseConfig()
        {
            AuthSecret = "m8qwx7IRwzpKKbWIRqL3blTIuhWNPjJYyPjRiBmJ",
            BasePath = "https://fitlife-e847e-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SetDanisan(string isim, string soyisim, string dogumtarihi, string cinsiyet, string email, string sifre, int telefon)
        {

            if (BaglantiAc())
            {
                //AdminModel admin = new AdminModel();
                //admin.email = email;
                //admin.sifre = sifre;
                //client.Push("Admin/", admin);

                //AntrenorModel antrenor = new AntrenorModel();
                //antrenor.isim = isim;
                //antrenor.soyisim = soyisim;
                //antrenor.dogumtarihi = dogumtarihi;
                //antrenor.cinsiyet = cinsiyet;
                //antrenor.email = email;
                //antrenor.sifre = sifre;
                //antrenor.telefon = telefon;
                //antrenor.kasKazanma = false;
                //antrenor.kiloAldirma = false;
                //antrenor.kiloKoruma = false;
                //antrenor.kiloVerdirme = false;
                //antrenor.hesapAktifMi = true;
                //antrenor.kalanDanisanSayisi = 5;
                //client.Push("Antrenor/", antrenor);

                DanisanModel danisan = new DanisanModel();
                danisan.isim = isim;
                danisan.soyisim = soyisim;
                danisan.dogumtarihi = dogumtarihi;
                danisan.cinsiyet = cinsiyet;
                danisan.email = email;
                danisan.sifre = sifre;
                danisan.telefon = telefon;
                danisan.dogrulamakodu = 0;
                danisan.hesapAktifMi = true;
                danisan.kasKazanma = false;
                danisan.kiloAlma = false;
                danisan.kiloKoruma = false;
                danisan.kiloVerme = false;
                danisan.antrenorKey = "bos";

                client.Push("Danisan/", danisan);

                //DanisanOlustur();


            }
            return RedirectToAction("Index", "Home");
        }

        public bool BaglantiAc()
        {
            try
            {
                client = new FirebaseClient(config);
                return true;
            }
            catch (Exception)
            {
                //googledaki gibi hata mesajı düşecek
                return false;
            }
        }

        public void KeyOku()
        {
            if (BaglantiAc()) 
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());
                var list = new List<string>();
                if(data != null)
                {
                    foreach (var key in data)
                    {
                        
                    }
                }
            }


        }

        public void DanisanOlustur()
        {

            for(int i = 0; i < 20; i++)
            {
                DanisanModel danisan = new DanisanModel();
                danisan.isim = "isim";
                danisan.soyisim = "soyisim";
                danisan.dogumtarihi = "dogumtarihi";
                danisan.cinsiyet = "cinsiyet";
                danisan.email = danisan.isim + "@gmail.com" ;
                danisan.sifre = danisan.isim + 123;
                danisan.telefon = 666333444;
                danisan.dogrulamakodu = 0;
                danisan.hesapAktifMi = true;
                danisan.kasKazanma = false;
                danisan.kiloAlma = false;
                danisan.kiloKoruma = false;
                danisan.kiloVerme = false;
                danisan.antrenorKey = "bos";

                client.Push("Danisan/", danisan);
            }
            
        }

    }
}
