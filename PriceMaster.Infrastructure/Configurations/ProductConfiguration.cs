using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class ProductConfiguration : IEntityTypeConfiguration<Product> {
        public void Configure(EntityTypeBuilder<Product> builder) {
            builder.ToTable("Products");

            builder.HasKey(p => p.ProductId);

            builder.Property(p => p.ProductCode)
                   .HasColumnType("TEXT")
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(p => p.SizeWidth)
                   .HasColumnType("INTEGER")
                   .IsRequired();

            builder.Property(p => p.SizeHeight)
                   .HasColumnType("INTEGER")
                   .IsRequired();

            builder.Property(p => p.RecommendedPrice)
                   .HasColumnType("INTEGER")    // Price in hryvnias, rounded value without kopecks
                   .IsRequired();
            
            builder.Property(p => p.CreatedAt)
                    .HasColumnType("TEXT")
                    .IsRequired();

            builder.HasIndex(p => p.ProductCode)
                   .IsUnique();         // Product Code must be unique 

            builder.HasOne(product => product.Series)
                .WithMany(series => series.Products)
                .HasForeignKey(product => product.SeriesId)
                .IsRequired();

            builder.HasMany(product => product.BomItems)
                .WithOne(bomItem => bomItem.Product)
                .HasForeignKey(bomItem => bomItem.ProductId)
                .IsRequired();

            builder.HasMany(product => product.ProductionHistories)
                .WithOne(productionHistory => productionHistory.Product)
                .HasForeignKey(productionHistory => productionHistory.ProductId)
                .IsRequired();

        }
    }
}
