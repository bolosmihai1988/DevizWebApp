using Microsoft.EntityFrameworkCore;
using DevizWebApp.Models;

namespace DevizWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        // Tabele
        public DbSet<Deviz> Devize { get; set; }
        // Păstrează orice alte DbSet-uri existente
        public DbSet<MyEntity> MyEntities { get; set; }
    }

    // Clasa MyEntity dacă o mai folosești
    public class MyEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
