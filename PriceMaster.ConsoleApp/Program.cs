using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Commands;
using PriceMaster.Contracts.DTOs.ProductionHistory;
using PriceMaster.Contracts.DTOs.Products;
using PriceMaster.Contracts.DTOs.Reports;
using PriceMaster.Contracts.Mappers;
using PriceMaster.Infrastructure;
using PriceMaster.Infrastructure.Repositories;
using System.Globalization;

namespace PriceMaster.ConsoleApp {
    public static class Program {

        // This method simulates incoming payload data that would normally be
        //   received from an API request.
        private static CreateProductRequest RecieveCreateProductRequest() => new CreateProductRequest {
            ProductCode = "110",
            SeriesId = 1,
            SizeWidth = 60,
            SizeHeight = 30,
            RecommendedPrice = 2300,

            BOMItems = new List<BOMItemDto> {
                new BOMItemDto { ComponentId = 1, Quantity = 4 },   // Button
                new BOMItemDto { ComponentId = 2, Quantity = 4 },   // Ring
                new BOMItemDto { ComponentId = 4, Quantity = 8 },   // Harness Component (simple) 
                new BOMItemDto { ComponentId = 53, Quantity = 4 },  // Silver coin (copy)
                new BOMItemDto { ComponentId = 90, Quantity = 3 },  // Spherical ball
                new BOMItemDto { ComponentId = 110, Quantity = 1 }, // Inkwell
                new BOMItemDto { ComponentId = 500, Quantity = 1 }, // Baguette 60*30 Verona
                new BOMItemDto { ComponentId = 900, Quantity = 1 }, // Printout 110
                new BOMItemDto { ComponentId = 950, Quantity = 1 }, // Hardware
                new BOMItemDto { ComponentId = 961, Quantity = 0.18m }, // Textile, per sq.m.
                new BOMItemDto { ComponentId = 962, Quantity = 0.18m }, // Polyurethane, per sq.m.
                new BOMItemDto { ComponentId = 1000, Quantity = 1 } // Work
            }
        };


        // Check for any pending EF Core migrations and apply them to ensure
        //   the database schema is up to date.
        private async static Task CheckAndPerformMigrations(PriceMasterDbContext context) {
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
        }

        public static async Task Main() {
            var options = new DbContextOptionsBuilder<PriceMasterDbContext>()
                .UseSqlite("Data Source=pricemaster.db")
                .Options;

            using (var context = new PriceMasterDbContext(options)) {
                CheckAndPerformMigrations(context);

                //TODO move this "testing" code to proper Unit Tests project.
                #region TESTBLOCK 01. Create Product with ProductCode = "110" for testing purpose
                var productService = new ProductService(new ProductRepository(context));
                var createProductRequest = RecieveCreateProductRequest();

                var createProductDomain = createProductRequest.ToDomain(); // DTO -> Domain
                var createProductServiceResult = await productService.CreateProduct(createProductDomain);
                var createProductResponse = createProductDomain.ToResponse(createProductServiceResult.Success, createProductServiceResult.Message); //Domain → DTO

                Console.WriteLine(createProductResponse.Message);
                Console.WriteLine();
                #endregion

                #region TESTBLOCK 02. Save Product with ProductCode = "110" to ProductionHistory for testing purpose and test some reports.  
                var historyService = new ProductionHistoryService(
                    new ProductRepository(context),
                    new ProductionHistoryRepository(context)
                );

                var historyServiceResult = await historyService.AddProductionHistoryEntryAsync("110");

                if (historyServiceResult.Success) {
                    var saveDto = new AddProductionHistoryResponse {
                        Success = historyServiceResult.Success,
                        Message = historyServiceResult.Message
                        // Если сервис будет возвращать ProductionHistory через OperationResult<ProductionHistory>,
                        // то можно заполнить CreatedAt, Price, WorkCost и т.д.
                    };

                    Console.WriteLine(saveDto.Message);
                } else {
                    Console.WriteLine($"Failed to save production history: {historyServiceResult.Message}");
                }
                Console.WriteLine();
                #endregion

                #region TESTBLOCK 03. Test total production value report.
                var total = await historyService.GetTotalProductionValueReportAsync();
                Console.WriteLine($"Total value of all manufactured products (based on recommended price): {total:C}");
                Console.WriteLine();
                #endregion

                #region TESTBLOCK 04. Test detailed production value report by specific product code.
                var report = await historyService.GetProductDetailedReportAsync(
                    "110",
                    startDate: new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    endDate: new DateTime(2025, 1, 31, 0, 0, 0, DateTimeKind.Utc)
                );

                if (report is null) {
                    Console.WriteLine("No data for the specified product in the selected period.");
                } else {
                    var dto = report.ToResponse(); // Domain -> DTO via extension method

                    Console.WriteLine($"Report for product {dto.ProductCode}");
                    Console.WriteLine($"Period: {dto.PeriodFrom.ToString("yyyy-MMMM-dd", CultureInfo.InvariantCulture)} " +
                        $"— {dto.PeriodTo.ToString("yyyy-MMMM-dd", CultureInfo.InvariantCulture)}");
                    Console.WriteLine($"Quantity: {dto.Count}");
                    Console.WriteLine($"Total value: {dto.TotalValue:C}");
                    Console.WriteLine($"Including work cost: {dto.WorkCost:C}");
                }
                #endregion
            }
        }
    }
}
