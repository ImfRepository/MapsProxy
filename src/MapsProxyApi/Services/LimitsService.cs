using MapsProxyApi.Entities;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MapsProxyApi.Services
{
    public class LimitsService
    {
        private readonly string _key_templete = @"{0} limits for service {1}";
        private readonly IDistributedCache _cache;

        public LimitsService(IDistributedCache cache) {
            _cache = cache;
        }

        public async Task<bool> IsAvailableFor(string idToken, string serviceName,
            CancellationToken cancellationToken = default)
        {
            var limit = await GetOrSetCacheFor(idToken, serviceName, cancellationToken);

            return limit.UsedTimes < limit.MaxUses;
        }

        public async Task FixUsage(string idToken, string serviceName,
            CancellationToken cancellationToken = default)
        {
            var limit = await GetOrSetCacheFor(idToken, serviceName, cancellationToken);

            if (limit.UsedTimes >= limit.MaxUses)
                throw new ArgumentOutOfRangeException();

            limit.UsedTimes += 1;

            var serializedLimit = JsonSerializer.Serialize(limit);
            var key = string.Format(_key_templete, idToken, serviceName);
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(15))
                .SetAbsoluteExpiration(DateTime.Now.AddSeconds(15));

            await _cache.SetStringAsync(key, serializedLimit, options, cancellationToken);
        }

        private async Task<ServiceUsageLimitEntity> GetOrSetCacheFor(string idToken, 
            string serviceName, CancellationToken cancellationToken = default)
        {
            var key = string.Format(_key_templete, idToken, serviceName);

            var serializedLimit = await _cache.GetStringAsync(key, cancellationToken);

            ServiceUsageLimitEntity limit;

            if (null != serializedLimit)
            {
                limit = JsonSerializer.Deserialize<ServiceUsageLimitEntity>(serializedLimit)
                        ?? throw new NullReferenceException();
            }
            else
            {
                // request from db
                limit = new ServiceUsageLimitEntity()
                {
                    UsedTimes = 0,
                    MaxUses = 50
                };

                // save in cache
                serializedLimit = JsonSerializer.Serialize(limit);
                var options = new DistributedCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(15))
                    .SetAbsoluteExpiration(DateTime.Now.AddSeconds(15));

                await _cache.SetStringAsync(key, serializedLimit, options, cancellationToken);
            }

            return limit;
        }
    }
}
