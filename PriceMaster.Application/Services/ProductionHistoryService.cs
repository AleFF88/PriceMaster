using FluentValidation;
using PriceMaster.Application.Models;
using PriceMaster.Application.Requests;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Domain.Reports;

namespace PriceMaster.Application.Services {

    public class ProductionHistoryService {
        private readonly IProductRepository _productRepository;
        private readonly IProductionHistoryRepository _historyRepository;
        private readonly IProductionHistoryQueries _historyQueries; 
        private readonly IValidator<CreateProductionHistoryRequest> _validator;

        public ProductionHistoryService(IProductRepository productRepository, IProductionHistoryRepository historyRepository, IProductionHistoryQueries historyQueries, IValidator<CreateProductionHistoryRequest> validator) {
            _productRepository = productRepository;
            _historyRepository = historyRepository;
            _historyQueries = historyQueries;
            _validator = validator;
        }

        /// <summary>
        /// Records a new production history entry for the specified product.
        /// Retrieves the product by its code with Bom (Bill of materials), performs necessary calculations,
        /// and saves the resulting data into the production history.
        /// </summary>
        /// <param name="productCode">Unique product code to identify the product.</param>
        /// <param name="notes">Optional notes to append to the history record.</param>
        /// <returns>Operation result with success flag and message.</returns>
        public async Task<ServiceResult> AddProductionHistoryEntryAsync(CreateProductionHistoryRequest request) {
            // Validation
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid) {
                var errors = string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage));
                return ServiceResult.Failure(errors);
            }

            try {
                var product = await _productRepository.GetByProductCodeWithBomAsync(request.ProductCode);

                if (product == null) {
                    return ServiceResult.Failure($"Product with code {request.ProductCode} not found.");
                }

                var totalPrice = Math.Ceiling(product.BomItems.Sum(i => i.Quantity * i.Component!.PricePerUnit));

                var workCost = Math.Ceiling(product.BomItems
                    .Where(i => i.Component!.CategoryId == 3)
                    .Sum(i => i.Quantity * i.Component!.PricePerUnit));

                var historyEntry = new ProductionHistory {
                    ProductId = product.ProductId,
                    CreatedAt = request.ProductionDate ?? DateTime.UtcNow,
                    Price = totalPrice,
                    RecommendedPrice = product.RecommendedPrice,
                    WorkCost = workCost,
                    Notes = $"{request.Notes ?? ""} {product.Notes}".Trim()
                };

                await _historyRepository.AddAsync(historyEntry);
                return ServiceResult.Success();
            }
            catch (Exception ex) {
                return ServiceResult.Failure($"Internal error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculates the total value of all manufactured products
        /// based on their recommended prices stored in production history.
        /// </summary>
        /// <returns>The aggregated total value as a decimal.</returns>
        public async Task<decimal> GetTotalValueAsync()
            => await _historyQueries.GetTotalProductionValueReportAsync();

        /// <summary>
        /// Retrieves a detailed production report for a specific product within the given date range.
        /// If no start or end date is provided, the report covers the entire available history.
        /// Returns null if no matching records are found.
        /// </summary>
        /// <param name="productCode">Unique product code identifying the product.</param> 
        /// <param name="startDate">Optional start date of the reporting period; if null, uses the earliest available record.</param> 
        /// <param name="endDate">Optional end date of the reporting period; if null, uses the latest available record.</param> 
        /// <returns>
        /// A <see cref="ProductDetailedReport"/> containing aggregated data for the product, 
        /// or null if no matching records are found. 
        /// </returns>
        public async Task<ProductDetailedReport?> GetProductDetailedReportAsync(string productCode, DateTime? startDate = null, DateTime? endDate = null)
            => await _historyQueries.GetProductDetailedReportAsync(productCode, startDate, endDate);

    }
}