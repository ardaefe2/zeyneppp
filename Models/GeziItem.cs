using System;

namespace ZeynepsNotebook.Models
{
    public class GeziItem
    {
        public int Id { get; set; }
        public string YerAdi { get; set; } // Örn: Galata Kulesi, Şirin Kafe
        public string Sehir { get; set; } // Örn: İstanbul, Eskişehir
        public bool GidildiMi { get; set; } // Gittik mi, gitmedik mi?
    }
}