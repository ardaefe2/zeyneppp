using System;

namespace ZeynepsNotebook.Models
{
    public class DersProgramiItem
    {
        public int Id { get; set; }
        public string Gun { get; set; } // Pazartesi, Salı vb.
        public string Saat { get; set; } // 14:00 - 15:00 vb.
        public string DersAdi { get; set; } // Örn: Algoritma, Veritabanı...
    }
}