using System;

namespace ZeynepsNotebook.Models
{
    public class FilmDiziItem
    {
        public int Id { get; set; }
        public string Baslik { get; set; } // Örn: Inception, Breaking Bad
        public string Tur { get; set; } // "Film" veya "Dizi" seçeneği
        public bool IzlendiMi { get; set; } // İzledik mi, izlemedik mi?
    }
}