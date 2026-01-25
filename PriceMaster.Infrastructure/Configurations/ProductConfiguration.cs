using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class ProductConfiguration : IEntityTypeConfiguration<Product> {
        public void Configure(EntityTypeBuilder<Product> builder) {
            builder.ToTable("Products");

            builder.HasKey(p => p.ProductId);

            builder.Property(p => p.ProductCode)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(p => p.SizeWidth)
                   .HasPrecision(12, 2)
                   .IsRequired();

            builder.Property(p => p.SizeHeight)
                   .HasPrecision(12, 2)
                   .IsRequired();

            builder.Property(p => p.RecommendedPrice)
                   .HasPrecision(18, 2)                 // Price in hryvnias, rounded value without kopecks
                   .IsRequired();
            
            builder.Property(p => p.CreatedAt)
                    .IsRequired();

            builder.HasIndex(p => p.ProductCode)
                   .IsUnique();                         // Product Code must be unique 

            builder.HasOne(product => product.Series)
                .WithMany(series => series.Products)
                .HasForeignKey(product => product.SeriesId)
                .OnDelete(DeleteBehavior.Restrict)      // Cannot delete series if it contains products
                .IsRequired();

            builder.HasMany(product => product.BomItems)
                .WithOne(bomItem => bomItem.Product)
                .HasForeignKey(bomItem => bomItem.ProductId)
                .OnDelete(DeleteBehavior.Cascade)       // Product deleted -> BOM deleted
                .IsRequired();

            builder.HasMany(product => product.ProductionHistories)
                .WithOne(productionHistory => productionHistory.Product)
                .HasForeignKey(productionHistory => productionHistory.ProductId)
                .OnDelete(DeleteBehavior.Restrict)      // Ensures historical data remains preserved 
                .IsRequired();
        }
    }
}
