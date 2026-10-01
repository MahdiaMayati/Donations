using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class FamilyMemberConfiguration : IEntityTypeConfiguration<FamilyMember>
{
    public void Configure(EntityTypeBuilder<FamilyMember> builder)
    {
        builder.ToTable("FamilyMembers");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(f => f.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.ShoeSize)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(f => f.ClothingSize)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(f => f.Gender).IsRequired();

        builder.HasIndex(f => f.BeneficiaryId);
        builder.HasQueryFilter(f => !f.IsDeleted);

        builder.HasOne(f => f.Beneficiary)
            .WithMany(b => b.FamilyMembers)
            .HasForeignKey(f => f.BeneficiaryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
