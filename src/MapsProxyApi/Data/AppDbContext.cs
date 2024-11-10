using MapsProxyApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MapsProxyApi.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<ServiceEntity> Services { get; set; }
        public DbSet<ServiceUsageLimitEntity> UsageLimits { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
