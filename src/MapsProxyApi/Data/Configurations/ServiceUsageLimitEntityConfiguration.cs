using MapsProxyApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapsProxyApi.Data.Configurations
{
    public class ServiceUsageLimitEntityConfiguration : IEntityTypeConfiguration<ServiceUsageLimitEntity>
    {
        public void Configure(EntityTypeBuilder<ServiceUsageLimitEntity> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.UsageLimits)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            builder.HasOne(x => x.Service)
                .WithMany()
                .HasForeignKey(x => x.ServiceId)
                .IsRequired();
        }
    }
}
