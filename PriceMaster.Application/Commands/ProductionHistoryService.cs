using PriceMaster.Application.DTOs;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;
using System.Runtime.Intrinsics.Arm;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        /// <returns>
        /// A ProductResponse indicating success or failure, including details of the saved record.
        /// </returns>

        public async Task<ProductResponse> AddProductionHistoryEntryAsync(string productCode, string notes = "") {
            var product = await _productRepository.GetByProductCodeWithBomAsync(productCode);
            if (product is null) {
                return new ProductResponse { Success = false, Message = $"Product {productCode} not found." };
            }

            decimal totalPrice = product.BOMItems.Sum(i => i.Quantity * i.Component!.PricePerUnit);

            var history = new ProductionHistory {
                ProductId = product.ProductId,
                CreatedAt = DateTime.UtcNow,
                Price = totalPrice,
                RecommendedPrice = product.RecommendedPrice,
                WorkCost = product.BOMItems
                    .Where(i => i.Component!.CategoryId == 3) 
                    .Sum(i => i.Quantity * i.Component!.PricePerUnit),
                Notes = notes + product.Notes
            };

            await _historyRepository.AddAsync(history);

            return new ProductResponse {
                Success = true,
                Message = $"{history.CreatedAt:yyyy-MM-dd} The data is successfully saved. \r\n Product code: {product.ProductCode}, Price: {history.Price:C}, Recommended Price: {product.RecommendedPrice:C}, Work Cost: {history.WorkCost:C}, Notes: {history.Notes}",
                ProductCode = product.ProductCode
            };
        }

        /// <summary>
        /// Calculates the total value of all manufactured products
        /// based on their recommended prices stored in production history.
        /// </summary>
        /// <returns>The aggregated total value as a decimal.</returns>
        public async Task<decimal> GetTotalProductionValueReportAsync() {
            return await _historyRepository.GetTotalProductionValueReportAsync();
        }
    }
}
