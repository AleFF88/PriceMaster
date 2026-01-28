using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PriceMaster.Application;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Infrastructure;

namespace PriceMaster.ConsoleApp {
    internal static class Program {
        static async Task Main(string[] args) {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddInfrastructure(options =>
                options.UseSqlite("Data Source=pricemaster.db"));
            builder.Services.AddApplication();

            using IHost host = builder.Build();

            var repo = host.Services.GetRequiredService<IProductRepository>();

            Console.WriteLine($"Path to the database file: {Path.GetFullPath("pricemaster.db")}");
        }
    }
}
