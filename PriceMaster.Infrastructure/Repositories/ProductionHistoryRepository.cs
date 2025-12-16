using Microsoft.EntityFrameworkCore;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Domain.Reports;

namespace PriceMaster.Infrastructure.Repositories {
    public class ProductionHistoryRepository : IProductionHistoryRepository {
        private readonly PriceMasterDbContext _context;

        public ProductionHistoryRepository(PriceMasterDbContext context) {
            _context = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(ProductionHistory history) {
            _context.ProductionHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<decimal> GetTotalProductionValueReportAsync() {
            return await _context.ProductionHistories.SumAsync(ph => ph.RecommendedPrice);
        }

        // <inheritdoc />
        public async Task<ProductDetailedReport?> GetProductDetailedReportAsync(string productCode, DateTime? startDate = null, DateTime? endDate = null) {
            var query = _context.ProductionHistories
                .Where(ph => ph.Product!.ProductCode == productCode);

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedAt <= endDate.Value);

            var list = await query.ToListAsync();

            if (!list.Any())
                return null;

            return new ProductDetailedReport {
                ProductCode = productCode,
                Count = list.Count,
                TotalValue = list.Sum(p => p.RecommendedPrice),
                WorkCost = list.Sum(p => p.WorkCost),
                PeriodFrom = startDate ?? list.Min(p => p.CreatedAt),
                PeriodTo = endDate ?? list.Max(p => p.CreatedAt)
            };
        }
    }
}
