using Microsoft.EntityFrameworkCore;
using PriceMaster.Application.Commands;
using PriceMaster.Contracts.DTOs.ProductionHistory;
using PriceMaster.Contracts.DTOs.Products;
using PriceMaster.Contracts.DTOs.Reports;
using PriceMaster.Contracts.Mappers;
using PriceMaster.Infrastructure;
using PriceMaster.Infrastructure.Repositories;
using System.Globalization;

namespace PriceMaster.ConsoleApp {
    public static class Program {

        // Check for any pending EF Core migrations and apply them to ensure
        //   the database schema is up to date.
        private async static Task CheckAndPerformMigrations(PriceMasterDbContext context) {
            var pending = await context.Database.GetPendingMigrationsAsync();
            if (pending.Any()) {
                Console.WriteLine($"Apply {pending.Count()} pending migrations.");
                await context.Database.MigrateAsync();
                Console.WriteLine("Database created and migrated successfully.");
                Console.WriteLine();
            } else {
                Console.WriteLine($"No pending migrations.");
                Console.WriteLine();
            }
        }

        public static async Task Main() {
            var options = new DbContextOptionsBuilder<PriceMasterDbContext>()
                .UseSqlite("Data Source=pricemaster.db")
                .Options;

            using (var context = new PriceMasterDbContext(options)) {

                //TODO: Move migration to separate layer
                _ = CheckAndPerformMigrations(context);

            }
        }
    }
}
