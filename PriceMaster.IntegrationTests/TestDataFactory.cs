using PriceMaster.Application.Requests;

namespace PriceMaster.IntegrationTests {
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
                BomItems = new List<BomItemDto> {
                    new BomItemDto { ComponentId = 1,    Quantity = 4 },       // Button
                    new BomItemDto { ComponentId = 2,    Quantity = 4 },       // Ring
                    new BomItemDto { ComponentId = 4,    Quantity = 8 },       // Harness Component (simple) 
                    new BomItemDto { ComponentId = 53,   Quantity = 4 },       // Silver coin (copy)
                    new BomItemDto { ComponentId = 90,   Quantity = 3 },       // Spherical ball
                    new BomItemDto { ComponentId = 110,  Quantity = 1 },       // Inkwell
                    new BomItemDto { ComponentId = 500,  Quantity = 1 },       // Baguette 60*30 Verona
                    new BomItemDto { ComponentId = 900,  Quantity = 1 },       // Printout 110
                    new BomItemDto { ComponentId = 950,  Quantity = 1 },       // Hardware
                    new BomItemDto { ComponentId = 961,  Quantity = 0.18m },   // Textile, per sq.m.
                    new BomItemDto { ComponentId = 962,  Quantity = 0.18m },   // Polyurethane, per sq.m.
                    new BomItemDto { ComponentId = 1000, Quantity = 1 }        // Work
                }
            };
        }

        /// <summary>
        /// Simulate Payload for creating a product with ID = 150.
        /// </summary>
        internal static CreateProductRequest CreateProduct150Request() {
            return new CreateProductRequest {
                ProductCode = "150",
                SeriesId = 1,
                SizeWidth = 60,
                SizeHeight = 30,
                RecommendedPrice = 2300,
                BomItems = new List<BomItemDto> {
                    new BomItemDto { ComponentId = 1,    Quantity = 2 },       // Button
                    new BomItemDto { ComponentId = 2,    Quantity = 2 },       // Ring
                    new BomItemDto { ComponentId = 4,    Quantity = 6 },       // Harness Component (simple) 
                    new BomItemDto { ComponentId = 6,    Quantity = 2 },       // Handle 
                    new BomItemDto { ComponentId = 53,   Quantity = 4 },       // Silver coin (copy)
                    new BomItemDto { ComponentId = 90,   Quantity = 3 },       // Spherical ball
                    new BomItemDto { ComponentId = 110,  Quantity = 1 },       // Inkwell
                    new BomItemDto { ComponentId = 111,  Quantity = 1 },       // Tobacco pipe
                    new BomItemDto { ComponentId = 500,  Quantity = 1 },       // Baguette 60*30 Verona
                    new BomItemDto { ComponentId = 901,  Quantity = 1 },       // Printout 150
                    new BomItemDto { ComponentId = 950,  Quantity = 1 },       // Hardware
                    new BomItemDto { ComponentId = 961,  Quantity = 0.18m },   // Textile, per sq.m.
                    new BomItemDto { ComponentId = 962,  Quantity = 0.18m },   // Polyurethane, per sq.m.
                    new BomItemDto { ComponentId = 1000, Quantity = 1 }        // Work
                }
            };
        }
    }
}
