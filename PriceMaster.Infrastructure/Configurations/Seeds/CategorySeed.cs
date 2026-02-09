using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceMaster.Domain.Entities;
using PriceMaster.Domain.Enums;

namespace PriceMaster.Infrastructure.Configurations.Seeds {
    public class CategorySeed : IEntityTypeConfiguration<Category> {
        public void Configure(EntityTypeBuilder<Category> builder) {
            builder.HasData(
                new Category { CategoryId = (int)CategoryType.Artifact, CategoryName = "Artifact" },
                new Category { CategoryId = (int)CategoryType.BaseMaterial, CategoryName = "BaseMaterial" },
                new Category { CategoryId = (int)CategoryType.AssemblyWork, CategoryName = "AssemblyWork" }
            );
        }
    }
}
