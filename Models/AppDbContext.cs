using Microsoft.EntityFrameworkCore;

namespace ZeynepsNotebook.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<NotItem> Notlar { get; set; }
        public DbSet<GorevItem> Gorevler { get; set; }
        public DbSet<GunlukItem> Gunlukler { get; set; }
        public DbSet<ResimItem> Resimler { get; set; }
        public DbSet<SayacItem> Sayaclar { get; set; }
        public DbSet<DersProgramiItem> DersProgramlari { get; set; }
        public DbSet<FilmDiziItem> FilmDiziler { get; set; }

        // YENİ EKLENEN GEZİ TABLOSU
        public DbSet<GeziItem> GezilecekYerler { get; set; }
    }
}