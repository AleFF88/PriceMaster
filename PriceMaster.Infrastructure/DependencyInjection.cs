using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Infrastructure.Repositories;

namespace PriceMaster.Infrastructure {
    public static class DependencyInjection {,
        /// Registers infrastructure services, repositories, and the database context.
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDbContext) {
            // Configure the main database context with provided options
            services.AddDbContext<PriceMasterDbContext>(configureDbContext);

            // Register data access repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductionHistoryRepository, ProductionHistoryRepository>();

            // Specialized read-only data retrieval (queties) for reports and complex views
            services.AddScoped<IProductionHistoryQueries, ProductionHistoryQueries>();


            return services;
        }
    }
}
