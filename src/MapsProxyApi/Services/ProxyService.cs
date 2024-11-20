using MapsProxyApi.Interfaces;

namespace MapsProxyApi.Services
{
    public class ProxyService : IProxyService
    {
        private readonly ILimitingService _recordingService;
        private readonly string _url;

        private static HttpClient _client = _client = new HttpClient();

        public ProxyService(ILimitingService recordingService,
            IConfiguration config)
        {
            _recordingService = recordingService;
            _url = config["MAPS_URL"]!;
        }

        public async Task<string> GetAsync(string serviceName, string path, string query)
        {
            var url = $"{_url}{serviceName}/{path}{query}";

            if (await _recordingService.IsAvailableToUse(serviceName))
                return await _client.GetStringAsync(url);

            return $"{serviceName} reached request limit.";

            // better performance, can request when reach limit
            //var getDataTask = _client.GetStringAsync(url);
            //var incCounterTask = _recordingService.TryRecordTheRequestTo(serviceName);

            //await Task.WhenAll(incCounterTask, getDataTask);

            //if (!incCounterTask.Result)
            //    return $"{serviceName} reached request limit.";

            //return getDataTask.Result;
        }
    }
}
