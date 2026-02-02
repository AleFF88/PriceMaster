using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Services;
using PriceMaster.Application.Validators;
using PriceMaster.Infrastructure.Repositories;

namespace PriceMaster.IntegrationTests {

    [TestClass]
    public class ProductTests : IntegrationTestBase {
        private ProductService _productService = null!;

        [TestInitialize]
        public void TestInit() {
            // Initialize the service before each test
            var repository = new ProductRepository(Context);


            // Validator instance
            var validator = new CreateProductDtoValidator();

            _productService = new ProductService(repository, validator);
        }

        /// <summary>
        /// Validates that a new product with a complex Bill of Materials (BOM) is successfully created.
        /// Verifies that data is saved correctly, including sizes, prices, and component links.
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
            Assert.AreEqual(expectedBomCount, productInDb.BomItems.Count, $"BOM should contain exactly {expectedBomCount} items.");
            Assert.IsTrue(productInDb.BomItems.All(b => b.Component != null), "All BOM items must be linked to their respective components.");
            Assert.IsTrue(productInDb.BomItems.Any(b => b.Component!.PricePerUnit > 0), "Components must retain their prices from the seed data.");
        }

        /// <summary>
        /// Verifies that the system prevents creating a product with a code that already exists.
        /// Ensures the ProductCode uniqueness constraint is handled gracefully by the service.
        /// </summary>
        [TestMethod]
        public async Task CreateProduct_DuplicateCode_ShouldThrowException() {
            // 1. Arrange
            // Seed the initial product to occupy the ProductCode
            var dto = TestDataFactory.CreateProduct110Request();
            await _productService.CreateProductAsync(dto);

            var expectedErrorMessage = $"Product with code {dto.ProductCode} already exists.";
            var expectedNumberOfRecords = 1;

            // 2. Act
            // Attempt to create a product with the same code
            IntegrationTestHelper.ClearChangeTracker(Context);
            var result = await _productService.CreateProductAsync(dto);

            // 3. Assert
            // The result should indicate failure rather than throwing an unhandled DB exception
            Assert.IsFalse(result.IsSuccess, "The service should return a failure result when creating a duplicate ProductCode.");

            // Verify the exact error message returned to the user/API
            Assert.AreEqual(expectedErrorMessage, result.Message, "The error message for a duplicate ProductCode is incorrect.");

            // Verify that no duplicate record was actually added to the database
            var countInDb = await Context.Products.CountAsync(p => p.ProductCode == dto.ProductCode);
            Assert.AreEqual(expectedNumberOfRecords, countInDb, "Database integrity check failed: duplicate product was persisted.");
        }
    }
}