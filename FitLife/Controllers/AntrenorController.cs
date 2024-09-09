using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using FitLife.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FitLife.Controllers
{
    public class AntrenorController : Controller
    {
        IFirebaseConfig fc = new FirebaseConfig()
        {
            AuthSecret = "m8qwx7IRwzpKKbWIRqL3blTIuhWNPjJYyPjRiBmJ",
            BasePath = "https://fitlife-e847e-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;
        public IActionResult Antrenor(string antrenorKey)
        {
            HttpContext.Session.Clear();
            HttpContext.Session.SetString("Deneme", antrenorKey);

            return View(DanisanlariBul(antrenorKey));
        }

        public IActionResult Profile()
        {
            var antrenorKey = HttpContext.Session.GetString("Deneme");
            return View(AntrenorBul(antrenorKey));
        }

        public IActionResult BeslenmePlani(string key)
        {
            return View(DanisanBul(key));
        }

        public IActionResult EgzersizPlani(string key)
        {
            return View(DanisanBul(key));
        }

        public IActionResult AntrenorSayfasinaDon()
        {
            var antrenorKey = HttpContext.Session.GetString("Deneme");

            return RedirectToAction("Antrenor", "Antrenor", new { antrenorKey });
        }



        public IActionResult AntrenorProfilGuncelle(AntrenorModel antrenor)
        {
            AntrenorGuncelle(antrenor);
            return RedirectToAction("Profile", "Antrenor", antrenor);
        }

        public IActionResult BeslenmePlaniOlustur(DanisanModel danisan, int kaloriHedefi, string ogun1, string ogun2, string ogun3)
        {
            BeslenmeModel beslenme = new BeslenmeModel();
            beslenme.danisanKey = danisan.danisanKey;
            beslenme.kaloriHedefi = kaloriHedefi;
            beslenme.ogun1 = ogun1;
            beslenme.ogun2 = ogun2;
            beslenme.ogun3 = ogun3;

            BeslenmePlanıEkle(beslenme);
            return RedirectToAction("AntrenorSayfasinaDon", "Antrenor");
        }

        public IActionResult BeslenmePlaniGuncelle(string key)
        {
            return View(BeslenmeBul(key));
        }

        public IActionResult EgzersizPlaniGuncelle(string key)
        {
            return View(EgzersizBul(key));
        }
        public IActionResult BeslenmePlaniGuncelleAction(BeslenmeModel beslenme)
        {
            if (BaglantiAc())
            {
                client.Update("BeslenmePlani/" + beslenme.beslenmeKey, beslenme);
            }
            
            return RedirectToAction("AntrenorSayfasinaDon", "Antrenor");
        }

        public IActionResult EgzersizPlaniOlustur(DanisanModel danisan, int setSayisi, int tekrarSayisi, DateTime baslangicTarihi, int programSuresi, bool kasKazanma, bool kiloAlma, bool kiloVerme, bool kiloKoruma)
        {
            EgzersizModel egzersiz = new EgzersizModel();
            egzersiz.danisanKey = danisan.danisanKey;
            egzersiz.setSayisi = setSayisi;
            egzersiz.tekrarSayisi = tekrarSayisi;
            egzersiz.baslangicTarihi = baslangicTarihi;
            egzersiz.programSuresi = programSuresi;
            egzersiz.kasKazanma = kasKazanma;
            egzersiz.kiloAlma = kiloAlma;
            egzersiz.kiloVerme = kiloVerme;
            egzersiz.kiloKoruma = kiloKoruma;


            EgzersizPlanıEkle(egzersiz);
            return RedirectToAction("AntrenorSayfasinaDon", "Antrenor");
        }

        public IActionResult EgzersizPlaniGuncelleAction(EgzersizModel egzersiz)
        {
            if (BaglantiAc())
            {
                client.Update("EgzersizPlani/" + egzersiz.egzersizKey, egzersiz);
            }
            
            return RedirectToAction("AntrenorSayfasinaDon", "Antrenor");
        }

        public IActionResult MesajGonder(MesajModel mesaj)
        {
            return View();
        }

        public IActionResult MesajGonderAction(string key)
        {
            var antrenorKey = HttpContext.Session.GetString("Deneme");

            MesajModel mesaj = new MesajModel();
            mesaj.gondericiKey = antrenorKey;
            mesaj.aliciKey = key;

            return RedirectToAction("MesajGonder", "Antrenor", mesaj );
        }

        public IActionResult DanisanaMesajAt(MesajModel mesaj)
        {
            MesajAt(mesaj);
            var antrenorKey = HttpContext.Session.GetString("Deneme");
            return RedirectToAction("Antrenor", "Antrenor", new { antrenorKey });
        }

        public IActionResult GelenKutusu()
        {
            var antrenorKey = HttpContext.Session.GetString("Deneme");
            return View(MesajBul(antrenorKey));
        }

        public void AntrenorProfilDoldur(string antrenorKey)
        {
            AntrenorModel antrenor = new AntrenorModel();

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Antrenor");
                Dictionary<string, AntrenorModel> data = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Key == antrenorKey)
                    {
                        antrenor.isim = item.Value.isim;
                        antrenor.soyisim = item.Value.soyisim;
                        antrenor.dogumtarihi = item.Value.dogumtarihi;
                        antrenor.cinsiyet = item.Value.cinsiyet;
                        antrenor.email = item.Value.email;
                        antrenor.sifre = item.Value.sifre;
                        antrenor.telefon = item.Value.telefon;
                        antrenor.kasKazanma = item.Value.kasKazanma;
                        antrenor.kiloAldirma = item.Value.kiloAldirma;
                        antrenor.kiloKoruma = item.Value.kiloKoruma;
                        antrenor.kiloVerdirme = item.Value.kiloVerdirme;
                        antrenor.kalanDanisanSayisi = item.Value.kalanDanisanSayisi;
                        antrenor.antrenorKey = item.Key;
                    }
                }
            }
            RedirectToAction("Profile", "Antrenor", antrenor);
        }
        public void AntrenorGuncelle(AntrenorModel antrenor)
        {
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Antrenor");
                Dictionary<string, AntrenorModel> data = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Key == antrenor.antrenorKey)
                    {
                        client.Update("Antrenor/" + item.Key, antrenor);
                    }
                }
            }
        }

        public bool BaglantiAc()
        {
            try
            {
                client = new FirebaseClient(fc);
                return true;
            }
            catch (Exception)
            {
                //googledaki gibi hata mesajı düşecek
                return false;
            }
        }

        public List<DanisanModel> DanisanlariBul(string antrenorKey)
        {
            List<DanisanModel> danisanList = new List<DanisanModel>();
            DanisanModel danisan = new DanisanModel();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.antrenorKey == antrenorKey)
                    {
                        danisan = item.Value;
                        danisanList.Add(danisan);
                    }
                }
            }
            return danisanList;
        }

        public AntrenorModel AntrenorBul(string antrenorKey)
        {
            AntrenorModel antrenor = new AntrenorModel();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Antrenor");
                Dictionary<string, AntrenorModel> data = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Key == antrenorKey)
                    {
                        antrenor = item.Value;
                    }
                }

            }
            return antrenor;
        }

        public DanisanModel DanisanBul(string danisanKey)
        {
            DanisanModel danisan = new DanisanModel();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Key == danisanKey)
                    {
                        danisan = item.Value;
                    }
                }

            }
            return danisan;
        }

        public BeslenmeModel BeslenmeBul(string danisanKey)
        {
            BeslenmeModel beslenme = new BeslenmeModel();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"BeslenmePlani");
                Dictionary<string, BeslenmeModel> data = JsonConvert.DeserializeObject<Dictionary<string, BeslenmeModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.danisanKey == danisanKey)
                    {
                        beslenme = item.Value;
                        beslenme.beslenmeKey = item.Key;
                    }
                }

            }
            return beslenme;
        }

        public EgzersizModel EgzersizBul(string danisanKey)
        {
            EgzersizModel egzersiz = new EgzersizModel();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"EgzersizPlani");
                Dictionary<string, EgzersizModel> data = JsonConvert.DeserializeObject<Dictionary<string, EgzersizModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.danisanKey == danisanKey)
                    {
                        egzersiz = item.Value;
                        egzersiz.egzersizKey = item.Key;
                    }
                }

            }
            return egzersiz;
        }

        public void BeslenmePlanıEkle(BeslenmeModel beslenme)
        {
            if (BaglantiAc())
            {
                client.Push("BeslenmePlani/", beslenme);
            }

        }

        public void EgzersizPlanıEkle(EgzersizModel egzersiz)
        {
            if (BaglantiAc())
            {
                client.Push("EgzersizPlani/", egzersiz);
            }

        }

        public List<MesajModel> MesajBul(string antrenorKey)
        {
            MesajModel mesaj = new MesajModel();
            List<MesajModel> mesajList = new List<MesajModel>();

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Mesajlar");
                Dictionary<string, MesajModel> data = JsonConvert.DeserializeObject<Dictionary<string, MesajModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.aliciKey == antrenorKey)
                    {
                        mesaj = item.Value;
                        mesajList.Add(mesaj);
                    }
                }

            }
            return mesajList;
        }

        public void MesajAt(MesajModel mesaj)
        {
            if (BaglantiAc())
            {
                client.Push(@"Mesajlar/", mesaj);
            }
        }


    }
}
