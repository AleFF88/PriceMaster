using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Commands;
using PriceMaster.Contracts.Mappers;
using PriceMaster.Infrastructure.Repositories;
using PriceMaster.IntegrationTests.Common;
using static PriceMaster.IntegrationTests.Common.IntegrationTestHelper;
using static PriceMaster.IntegrationTests.Common.TestDataFactory;

namespace PriceMaster.IntegrationTests {
    [TestClass]
    public sealed class ProductTests : IntegrationTestBase {
        private ProductService _productService = null!;

        [TestInitialize]
        public void TestInit() {
            _productService = new ProductService(new ProductRepository(Context));
        }

        /// <summary>
        /// Validates that a new product with valid data (Code 110) is successfully created.
        /// Verifies that the service processes the request without errors and persists the entity 
        /// to the database with BOM (Bill of materials).
        /// </summary>
        [TestMethod]
        public async Task CreateProduct_WithCode110_ShouldSucceed() {
            // 1. Arrange
            var request = CreateProduct110Request();    // Incoming payload
            var domain = request.ToDomain();            // Mapping to domain entity
            var expectedBomCount = request.BomItems.Count;
            var expectedProductCode = request.ProductCode;

            // 2. Act
            var result = await _productService.CreateProduct(domain);   // Create the product through the service

            // 3. Assert
            Assert.IsTrue(result.Success, "Product creation should succeed.");
            
            // Checking the presence of a record in the database with all BomItems
            ClearChangeTracker(Context);       // Reset the tracker state.
            var productInDb = await Context.Products
                .Include(p => p.BomItems)
                .FirstOrDefaultAsync(p => p.ProductCode == "110");

            Assert.IsNotNull(productInDb, "Product should exist in database.");
            Assert.AreEqual(expectedProductCode, productInDb.ProductCode);
            Assert.AreEqual(expectedBomCount, productInDb.BomItems.Count, "All BOM (Bill of materials) items should be saved.");
        }

        /// <summary>
        /// Verifies that the system prevents creating a product with a code that already exists.
        /// The ProductCode must be unique across the entire database to ensure data integrity.
        /// </summary>
        [TestMethod]
        public async Task CreateProduct_DuplicateCode110_ShouldFail() {
            // 1. Arrange. Create the initial state directly through the contexts
            await SeedProduct110Async(Context); // Save the initial product
            ClearChangeTracker(Context);        // Reset the tracker state.
            var expectedResultMessage = "Product with code 110 already exists.";

            // 2. Act. Attempt to create a duplicate product
            var duplicateRequest = CreateProduct110Request();                   // Duplicate incoming payload 
            var duplicateDomain = duplicateRequest.ToDomain();                  // Mapping to domain entity 
            var result = await _productService.CreateProduct(duplicateDomain);  // Duplicate creation attempt

            // 3. Assert
            Assert.IsFalse(result.Success, "Duplicate product creation should fail.");
            Assert.AreEqual(expectedResultMessage, result.Message);
        }
    }
}
