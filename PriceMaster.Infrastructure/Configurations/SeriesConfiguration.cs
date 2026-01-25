using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class SeriesConfiguration : IEntityTypeConfiguration<Series> {
        public void Configure(EntityTypeBuilder<Series> builder) {
            builder.ToTable("Series");

            builder.HasKey(s => s.SeriesId);

            builder.Property(s => s.SeriesName)
                   .HasMaxLength(90)
                   .IsRequired();

            builder.HasIndex(s => s.SeriesName)
                   .IsUnique();

            builder.HasData(
                new Series { SeriesId = 1, SeriesName = "Cossacks. Birth Of Liberty." },
                new Series { SeriesId = 2, SeriesName = "UNR 1917-1921. The Ukrainian Liberation struggle." },
                new Series { SeriesId = 3, SeriesName = "Kyivan Rus. The Golden Legacy." }
            );

            builder.HasMany(series => series.Products)
                   .WithOne(product => product.Series)
                   .HasForeignKey(product => product.SeriesId)
                   .OnDelete(DeleteBehavior.Restrict)     // Cannot delete series if it contains products
                   .IsRequired();
        }
    }
}
