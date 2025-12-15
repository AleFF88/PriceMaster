using PriceMaster.Application.DTOs;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;

namespace PriceMaster.Application.Commands {
    public class ProductService {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        public async Task<ProductResponse> CreateProduct(Product product) {
            if (await _productRepository.Exists(product.ProductCode)) {
                return new ProductResponse {
                    Success = false,
                    Message = $"Product with code {product.ProductCode} already exists.",
                    ProductCode = product.ProductCode
                };
            }

            await _productRepository.Add(product);

            return new ProductResponse {
                Success = true,
                Message = $"Product {product.ProductCode} created successfully.",
                ProductCode = product.ProductCode
            };
        }
    }
}
