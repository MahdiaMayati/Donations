using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class ItemPhotoConfiguration : IEntityTypeConfiguration<ItemPhoto>
{
    public void Configure(EntityTypeBuilder<ItemPhoto> builder)
    {
        builder.ToTable("ItemPhotos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(p => p.Url)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasIndex(p => p.ItemId);

        builder.HasOne(p => p.Item)
            .WithMany(i => i.Photos)
            .HasForeignKey(p => p.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
