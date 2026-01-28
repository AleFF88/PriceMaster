using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.DTOs;
using PriceMaster.Application.Services;
using PriceMaster.Infrastructure.Repositories;

namespace PriceMaster.IntegrationTests {

    [TestClass]
    public class ProductTests : IntegrationTestBase {
        private ProductService _productService;

        [TestInitialize]
        public void TestInit() {
            // Initialize the service before each test
            var repository = new ProductRepository(Context);
            _productService = new ProductService(repository);
        }

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
            var productInDb = await Context.Products
                 .Include(p => p.BomItems)
                 .FirstOrDefaultAsync(p => p.ProductCode == expectedProductCode);

            Assert.IsNotNull(productInDb, $"Product with code {expectedProductCode} must be in the database.");
            Assert.AreEqual(expectedSizeWidth, productInDb.SizeWidth);
            Assert.AreEqual(expectedSizeHeight, productInDb.SizeHeight);
            Assert.AreEqual(dto.RecommendedPrice, productInDb.RecommendedPrice);
            Assert.AreEqual(expectedBomCount, productInDb.BomItems.Count, $"The number of positions in the BOM must be {expectedBomCount}.");
        }

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
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(action,
                "An exception should be thrown when attempting to create a duplicate product code.");
        }
    }
}