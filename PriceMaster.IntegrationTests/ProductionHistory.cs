using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Commands;
using PriceMaster.Infrastructure.Repositories;
using PriceMaster.IntegrationTests.Common;
using static PriceMaster.IntegrationTests.Common.IntegrationTestHelper;
using static PriceMaster.IntegrationTests.Common.TestDataFactory;

namespace PriceMaster.IntegrationTests {
    [TestClass]
    public sealed class ProductionHistory : IntegrationTestBase {
        private ProductionHistoryService _historyService = null!;

        [TestInitialize]
        public void TestInit() {
            _historyService = new ProductionHistoryService(new ProductRepository(Context), new ProductionHistoryRepository(Context));
        }

        /// <summary>
        /// Ensures that a production history entry can be successfully added for an existing product.
        /// Verifies that the service correctly retrieves product details and records the production 
        /// event with the proper price.
        /// </summary>
        [TestMethod]
        public async Task CreateProductHistory_WithCode110_ShouldSucceed() {
            // 1. Arrange. Create the initial state directly through the context
            await SeedProduct110Async(Context); // Save the initial product
            ClearChangeTracker(Context);        // Reset the tracker state.
            var expectedRecommendedPrice = 2300;

            // 2. Act
            var historyInDb = await _historyService.AddProductionHistoryEntryAsync("110");

            // 3. Assert
            Assert.IsTrue(historyInDb.Success);
            Assert.AreNotEqual("Product 110 not found.", historyInDb.Message);

            ClearChangeTracker(Context);       // Reset the tracker state.
            var savedHistory = await Context.ProductionHistories
                .FirstOrDefaultAsync(h => h.Product!.ProductCode == "110");
            Assert.IsNotNull(savedHistory, "Production history record should be saved in DB.");
            Assert.AreEqual(expectedRecommendedPrice, savedHistory.RecommendedPrice, "Recommended price should match product data.");
        }

        /// <summary>
        /// Verifies the total production value report (Total Value).
        /// Confirms that the service correctly aggregates the RecommendedPrice of all manufactured products 
        /// to provide an accurate total financial summary.
        /// </summary>
        [TestMethod]
        public async Task GetTotalProductionValueReport_WithMultipleEntries_ShouldReturnCorrectSum() {
            // 1. Arrange
            await SeedProduct110Async(Context); // Save the initial product
            await _historyService.AddProductionHistoryEntryAsync("110");    // Add two records to the production history
            await _historyService.AddProductionHistoryEntryAsync("110");
            decimal expectedValue = 4600;
            ClearChangeTracker(Context); // Reset the tracker state.

            // 2. Act
            var totalValue = await _historyService.GetTotalProductionValueReportAsync();

            // 3. Assert
            Assert.AreEqual(expectedValue, totalValue, "Total production value should be the sum of all entries.");
        }

        /// <summary>
        /// Validates the filtering logic of the detailed production report.
        /// Ensures that only production entries within the specified date range are included 
        /// in the calculations (Count, TotalValue, WorkCost), while entries outside the range are ignored.
        /// </summary>
        [TestMethod]
        public async Task GetProductDetailedReport_WithThreeEntries_ShouldFilterCorrectly() {
            // 1. Arrange
            await SeedProduct110Async(Context); // Save the initial product

            // Create 3 production history entries via the service 
            await _historyService.AddProductionHistoryEntryAsync("110");
            await _historyService.AddProductionHistoryEntryAsync("110");
            await _historyService.AddProductionHistoryEntryAsync("110");

            // Retrieve all created entries from the database as a list of domain entities
            var entries = await Context.ProductionHistories.ToListAsync();

            // Set dates for entries to test the date range filter logic
            // Distribute dates: two inside the range, one outside
            entries[0].CreatedAt = new DateTime(2024, 01, 10, 0, 0, 0, DateTimeKind.Utc);
            entries[1].CreatedAt = new DateTime(2023, 01, 10, 0, 0, 0, DateTimeKind.Utc);
            entries[2].CreatedAt = new DateTime(2022, 01, 10, 0, 0, 0, DateTimeKind.Utc);

            await Context.SaveChangesAsync();   // Persist date changes to the database
            ClearChangeTracker(Context);        // Reset the tracker state.

            // Define the report period parameters
            var startDate = new DateTime(2023, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var endDate = new DateTime(2025, 01, 31, 0, 0, 0, DateTimeKind.Utc);

            // Define expected values for Assertions based on the entries within the range
            var expectedProductCode = "110";
            int expectedCount = 2;
            var expectedTotalValue = entries.First().RecommendedPrice * expectedCount;
            var expectedWorkCostValue = entries.First().WorkCost * expectedCount;

            // 2. Act
            // Call the service method to generate the detailed report for product "110"
            var report = await _historyService.GetProductDetailedReportAsync("110", startDate, endDate);

            // 3. Assert
            Assert.IsNotNull(report, "The report should be generated and not be null.");
            Assert.AreEqual(expectedProductCode, report.ProductCode, "The report must correspond to the requested product code.");
            Assert.AreEqual(expectedCount, report.Count, $"The report should only include {expectedCount} entries that fall within the specified date range.");
            Assert.AreEqual(expectedTotalValue, report.TotalValue, $"The total value should be the sum of RecommendedPrice for the {expectedCount} valid entries.");
            Assert.AreEqual(expectedWorkCostValue, report.WorkCost, $"The work cost should be the sum of WorkCost for the {expectedCount} valid entries.");
        }
    }
}
