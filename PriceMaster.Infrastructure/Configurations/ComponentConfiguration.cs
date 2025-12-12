using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class ComponentConfiguration : IEntityTypeConfiguration<Component> {
        public void Configure(EntityTypeBuilder<Component> builder) {
            builder.ToTable("Components");

            builder.HasKey(с => с.ComponentId);

            builder.Property(с => с.ComponentName)
                   .HasColumnType("TEXT")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasIndex(с => с.ComponentName)
                   .IsUnique();        

            builder.Property(с => с.PricePerUnit)
                   .HasColumnType("INTEGER")
                   .IsRequired();

            builder.HasOne(component => component.Category)
                   .WithMany(category => category.Components)
                   .HasForeignKey(component => component.CategoryId)
                   .IsRequired();

            builder.HasOne(component => component.Unit)
                   .WithMany(unit => unit.Components)
                   .HasForeignKey(component => component.UnitId)
                   .IsRequired();

            builder.HasMany(component => component.BOMItems)
                   .WithOne(bomItem => bomItem.Component)
                   .HasForeignKey(bomItem => bomItem.ComponentId)
                   .IsRequired();
        }
    }
}
