using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(w => w.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(w => w.OrganizationId);
        builder.HasIndex(w => w.AddressId);
        builder.HasIndex(w => new { w.OrganizationId, w.Name });

        builder.HasQueryFilter(w => !w.IsDeleted);

        builder.HasOne(w => w.Organization)
            .WithMany(o => o.Warehouses)
            .HasForeignKey(w => w.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(w => w.Address)
            .WithMany()
            .HasForeignKey(w => w.AddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
