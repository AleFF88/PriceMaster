using Microsoft.EntityFrameworkCore;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Domain.Reports;

namespace PriceMaster.Infrastructure.Repositories {
    public class ProductionHistoryQueries : IProductionHistoryQueries {
        private readonly PriceMasterDbContext _context;

        public ProductionHistoryQueries(PriceMasterDbContext context) {
            _context = context;
        }

        // <inheritdoc />
        public async Task<ProductDetailedReport?> GetProductDetailedReportAsync(string productCode, DateTime? startDate = null, DateTime? endDate = null) {

            var query = _context.ProductionHistories
                .AsNoTracking()
                .Where(ph => ph.Product!.ProductCode == productCode);

            if (startDate.HasValue)
                query = query.Where(p => p.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.CreatedAt <= endDate.Value);

            // Use projection to fetch only necessary columns.
            var data = await query
                .Select(p => new {
                    p.RecommendedPrice,
                    p.WorkCost,
                    p.CreatedAt
                })
                .ToListAsync();

            if (!data.Any())
                return null;

            return new ProductDetailedReport {
                ProductCode = productCode,
                Count = data.Count,
                TotalValue = data.Sum(p => p.RecommendedPrice),
                WorkCost = data.Sum(p => p.WorkCost),
                PeriodFrom = startDate ?? data.Min(p => p.CreatedAt),
                PeriodTo = endDate ?? data.Max(p => p.CreatedAt)
            };
        }

        // <inheritdoc />
        public async Task<decimal> GetTotalProductionValueReportAsync() {
            return await _context.ProductionHistories
                .AsNoTracking()
                .SumAsync(ph => (decimal?)ph.RecommendedPrice) ?? 0m;
        }
    }
}