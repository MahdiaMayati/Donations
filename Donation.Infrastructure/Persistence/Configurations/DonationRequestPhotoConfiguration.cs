using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class DonationRequestPhotoConfiguration : IEntityTypeConfiguration<DonationRequestPhoto>
{
    public void Configure(EntityTypeBuilder<DonationRequestPhoto> builder)
    {
        builder.ToTable("DonationRequestPhotos");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(p => p.Url)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasIndex(p => p.DonationRequestId);

        builder.HasOne(p => p.DonationRequest)
            .WithMany(r => r.Photos)
            .HasForeignKey(p => p.DonationRequestId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
