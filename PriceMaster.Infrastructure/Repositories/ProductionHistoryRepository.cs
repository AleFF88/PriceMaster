using Microsoft.EntityFrameworkCore;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;

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
    }
}
