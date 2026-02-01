using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PriceMaster.Application.Services;
using System.Reflection;

namespace PriceMaster.Application {

    public static class DependencyInjection {
        public static IServiceCollection AddApplication(this IServiceCollection services) {

            // SERVICES
            services.AddScoped<ProductService>();
            services.AddScoped<ProductionHistoryService>();

            // VALIDATORS
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}