using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Infrastructure;

namespace PriceMaster.ConsoleApp {
    internal static class Program {
        static void Main(string[] args) {
            // Создаем "хост" приложения (это стандартный способ в современном .NET)
            var builder = Host.CreateApplicationBuilder(args);

            // ВЫЗОВ НАШЕГО МЕТОДА
            builder.Services.AddInfrastructure(options =>
                options.UseSqlite("Data Source=pricemaster.db"));

            using IHost host = builder.Build();

            // ТЕПЕРЬ МАГИЯ: Мы не пишем new ProductRepository...
            // Мы просим сервис у контейнера:
            var repo = host.Services.GetRequiredService<IProductRepository>();

        }
    }
}
