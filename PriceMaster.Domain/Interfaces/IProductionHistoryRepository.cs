using PriceMaster.Domain.Entities;

namespace PriceMaster.Domain.Interfaces {
    public interface IProductionHistoryRepository {
        // <summary>
        /// Adds a new production history record to the database and saves changes.
        /// </summary>
        /// <param name="history">The production history entity to add.</param>
        Task AddAsync(ProductionHistory history);

    }
}
