using MapsProxyApi.Data;
using MapsProxyApi.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace MapsProxyApi.Services
{
    public class ProxyTestsService
    {
        private readonly IDistributedCache _cache;
        private readonly object _lock = new object();
        private readonly AppDbContext _context;
        private readonly string _url;

        private static HttpClient _client = _client = new HttpClient();

        public ProxyTestsService(IDistributedCache cache, AppDbContext context, IConfiguration config)
        {
            _cache = cache;
            _context = context;
            _url = config["MAPS_URL"]!;
        }

        public async Task<string> Proxy(string token, string service, string path, string query)
        {
            if (!TryIncCounter(token, service))
                throw new InvalidOperationException($"Reached uses limit for token {token}.");

            query = RemoveTokenFrom(query);

            var url = $"{_url}{service}/{path}{query}";

            var response = await _client.GetStringAsync(url);

            return response;
        }

        public string RemoveTokenFrom(string query)
        {
            var span = query.AsSpan();
            var length = span.IndexOf(['&', 't', 'o', 'k', 'e', 'n', '=']);
            return span[..length].ToString();
        }

        private bool TryIncCounter(string token, string serviceName)
        {
            lock(_lock)
            {
                var key = $"{token} limits for service {serviceName}";
                var jsonLimit = _cache.GetString(key);

                if (string.IsNullOrEmpty(jsonLimit))
                {
                    // db request
                    // var entity = db...
                    var entity = _context.UsageLimits
                        .Where(x => x.User.Token == token)
                        .Where(x => x.Service.Name == serviceName)
                        .FirstOrDefault();

                    var dto = new CachedLimitDto
                    {
                        UsedTimes = entity.UsedTimes,
                        MaxUses = entity.MaxUses,
                    };

                    if (dto.UsedTimes >= dto.MaxUses)
                        return false;

                    dto.UsedTimes += 1;
                    Console.WriteLine(dto.UsedTimes);
                    jsonLimit = JsonSerializer.Serialize(dto);

                    _cache.SetString(key, jsonLimit, GetOptions());
                    return true;
                }
                else
                {
                    var dto = JsonSerializer.Deserialize<CachedLimitDto>(jsonLimit)!;
                    if (dto.UsedTimes >= dto.MaxUses)
                        return false;

                    dto.UsedTimes += 1;
                    Console.WriteLine(dto.UsedTimes);
                    jsonLimit = JsonSerializer.Serialize(dto);

                    _cache.SetString(key, jsonLimit, GetOptions());
                    return true;
                }
            }
        }

        private DistributedCacheEntryOptions GetOptions() => new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
            .SetSlidingExpiration(TimeSpan.FromMinutes(3));
    } 
}
