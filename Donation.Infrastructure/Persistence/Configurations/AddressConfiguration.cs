using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Addresses");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(a => a.Details)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasOne(a => a.Area)
            .WithMany(area => area.Addresses)
            .HasForeignKey(a => a.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.AreaId);
        builder.HasIndex(a => a.UserId);
    }
}
