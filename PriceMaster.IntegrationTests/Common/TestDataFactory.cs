using PriceMaster.Contracts.DTOs.Products;
using PriceMaster.Contracts.Mappers;
using PriceMaster.Infrastructure;

namespace PriceMaster.IntegrationTests.Common {
    /// <summary>
    /// Test data factory for simulating incoming Payload from API.
    /// </summary>
    internal static class TestDataFactory {

        /// <summary>
        /// Simulate Payload for creating a product with ID = 110.
        /// </summary>
        internal static CreateProductRequest CreateProduct110Request() {
            return new CreateProductRequest {
                ProductCode = "110",
                SeriesId = 1,
                SizeWidth = 60,
                SizeHeight = 30,
                RecommendedPrice = 2300,
                BOMItems = new List<BOMItemDto> {
                    new BOMItemDto { ComponentId = 1, Quantity = 4 },         // Button
                    new BOMItemDto { ComponentId = 2, Quantity = 4 },         // Ring
                    new BOMItemDto { ComponentId = 4, Quantity = 8 },         // Harness Component (simple) 
                    new BOMItemDto { ComponentId = 53, Quantity = 4 },        // Silver coin (copy)
                    new BOMItemDto { ComponentId = 90, Quantity = 3 },        // Spherical ball
                    new BOMItemDto { ComponentId = 110, Quantity = 1 },       // Inkwell
                    new BOMItemDto { ComponentId = 500, Quantity = 1 },       // Baguette 60*30 Verona
                    new BOMItemDto { ComponentId = 900, Quantity = 1 },       // Printout 110
                    new BOMItemDto { ComponentId = 950, Quantity = 1 },       // Hardware
                    new BOMItemDto { ComponentId = 961, Quantity = 0.18m },   // Textile, per sq.m.
                    new BOMItemDto { ComponentId = 962, Quantity = 0.18m },   // Polyurethane, per sq.m.
                    new BOMItemDto { ComponentId = 1000, Quantity = 1 }       // Work
                }
            };
        }

        /// <summary>
        /// Seeds the database with a default product (Code 110) to create an initial state for integration tests.
        /// This bypasses the service layer to ensure a clean, predictable state directly in the database.
        /// </summary>
        /// <param name="context">The database context instance to use for seeding.</param>
        public static async Task SeedProduct110Async(PriceMasterDbContext context) {
            var request = CreateProduct110Request();    // Incoming payload
            var domain = request.ToDomain();            // Mapping to domain entity
            context.Products.Add(domain);               // Save the initial product
            await context.SaveChangesAsync();
        }
    }
}
