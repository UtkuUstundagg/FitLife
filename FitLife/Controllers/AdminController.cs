using Microsoft.AspNetCore.Mvc;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using FitLife.Models;
using Newtonsoft.Json;

namespace FitLife.Controllers
{
    public class AdminController : Controller
    {
        IFirebaseConfig fc = new FirebaseConfig()
        {
            AuthSecret = "m8qwx7IRwzpKKbWIRqL3blTIuhWNPjJYyPjRiBmJ",
            BasePath = "https://fitlife-e847e-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;
        public bool BaglantiAc()
        {
            try
            {
                client = new FirebaseClient(fc);
                return true;
            }
            catch (Exception)
            {
                
                return false;
            }
        }

        public IActionResult Admin(string adminKey)
        {
            return View(TupleGonder());
        }

        public IActionResult YeniHesap()
        {
            return RedirectToAction("Register", "Register");
        }


        
        public IActionResult Guncelle(string key)
        {
            if (DanisanMi(key))
            {
                return RedirectToAction("Profile", "Danisan", DanisanBul(key));
            }
            else
            {
                string antrenorKey = key;
                return RedirectToAction("Antrenor", "Antrenor", new { antrenorKey });
            }

        }

        
        public IActionResult Etkinlestir(string key)
        {
            HesapDurumGuncelle(key, true);
            return RedirectToAction("Admin", "Admin");
        }

        
        public IActionResult DevreDisiBirak(string key)
        {
            HesapDurumGuncelle(key, false);
            return RedirectToAction("Admin", "Admin");
        }



        public Tuple<List<DanisanModel>, List<AntrenorModel>> TupleGonder()
        {
            List<DanisanModel> danisanList = new List<DanisanModel>();
            List<AntrenorModel> antrenorList = new List<AntrenorModel>();

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    DanisanModel danisan = new DanisanModel();

                    danisan.danisanKey = item.Key;
                    danisan.isim = item.Value.isim;
                    danisan.soyisim = item.Value.soyisim;
                    danisan.telefon = item.Value.telefon;
                    danisan.email = item.Value.email;
                    danisan.sifre = item.Value.sifre;
                    danisan.cinsiyet = item.Value.cinsiyet;
                    danisan.dogumtarihi = item.Value.dogumtarihi;
                    danisan.hesapAktifMi = item.Value.hesapAktifMi;

                    danisanList.Add(danisan);
                }

                response = client.Get(@"Antrenor");
                Dictionary<string, AntrenorModel> data1 = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());

                foreach (var item in data1)
                {
                    AntrenorModel antrenor = new AntrenorModel();

                    antrenor.antrenorKey = item.Key;
                    antrenor.isim = item.Value.isim;
                    antrenor.soyisim = item.Value.soyisim;
                    antrenor.telefon = item.Value.telefon;
                    antrenor.email = item.Value.email;
                    antrenor.sifre = item.Value.sifre;
                    antrenor.cinsiyet = item.Value.cinsiyet;
                    antrenor.dogumtarihi = item.Value.dogumtarihi;
                    antrenor.hesapAktifMi = item.Value.hesapAktifMi;

                    antrenorList.Add(antrenor);
                }
            }

            Tuple<List<DanisanModel>, List<AntrenorModel>> tuple = new Tuple<List<DanisanModel>, List<AntrenorModel>>(danisanList, antrenorList);
            return tuple;



        }

        public void HesapDurumGuncelle(string key, bool durum)
        {
            if (durum)
            {
                if (BaglantiAc())
                {
                    FirebaseResponse response = client.Get(@"Danisan");
                    Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            if (item.Key == key)
                            {
                                item.Value.hesapAktifMi = true;
                                client.Update("Danisan/" + item.Key, item.Value);
                            }
                        }
                    }

                    response = client.Get(@"Antrenor");
                    Dictionary<string, AntrenorModel> data1 = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());
                    if (data1 != null)
                    {
                        foreach (var item in data1)
                        {
                            if (item.Key == key)
                            {
                                item.Value.hesapAktifMi = true;
                                client.Update("Danisan/" + item.Key, item.Value);
                            }
                        }
                    }
                }
            }
            else
            {
                if (BaglantiAc())
                {
                    FirebaseResponse response = client.Get(@"Danisan");
                    Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());
                    if (data != null)
                    {
                        foreach (var item in data)
                        {
                            if (item.Key == key)
                            {
                                item.Value.hesapAktifMi = false;
                                client.Update("Danisan/" + item.Key, item.Value);
                            }
                        }
                    }

                    response = client.Get(@"Antrenor");
                    Dictionary<string, AntrenorModel> data1 = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());
                    if (data1 != null)
                    {
                        foreach (var item in data1)
                        {
                            if (item.Key == key)
                            {
                                item.Value.hesapAktifMi = false;
                                client.Update("Danisan/" + item.Key, item.Value);
                            }
                        }
                    }
                }
            }

        }

        public bool DanisanMi(string key)
        {
            bool result = false;
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Key == key)
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        public DanisanModel DanisanBul(string key)
        {
            DanisanModel danisan = new DanisanModel();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Key == key)
                        {
                            danisan = item.Value;
                            danisan.danisanKey = item.Key;
                        }
                    }
                }
            }
            return danisan;
            
        }

        public AntrenorModel AntrenorBul(string key)
        {
            AntrenorModel antrenor = new AntrenorModel();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, AntrenorModel> data = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.Key == key)
                        {
                            antrenor = item.Value;
                            antrenor.antrenorKey = item.Key;
                        }
                    }
                }
            }
            return antrenor;

        }

    }
}
