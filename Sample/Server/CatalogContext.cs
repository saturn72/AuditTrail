using Server.Domain;

namespace Server
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasMany(dm => dm.Categories)
                .WithMany();

            modelBuilder.Entity<Product>()
                .Navigation(e => e.Categories).AutoInclude();
        }
    }
}
