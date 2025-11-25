using Microsoft.EntityFrameworkCore;

namespace DevizWebApp.Data
{
    public class AppDbContext : DbContext
    {
        // Constructor cu DbContextOptions
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Exemplu DbSet
        public DbSet<MyEntity> MyEntities { get; set; }
    }

    public class MyEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
