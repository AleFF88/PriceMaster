using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PriceMaster.Infrastructure;

namespace PriceMaster.IntegrationTests.Common {
    /// <summary>
    /// Base class for integration tests providing a shared infrastructure.
    /// Implements the full Dispose pattern to ensure proper resource cleanup.
    /// </summary>
    public abstract class IntegrationTestBase : IDisposable {
        protected readonly PriceMasterDbContext Context;
        protected readonly SqliteConnection Connection;

        protected IntegrationTestBase() {
            // Setting up in-memory SQLite database for testing
            Connection = new SqliteConnection("Data Source=:memory:");
            Connection.Open();

            var options = new DbContextOptionsBuilder<PriceMasterDbContext>()
                .UseSqlite(Connection)
                .Options;

            // Creating the DbContext with the in-memory database
            Context = new PriceMasterDbContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose() {
            Context?.Dispose();
            Connection?.Dispose();
        }
    }
}
