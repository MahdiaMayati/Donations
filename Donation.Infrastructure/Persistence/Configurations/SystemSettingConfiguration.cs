using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Value)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(s => s.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Unique key per organization scope (NULL OrganizationId = global settings).
        builder.HasIndex(s => new { s.OrganizationId, s.Key })
            .IsUnique();

        builder.HasIndex(s => s.Type);
        builder.HasIndex(s => s.OrganizationId);

        builder.HasOne(s => s.Organization)
            .WithMany(o => o.SystemSettings)
            .HasForeignKey(s => s.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
