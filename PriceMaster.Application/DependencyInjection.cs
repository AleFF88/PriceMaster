using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PriceMaster.Application.Services;
using System.Reflection;

namespace PriceMaster.Application {

    public static class DependencyInjection {
        /// Registers application-level services and validators into the dependency injection container.
        public static IServiceCollection AddApplication(this IServiceCollection services) {

            // Register business logic services with scoped lifetime
            services.AddScoped<ProductService>();
            services.AddScoped<ProductionHistoryService>();

            // Automatically discover and register all FluentValidation validators in this assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}