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

            builder.HasMany(category => category.Components)
                   .WithOne(component => component.Category)
                   .HasForeignKey(component => component.CategoryId)
                   .IsRequired();
        }
    }
}
