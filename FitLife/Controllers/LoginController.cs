using Microsoft.AspNetCore.Mvc;
using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using FitLife.Models;
using System.Net.Mail;


namespace FitLife.Controllers
{
    public class LoginController : Controller
    {
        public bool dogrulama = false;

        IFirebaseConfig fc = new FirebaseConfig()
        {
            AuthSecret = "m8qwx7IRwzpKKbWIRqL3blTIuhWNPjJYyPjRiBmJ",
            BasePath = "https://fitlife-e847e-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GirisYap(string email, string sifre)
        {
            if (AdminMi(email,sifre).adminKey != null)
            {
                return RedirectToAction("Admin", "Admin", new { AdminMi(email, sifre).adminKey });
            }
            else if (AntrenorMu(email, sifre).antrenorKey != null)
            {
                return RedirectToAction("Antrenor", "Antrenor", new { AntrenorMu(email, sifre).antrenorKey });
            }
            else if (DanisanMi(email, sifre).danisanKey != null)
            {
                return RedirectToAction("Authentication", "Login", new { DanisanMi(email, sifre).danisanKey });
            }

            return RedirectToAction("Index", "Home");


        }

        public IActionResult Authentication(string danisanKey)
        {
            return View();
        }

        public IActionResult PasswordAuthentication(string email)
        {
            MailGonder(email);
            return View();
        }

        [HttpPost]
        public IActionResult DogrulamaKodu(int kod, string danisanKey)
        {
            DogrulandiMi(kod);
            if (dogrulama)
            {
                return RedirectToAction("Danisan", "Danisan", new { danisanKey });

            }
            return View();
        }

        [HttpPost]
        public IActionResult PasswordDogrulamaKodu(int kod)
        {
            DogrulandiMi(kod);
            if (dogrulama)
            {
                return RedirectToAction("ChangePassword", "Login");
            }
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult SifreDegis(string email, string sifre)
        {
            SifreGuncelle(email, sifre);
            return RedirectToAction("Index", "Home");
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

        public int DogrulamaKoduOlustur()
        {
            Random rnd = new Random();

            return rnd.Next(100000, 1000000);
        }

        public void MailGonder(string email)
        {
            int dogrulamaKodu = DogrulamaKoduOlustur();
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(email);
            mailMessage.From = new MailAddress("utkuotomatikmail@gmail.com");
            mailMessage.Subject = "Fit-Life Doğrulama Kodu";
            mailMessage.Body = "Fit-Life Sitesi Girişi İçin Doğrulama Kodunuz: " + dogrulamaKodu;

            SmtpClient mailClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("utkuotomatikmail@gmail.com", "ypqa qtns oarv nseq")
            };

            mailClient.Send(mailMessage);

            DogrulamaKoduGuncelle(email, dogrulamaKodu);
        }

        public void MailGonder(string email, string password)
        {
            int dogrulamaKodu = DogrulamaKoduOlustur();
            MailMessage mailMessage = new MailMessage();
            mailMessage.To.Add(email);
            mailMessage.From = new MailAddress("utkuotomatikmail@gmail.com");
            mailMessage.Subject = "Fit-Life Doğrulama Kodu";
            mailMessage.Body = "Fit-Life Sitesi Girişi İçin Doğrulama Kodunuz: " + dogrulamaKodu;

            SmtpClient mailClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("utkuotomatikmail@gmail.com", "ypqa qtns oarv nseq")
            };

            mailClient.Send(mailMessage);
            DogrulamaKoduGuncelle(email, password, dogrulamaKodu);
        }

        public void DogrulamaKoduGuncelle(string email, int dogrulamaKodu)
        {
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.email == email)
                    {
                        item.Value.dogrulamakodu = dogrulamaKodu;
                        client.Update("Danisan/" + item.Key, item.Value);
                    }
                }
            }
        }

        public void DogrulamaKoduGuncelle(string email, string password, int dogrulamaKodu)
        {
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.email == email && item.Value.sifre == password)
                    {
                        item.Value.dogrulamakodu = dogrulamaKodu;
                        client.Update("Danisan/" + item.Key, item.Value);
                    }
                }
            }
        }

        public bool DogrulandiMi(int kod)
        {
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.dogrulamakodu == kod)
                    {
                        dogrulama = true;
                    }
                }
            }
            else
            {
                dogrulama = false;
            }
            return dogrulama;

        }

        public void SifreGuncelle(string email, string sifre)
        {
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.email == email)
                    {
                        item.Value.sifre = sifre;
                        client.Update("Danisan/" + item.Key, item.Value);
                    }
                }
            }
        }

        public AdminModel AdminMi(string email, string sifre)
        {
            AdminModel admin = new AdminModel();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Admin");
                Dictionary<string, AdminModel> data = JsonConvert.DeserializeObject<Dictionary<string, AdminModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.email == email && item.Value.sifre == sifre)
                    {
                        admin = item.Value;
                        admin.adminKey = item.Key;
                    }
                }
            }
            return admin;
        }
        public AntrenorModel AntrenorMu(string email, string sifre)
        {
            AntrenorModel antrenor = new AntrenorModel();
            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Antrenor");
                Dictionary<string, AntrenorModel> data = JsonConvert.DeserializeObject<Dictionary<string, AntrenorModel>>(response.Body.ToString());

                foreach (var item in data)
                {
                    if (item.Value.email == email && item.Value.sifre == sifre && item.Value.hesapAktifMi == true)
                    {
                        antrenor = item.Value;
                        antrenor.antrenorKey = item.Key;

                    }
                }
            }
            return antrenor;
        }
        public DanisanModel DanisanMi(string email, string sifre)
        {
            DanisanModel danisan = new DanisanModel();
            string danisanKey;

            if (BaglantiAc())
            {
                FirebaseResponse response = client.Get(@"Danisan");
                Dictionary<string, DanisanModel> data = JsonConvert.DeserializeObject<Dictionary<string, DanisanModel>>(response.Body.ToString());
                //var data = JsonConvert.DeserializeObject<DanisanModel>(response.Body);

                foreach (var item in data)
                {
                    if (item.Value.email == email && item.Value.sifre == sifre && item.Value.hesapAktifMi == true)
                    {
                        MailGonder(email, sifre);
                        danisan = item.Value;
                        danisan.danisanKey = item.Key;
                    }
                }
            }
            return danisan;

        }


    }
}
