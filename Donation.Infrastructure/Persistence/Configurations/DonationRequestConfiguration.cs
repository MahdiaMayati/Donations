using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class DonationRequestConfiguration : IEntityTypeConfiguration<DonationRequest>
{
    public void Configure(EntityTypeBuilder<DonationRequest> builder)
    {
        builder.ToTable("DonationRequests");
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(d => d.DeliveryMethod)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(d => d.EstimatedItemCount)
            .IsRequired();

        builder.Property(d => d.SubmittedAt)
            .IsRequired();

        builder.Property(d => d.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.HasIndex(d => d.OrganizationId);
        builder.HasIndex(d => d.DonorId);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.DeliveryMethod);
        builder.HasIndex(d => new { d.OrganizationId, d.Status });

        builder.HasQueryFilter(d => !d.IsDeleted);

        builder.HasOne(d => d.Organization)
            .WithMany(o => o.DonationRequests)
            .HasForeignKey(d => d.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.Donor)
            .WithMany(donor => donor.DonationRequests)
            .HasForeignKey(d => d.DonorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.PickupAddress)
            .WithMany()
            .HasForeignKey(d => d.PickupAddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
