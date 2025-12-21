using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class BomItemConfiguration : IEntityTypeConfiguration<BomItem> {
        public void Configure(EntityTypeBuilder<BomItem> builder) {
            builder.ToTable("BomItems");

            builder.HasKey(b => b.BomItemId);

            builder.Property(b => b.Quantity)
                   .HasColumnType("REAL")
                   .IsRequired();

            builder.HasOne(bomItem => bomItem.Product)
                   .WithMany(product => product.BomItems)
                   .HasForeignKey(bomItem => bomItem.ProductId)
                   .IsRequired();

            builder.HasOne(bomItem => bomItem.Component)
                   .WithMany(component => component.BomItems)
                   .HasForeignKey(bomItem => bomItem.ComponentId)
                   .IsRequired();
        }
    }
}
