using Microsoft.EntityFrameworkCore;
using PriceMaster.Infrastructure;

namespace PriceMaster.ConsoleApp {
    public static class Program {
        static void Main() {
            var options = new DbContextOptionsBuilder<PriceMasterDbContext>()
                .UseSqlite("Data Source=pricemaster.db")
                .Options;

            using (var context = new PriceMasterDbContext(options)) {
                var pending = context.Database.GetPendingMigrations();
                if (pending.Any()) {
                    Console.WriteLine($"Apply {pending.Count()} pending migrations.");
                    context.Database.Migrate(); 
                    Console.WriteLine("Database created and migrated successfully.");
                } else {
                    Console.WriteLine($"No pending migrations.");
                }
            }
        }
    }
}
