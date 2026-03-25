using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeynepsNotebook.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace ZeynepsNotebook.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ==========================================
        // 🔐 GÜVENLİK DUVARI (ŞİFRE KONTROLÜ)
        // ==========================================
        private bool SifreOnayliMi()
        {
            // Tarayıcıda 30 günlük giriş izni (çerez) var mı bakıyoruz
            return Request.Cookies["ZeynepKasa"] == "Acik";
        }

        public IActionResult Giris()
        {
            return View(); // Sadece şifre ekranını göster
        }

        [HttpPost]
        public IActionResult GirisYap(string sifre)
        {
            // 🚨 ŞİFRENİ BURAYA YAZIYORSUN 🚨
            string bizimSifremiz = "2102"; // <-- İstediğin şifreyi bu tırnakların içine yaz!

            if (sifre == bizimSifremiz)
            {
                // YENİ SİSTEM: Süre (Tarih) vermiyoruz. Tarayıcı kapanınca şifre silinir (Session Cookie)
                Response.Cookies.Append("ZeynepKasa", "Acik");
                return RedirectToAction("Index"); // Ana sayfaya yolla
            }

            // Şifre yanlışsa
            ViewBag.Hata = "Bilemedin! Lütfen tekrar dene 🥺";
            return View("Giris");
        }

        public IActionResult CikisYap()
        {
            Response.Cookies.Delete("ZeynepKasa"); // Çerezi sil (Kapıyı kilitle)
            return RedirectToAction("Giris");
        }

        // ==========================================
        // 🏠 MEVCUT SAYFALAR (Hepsi Korumaya Alındı)
        // ==========================================

        public IActionResult Index()
        {
            if (!SifreOnayliMi()) return RedirectToAction("Giris"); // Şifre yoksa Giriş'e yolla!

            ViewBag.KayitliNot = _context.Notlar.FirstOrDefault()?.Icerik ?? "";
            var bugun = DateTime.Today; ViewBag.BugununGunlugu = _context.Gunlukler.FirstOrDefault(g => g.Tarih.Date == bugun)?.GunlukMetni ?? "";
            ViewBag.Gorevler = _context.Gorevler.ToList();
            ViewBag.Sayac = _context.Sayaclar.FirstOrDefault();

            int saat = DateTime.Now.Hour; string karsilama = "";
            if (saat >= 6 && saat < 12) karsilama = "Günaydın Zeynep ☀️";
            else if (saat >= 12 && saat < 18) karsilama = "Tünaydın Zeynep 🌼";
            else if (saat >= 18 && saat < 22) karsilama = "İyi Akşamlar Zeynep 🌆";
            else karsilama = "İyi Geceler, Tatlı Rüyalar 🌙";
            ViewBag.KarsilamaMesaji = karsilama;

            var motivasyonSozleri = new List<string> { "Bugün de harika görünüyorsun! ✨", "Sınavları falan hiç dert etme, sen her şeyi halledersin! 💪", "Gülümsemek sana çok yakışıyor, lütfen hep gül! 🌸", "Su içmeyi unutma, kendine iyi bak! 💧", "Bugün senin günün olsun! 🌟", "Kendine inanmaktan asla vazgeçme, seninle gurur duyuyorum! 💖", "Derin bir nefes al, her şey çok güzel olacak! ☕", "Senin enerjin her yeri aydınlatıyor! ✨", "Küçük adımlar büyük başarılar getirir, yola devam! 🚀", "Ne olursa olsun, her şeyin üstesinden gelebilirsin! 🦋" };
            ViewBag.GununSozu = motivasyonSozleri[new Random().Next(motivasyonSozleri.Count)];

            var tumResimler = _context.Resimler.ToList();
            if (tumResimler.Any()) ViewBag.GununResmi = tumResimler[new Random().Next(tumResimler.Count)].ResimYolu;

            return View();
        }

        public IActionResult Galerim() { if (!SifreOnayliMi()) return RedirectToAction("Giris"); return View(_context.Resimler.OrderByDescending(r => r.EklenmeTarihi).ToList()); }
        public IActionResult DersProgrami() { if (!SifreOnayliMi()) return RedirectToAction("Giris"); return View(_context.DersProgramlari.ToList()); }
        public IActionResult Gecmis() { if (!SifreOnayliMi()) return RedirectToAction("Giris"); return View(_context.Gunlukler.Where(g => g.Tarih.Date < DateTime.Today).OrderByDescending(g => g.Tarih).ToList()); }
        public IActionResult FilmDiziListesi() { if (!SifreOnayliMi()) return RedirectToAction("Giris"); return View(_context.FilmDiziler.OrderBy(f => f.IzlendiMi).ThenByDescending(f => f.Id).ToList()); }
        public IActionResult GeziListesi() { if (!SifreOnayliMi()) return RedirectToAction("Giris"); return View(_context.GezilecekYerler.OrderBy(g => g.GidildiMi).ThenByDescending(g => g.Id).ToList()); }

        // --- KAYDETME VE SİLME İŞLEMLERİ (Aynen duruyor) ---
        [HttpPost] public IActionResult GeziEkle(string yerAdi, string sehir) { if (!string.IsNullOrWhiteSpace(yerAdi) && !string.IsNullOrWhiteSpace(sehir)) { _context.GezilecekYerler.Add(new GeziItem { YerAdi = yerAdi, Sehir = sehir, GidildiMi = false }); _context.SaveChanges(); } return RedirectToAction("GeziListesi"); }
        public IActionResult GeziDurumu(int id) { var g = _context.GezilecekYerler.Find(id); if (g != null) { g.GidildiMi = !g.GidildiMi; _context.SaveChanges(); } return RedirectToAction("GeziListesi"); }
        public IActionResult GeziSil(int id) { var g = _context.GezilecekYerler.Find(id); if (g != null) { _context.GezilecekYerler.Remove(g); _context.SaveChanges(); } return RedirectToAction("GeziListesi"); }
        [HttpPost] public IActionResult FilmEkle(string baslik, string tur) { if (!string.IsNullOrWhiteSpace(baslik) && !string.IsNullOrWhiteSpace(tur)) { _context.FilmDiziler.Add(new FilmDiziItem { Baslik = baslik, Tur = tur, IzlendiMi = false }); _context.SaveChanges(); } return RedirectToAction("FilmDiziListesi"); }
        public IActionResult FilmDurumu(int id) { var f = _context.FilmDiziler.Find(id); if (f != null) { f.IzlendiMi = !f.IzlendiMi; _context.SaveChanges(); } return RedirectToAction("FilmDiziListesi"); }
        public IActionResult FilmSil(int id) { var f = _context.FilmDiziler.Find(id); if (f != null) { _context.FilmDiziler.Remove(f); _context.SaveChanges(); } return RedirectToAction("FilmDiziListesi"); }
        [HttpPost] public IActionResult DersEkle(string gun, string saat, string dersAdi) { if (!string.IsNullOrWhiteSpace(gun) && !string.IsNullOrWhiteSpace(saat) && !string.IsNullOrWhiteSpace(dersAdi)) { _context.DersProgramlari.Add(new DersProgramiItem { Gun = gun, Saat = saat, DersAdi = dersAdi }); _context.SaveChanges(); } return RedirectToAction("DersProgrami"); }
        public IActionResult DersSil(int id) { var ders = _context.DersProgramlari.Find(id); if (ders != null) { _context.DersProgramlari.Remove(ders); _context.SaveChanges(); } return RedirectToAction("DersProgrami"); }
        [HttpPost] public IActionResult SayacKaydet(string baslik, DateTime hedefTarih) { var mSayac = _context.Sayaclar.FirstOrDefault(); if (mSayac == null) { _context.Sayaclar.Add(new SayacItem { Baslik = baslik, HedefTarih = hedefTarih }); } else { mSayac.Baslik = baslik; mSayac.HedefTarih = hedefTarih; _context.Sayaclar.Update(mSayac); } _context.SaveChanges(); return RedirectToAction("Index"); }
        public IActionResult SayacSil() { var mSayac = _context.Sayaclar.FirstOrDefault(); if (mSayac != null) { _context.Sayaclar.Remove(mSayac); _context.SaveChanges(); } return RedirectToAction("Index"); }
        [HttpPost] public IActionResult NotKaydet(string notIcerigi) { var vNot = _context.Notlar.FirstOrDefault(); if (vNot == null) _context.Notlar.Add(new NotItem { Icerik = notIcerigi }); else { vNot.Icerik = notIcerigi; _context.Notlar.Update(vNot); } _context.SaveChanges(); return RedirectToAction("Index"); }
        [HttpPost] public IActionResult GunlukKaydet(string gunlukMetni) { var bugun = DateTime.Today; var vGunluk = _context.Gunlukler.FirstOrDefault(g => g.Tarih.Date == bugun); if (vGunluk == null) _context.Gunlukler.Add(new GunlukItem { GunlukMetni = gunlukMetni, Tarih = DateTime.Now }); else { vGunluk.GunlukMetni = gunlukMetni; _context.Gunlukler.Update(vGunluk); } _context.SaveChanges(); return RedirectToAction("Index"); }
        [HttpPost] public IActionResult GorevEkle(string gorevAdi) { if (!string.IsNullOrWhiteSpace(gorevAdi)) { _context.Gorevler.Add(new GorevItem { GorevAdi = gorevAdi, YapildiMi = false }); _context.SaveChanges(); } return RedirectToAction("Index"); }
        public IActionResult GorevDurumu(int id) { var g = _context.Gorevler.Find(id); if (g != null) { g.YapildiMi = !g.YapildiMi; _context.SaveChanges(); } return RedirectToAction("Index"); }
        public IActionResult GorevSil(int id) { var g = _context.Gorevler.Find(id); if (g != null) { _context.Gorevler.Remove(g); _context.SaveChanges(); } return RedirectToAction("Index"); }
        [HttpPost] public async Task<IActionResult> ResimYukle(IFormFile yuklenenResim) { if (yuklenenResim != null && yuklenenResim.Length > 0) { string uFolder = Path.Combine(_webHostEnvironment.WebRootPath, "resimler"); if (!Directory.Exists(uFolder)) Directory.CreateDirectory(uFolder); string uName = Guid.NewGuid().ToString() + "_" + yuklenenResim.FileName; string fPath = Path.Combine(uFolder, uName); using (var fStream = new FileStream(fPath, FileMode.Create)) { await yuklenenResim.CopyToAsync(fStream); } _context.Resimler.Add(new ResimItem { ResimYolu = "/resimler/" + uName }); _context.SaveChanges(); } return RedirectToAction("Galerim"); }
        public IActionResult ResimSil(int id) { var r = _context.Resimler.Find(id); if (r != null) { string fPath = Path.Combine(_webHostEnvironment.WebRootPath, r.ResimYolu.TrimStart('/')); if (System.IO.File.Exists(fPath)) System.IO.File.Delete(fPath); _context.Resimler.Remove(r); _context.SaveChanges(); } return RedirectToAction("Galerim"); }
    }
}