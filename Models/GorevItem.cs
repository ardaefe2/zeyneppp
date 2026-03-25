namespace ZeynepsNotebook.Models
{
    public class GorevItem
    {
        public int Id { get; set; }
        public string? GorevAdi { get; set; } // "Su iç" gibi görevler
        public bool YapildiMi { get; set; } // Tik atılıp atılmadığını tutar
    }
}