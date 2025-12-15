using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Commands;
using PriceMaster.Infrastructure;
using PriceMaster.Infrastructure.Repositories;

namespace PriceMaster.ConsoleApp {
    public static class Program {

        public static async Task Main() {
            var options = new DbContextOptionsBuilder<PriceMasterDbContext>()
                .UseSqlite("Data Source=pricemaster.db")
                .Options;

            using (var context = new PriceMasterDbContext(options)) {
                var pending = await context.Database.GetPendingMigrationsAsync();
                if (pending.Any()) {
                    Console.WriteLine($"Apply {pending.Count()} pending migrations.");
                    await context.Database.MigrateAsync();
                    Console.WriteLine("Database created and migrated successfully.");
                } else {
                    Console.WriteLine($"No pending migrations.");
                }

                #region Create Product with ProductCode = "110" for testing purpose
                var productService = new ProductService(new ProductRepository(context));

                var product = new Application.DTOs.CreateProductRequest {
                    ProductCode = "110",
                    SeriesId = 1,
                    SizeWidth = 60,
                    SizeHeight = 30,
                    RecommendedPrice = 2300,

                    BOMItems = new List<Application.DTOs.BOMItemDto> {
                            new Application.DTOs.BOMItemDto { ComponentId = 1, Quantity = 4 },   // Button
                            new Application.DTOs.BOMItemDto { ComponentId = 2, Quantity = 4 },   // Ring
                            new Application.DTOs.BOMItemDto { ComponentId = 4, Quantity = 8 },   // Harness component (simple)
                            new Application.DTOs.BOMItemDto { ComponentId = 53, Quantity = 4 },  // Silver coin (copy)
                            new Application.DTOs.BOMItemDto { ComponentId = 90, Quantity = 3 },  // Spherical ball
                            new Application.DTOs.BOMItemDto { ComponentId = 110, Quantity = 1 }, // Inkwell
                            new Application.DTOs.BOMItemDto { ComponentId = 500, Quantity = 1 }, // Baguette 60*30 Verona
                            new Application.DTOs.BOMItemDto { ComponentId = 900, Quantity = 1 }, // Printout 110
                            new Application.DTOs.BOMItemDto { ComponentId = 950, Quantity = 1 }, // Hardware
                            new Application.DTOs.BOMItemDto { ComponentId = 961, Quantity = 0.18m }, // Textile, per sq.m
                            new Application.DTOs.BOMItemDto { ComponentId = 962, Quantity = 0.18m }, // Polyurethane, per sq.m
                            new Application.DTOs.BOMItemDto { ComponentId = 1000, Quantity = 1 } // Work
                    }
                };

                var result = await productService.CreateProduct(product);
                Console.WriteLine(result.Message);
                #endregion

                #region Save Product with ProductCode = "110" to ProductionHistory for testing purpose
                var historyService = new ProductionHistoryService(
                    new ProductRepository(context),
                    new ProductionHistoryRepository(context)
                );

                var saveResult = await historyService.AddProductionHistoryEntryAsync("110");
                Console.WriteLine(saveResult.Message);

                var total = await historyService.GetTotalProductionValueReportAsync();
                Console.WriteLine($"Total value of all manufactured products (based on recommended price): {total:C}");
                #endregion
            }
        }
    }
}
