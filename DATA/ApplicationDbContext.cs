using Microsoft.EntityFrameworkCore;
using PROJEKTDB.Models;

namespace PROJEKTDB.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Galeri> Galeris { get; set; }
        public DbSet<Pikture> Piktures { get; set; }
        public DbSet<Fature> Fature { get; set; }
        public DbSet<Rresht> Rresht { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Rresht>()
    .HasKey(r => new { r.FatId, r.RreId });


            /* ===== PIKTURE ===== */
            modelBuilder.Entity<Pikture>()
                .HasOne(p => p.Person)
                .WithMany()
                .HasForeignKey(p => p.PerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pikture>()
                .HasOne(p => p.Galeri)
                .WithMany()
                .HasForeignKey(p => p.GalId)
                .OnDelete(DeleteBehavior.Restrict);

                 // ✅ PIKTURE ka trigger => mos përdor OUTPUT clause
    modelBuilder.Entity<Pikture>()
        .ToTable("PIKTURE", tb =>
        {
            tb.HasTrigger("trg_Pikture_SavePriceHistory");
            tb.UseSqlOutputClause(false);
        });

            /* ===== FATURE ===== */
            modelBuilder.Entity<Fature>()
                .HasOne(f => f.Person)
                .WithMany()
                .HasForeignKey(f => f.PerId)
                .OnDelete(DeleteBehavior.Restrict);

            /* ===== RRESHT ===== */
            // Composite Primary Key
            modelBuilder.Entity<Rresht>()
                .HasKey(r => new { r.FatId, r.RreId });

            // FATURE (1) -> RRESHT (many)
            modelBuilder.Entity<Rresht>()
                .HasOne(r => r.Fature)
                .WithMany(f => f.Rresht)   // ⚠️ shumë e rëndësishme
                .HasForeignKey(r => r.FatId)
                .OnDelete(DeleteBehavior.Cascade); // si në SQL

            // PIKTURE (1) -> RRESHT (many)
            modelBuilder.Entity<Rresht>()
                .HasOne(r => r.Pikture)
                .WithMany()
                .HasForeignKey(r => r.PikId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
