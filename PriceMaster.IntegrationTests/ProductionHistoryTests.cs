using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Requests;
using PriceMaster.Application.Services;
using PriceMaster.Application.Validators;
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

            // Validator instance
            var productValidator = new CreateProductRequestValidator();
            var historyValidator = new CreateProductionHistoryRequestValidator();

            _productService = new ProductService(productRepo, productValidator);
            _historyService = new ProductionHistoryService(productRepo, historyRepo, historyQueries, historyValidator);

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
                new CreateProductionHistoryRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = startDate,
                    Notes = "In-Range"
                }
            );
            await _historyService.AddProductionHistoryEntryAsync(
                new CreateProductionHistoryRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = new DateTime(2024, 06, 01, 12, 0, 0, DateTimeKind.Utc),
                    Notes = "In-Range"
                }
            );

            await _historyService.AddProductionHistoryEntryAsync(
                new CreateProductionHistoryRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = endDate,
                    Notes = "In-Range"
                }
            );

            // Add 2 records outside range 
            await _historyService.AddProductionHistoryEntryAsync(
                new CreateProductionHistoryRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = startDate.AddSeconds(-1),
                    Notes = "Out-of-range"
                }
            );

            await _historyService.AddProductionHistoryEntryAsync(
                new CreateProductionHistoryRequest {
                    ProductCode = targetProductCode,
                    ProductionDate = endDate.AddSeconds(1),
                    Notes = "Out-of-range"
                }
            );

            // Ensure the context is fresh and reads from the DB, not from memory cache
            ClearChangeTracker(Context);

            // 2. Act
            var request = new GetProductDetailedReportRequest {
                ProductCode = targetProductCode,
                StartDate = startDate,
                EndDate = endDate
            };

            var report = await _historyService.GetProductDetailedReportAsync(request);

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
        ///   history records within the specified date range.
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
            var request = new GetProductDetailedReportRequest {
                ProductCode = productCode,
                StartDate = startDate,
                EndDate = endDate
            };

            var report = await _historyService.GetProductDetailedReportAsync(request);

            // 3. Assert
            // If the query returns null for empty results, we verify that here
            Assert.IsNull(report, $"Report for product {productCode} should be null for a historical range with no data.");
        }

        /// <summary>
        /// Verifies that a history record captures and persists the correct 
        ///   product price at the moment of creation.
        /// This is crucial for financial integrity, ensuring historical reports 
        ///   remain accurate even if product prices change later.
        /// </summary>
        [TestMethod]
        public async Task AddProductionHistoryEntry_ShouldPersistCurrentProductPrice() {
            // 1. Arrange
            // Seed the base product '110' into the in-memory database
            var dto = TestDataFactory.CreateProduct110Request();
            await _productService.CreateProductAsync(dto);

            var expectedPrice = dto.RecommendedPrice;
            var expectedNote = "Snapshot Price Verification";

            var request = new CreateProductionHistoryRequest {
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

        /// <summary>
        /// Checks that the price in the production history remains unchanged ("frozen"),
        ///   even if the product's current recommended price has been updated.
        /// </summary>
        [TestMethod]
        public async Task AddProductionHistoryEntry_ShouldFreezePrice_WhenProductPriceChangesLater() {
            // 1. Arrange
            // Create a product with an initial price 
            var dto = TestDataFactory.CreateProduct110Request();
            var initialPrice = dto.RecommendedPrice;
            var updatedPrice = dto.RecommendedPrice + 500;
            await _productService.CreateProductAsync(dto);

            // Create a production history record to "freeze" the current price
            var historyRequest = new CreateProductionHistoryRequest {
                ProductCode = dto.ProductCode,
                ProductionDate = DateTime.UtcNow,
                Notes = "Initial snapshot"
            };
            await _historyService.AddProductionHistoryEntryAsync(historyRequest);

            // 2. Act
            // Simulate a price update for the product in the catalog (e.g., due to inflation)
            var productInDb = await Context.Products
                .FirstOrDefaultAsync(p => p.ProductCode == dto.ProductCode);

            Assert.IsNotNull(productInDb);
            productInDb.RecommendedPrice = updatedPrice;
            await Context.SaveChangesAsync();

            // Clear the EF Core change tracker to ensure we fetch fresh data from the database
            ClearChangeTracker(Context);

            // 3. Assert
            // Retrieve the history entry and check if the price remained unchanged
            var historyEntry = await Context.ProductionHistories
                .FirstOrDefaultAsync(h => h.ProductId == productInDb.ProductId);

            Assert.IsNotNull(historyEntry, "History record should exist.");

            // CORE CHECK: The price in history must remain at the initial level, not the updated one 
            Assert.AreEqual(initialPrice, historyEntry.RecommendedPrice, "The price in history must remain unchanged (frozen) after the product catalog price update.");

            // Verify that the actual product price WAS updated
            var updatedProduct = await Context.Products.FindAsync(productInDb.ProductId);
            Assert.AreEqual(updatedPrice, updatedProduct!.RecommendedPrice, "The current product price in the catalog should reflect the new updated value.");
        }

        /// <summary>
        /// Verifies that the production history remains consistent even if the 
        /// product's Bill of Materials (BOM) is modified later.
        /// </summary>
        [TestMethod]
        public async Task AddProductionHistoryEntry_ShouldMaintainDataIntegrity_WhenProductBomIsModified() {
            // 1. Arrange
            // Create a product with a standard BOM (defined in TestDataFactory)
            var dto = TestDataFactory.CreateProduct110Request();
            await _productService.CreateProductAsync(dto);

            // Record production history. 
            // This captures the state of the product at this point in time.
            var historyRequest = new CreateProductionHistoryRequest {
                ProductCode = dto.ProductCode,
                ProductionDate = DateTime.UtcNow,
                Notes = "Original BOM snapshot"
            };
            await _historyService.AddProductionHistoryEntryAsync(historyRequest);

            // 2. Act
            // Simulate a BOM change: Retrieve the product and remove all its components
            var productInDb = await Context.Products
                .Include(p => p.BomItems)
                .FirstOrDefaultAsync(p => p.ProductCode == dto.ProductCode);

            Assert.IsNotNull(productInDb, "Product should be found in the database.");

            // Clear the current BOM items to simulate a major product redesign
            productInDb.BomItems.Clear();
            await Context.SaveChangesAsync();

            // Clear tracker to ensure fresh data fetch
            ClearChangeTracker(Context);

            // 3. Assert
            // Fetch the history record again
            var historyEntry = await Context.ProductionHistories
                .FirstOrDefaultAsync(h => h.ProductId == productInDb.ProductId);

            Assert.IsNotNull(historyEntry, "Historical record should still exist.");

            // Verify that the product's current BOM is indeed empty
            var updatedProduct = await Context.Products
                .Include(p => p.BomItems)
                .FirstOrDefaultAsync(p => p.ProductId == productInDb.ProductId);

            Assert.AreEqual(0, updatedProduct!.BomItems.Count,
                "The current product's BOM should be updated and empty.");

            // Verify history integrity
            // Note: In your current architecture, history links to ProductId.
            // This test confirms that the history record itself is not deleted 
            // and its 'PriceAtCreation' remains intact regardless of BOM changes.
            Assert.AreEqual(dto.RecommendedPrice, historyEntry.RecommendedPrice,
                "Historical price snapshot must remain intact even if the current product BOM is destroyed.");
        }

        /// <summary>
        /// Ensures that the system prevents deleting a component if it is still 
        ///   referenced in any product's Bill of Materials (BOM).
        /// </summary>
        [TestMethod]
        public async Task DeleteComponent_ShouldThrowException_WhenComponentIsReferencedInBom() {
            // 1. Arrange
            // Create a product with a BOM (this creates component-product links)
            var productDto = TestDataFactory.CreateProduct110Request();
            await _productService.CreateProductAsync(productDto);

            var componentIdInUse = productDto.BomItems.First().ComponentId;

            // 2. Act & Assert
            var component = await Context.Components.FindAsync(componentIdInUse);
            Assert.IsNotNull(component, "Component should exist in the database.");

            // EF Core throws InvalidOperationException during Remove() when it detects 
            // that a required relationship will be severed.
            Assert.ThrowsException<InvalidOperationException>(() =>
            {
                Context.Components.Remove(component);
            }, "Should not be able to mark for deletion a component that is linked to a BOM.");
        }

        /// <summary>
        /// Ensures that the detailed report for a specific product does not include 
        /// production data from other products present in the database.
        /// </summary>
        [TestMethod]
        public async Task GetProductDetailedReport_ShouldNotIncludeData_FromOtherProducts() {
            // 1. Arrange
            // Create two different products
            var product110Dto = TestDataFactory.CreateProduct110Request();
            var product150Dto = TestDataFactory.CreateProduct150Request();

            await _productService.CreateProductAsync(product110Dto);
            await _productService.CreateProductAsync(product150Dto);

            // Add production record for Product 110
            await _historyService.AddProductionHistoryEntryAsync(new CreateProductionHistoryRequest {
                ProductCode = product110Dto.ProductCode,
                ProductionDate = DateTime.UtcNow,
                Notes = "Entry for N110"
            });

            // Add production record for Product 150
            await _historyService.AddProductionHistoryEntryAsync(new CreateProductionHistoryRequest {
                ProductCode = product150Dto.ProductCode,
                ProductionDate = DateTime.UtcNow,
                Notes = "Entry for N150"
            });

            // Clear tracker to ensure fresh DB query
            ClearChangeTracker(Context);

            // 2. Act
            // Request report specifically for Product 110
            var request = new GetProductDetailedReportRequest {
                ProductCode = product110Dto.ProductCode
            };

            var report = await _historyService.GetProductDetailedReportAsync(request);

            // 3. Assert
            Assert.IsNotNull(report, "Report should be generated.");
            Assert.AreEqual(product110Dto.ProductCode, report.ProductCode, "Report must belong to the requested product.");

            // The crucial check: count must be 1, because product 150 must be filtered out.
            Assert.AreEqual(1, report.Count, "The report should only count records for the specific product code, excluding others.");
        }
    }
}