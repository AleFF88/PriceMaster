using PriceMaster.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PriceMaster.Infrastructure.Configurations {
    public class UnitConfiguration : IEntityTypeConfiguration<Unit> {
        public void Configure(EntityTypeBuilder<Unit> builder) {
            builder.ToTable("Units");

            builder.HasKey(u => u.UnitId);

            builder.Property(u => u.UnitName)
                   .HasMaxLength(10)
                   .IsRequired();

            builder.HasIndex(u => u.UnitName)
                   .IsUnique();

            builder.HasMany(unit => unit.Components)
                   .WithOne(component => component.Unit)
                   .HasForeignKey(component => component.UnitId)
                   .OnDelete(DeleteBehavior.Restrict) // forbid deletion if in use
                   .IsRequired();
        }
    }
}
