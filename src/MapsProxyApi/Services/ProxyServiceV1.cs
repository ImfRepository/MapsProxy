namespace MapsProxyApi.Services
{
    public class ProxyServiceV1 : IProxyService
    {
        private readonly MemoryCacheService _cacheService;
        private readonly string _url;

        private static HttpClient _client = _client = new HttpClient();

        public ProxyServiceV1(MemoryCacheService cacheService,
            IConfiguration config)
        {
            _cacheService = cacheService;
            _url = config["MAPS_URL"]!;
        }

        public async Task<string> Proxy(string service, string path, string query)
        {
            var url = $"{_url}{service}/{path}{query}";

            var getDataTask = _client.GetStringAsync(url);
            var incCounterTask = _cacheService.TryIncCounter(service);

            await Task.WhenAll(incCounterTask, getDataTask);

            if (!incCounterTask.Result)
                return $"{service} reached request limit.";

            return getDataTask.Result;
        }
    }
}
