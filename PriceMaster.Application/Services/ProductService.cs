using PriceMaster.Application.Requests;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;
using FluentValidation;
using PriceMaster.Application.Models;


namespace PriceMaster.Application.Services {

    public class ProductService {
        private readonly IProductRepository _productRepository;
        private readonly IValidator<CreateProductRequest> _validator;

        public ProductService(IProductRepository productRepository, IValidator<CreateProductRequest> validator) {
            _productRepository = productRepository;
            _validator = validator;
        }

        /// <summary>
        /// Creates a new product in the system based on the provided data.
        /// Includes storing basic product information and its bill of materials (BOM).
        /// </summary>
        /// <param name="dto">A data transfer object containing product parameters and a list of components.</param>
        /// <returns>Operation result with success flag and message.</returns>
        public async Task<ServiceResult> CreateProductAsync(CreateProductRequest dto) {
            // Validation
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid) {
                var errors = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult.Failure(errors);
            }

            // Check for duplicates (business rule)
            if (await _productRepository.Exists(dto.ProductCode)) {
                return ServiceResult.Failure($"Product with code {dto.ProductCode} already exists.");
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

            try {
                await _productRepository.AddAsync(product);
                return ServiceResult.Success();
            }
            catch (Exception ex) {
                return ServiceResult.Failure($"An error occurred while creating the product: {ex.Message}");
            }
        }
    }
}