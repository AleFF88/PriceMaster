using PriceMaster.Domain.Entities;

namespace PriceMaster.Domain.Interfaces {
    public interface IProductRepository {

        /// <summary>
        /// Adds a new product entity to the database and saves changes.
        /// Use this method when creating a new product record.
        /// </summary>
        /// <param name="product">The product entity to add.</param>
        Task AddAsync(Product product);

        /// <summary>
        /// Checks whether a product with the specified unique product code exists in the database.
        /// Useful for validation before creating a new product.
        /// </summary>
        /// <param name="productCode">The unique product code to check.</param>
        /// <returns>True if a product with the given code exists; otherwise, false.</returns>
        Task<bool> Exists(string productCode);

        /// <summary>
        /// Retrieves a product by its code including BOM items and their components.
        /// The query uses AsNoTracking, meaning the returned entity is not tracked by the DbContext.
        /// Suitable for read-only operations such as calculations or reporting.
        /// </summary>
        /// <param name="productCode">Unique product code.</param>
        /// <returns>The product with BOM and components, or null if not found.</returns>
        Task<Product?> GetByProductCodeWithBomAsync(string productCode);
    }
}
