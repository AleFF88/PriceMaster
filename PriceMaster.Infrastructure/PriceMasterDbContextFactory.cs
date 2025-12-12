using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PriceMaster.Infrastructure {
    public class PriceMasterDbContextFactory : IDesignTimeDbContextFactory<PriceMasterDbContext> {
        public PriceMasterDbContext CreateDbContext(string[] args) {
            var optionsBuilder = new DbContextOptionsBuilder<PriceMasterDbContext>();
            optionsBuilder.UseSqlite("Data Source=pricemaster.db");

            return new PriceMasterDbContext(optionsBuilder.Options);
        }
    }
}
