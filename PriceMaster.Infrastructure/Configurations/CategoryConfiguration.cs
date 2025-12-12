using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class CategoryConfiguration : IEntityTypeConfiguration<Category> {
        public void Configure(EntityTypeBuilder<Category> builder) {
            builder.ToTable("Categories");

            builder.HasKey(с => с.CategoryId);

            builder.Property(с => с.CategoryName)
                   .HasColumnType("TEXT")
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasIndex(с => с.CategoryName)
                   .IsUnique();        

            builder.HasData(
                new Category { CategoryId = 1, CategoryName = "Artifact" },
                new Category { CategoryId = 2, CategoryName = "BaseMaterial" },
                new Category { CategoryId = 3, CategoryName = "AssemblyWork" }
            );

            builder.HasMany(category => category.Components)
                   .WithOne(component => component.Category)
                   .HasForeignKey(component => component.CategoryId)
                   .IsRequired();
        }
    }
}
