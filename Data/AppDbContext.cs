using Microsoft.EntityFrameworkCore;

namespace DevizWebApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Tabel pentru devize
        public DbSet<Deviz> Devize { get; set; }
    }

    // Modelul Deviz — trebuie să existe pentru a salva în DB!
    public class Deviz
    {
        public int Id { get; set; }  // autoincrement
        public string Client { get; set; }
        public string Descriere { get; set; }
        public decimal Total { get; set; }
        public DateTime DataCreare { get; set; } = DateTime.UtcNow;
    }
}
