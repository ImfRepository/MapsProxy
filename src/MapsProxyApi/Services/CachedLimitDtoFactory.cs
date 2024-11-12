using MapsProxyApi.Data;
using MapsProxyApi.Domain.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MapsProxyApi.Services
{
    public class CachedLimitDtoFactory
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public CachedLimitDtoFactory(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }

        public CachedLimitDto Get(string service)
        {
            using var context = _factory.CreateDbContext();

            var entity = context.UsageLimits
                .Where(x => x.Service.Name == service)
                .FirstOrDefault();

            var dto = new CachedLimitDto
            {
                UsedTimes = null == entity ? 0 : entity.UsedTimes,
                MaxUses = null == entity ? 0 : entity.MaxUses,
            };

            return dto;
        }

    }
}
