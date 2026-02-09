using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Enums;

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
                new Component { ComponentId = 1, ComponentName = "Button", UnitId = (int)UnitType.Piece, PricePerUnit = 5, CategoryId = (int)CategoryType.Artifact },
                new Component { ComponentId = 2, ComponentName = "Ring", UnitId = (int)UnitType.Piece, PricePerUnit = 5, CategoryId = (int)CategoryType.Artifact },
                new Component { ComponentId = 3, ComponentName = "Nail", UnitId = (int)UnitType.Piece, PricePerUnit = 5, CategoryId = (int)CategoryType.Artifact },
                new Component { ComponentId = 4, ComponentName = "Harness Component (simple)", UnitId = (int)UnitType.Piece, PricePerUnit = 10, CategoryId = (int)CategoryType.Artifact },
                new Component { ComponentId = 5, ComponentName = "Harness Component (stars)", UnitId = (int)UnitType.Piece, PricePerUnit = 20, CategoryId = (int)CategoryType.Artifact },
                new Component { ComponentId = 6, ComponentName = "Handle", UnitId = (int)UnitType.Piece, PricePerUnit = 50, CategoryId = (int)CategoryType.Artifact },

                // Artifacts, coins
                new Component { ComponentId = 50, ComponentName = "Copper coin (n3)", UnitId = (int)UnitType.Piece, PricePerUnit = 5, CategoryId = (int)CategoryType.Artifact },
                new Component { ComponentId = 51, ComponentName = "Copper coin (n2)", UnitId = (int)UnitType.Piece, PricePerUnit = 10, CategoryId = (int)CategoryType.Artifact },
                new Component { ComponentId = 52, ComponentName = "Copper coin (n1)", UnitId = (int)UnitType.Piece, PricePerUnit = 20, CategoryId = (int)CategoryType.Artifact },
                new Component { ComponentId = 53, ComponentName = "Silver coin (copy)", UnitId = (int)UnitType.Piece, PricePerUnit = 72, CategoryId = (int)CategoryType.Artifact },

                // Artifacts, balls
                new Component { ComponentId = 90, ComponentName = "Spherical ball", UnitId = (int)UnitType.Piece, PricePerUnit = 5, CategoryId = (int)CategoryType.Artifact },

                // Artifacts, chalk copy
                new Component { ComponentId = 110, ComponentName = "Inkwell", UnitId = (int)UnitType.Piece, PricePerUnit = 30, CategoryId = (int)CategoryType.Artifact },
                new Component { ComponentId = 111, ComponentName = "Tobacco pipe", UnitId = (int)UnitType.Piece, PricePerUnit = 25, CategoryId = (int)CategoryType.Artifact },

                // Artifacts, ?
                new Component { ComponentId = 120, ComponentName = "Flint", UnitId = (int)UnitType.Piece, PricePerUnit = 80, CategoryId = (int)CategoryType.Artifact },



                //Base materials, baguettes
                new Component { ComponentId = 500, ComponentName = "Baguette 60*30", UnitId = (int)UnitType.Piece, PricePerUnit = 661, CategoryId = (int)CategoryType.BaseMaterial },

                //Base materials, printout
                new Component { ComponentId = 900, ComponentName = "Printout 110", UnitId = (int)UnitType.Piece, PricePerUnit = 71, CategoryId = (int)CategoryType.BaseMaterial },
                new Component { ComponentId = 901, ComponentName = "Printout 150", UnitId = (int)UnitType.Piece, PricePerUnit = 71, CategoryId = (int)CategoryType.BaseMaterial },

                //Base materials, other
                new Component { ComponentId = 950, ComponentName = "Hardware", UnitId = (int)UnitType.Piece, PricePerUnit = 25, CategoryId = (int)CategoryType.BaseMaterial },
                new Component { ComponentId = 961, ComponentName = "Textile, per sq.m.", UnitId = (int)UnitType.SquareMeter, PricePerUnit = 150, CategoryId = (int)CategoryType.BaseMaterial },
                new Component { ComponentId = 962, ComponentName = "Polyurethane, per sq.m.", UnitId = (int)UnitType.SquareMeter, PricePerUnit = 85, CategoryId = (int)CategoryType.BaseMaterial },



                //Assembly work
                new Component { ComponentId = 1000, ComponentName = "Work", UnitId = (int)UnitType.Piece, PricePerUnit = 800, CategoryId = (int)CategoryType.BaseMaterial }
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
