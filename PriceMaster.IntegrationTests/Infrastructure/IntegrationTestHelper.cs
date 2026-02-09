using PriceMaster.Infrastructure;

namespace PriceMaster.IntegrationTests.Infrastructure {
    internal static class IntegrationTestHelper {
        /// <summary>
        /// Forces the EF Core Change Tracker to clear all tracked entities.
        /// This ensures test isolation by bypassing the in-memory cache and 
        /// forcing the context to reload data directly from the database.
        /// </summary>
        /// <param name="context">The database context whose tracker should be cleared.</param>
        internal static void ClearChangeTracker(PriceMasterDbContext context) {
            context.ChangeTracker.Clear();
        }
    }
}
