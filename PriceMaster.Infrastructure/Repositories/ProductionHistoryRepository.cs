using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;

namespace PriceMaster.Infrastructure.Repositories {
    public class ProductionHistoryRepository : IProductionHistoryRepository {
        private readonly PriceMasterDbContext _context;

        public ProductionHistoryRepository(PriceMasterDbContext context) {
            _context = context;
        }

        // <inheritdoc />
        public async Task AddAsync(ProductionHistory history) {
            await _context.ProductionHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }
    }
}