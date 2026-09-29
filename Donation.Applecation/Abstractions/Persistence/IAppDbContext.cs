using Donation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Donation.Application.Abstractions.Persistence;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<City> Cities { get; }
    DbSet<Area> Areas { get; }
    DbSet<Address> Addresses { get; }
    DbSet<Organization> Organizations { get; }
    DbSet<Donor> Donors { get; }
    DbSet<Beneficiary> Beneficiaries { get; }
    DbSet<FamilyMember> FamilyMembers { get; }
    DbSet<Volunteer> Volunteers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
