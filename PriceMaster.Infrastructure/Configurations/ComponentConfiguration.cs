using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

            builder.HasData(
                // Artifacts, common
                new Component { ComponentId = 1, ComponentName = "Button", UnitId = 1, PricePerUnit = 5, CategoryId = 1 },
                new Component { ComponentId = 2, ComponentName = "Ring", UnitId = 1, PricePerUnit = 5, CategoryId = 1 },
                new Component { ComponentId = 3, ComponentName = "Nail", UnitId = 1, PricePerUnit = 5, CategoryId = 1 },
                new Component { ComponentId = 4, ComponentName = "Harness Component (simple)", UnitId = 1, PricePerUnit = 10, CategoryId = 1 },
                new Component { ComponentId = 5, ComponentName = "Harness Component (stars)", UnitId = 1, PricePerUnit = 20, CategoryId = 1 },
                new Component { ComponentId = 6, ComponentName = "Handle", UnitId = 1, PricePerUnit = 50, CategoryId = 1 },

                // Artifacts, coins
                new Component { ComponentId = 50, ComponentName = "Copper coin (n3)", UnitId = 1, PricePerUnit = 5, CategoryId = 1 },
                new Component { ComponentId = 51, ComponentName = "Copper coin (n2)", UnitId = 1, PricePerUnit = 10, CategoryId = 1 },
                new Component { ComponentId = 52, ComponentName = "Copper coin (n1)", UnitId = 1, PricePerUnit = 20, CategoryId = 1 },
                new Component { ComponentId = 53, ComponentName = "Silver coin (copy)", UnitId = 1, PricePerUnit = 72, CategoryId = 1 },

                // Artifacts, balls
                new Component { ComponentId = 90, ComponentName = "Spherical ball", UnitId = 1, PricePerUnit = 5, CategoryId = 1 },

                // Artifacts, chalk copy
                new Component { ComponentId = 110, ComponentName = "Inkwell", UnitId = 1, PricePerUnit = 30, CategoryId = 1 },
                new Component { ComponentId = 111, ComponentName = "Tobacco pipe", UnitId = 1, PricePerUnit = 25, CategoryId = 1 },

                // Artifacts, ?
                new Component { ComponentId = 120, ComponentName = "Flint", UnitId = 1, PricePerUnit = 80, CategoryId = 1 },



                //Base materials, baguettes
                new Component { ComponentId = 500, ComponentName = "Baguette 60*30", UnitId = 1, PricePerUnit = 661, CategoryId = 2 },

                //Base materials, printout
                new Component { ComponentId = 900, ComponentName = "Printout 110", UnitId = 1, PricePerUnit = 71, CategoryId = 2 },
                new Component { ComponentId = 901, ComponentName = "Printout 150", UnitId = 1, PricePerUnit = 71, CategoryId = 2 },

                //Base materials, other
                new Component { ComponentId = 950, ComponentName = "Hardware", UnitId = 1, PricePerUnit = 25, CategoryId = 2 },
                new Component { ComponentId = 961, ComponentName = "Textile, per sq.m.", UnitId = 2, PricePerUnit = 150, CategoryId = 2 },
                new Component { ComponentId = 962, ComponentName = "Polyurethane, per sq.m.", UnitId = 2, PricePerUnit = 85, CategoryId = 2 },



                //Assembly work
                new Component { ComponentId = 1000, ComponentName = "Work", UnitId = 1, PricePerUnit = 800, CategoryId = 3 }
            );

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
