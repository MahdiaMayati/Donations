using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class ItemStatusHistoryConfiguration : IEntityTypeConfiguration<ItemStatusHistory>
{
    public void Configure(EntityTypeBuilder<ItemStatusHistory> builder)
    {
        builder.ToTable("ItemStatusHistory");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(h => h.OldStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(h => h.NewStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.ChangedAt)
            .IsRequired();

        builder.HasIndex(h => h.ItemId);
        builder.HasIndex(h => h.ChangedAt);
        builder.HasIndex(h => h.ChangedByUserId);

        builder.HasOne(h => h.Item)
            .WithMany(i => i.StatusHistory)
            .HasForeignKey(h => h.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.ChangedByUser)
            .WithMany()
            .HasForeignKey(h => h.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
