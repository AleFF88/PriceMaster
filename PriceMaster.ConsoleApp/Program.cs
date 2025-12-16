using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Commands;
using PriceMaster.Contracts.DTOs.Products;
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
                    Console.WriteLine();
                } else {
                    Console.WriteLine($"No pending migrations.");
                    Console.WriteLine();
                }

                #region Create Product with ProductCode = "110" for testing purpose
                var productService = new ProductService(new ProductRepository(context));

                var product = new CreateProductRequest {
                    ProductCode = "110",
                    SeriesId = 1,
                    SizeWidth = 60,
                    SizeHeight = 30,
                    RecommendedPrice = 2300,

                    BOMItems = new List<BOMItemDto> {
                            new BOMItemDto { ComponentId = 1, Quantity = 4 },   // Button
                            new BOMItemDto { ComponentId = 2, Quantity = 4 },   // Ring
                            new BOMItemDto { ComponentId = 4, Quantity = 8 },   // Harness co
                            new BOMItemDto { ComponentId = 53, Quantity = 4 },  // Silver co
                            new BOMItemDto { ComponentId = 90, Quantity = 3 },  // Sp
                            new BOMItemDto { ComponentId = 110, Quantity = 1 }, // Inkwell
                            new BOMItemDto { ComponentId = 500, Quantity = 1 }, // Baguette 60ducts.
                            new BOMItemDto { ComponentId = 900, Quantity = 1 }, // Printout 11
                            new BOMItemDto { ComponentId = 950, Quantity = 1 }, // Hardware
                            new BOMItemDto { ComponentId = 961, Quantity = 0.18m }, // Textile, per sq.m.
                            new BOMItemDto { ComponentId = 962, Quantity = 0.18m }, // Polyurethane, per sq.m.
                            new BOMItemDto { ComponentId = 1000, Quantity = 1 } // Work
                    }
                };

                var result = await productService.CreateProduct(product);
                Console.WriteLine(result.Message);
                Console.WriteLine();
                #endregion

                #region Save Product with ProductCode = "110" to ProductionHistory for testing purpose and test some reports.  
                var historyService = new ProductionHistoryService(
                    new ProductRepository(context),
                    new ProductionHistoryRepository(context)
                );

                var saveResult = await historyService.AddProductionHistoryEntryAsync("110");
                if (!saveResult.Success) {
                    Console.WriteLine($"Failed to save production history entry: {saveResult.Message}");
                    Console.WriteLine();
                    return;
                } else {
                    Console.WriteLine($"{saveResult.Message}");
                    Console.WriteLine($"Date: {saveResult.CreatedAt:yyyy-MM-dd}, Price: {saveResult.Price:C}, Recommended Price: {saveResult.RecommendedPrice:C}, Work Cost: {saveResult.WorkCost:C}");
                    Console.WriteLine();
                }

                // Total production value report.
                var total = await historyService.GetTotalProductionValueReportAsync();
                Console.WriteLine($"Total value of all manufactured products (based on recommended price): {total:C}");
                Console.WriteLine();
                #endregion
            }
        }
    }
}
