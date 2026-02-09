using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceMaster.Domain.Entities;

namespace PriceMaster.Infrastructure.Configurations {
    public class ComponentConfiguration : IEntityTypeConfiguration<Component> {
        public void Configure(EntityTypeBuilder<Component> builder) {
            builder.ToTable("Components");

            builder.HasKey(c => c.ComponentId);

            builder.Property(c => c.ComponentName)
                   .HasMaxLength(90)
                   .IsRequired();

            builder.HasIndex(c => c.ComponentName)
                   .IsUnique();

            builder.Property(c => c.PricePerUnit)
                   .HasPrecision(18, 2)    // Price in hryvnias, rounded value without kopecks
                   .IsRequired();

            builder.HasOne(component => component.Category)
                   .WithMany(category => category.Components)
                   .HasForeignKey(component => component.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict) // Cannot delete category if it contains components
                   .IsRequired();

            builder.HasOne(component => component.Unit)
                   .WithMany(unit => unit.Components)
                   .HasForeignKey(component => component.UnitId)
                   .OnDelete(DeleteBehavior.Restrict) // Cannot delete unit of measure if it is in use
                   .IsRequired();

            builder.HasMany(component => component.BomItems)
                   .WithOne(bomItem => bomItem.Component)
                   .HasForeignKey(bomItem => bomItem.ComponentId)
                   .OnDelete(DeleteBehavior.Restrict) // Cannot delete component if it is in BOM
                   .IsRequired();
        }
    }
}
