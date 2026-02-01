using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.DTOs;
using PriceMaster.Application.Services;
using PriceMaster.Infrastructure.Repositories;
using static PriceMaster.IntegrationTests.IntegrationTestHelper;

namespace PriceMaster.IntegrationTests {
    /// <summary>
    /// Integration tests for the production history logic and reporting.
    /// These tests verify that production events are recorded with correct prices 
    /// and that reports aggregate data accurately over time.
    /// </summary>
    [TestClass]
    public sealed class ProductionHistoryTests : IntegrationTestBase {
        private ProductionHistoryService _historyService = null!;
        private ProductService _productService = null!;

        [TestInitialize]
        public void TestInit() {
            // Initializing repositories and service for each test
            var productRepo = new ProductRepository(Context);
            var historyRepo = new ProductionHistoryRepository(Context);
            var historyQueries = new ProductionHistoryQueries(Context);

            _historyService = new ProductionHistoryService(productRepo, historyRepo, historyQueries);
            _productService = new ProductService(productRepo);

        }

        /// <summary>
        /// Summary: Verifies that the detailed report correctly aggregates data and filters records by date range.
        /// Description: Adds multiple production records (including boundaries and out-of-range entries) 
        /// to ensure that the report sums only valid entries and that data integrity is maintained in the database.
        /// </summary>
        [TestMethod]
        public async Task GetProductDetailedReport_DateFiltering_ShouldReturnExpectedRecordsAndValues() {
            // 1. Arrange
            // Seed the base product '110' into the in-memory database
            var dto = TestDataFactory.CreateProduct110Request();
            await _productService.CreateProductAsync(dto);

            var targetProductCode = dto.ProductCode;
            var startDate = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var endDate = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            var expectedCountTotal = 5;
            var expectedCountInRange = 3;
            var expectedTotalValue = dto.RecommendedPrice * expectedCountInRange;

            // Add 3 records within range
            await _historyService.AddProductionHistoryEntryAsync(
                new ProductionHistoryCreateRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = startDate,
                    Notes = "In-Range"
                }
            );
            await _historyService.AddProductionHistoryEntryAsync(
                new ProductionHistoryCreateRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = new DateTime(2024, 06, 01, 12, 0, 0, DateTimeKind.Utc),
                    Notes = "In-Range"
                }
            );

            await _historyService.AddProductionHistoryEntryAsync(
                new ProductionHistoryCreateRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = endDate,
                    Notes = "In-Range"
                }
            );

            // Add 2 records outside range 
            await _historyService.AddProductionHistoryEntryAsync(
                new ProductionHistoryCreateRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = startDate.AddSeconds(-1),
                    Notes = "Out-of-range"
                }
            );

            await _historyService.AddProductionHistoryEntryAsync(
                new ProductionHistoryCreateRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = endDate.AddSeconds(1),
                    Notes = "Out-of-range"
                }
            );

            // Ensure the context is fresh and reads from the DB, not from memory cache
            ClearChangeTracker(Context);

            // 2. Act
            var report = await _historyService.GetProductDetailedReportAsync(targetProductCode, startDate, endDate);

            // 3. Assert
            // Part A: Verify the Report DTO (Business Logic)
            Assert.IsNotNull(report, "The generated report should not be null.");
            Assert.AreEqual(targetProductCode, report.ProductCode, "Product code in report mismatch.");
            Assert.AreEqual(expectedCountInRange, report.Count, "The report failed to filter records by date range correctly.");
            Assert.AreEqual(expectedTotalValue, report.TotalValue, "The total sum in the report is incorrect.");

            // Part B: Verify Database State (Data Integrity)
            // We fetch the product to get its ID for direct database querying
            var productInDb = await Context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == targetProductCode);

            Assert.IsNotNull(productInDb, "The product must exist in the database.");

            var entriesInDb = await Context.ProductionHistories
                .Where(h => h.ProductId == productInDb.ProductId)
                .ToListAsync();

            // Verify total persistence
            Assert.AreEqual(expectedCountTotal, entriesInDb.Count, $"Total record count in DB should be {expectedCountTotal}.");

            // Verify date filtering logic
            var filteredInDb = entriesInDb
                .Where(h => h.CreatedAt >= startDate && h.CreatedAt <= endDate)
                .ToList();

            Assert.AreEqual(expectedCountInRange, filteredInDb.Count, $"There must be exactly {expectedCountInRange} entries within the range.");

            // Verify that the correct entries were picked by checking their notes
            Assert.IsTrue(filteredInDb.All(e => e.Notes != null && e.Notes.Contains("In-Range")),
                "All records within a period must be labeled 'In-Range'.");

            Assert.IsFalse(filteredInDb.Any(e => e.Notes != null && e.Notes.Contains("Out-of-range")),
                "'Out-of-range' records should not be included in the sample for the period.");
        }

        /// <summary>
        /// Verifies that the system handles requests for periods with no data gracefully.
        /// To run this test, the product must exist in the database, but have no associated 
        ///    history records within the specified date range.
        /// </summary>
        [TestMethod]
        public async Task GetProductDetailedReport_WithNoEntriesInRange_ShouldReturnNull() {
            // 1. Arrange
            // Ensure the product exists so the query doesn't fail on "Product Not Found" logic
            var dto = TestDataFactory.CreateProduct110Request();
            await _productService.CreateProductAsync(dto);

            // Define a date range in the past where we definitely have no data
            var startDate = DateTime.UtcNow.AddYears(-10);
            var endDate = DateTime.UtcNow.AddYears(-9);
            var productCode = dto.ProductCode;

            // 2. Act
            var report = await _historyService.GetProductDetailedReportAsync(productCode, startDate, endDate);

            // 3. Assert
            // If the query returns null for empty results, we verify that here
            Assert.IsNull(report, $"Report for product {productCode} should be null for a historical range with no data.");
        }

        /// <summary>
        /// Verifies that a history record captures and persists the correct 
        ///    product price at the moment of creation.
        /// This is crucial for financial integrity, ensuring historical reports 
        ///    remain accurate even if product prices change later.
        /// </summary>
        [TestMethod]
        public async Task AddProductionHistoryEntry_ShouldPersistCurrentProductPrice() {
            // 1. Arrange
            // Seed the base product '110' into the in-memory database
            var dto = TestDataFactory.CreateProduct110Request();
            await _productService.CreateProductAsync(dto);
            
            var expectedPrice = dto.RecommendedPrice;
            var expectedNote = "Snapshot Price Verification";


            var request = new ProductionHistoryCreateRequest {
                ProductCode = dto.ProductCode,
                ProductionDate = DateTime.UtcNow,
                Notes = expectedNote
            };

            // 2. Act
            // Using the service which now returns a ServiceResult
            var result = await _historyService.AddProductionHistoryEntryAsync(request);
            IntegrationTestHelper.ClearChangeTracker(Context);

            // 3. Assert
            Assert.IsTrue(result.IsSuccess, $"Service failed to create entry: {result.Message}");

            // Fetch the created record directly from the DB to check the persisted price
            var persistedEntry = await Context.ProductionHistories
                .FirstOrDefaultAsync(h => h.Notes != null && h.Notes.Contains(expectedNote));

            Assert.IsNotNull(persistedEntry, "The history entry was not found in the database.");
            Assert.AreEqual(expectedPrice, persistedEntry.RecommendedPrice,
                "The history entry failed to capture the product's recommended price at the moment of creation.");
        }


    }
}