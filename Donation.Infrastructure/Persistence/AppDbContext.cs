using Donation.Application.Abstractions.Persistence;
using Donation.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Donation.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public new DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Donor> Donors => Set<Donor>();
    public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();
    public DbSet<FamilyMember> FamilyMembers => Set<FamilyMember>();
    public DbSet<Volunteer> Volunteers => Set<Volunteer>();
    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
    public DbSet<ItemType> ItemTypes => Set<ItemType>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<StorageLocation> StorageLocations => Set<StorageLocation>();
    public DbSet<Color> Colors => Set<Color>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<DonationRequest> DonationRequests => Set<DonationRequest>();
    public DbSet<DonationRequestPhoto> DonationRequestPhotos => Set<DonationRequestPhoto>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemPhoto> ItemPhotos => Set<ItemPhoto>();
    public DbSet<ItemColor> ItemColors => Set<ItemColor>();
    public DbSet<ItemStatusHistory> ItemStatusHistories => Set<ItemStatusHistory>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<TaskType> TaskTypes => Set<TaskType>();

    public Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
        => Database.BeginTransactionAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        modelBuilder.Entity<User>().ToTable("Users");

        modelBuilder.Entity<UserRole>(b =>
        {
            b.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            b.HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId);
        });
    }
}
