using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PriceMaster.Infrastructure {
    public class PriceMasterDbContext : DbContext {
        public DbSet<Product> Products { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BomItem> BomItems { get; set; }
        public DbSet<ProductionHistory> ProductionHistories { get; set; }

        public PriceMasterDbContext(DbContextOptions<PriceMasterDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PriceMasterDbContext).Assembly);
        }
    }
}
