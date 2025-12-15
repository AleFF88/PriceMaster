using PriceMaster.Application.DTOs;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;

namespace PriceMaster.Application.Commands {
    public class ProductService {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Creates a new product based on the provided request DTO.
        /// Validates that the product code does not already exist, constructs the product entity
        /// including BOM items, and saves it to the repository.
        /// </summary>
        /// <param name="dto">The request containing product details and BOM items.</param>
        /// <returns>
        /// A ProductResponse indicating success or failure, including a message and the product code.
        /// </returns>
        public async Task<ProductResponse> CreateProduct(CreateProductRequest dto) {
            if (await _productRepository.Exists(dto.ProductCode)) {
                return new ProductResponse {
                    Success = false,
                    Message = $"Product with code {dto.ProductCode} already exists.",
                    ProductCode = dto.ProductCode
                };
            }

            var product = new Product {
                ProductCode = dto.ProductCode,
                SeriesId = dto.SeriesId,
                SizeWidth = dto.SizeWidth,
                SizeHeight = dto.SizeHeight,
                RecommendedPrice = dto.RecommendedPrice,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in dto.BOMItems) {
                product.BOMItems.Add(new BOMItem {
                    ComponentId = item.ComponentId,
                    Quantity = item.Quantity
                });
            }

            await _productRepository.Add(product);

            return new ProductResponse {
                Success = true,
                Message = $"Product {dto.ProductCode} created successfully.",
                ProductCode = dto.ProductCode
            };
        }
    }
}
