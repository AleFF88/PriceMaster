using Microsoft.Extensions.DependencyInjection;
using PriceMaster.Application.Services;

namespace PriceMaster.Application {

    public static class DependencyInjection {
        public static IServiceCollection AddApplication(this IServiceCollection services) {

            // SERVICES
            services.AddScoped<ProductService>();
            services.AddScoped<ProductionHistoryService>();

            return services;
        }
    }
}