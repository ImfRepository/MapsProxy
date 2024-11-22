using MapsProxyApi.Extensions;
using MapsProxyApi.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.IO;

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

        public async Task<HttpResponseMessage> SendAsync(HttpContext context, string serviceName, string path, string query)
        {
            var uri = new Uri($"{_url}{serviceName}/{path}{query}");

            var request = context.GetRequestMessageAsync(uri);

            if (await _recordingService.IsAvailableToUse(serviceName))
                return await _client.SendAsync(request);

            return new HttpResponseMessage(System.Net.HttpStatusCode.NoContent);

            // better performance, can request when reached limit
            //var getDataTask = _client.GetStringAsync(url);
            //var incCounterTask = _recordingService.TryRecordTheRequestTo(serviceName);

            //await Task.WhenAll(incCounterTask, getDataTask);

            //if (!incCounterTask.Result)
            //    return $"{serviceName} reached request limit.";

            //return getDataTask.Result;
        }
    }
}
