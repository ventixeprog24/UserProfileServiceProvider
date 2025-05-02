using Microsoft.EntityFrameworkCore;
using UserProfileServiceProvider.Data.Entities;

namespace UserProfileServiceProvider.Data.Contexts;

public class DataContext(DbContextOptions<DataContext> context) : DbContext(context)
{
    public virtual DbSet<UserProfileEntity> UserProfiles { get; set; }
    public virtual DbSet<UserProfileAddressEntity> UserProfileAddresses { get; set; }
}