using System;

namespace ZeynepsNotebook.Models
{
    public class ResimItem
    {
        public int Id { get; set; }
        public string? ResimYolu { get; set; } // Resmin bilgisayardaki/sunucudaki yeri
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
    }
}