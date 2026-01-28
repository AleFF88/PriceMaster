using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Infrastructure;

namespace PriceMaster.ConsoleApp {
    internal static class Program {
        static void Main(string[] args) {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddInfrastructure(options =>
                options.UseSqlite("Data Source=pricemaster.db"));

            using IHost host = builder.Build();

            var repo = host.Services.GetRequiredService<IProductRepository>();

            Console.WriteLine($"Path to the database file: {Path.GetFullPath("pricemaster.db")}");
        }
    }
}
