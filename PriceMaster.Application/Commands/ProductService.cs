using PriceMaster.Domain.Common;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;

namespace PriceMaster.Application.Commands {
    public class ProductService {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        /// <summary>
        /// Creates a new product in the repository.
        /// Validates uniqueness of product code and saves the domain entity.
        /// </summary>
        /// <param name="product">Domain product entity to create.</param>
        /// <returns>Operation result with success flag and message.</returns>
        public async Task<OperationResult> CreateProduct(Product product) {
            if (await _productRepository.Exists(product.ProductCode)) {
                return OperationResult.Fail($"Product with code {product.ProductCode} already exists.");
            }

            product.CreatedAt = DateTime.UtcNow;

            await _productRepository.Add(product);

            return OperationResult.Ok($"Product {product.ProductCode} created successfully.");
        }
    }
}
