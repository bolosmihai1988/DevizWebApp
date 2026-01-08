using DevizWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DevizWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Deviz> Devize => Set<Deviz>();
        public DbSet<DevizItem> DevizItems => Set<DevizItem>();

        public DbSet<Factura> Facturi => Set<Factura>();
        public DbSet<FacturaItem> FacturaItems => Set<FacturaItem>();
        public DbSet<FacturaDeviz> FacturaDevize => Set<FacturaDeviz>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Deviz>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.NrDeviz).IsUnique();

                // Permitem câmpuri goale (NULL) în DB (ca să nu mai crape dacă nu le completezi)
                e.Property(x => x.Firma).IsRequired(false).HasMaxLength(200);
                e.Property(x => x.CUI).IsRequired(false).HasMaxLength(50);
                e.Property(x => x.Adresa).IsRequired(false).HasMaxLength(300);
                e.Property(x => x.Telefon).IsRequired(false).HasMaxLength(50);
                e.Property(x => x.Data).IsRequired(false).HasMaxLength(50);

                e.Property(x => x.Masina).IsRequired(false).HasMaxLength(200);
                e.Property(x => x.NrInmat).IsRequired(false).HasMaxLength(50);
                e.Property(x => x.KM).IsRequired(false).HasMaxLength(50);
                e.Property(x => x.SerieCaroserie).IsRequired(false).HasMaxLength(50);
                e.Property(x => x.SerieMotor).IsRequired(false).HasMaxLength(50);

                e.Property(x => x.Constatare).IsRequired(false);
                e.Property(x => x.LucrariConvenite).IsRequired(false);
                e.Property(x => x.PieseAduseClient).IsRequired(false);

                e.HasMany(x => x.Items)
                 .WithOne(x => x.Deviz!)
                 .HasForeignKey(x => x.DevizId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DevizItem>(e =>
            {
                e.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Factura>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.NrFactura).IsUnique();

                e.HasMany(x => x.Items)
                 .WithOne(x => x.Factura!)
                 .HasForeignKey(x => x.FacturaId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FacturaItem>(e =>
            {
                e.HasKey(x => x.Id);
            });

            modelBuilder.Entity<FacturaDeviz>(e =>
            {
                e.HasKey(x => x.Id);

                e.HasOne(x => x.Factura)
                 .WithMany(f => f.FacturaDevize)
                 .HasForeignKey(x => x.FacturaId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Deviz)
                 .WithMany()
                 .HasForeignKey(x => x.DevizId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
