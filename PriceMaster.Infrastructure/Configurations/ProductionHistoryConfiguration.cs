using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class ProductionHistoryConfiguration : IEntityTypeConfiguration<ProductionHistory> {
        public void Configure(EntityTypeBuilder<ProductionHistory> builder) {
            builder.ToTable("ProductionHistories");

            builder.HasKey(ph => ph.ProductionHistoryId);

            builder.Property(ph => ph.CreatedAt)
                   .HasColumnType("TEXT")
                   .IsRequired();

            builder.Property(ph => ph.Price)
                   .HasColumnType("INTEGER")
                   .IsRequired();

            builder.Property(ph => ph.RecommendedPrice)
                   .HasColumnType("INTEGER")
                   .IsRequired();

            builder.Property(ph => ph.WorkCost)
                   .HasColumnType("INTEGER")
                   .IsRequired();

            builder.HasOne(productionHistory => productionHistory.Product)
                   .WithMany(product => product.ProductionHistories)
                   .HasForeignKey(productionHistory => productionHistory.ProductId)
                   .IsRequired();
        }
    }
}
