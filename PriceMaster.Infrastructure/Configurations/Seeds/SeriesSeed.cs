using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceMaster.Domain.Entities;

namespace PriceMaster.Infrastructure.Configurations.Seeds {
    public class SeriesSeed : IEntityTypeConfiguration<Series> {
        public void Configure(EntityTypeBuilder<Series> builder) {
            builder.HasData(
                new Series { SeriesId = 1, SeriesName = "Cossacks. Birth Of Liberty." },
                new Series { SeriesId = 2, SeriesName = "UNR 1917-1921. The Ukrainian Liberation struggle." },
                new Series { SeriesId = 3, SeriesName = "Kyivan Rus. The Golden Legacy." }
            );
        }
    }
}