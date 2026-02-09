using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PriceMaster.Infrastructure;

namespace PriceMaster.IntegrationTests.Infrastructure {

    /// <summary>
    /// Base class for integration tests providing a shared infrastructure.
    /// Implements the full Dispose pattern to ensure proper resource cleanup.
    /// </summary>
    public abstract class IntegrationTestBase : IDisposable {
        protected readonly PriceMasterDbContext Context;
        private readonly SqliteConnection _connection;

        protected IntegrationTestBase() {
            // Setting up in-memory SQLite database for testing
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<PriceMasterDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Creating the DbContext with the in-memory database
            Context = new PriceMasterDbContext(options);

            // This will create tables based on configurations from Infrastructure
            // and automatically apply Seed Data (categories, components).
            Context.Database.EnsureCreated();
        }

        public void Dispose() {
            Context.Dispose();
            _connection.Dispose();
        }
    }
}