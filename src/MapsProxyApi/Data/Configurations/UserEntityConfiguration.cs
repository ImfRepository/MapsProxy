using MapsProxyApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MapsProxyApi.Data.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasMany(x => x.UsageLimits)
                .WithOne(x => x.User)
                .HasPrincipalKey(x => x.Id)
                .IsRequired();

            builder.HasIndex(x => x.Token);
        }
    }
}
