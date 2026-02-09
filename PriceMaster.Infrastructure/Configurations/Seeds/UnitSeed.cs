using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceMaster.Domain.Entities;

namespace PriceMaster.Infrastructure.Configurations.Seeds {
    public class UnitSeed : IEntityTypeConfiguration<Unit> {
        public void Configure(EntityTypeBuilder<Unit> builder) {
            builder.HasData(
                new Unit { UnitId = 1, UnitName = "piece" },
                new Unit { UnitId = 2, UnitName = "sq.m" }
            );
        }
    }
}