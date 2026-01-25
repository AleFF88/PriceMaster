using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class BomItemConfiguration : IEntityTypeConfiguration<BomItem> {
        public void Configure(EntityTypeBuilder<BomItem> builder) {
            builder.ToTable("BomItems");

            builder.HasKey(b => b.BomItemId);

            builder.Property(b => b.Quantity)
                   .HasPrecision(18, 3)
                   .IsRequired();

            builder.HasOne(bomItem => bomItem.Product)
                   .WithMany(product => product.BomItems)
                   .HasForeignKey(bomItem => bomItem.ProductId)
                   .OnDelete(DeleteBehavior.Cascade)   // If a product is deleted, its BOM is no longer required.
                   .IsRequired();

            builder.HasOne(bomItem => bomItem.Component)
                   .WithMany(component => component.BomItems)
                   .HasForeignKey(bomItem => bomItem.ComponentId)
                   .OnDelete(DeleteBehavior.Restrict)  // A component cannot be deleted if it is used in a product.
                   .IsRequired();
        }
    }
}
