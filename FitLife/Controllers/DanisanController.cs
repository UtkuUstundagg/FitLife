using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using FitLife.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FitLife.Controllers
{
    public class DanisanController : Controller
    {
        IFirebaseConfig fc = new FirebaseConfig()
        {
            AuthSecret = "m8qwx7IRwzpKKbWIRqL3blTIuhWNPjJYyPjRiBmJ",
            BasePath = "https://fitlife-e847e-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;
        public IActionResult Danisan(string danisanKey)
        {
            HttpContext.Session.Clear();
            HttpContext.Session.SetString("Deneme", danisanKey);

            return View(DanisanBul(danisanKey));
        }

        public IActionResult DanisanSayfasinaDon()
        {
            var danisanKey = HttpContext.Session.GetString("Deneme");
            return RedirectToAction("Danisan", "Danisan", new { danisanKey });
        }

        public IActionResult Profile(DanisanModel danisan)
        {
            return View();
        }

        public IActionResult DanisanProfilDoldur(string danisanKey)
        {
            var temp = HttpContext.Session.GetString("Deneme");
            danisanKey = temp;

            DanisanModel danisan = new DanisanModel();

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Key == danisanKey)
                    {
                        danisan.isim = item.Value.isim;
                        danisan.soyisim = item.Value.soyisim;
                        danisan.dogumtarihi = item.Value.dogumtarihi;
                        danisan.cinsiyet = item.Value.cinsiyet;
                        danisan.email = item.Value.email;
                        danisan.sifre = item.Value.sifre;
                        danisan.telefon = item.Value.telefon;
                        danisan.kasKazanma = item.Value.kasKazanma;
                        danisan.kiloAlma = item.Value.kiloAlma;
                        danisan.kiloVerme = item.Value.kiloVerme;
                        danisan.kiloKoruma = item.Value.kiloKoruma;
                        danisan.hesapAktifMi = item.Value.hesapAktifMi;
                        danisan.danisanKey = item.Key;
                        danisan.antrenorKey = item.Value.antrenorKey;
                    }
                }
            }


            return RedirectToAction("Profile", "Danisan", danisan);
        }

        public IActionResult DanisanProfilGuncelle(DanisanModel danisan)
        {
            DanisanGuncelle(danisan);
            return RedirectToAction("Profile", "Danisan", danisan);
        }

        public IActionResult AntrenorEslestir()
        {
            var danisanKey = HttpContext.Session.GetString("Deneme");
            AntrenorEslestirFonksiyonu(danisanKey);
            return RedirectToAction("Danisan", "Danisan", new { danisanKey });
        }

        public IActionResult EgzersizPlani()
        {
            var danisanKey = HttpContext.Session.GetString("Deneme");
            return View(EgzersizBul(danisanKey));
        }

        public IActionResult BeslenmePlani()
        {
            var danisanKey = HttpContext.Session.GetString("Deneme");
            return View(BeslenmeBul(danisanKey));
        }

        public IActionResult IlerlemeKaydi()
        {
            var danisanKey = HttpContext.Session.GetString("Deneme");
            return View(IlerlemeKaydiBul(danisanKey));
        }

        public IActionResult IlerlemeKaydiOlustur(IlerlemeKaydi ilerlemeKaydi)
        {
            if (ilerlemeKaydi.kayitKey == null)
            {
                IlerlemeKaydiOlusturma(ilerlemeKaydi);
            }
            else
            {
                IlerlemeKaydiGuncelle(ilerlemeKaydi);
            }

            return RedirectToAction("Danisan", "Danisan", new { ilerlemeKaydi.danisanKey });
        }

        public IActionResult IlerlemeKaydiGoruntule()
        {
            return RedirectToAction("IlerlemeKaydi", "Danisan");
        }

        public IActionResult Raporlar()//düzenleme yapılacak
        {
            var danisanKey = HttpContext.Session.GetString("Deneme");
            return View(RaporBul(danisanKey));
        }

        public IActionResult Mesaj()
        {
            var danisanKey = HttpContext.Session.GetString("Deneme");
            return View(DanisanBul(danisanKey));
        }

        public IActionResult MesajGonder(DanisanModel danisan, string mesaj)
        {
            MesajModel mesajModel = new MesajModel();
            mesajModel.aliciKey = danisan.antrenorKey;
            mesajModel.gondericiKey = danisan.danisanKey;
            mesajModel.icerik = mesaj;

            AntrenoreMesajAt(mesajModel);

            return RedirectToAction("Mesaj","Danisan");
        }

        public IActionResult Yanitla(string key)
        {
            return RedirectToAction("Mesaj", "Danisan");
        }

        public IActionResult GelenMesaj()
        {
            var danisanKey = HttpContext.Session.GetString("Deneme");
            return View(MesajBul(danisanKey));
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

        public void DanisanGuncelle(DanisanModel danisan)
        {
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Key == danisan.danisanKey)
                    {
                        client.Update("Danisan/" + item.Key, danisan);
                    }
                }
            }
        }

        public void AntrenorEslestirFonksiyonu(string danisanKey)
        {
            bool kasKazanma = false;
            bool kiloAlma = false;
            bool kiloKoruma = false;
            bool kiloVerme = false;

            int antrenorSayisi = 0;

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Key == danisanKey)
                    {
                        kasKazanma = item.Value.kasKazanma;
                        kiloAlma = item.Value.kiloAlma;
                        kiloKoruma = item.Value.kiloKoruma;
                        kiloVerme = item.Value.kiloVerme;
                    }
                }

                response = client.Get(@"Antrenor");
                Dictionary<string, AntrenorModel> data1 = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());

                foreach (var item in data1)
                {
                    if (item.Value.kalanDanisanSayisi > 0)
                    {
                        if (kasKazanma == item.Value.kasKazanma)
                        {
                            if (kiloAlma == item.Value.kiloAldirma)
                            {
                                if (kiloKoruma == item.Value.kiloKoruma)
                                {
                                    if (kiloVerme == item.Value.kiloVerdirme)
                                    {
                                        AntrenorAta(danisanKey, item.Key);
                                        break;
                                    }
                                    AntrenorAta(danisanKey, item.Key);
                                    break;
                                }
                                AntrenorAta(danisanKey, item.Key);
                                break;
                            }

                        }
                        antrenorSayisi++;
                    }
                }
                if (antrenorSayisi == 5)
                {
                    SıradanAntrenorAta(danisanKey);
                }

            }
        }

        public void AntrenorAta(string danisanKey, string antrenorKey)
        {
            DanisanModel danisan = new DanisanModel();
            AntrenorModel antrenor = new AntrenorModel();

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Key == danisanKey)
                    {
                        danisan = item.Value;
                        danisan.danisanKey = item.Key;
                        item.Value.antrenorKey = antrenorKey;
                        break;
                    }
                }

                response = client.Get(@"Antrenor");
                Dictionary<string, AntrenorModel> data1 = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());

                foreach (var item in data1)
                {
                    if (item.Key == antrenorKey)
                    {
                        antrenor = item.Value;
                        antrenor.antrenorKey = item.Key;
                        antrenor.kalanDanisanSayisi = item.Value.kalanDanisanSayisi - 1;
                        break;
                    }
                }

                client.Set(@"Danisan/" + danisan.danisanKey, danisan);
                client.Set(@"Antrenor/" + antrenor.antrenorKey, antrenor);
            }
        }

        public void SıradanAntrenorAta(string danisanKey)
        {
            DanisanModel danisan = new DanisanModel();
            AntrenorModel antrenor = new AntrenorModel();

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Key == danisanKey)
                    {
                        danisan = item.Value;
                        danisan.danisanKey = item.Key;
                        break;
                    }
                }

                response = client.Get(@"Antrenor");
                Dictionary<string, AntrenorModel> data1 = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());

                foreach (var item in data1)
                {
                    if (item.Value.kalanDanisanSayisi > 0)
                    {
                        antrenor = item.Value;
                        antrenor.antrenorKey = item.Key;
                        antrenor.kalanDanisanSayisi = item.Value.kalanDanisanSayisi - 1;
                        danisan.antrenorKey = item.Key;
                        break;
                    }
                }

                client.Set(@"Danisan/" + danisan.danisanKey, danisan);
                client.Set(@"Antrenor/" + antrenor.antrenorKey, antrenor);
            }

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
                        danisan.danisanKey = item.Key;
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

        public IlerlemeKaydi IlerlemeKaydiBul(string danisanKey)
        {
            IlerlemeKaydi ilerlemeKaydi = new IlerlemeKaydi();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"IlerlemeKaydi");
                Dictionary<string, IlerlemeKaydi> data = JsonConvert.DeserializeObject<Dictionary<string, IlerlemeKaydi>>(response.Body.ToString());

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Value.danisanKey == danisanKey)
                        {
                            ilerlemeKaydi = item.Value;
                            ilerlemeKaydi.kayitKey = item.Key;
                        }
                    }
                }

                if (ilerlemeKaydi.danisanKey == null)
                {
                    ilerlemeKaydi.danisanKey = danisanKey;
                    ilerlemeKaydi.cinsiyet = DanisanBul(danisanKey).cinsiyet;
                }

            }
            return ilerlemeKaydi;
        }

        public void IlerlemeKaydiOlusturma(IlerlemeKaydi ilerlemeKaydi)
        {
            RaporModel rapor = new RaporModel();

            if (BaglantiAc())
            {
                client.Push(@"IlerlemeKaydi/", ilerlemeKaydi);

                FirebaseResponse response = client.Get(@"IlerlemeKaydi");
                Dictionary<string, IlerlemeKaydi> data = JsonConvert.DeserializeObject<Dictionary<string, IlerlemeKaydi>>(response.Body.ToString());

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Value.danisanKey == ilerlemeKaydi.danisanKey)
                        {
                            ilerlemeKaydi.kayitKey = item.Key;
                            ilerlemeKaydi.endeks = ilerlemeKaydi.kilo / (ilerlemeKaydi.boy * ilerlemeKaydi.boy);
                            if (ilerlemeKaydi.cinsiyet == "Erkek" || ilerlemeKaydi.cinsiyet == "erkek")
                            {
                                ilerlemeKaydi.kasKutlesi = ilerlemeKaydi.kilo * (0.4);
                            }
                            else
                            {
                                ilerlemeKaydi.kasKutlesi = ilerlemeKaydi.kilo * (0.3);
                            }
                            client.Update(@"IlerlemeKaydi/" + item.Key, ilerlemeKaydi);

                            rapor.tarih = DateOnly.FromDateTime(DateTime.Now).ToString();
                            rapor.danisanKey = ilerlemeKaydi.danisanKey;
                            rapor.kilo = ilerlemeKaydi.kilo;
                            rapor.kasKutlesi = Convert.ToInt32(ilerlemeKaydi.kasKutlesi);
                            rapor.endeks = Convert.ToInt32(ilerlemeKaydi.endeks);

                            client.Push(@"Raporlar/", rapor);
                        }
                    }
                }
            }
        }

        public void IlerlemeKaydiGuncelle(IlerlemeKaydi ilerlemeKaydi)
        {
            RaporModel rapor = new RaporModel();

            if (BaglantiAc())
            {
                ilerlemeKaydi.endeks = ilerlemeKaydi.kilo / (ilerlemeKaydi.boy * ilerlemeKaydi.boy);
                if (ilerlemeKaydi.cinsiyet == "Erkek" || ilerlemeKaydi.cinsiyet == "erkek")
                {
                    ilerlemeKaydi.kasKutlesi = ilerlemeKaydi.kilo * (0.4);
                }
                else
                {
                    ilerlemeKaydi.kasKutlesi = ilerlemeKaydi.kilo * (0.3);
                }

                client.Update(@"IlerlemeKaydi/" + ilerlemeKaydi.kayitKey, ilerlemeKaydi);

                rapor.tarih = DateOnly.FromDateTime(DateTime.Now).ToString();
                rapor.danisanKey = ilerlemeKaydi.danisanKey;
                rapor.kilo = ilerlemeKaydi.kilo;
                rapor.kasKutlesi = Convert.ToInt32(ilerlemeKaydi.kasKutlesi);
                rapor.endeks = Convert.ToInt32(ilerlemeKaydi.endeks);

                client.Push(@"Raporlar/", rapor);
            }
        }

        public List<RaporModel> RaporBul(string danisanKey)
        {
            List<RaporModel> raporList = new List<RaporModel>();
            RaporModel rapor = new RaporModel();

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Raporlar");
                Dictionary<string, RaporModel> data = JsonConvert.DeserializeObject<Dictionary<string, RaporModel>>(response.Body.ToString());

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Value.danisanKey == danisanKey)
                        {
                            rapor = item.Value;
                            raporList.Add(rapor);
                        }
                    }
                }

            }
            return raporList;
        }

        public void AntrenoreMesajAt(MesajModel mesaj)
        {
            if (BaglantiAc())
            {
                client.Push(@"Mesajlar/", mesaj);
            }
        }

        public List<MesajModel> MesajBul(string danisanKey)
        {
            List<MesajModel> mesajList = new List<MesajModel>();
            MesajModel mesaj = new MesajModel();

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Mesajlar");
                Dictionary<string, MesajModel> data = JsonConvert.DeserializeObject<Dictionary<string, MesajModel>>(response.Body.ToString());

                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Value.aliciKey == danisanKey)
                        {
                            mesaj = item.Value;
                            mesajList.Add(mesaj);
                        }
                    }
                }

            }
            return mesajList;
        }


    }
}

