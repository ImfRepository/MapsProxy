using WebUI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace WebUI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ServiceEntity> Services { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
