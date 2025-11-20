using Microsoft.EntityFrameworkCore;

namespace DevizWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // TODO: aici vei pune tabelele tale
        // public DbSet<Deviz> Devize { get; set; }
    }
}
