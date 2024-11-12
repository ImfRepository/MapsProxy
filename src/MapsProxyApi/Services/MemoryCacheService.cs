using MapsProxyApi.Data;
using MapsProxyApi.Domain.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace MapsProxyApi.Services
{
    public class MemoryCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly CachedLimitDtoFactory _dtoFactory;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly SemaphoreSlim _semaphore;

        public MemoryCacheService(IMemoryCache cache, 
            CachedLimitDtoFactory dtoFactory, 
            IDbContextFactory<AppDbContext> contextFactory)
        {
            _cache = cache;
            _dtoFactory = dtoFactory;
            _contextFactory = contextFactory;
            _semaphore = new SemaphoreSlim(1,1);
        }

        public async Task<bool> TryIncCounter(string service)
        {
            if (!_cache.TryGetValue(service, out CachedLimitDto limit))
            {
                limit = _dtoFactory.Get(service);

                if (limit.UsedTimes >= limit.MaxUses)
                    return false;
            }

            await _semaphore.WaitAsync();

            _cache.TryGetValue(service, out limit);
            limit.UsedTimes += 1;
            _cache.Set(service, limit, GetOptions());

            _semaphore.Release();
            return true;
        }

        private MemoryCacheEntryOptions GetOptions() => new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(3))
            .SetSlidingExpiration(TimeSpan.FromMinutes(1))
            .RegisterPostEvictionCallback(PostEvictionCallback, _cache);

        private void PostEvictionCallback(
            object cacheKey, object cacheValue, EvictionReason evictionReason, object state)
        {
            if (evictionReason != EvictionReason.Expired)
                return;

            using var context = _contextFactory.CreateDbContext();

            var service = cacheKey as string;

            var entity = context.UsageLimits
                .Where(x => x.Service.Name == service)
                .AsTracking()
                .FirstOrDefault();

            var limit = cacheValue as CachedLimitDto;

            entity.UsedTimes = limit.UsedTimes;
            context.SaveChanges();

            Console.WriteLine($"Entry {cacheKey} was evicted: {evictionReason}.");
        }
    }
}
