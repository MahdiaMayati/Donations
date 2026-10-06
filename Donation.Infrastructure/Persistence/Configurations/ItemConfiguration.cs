using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(i => i.Barcode)
            .HasMaxLength(100);

        builder.Property(i => i.TargetGender)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.AgeGroup)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Size)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Season)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.Condition)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.SortingStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.AvailabilityStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(i => i.SorterNotes)
            .HasMaxLength(2000);

        builder.Property(i => i.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.HasIndex(i => i.OrganizationId);
        builder.HasIndex(i => i.DonationRequestId);
        builder.HasIndex(i => i.ItemTypeId);
        builder.HasIndex(i => i.MaterialId);
        builder.HasIndex(i => i.Barcode)
            .IsUnique()
            .HasFilter("[Barcode] IS NOT NULL AND [IsDeleted] = 0");
        builder.HasIndex(i => i.SortingStatus);
        builder.HasIndex(i => i.AvailabilityStatus);
        builder.HasIndex(i => i.StorageLocationId);
        builder.HasIndex(i => i.SortedByUserId);

        builder.HasQueryFilter(i => !i.IsDeleted);

        builder.HasOne(i => i.Organization)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.DonationRequest)
            .WithMany(r => r.Items)
            .HasForeignKey(i => i.DonationRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.ItemType)
            .WithMany(t => t.Items)
            .HasForeignKey(i => i.ItemTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Material)
            .WithMany(m => m.Items)
            .HasForeignKey(i => i.MaterialId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.SortedByUser)
            .WithMany()
            .HasForeignKey(i => i.SortedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.StorageLocation)
            .WithMany()
            .HasForeignKey(i => i.StorageLocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
