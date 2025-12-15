using PriceMaster.Domain.Entities;

namespace PriceMaster.Domain.Interfaces {
    public interface IProductionHistoryRepository {

        // <summary>
        /// Adds a new production history record to the database and saves changes.
        /// </summary>
        /// <param name="history">The production history entity to add.</param>
        Task AddAsync(ProductionHistory history);

        /// <summary>
        /// Calculates the total value of all manufactured products
        /// based on their recommended prices stored in production history.
        /// </summary>
        /// <returns>The aggregated total value as a decimal.</returns>
        Task<decimal> GetTotalProductionValueReportAsync(); 
    }
}
