using System;

namespace ZeynepsNotebook.Models
{
    public class SayacItem
    {
        public int Id { get; set; }
        public string Baslik { get; set; } // Örn: "Yıl dönümümüze kalan zaman"
        public DateTime HedefTarih { get; set; } // Zeynep'in seçeceği tarih ve saat
    }
}