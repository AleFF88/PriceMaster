using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class CategoryConfiguration : IEntityTypeConfiguration<Category> {
        public void Configure(EntityTypeBuilder<Category> builder) {
            builder.ToTable("Categories");

            builder.HasKey(c => c.CategoryId);

            builder.Property(c => c.CategoryName)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasIndex(c => c.CategoryName)
                   .IsUnique();        

            builder.HasData(
                new Category { CategoryId = 1, CategoryName = "Artifact" },
                new Category { CategoryId = 2, CategoryName = "BaseMaterial" },
                new Category { CategoryId = 3, CategoryName = "AssemblyWork" }
            );

            builder.HasMany(category => category.Components)
                   .WithOne(component => component.Category)
                   .HasForeignKey(component => component.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict)  // Category is a reference; forbid deletion if components are linked
                   .IsRequired();
        }
    }
}
