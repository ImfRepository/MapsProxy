using MapsProxyApi.Data;
using MapsProxyApi.Domain.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace MapsProxyApi.Services
{
    public class ProxyWithMemCacheService : IProxyService
    {
        private readonly IMemoryCache _cache;
        private readonly IDbContextFactory<AppDbContext> _factory;
        private readonly string _url;

        private static HttpClient _client = _client = new HttpClient();

        public ProxyWithMemCacheService(IMemoryCache cache, 
            IDbContextFactory<AppDbContext> factory, IConfiguration config)
        {
            _cache = cache;
            _factory = factory;
            _url = config["MAPS_URL"]!;
        }

        public async Task<string> Proxy(string token, string service, string path, string query)
        {
            query = RemoveTokenFrom(query);
            var url = $"{_url}{service}/{path}{query}";

            var getData = _client.GetStringAsync(url);
            var incCounter = TryIncCounter(token, service);

            await Task.WhenAll(incCounter, getData);

            if (!incCounter.Result)
                return $"{token} reached request limit for service {service}.";

            return getData.Result;
        }

        public string RemoveTokenFrom(string query)
        {
            var span = query.AsSpan();
            var length = span.IndexOf(['&', 't', 'o', 'k', 'e', 'n', '=']);
            return span[..length].ToString();
        }

        private async Task<bool> TryIncCounter(string token, string service)
        {
            var key = $"{token} limits for service {service}";
            
            if(!_cache.TryGetValue(key, out CachedLimitDto limit))
            {
                limit = GetCachedLimitDtoAsync(token, service);

                if (limit.UsedTimes >= limit.MaxUses)
                    return false;
            }

            limit.UsedTimes += 1;
            _cache.Set(key, limit, GetOptions());
            return true;
        }

        private CachedLimitDto GetCachedLimitDtoAsync(string token, string service)
        {
            using var context = _factory.CreateDbContext();

            var entity = context.UsageLimits
                .Where(x => x.User.Token == token)
                .Where(x => x.Service.Name == service)
                .FirstOrDefault();

            var dto = new CachedLimitDto
            {
                UsedTimes = null == entity ? 0 : entity.UsedTimes,
                MaxUses = null == entity ? 0 : entity.MaxUses,
            };

            return dto;
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

            using var context = _factory.CreateDbContext();

            var key = (cacheKey as string).AsSpan();
            var tokenLastIndex = key.IndexOf([' ', 'l', 'i', 'm', 'i', 't', 's']);
            var token = key[..tokenLastIndex].ToString();

            var serviceFirstIndex = key.IndexOf(['s', 'e', 'r', 'v', 'i', 'c', 'e', ' ']) + 8;
            var service = key[serviceFirstIndex..].ToString();

            var entity = context.UsageLimits
                .Where(x => x.User.Token == token)
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
