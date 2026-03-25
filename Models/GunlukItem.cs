using System;

namespace ZeynepsNotebook.Models
{
    public class GunlukItem
    {
        public int Id { get; set; }
        public string? GunlukMetni { get; set; } // O günün özeti
        public DateTime Tarih { get; set; } = DateTime.Now; // Yazdığı günün tarihi
    }
}