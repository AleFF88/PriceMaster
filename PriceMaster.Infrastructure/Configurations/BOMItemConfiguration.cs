using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class BOMItemConfiguration : IEntityTypeConfiguration<BOMItem> {
        public void Configure(EntityTypeBuilder<BOMItem> builder) {
            builder.ToTable("BOMItems");

            builder.HasKey(b => b.BOMItemId);

            builder.Property(b => b.Quantity)
                   .HasColumnType("REAL")
                   .IsRequired();

            builder.HasOne(bomItem => bomItem.Product)
                   .WithMany(product => product.BOMItems)
                   .HasForeignKey(bomItem => bomItem.ProductId)
                   .IsRequired();

            builder.HasOne(bomItem => bomItem.Component)
                   .WithMany(component => component.BOMItems)
                   .HasForeignKey(bomItem => bomItem.ComponentId)
                   .IsRequired();
        }
    }
}
