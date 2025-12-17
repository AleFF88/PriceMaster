using PriceMaster.Domain.Common;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Domain.Reports;

namespace PriceMaster.Application.Commands {
    public class ProductionHistoryService {
        private readonly IProductRepository _productRepository;
        private readonly IProductionHistoryRepository _historyRepository;

        public ProductionHistoryService(IProductRepository productRepository, IProductionHistoryRepository historyRepository) {
            _productRepository = productRepository;
            _historyRepository = historyRepository;
        }

        /// <summary>
        /// Records a new production history entry for the specified product.
        /// Retrieves the product by its code with BOM, performs necessary calculations,
        /// and saves the resulting data into the production history.
        /// </summary>
        /// <param name="productCode">Unique product code to identify the product.</param>
        /// <param name="notes">Optional notes to append to the history record.</param>
        /// <returns>Operation result with success flag and message.</returns>
        public async Task<OperationResult> AddProductionHistoryEntryAsync(string productCode, string notes = "") {
            var product = await _productRepository.GetByProductCodeWithBomAsync(productCode);
            if (product is null) {
                return OperationResult.Fail($"Product {productCode} not found.");
            }

            var totalPrice = Math.Ceiling(product.BOMItems.Sum(i => i.Quantity * i.Component!.PricePerUnit));

            var workCost = Math.Ceiling(product.BOMItems
                .Where(i => i.Component!.CategoryId == 3)
                .Sum(i => i.Quantity * i.Component!.PricePerUnit));

            var history = new ProductionHistory {
                ProductId = product.ProductId,
                CreatedAt = DateTime.UtcNow,
                Price = totalPrice,
                RecommendedPrice = product.RecommendedPrice,
                WorkCost = workCost,
                Notes = $"{notes} {product.Notes}".Trim()
            };

            await _historyRepository.AddAsync(history);

            return OperationResult.Ok($"Product {productCode} is successfully saved to Product Histories. Date: {history.CreatedAt}");
        }

        /// <summary>
        /// Calculates the total value of all manufactured products
        /// based on their recommended prices stored in production history.
        /// </summary>
        /// <returns>The aggregated total value as a decimal.</returns>
        public async Task<decimal> GetTotalProductionValueReportAsync() {
            return await _historyRepository.GetTotalProductionValueReportAsync();
        }

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
        public async Task<ProductDetailedReport?> GetProductDetailedReportAsync(string productCode, DateTime? startDate = null, DateTime? endDate = null) {
            return await _historyRepository.GetProductDetailedReportAsync(productCode, startDate, endDate);
        }

    }
}