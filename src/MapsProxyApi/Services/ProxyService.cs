using MapsProxyApi.Interfaces;

namespace MapsProxyApi.Services
{
    public class ProxyService : IProxyService
    {
        private readonly IRecordingService _recordingService;
        private readonly string _url;

        private static HttpClient _client = _client = new HttpClient();

        public ProxyService(IRecordingService recordingService,
            IConfiguration config)
        {
            _recordingService = recordingService;
            _url = config["MAPS_URL"]!;
        }

        public async Task<string> GetAsync(string serviceName, string path, string query)
        {
            var url = $"{_url}{serviceName}/{path}{query}";

            var getDataTask = _client.GetStringAsync(url);
            var incCounterTask = _recordingService.TryRecordTheRequestTo(serviceName);

            await Task.WhenAll(incCounterTask, getDataTask);

            if (!incCounterTask.Result)
                return $"{serviceName} reached request limit.";

            return getDataTask.Result;
        }
    }
}
