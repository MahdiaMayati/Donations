using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Donation.Infrastructure.Persistence.Configurations;

public class VolunteerConfiguration : IEntityTypeConfiguration<Volunteer>
{
    public void Configure(EntityTypeBuilder<Volunteer> builder)
    {
        builder.ToTable("Volunteers");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(v => v.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Active");

        builder.Property(v => v.Days)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.HoursCount)
            .IsRequired();

        builder.Property(v => v.Hobbies)
            .HasMaxLength(500);

        builder.Property(v => v.Skills)
            .HasMaxLength(500);

        builder.Property(v => v.Experiences)
            .HasMaxLength(1000);

        builder.Property(v => v.NeglectedTasksCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(v => v.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(v => v.UserId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        builder.HasIndex(v => v.OrganizationId);
        builder.HasQueryFilter(v => !v.IsDeleted);

        builder.HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Organization)
            .WithMany()
            .HasForeignKey(v => v.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
