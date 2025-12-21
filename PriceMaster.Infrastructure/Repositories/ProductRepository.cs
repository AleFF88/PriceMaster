using Microsoft.EntityFrameworkCore;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Interfaces;

namespace PriceMaster.Infrastructure.Repositories {
    public class ProductRepository : IProductRepository {
        private readonly PriceMasterDbContext _context;

        public ProductRepository(PriceMasterDbContext context) {
            _context = context;
        }

        /// <inheritdoc />
        public async Task AddAsync(Product product) {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<bool> Exists(string productCode) {
            return await _context.Products.AnyAsync(p => p.ProductCode == productCode);
        }

        /// <inheritdoc />
        public async Task<Product?> GetByProductCodeWithBomAsync(string productCode) {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.BomItems)
                    .ThenInclude(b => b.Component)
                .FirstOrDefaultAsync(p => p.ProductCode == productCode);
        }
    }
}