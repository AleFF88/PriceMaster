using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PriceMaster.Application;
using PriceMaster.Application.Services;
using PriceMaster.Domain.Interfaces;
using PriceMaster.Infrastructure;

namespace PriceMaster.IntegrationTests.Scenarios.System {
    [TestClass]
    public class DependencyInjectionTests {
        /// <summary>
        /// Ensures that all services are correctly registered and their dependencies 
        /// are satisfied within a valid service scope.
        /// </summary>
        [TestMethod]
        public void AllServices_ShouldBeRegisteredAndResolvable() {
            // 1. Arrange
            // Create a new service collection (the DI container builder)

            var services = new ServiceCollection();

            // 2. Act
            // Register Infrastructure layer services using SQLite in-memory for the test environment

            services.AddInfrastructure(options => options.UseSqlite("DataSource=:memory:"));
            // Register Application layer services (Services, etc.)

            services.AddApplication();

            // Build the ServiceProvider with strict validation options:
            // - ValidateOnBuild: Checks if all services can be created (constructor injection check)
            // - ValidateScopes: Checks for scope mismatch (e.g., Singleton depending on Scoped)
            var serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions {
                ValidateOnBuild = true,
                ValidateScopes = true
            });

            // 3. Assert
            // Verify that critical abstractions can be resolved to concrete implementations

            // Create a scope to resolve "Scoped" services (Repositories/DbContext)
            using (var scope = serviceProvider.CreateScope()) {
                var scopedProvider = scope.ServiceProvider;

                // Check Repository registration (Infrastructure Layer)
                var productRepo = scopedProvider.GetRequiredService<IProductRepository>();
                Assert.IsNotNull(productRepo, "IProductRepository should be registered.");

                // Check Service registration (Application Layer)
                var productService = scopedProvider.GetRequiredService<ProductService>();
                Assert.IsNotNull(productService, "ProductService should be registered.");
            }
        }
    }
}