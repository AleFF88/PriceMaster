using PriceMaster.Application.DTOs;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;

namespace PriceMaster.Application.Services {

    public class ProductService {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        public async Task CreateProductAsync(CreateProductDto dto) {
            // Check for duplicates (business rule)
            if (await _productRepository.Exists(dto.ProductCode))
                throw new InvalidOperationException($"Product with code {dto.ProductCode} already exists.");

            // Mapping DTO -> Entity
            var product = new Product {
                ProductCode = dto.ProductCode,
                SeriesId = dto.SeriesId,
                RecommendedPrice = dto.RecommendedPrice,
                CreatedAt = DateTime.UtcNow,
                BomItems = dto.BomItems.Select(b => new BomItem {
                    ComponentId = b.ComponentId,
                    Quantity = b.Quantity
                }).ToList()
            };

            await _productRepository.AddAsync(product);
        }
    }
}