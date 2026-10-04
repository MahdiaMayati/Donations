using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class StorageLocationConfiguration : IEntityTypeConfiguration<StorageLocation>
{
    public void Configure(EntityTypeBuilder<StorageLocation> builder)
    {
        builder.ToTable("StorageLocations");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(s => s.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(s => s.WarehouseId);

        builder.HasIndex(s => new { s.WarehouseId, s.Code })
            .IsUnique();

        // Align with Warehouse soft-delete filter so required FK navigation stays consistent.
        builder.HasQueryFilter(s => !s.Warehouse.IsDeleted);

        builder.HasOne(s => s.Warehouse)
            .WithMany(w => w.StorageLocations)
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
