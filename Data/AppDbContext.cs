using DevizWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DevizWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<Deviz> Devize { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Deviz>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.NrDeviz).IsUnique();
            });
        }
    }
}
