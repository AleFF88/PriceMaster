using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Infrastructure.Repositories;

namespace PriceMaster.Infrastructure {
    public static class DependencyInjection {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<DbContextOptionsBuilder> configureDbContext) { 
            services.AddDbContext<PriceMasterDbContext>(configureDbContext);

            // REPOSITORIES
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductionHistoryRepository, ProductionHistoryRepository>();

            // QUERIES
            services.AddScoped<IProductionHistoryQueries, ProductionHistoryQueries>();

            // SERVICES
            //

            return services;
        }
    }
}
