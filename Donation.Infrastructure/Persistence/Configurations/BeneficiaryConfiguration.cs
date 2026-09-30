using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class BeneficiaryConfiguration : IEntityTypeConfiguration<Beneficiary>
{
    public void Configure(EntityTypeBuilder<Beneficiary> builder)
    {
        builder.ToTable("Beneficiaries");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(b => b.IdPhotoUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.VerificationStatus)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(b => b.UserId).IsUnique();
        builder.HasIndex(b => b.AddressId);
        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Address)
            .WithMany()
            .HasForeignKey(b => b.AddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
