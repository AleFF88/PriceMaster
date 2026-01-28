using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Services;
using PriceMaster.Infrastructure.Repositories;

namespace PriceMaster.IntegrationTests {

    [TestClass]
    public class ProductTests : IntegrationTestBase {
        private ProductService _productService = null!;

        [TestInitialize]
        public void TestInit() {
            // Initialize the service before each test
            var repository = new ProductRepository(Context);
            _productService = new ProductService(repository);
        }

        /// <summary>
        /// Validates that a new product with valid data (Code 110) is successfully created.
        /// Verifies that the service processes the request without errors and persists the entity 
        /// to the database with BOM (Bill of materials).
        /// </summary>
        [TestMethod]
        public async Task CreateProduct_FullComplexBOM_ShouldSaveCorrectly() {
            // 1. Arrange 
            var dto = TestDataFactory.CreateProduct110Request();
            var expectedProductCode = dto.ProductCode;
            var expectedSizeWidth = dto.SizeWidth;
            var expectedSizeHeight = dto.SizeHeight;
            var expectedBomCount = dto.BomItems.Count;

            // 2. Act
            await _productService.CreateProductAsync(dto);

            // 3. Assert 
            IntegrationTestHelper.ClearChangeTracker(Context);       // Reset the tracker state.
            var productInDb = await Context.Products
                .AsNoTracking()
                .Include(p => p.BomItems)
                    .ThenInclude(b => b.Component)
                .FirstOrDefaultAsync(p => p.ProductCode == expectedProductCode);

            Assert.IsNotNull(productInDb, $"Product with code {expectedProductCode} must be in the database.");
            Assert.AreEqual(expectedSizeWidth, productInDb.SizeWidth);
            Assert.AreEqual(expectedSizeHeight, productInDb.SizeHeight);
            Assert.AreEqual(dto.RecommendedPrice, productInDb.RecommendedPrice);
            Assert.AreEqual(expectedBomCount, productInDb.BomItems.Count, $"The number of positions in the BOM must be {expectedBomCount}.");
            Assert.IsTrue(productInDb.BomItems.All(b => b.Component != null), "All components must be loaded.");
            Assert.IsTrue(productInDb.BomItems.Any(b => b.Component!.PricePerUnit> 0), "Components must have prices from seed data.");
        }

        /// <summary>
        /// Verifies that the system prevents creating a product with a code that already exists.
        /// The ProductCode must be unique across the entire database to ensure data integrity.
        /// </summary>
        [TestMethod]
        public async Task CreateProduct_DuplicateCode_ShouldThrowException() {
            // 1. Arrange
            // Create the initial product to occupy the code in the database
            var dto = TestDataFactory.CreateProduct110Request();
            await _productService.CreateProductAsync(dto);

            // 2. Act
            // Define the action that is expected to fail
            Func<Task> action = () => _productService.CreateProductAsync(dto);

            // 3. Assert
            // Verify that attempting to create a product with the same code 
            // throws an InvalidOperationException as defined in the business logic.
            IntegrationTestHelper.ClearChangeTracker(Context);       // Reset the tracker state.
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(action,
                "An exception should be thrown when attempting to create a duplicate product code.");
        }
    }
}