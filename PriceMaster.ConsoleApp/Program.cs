using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Commands;
using PriceMaster.Domain.Entities;
using PriceMaster.Infrastructure;
using PriceMaster.Infrastructure.Repositories;

namespace PriceMaster.ConsoleApp {
    public static class Program {

        public static async Task Main() {
            var options = new DbContextOptionsBuilder<PriceMasterDbContext>()
                .UseSqlite("Data Source=pricemaster.db")
                .Options;

            using (var context = new PriceMasterDbContext(options)) {
                var pending = context.Database.GetPendingMigrations();
                if (pending.Any()) {
                    Console.WriteLine($"Apply {pending.Count()} pending migrations.");
                    context.Database.Migrate();
                    Console.WriteLine("Database created and migrated successfully.");
                } else {
                    Console.WriteLine($"No pending migrations.");
                }

                #region Create Product with ProductCode = "110" for testing purpose
                var productService = new ProductService(new ProductRepository(context));

                var product = new Product {
                    ProductCode = "110",
                    SizeWidth = 0.6m,
                    SizeHeight = 0.3m,
                    RecommendedPrice = 2300,
                    CreatedAt = DateTime.UtcNow,
                    SeriesId = 1
                };

                var result = await productService.CreateProduct(product);
                Console.WriteLine(result.Message);
                #endregion
            }
        }
    }
}
