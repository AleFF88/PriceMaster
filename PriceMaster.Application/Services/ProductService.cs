using PriceMaster.Application.DTOs;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;

namespace PriceMaster.Application.Services {

    public class ProductService {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Creates a new product in the system based on the provided data.
        /// Includes storing basic product information and its bill of materials (BOM).
        /// </summary>
        /// <param name="dto">A data transfer object containing product parameters and a list of components.</param>
        /// <returns>Operation result with success flag and message.</returns>
        public async Task<ServiceResult> CreateProductAsync(CreateProductDto dto) {
            // Check for duplicates (business rule)
            if (await _productRepository.Exists(dto.ProductCode)) {
                return new ServiceResult {
                    IsSuccess = false,
                    Message = $"Product with code {dto.ProductCode} already exists."
                };
            }

            // Mapping DTO -> Entity
            var product = new Product {
                ProductCode = dto.ProductCode,
                SeriesId = dto.SeriesId,
                SizeWidth = dto.SizeWidth,
                SizeHeight = dto.SizeHeight,
                RecommendedPrice = dto.RecommendedPrice,
                CreatedAt = DateTime.UtcNow,
                BomItems = dto.BomItems.Select(b => new BomItem {
                    ComponentId = b.ComponentId,
                    Quantity = b.Quantity
                }).ToList()
            };

            await _productRepository.AddAsync(product);


            return new ServiceResult {
                IsSuccess = true,
                Message = string.Empty
            };
        }
    }
}