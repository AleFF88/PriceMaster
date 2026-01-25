using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class ProductionHistoryConfiguration : IEntityTypeConfiguration<ProductionHistory> {
        public void Configure(EntityTypeBuilder<ProductionHistory> builder) {
            builder.ToTable("ProductionHistories");

            builder.HasKey(ph => ph.ProductionHistoryId);

            builder.Property(ph => ph.CreatedAt)
                   .IsRequired();

            
            // Price in hryvnias, rounded value without kopecks
            builder.Property(ph => ph.Price)
                   .HasPrecision(18, 2)                 
                   .IsRequired();

            builder.Property(ph => ph.RecommendedPrice)
                   .HasPrecision(18, 2)               
                   .IsRequired();

            builder.Property(ph => ph.WorkCost)
                   .HasPrecision(18, 2)                 
                   .IsRequired();


            builder.HasOne(productionHistory => productionHistory.Product)
                   .WithMany(product => product.ProductionHistories)
                   .HasForeignKey(productionHistory => productionHistory.ProductId)
                   .OnDelete(DeleteBehavior.Restrict)   // Cannot delete product if it contains components
                   .IsRequired();
        }
    }
}
