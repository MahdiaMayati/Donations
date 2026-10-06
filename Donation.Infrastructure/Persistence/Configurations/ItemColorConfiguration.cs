using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class ItemColorConfiguration : IEntityTypeConfiguration<ItemColor>
{
    public void Configure(EntityTypeBuilder<ItemColor> builder)
    {
        builder.ToTable("ItemColors");
        builder.HasKey(ic => new { ic.ItemId, ic.ColorId });

        builder.HasOne(ic => ic.Item)
            .WithMany(i => i.ItemColors)
            .HasForeignKey(ic => ic.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ic => ic.Color)
            .WithMany(c => c.ItemColors)
            .HasForeignKey(ic => ic.ColorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ic => ic.ColorId);
    }
}
