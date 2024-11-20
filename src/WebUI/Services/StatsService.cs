using System.Security.Cryptography.X509Certificates;
using WebUI.Domain.Entities;

namespace WebUI.Services
{
    public class StatsService
    {
        private static HttpClient HttpClient;
        public IEnumerable<ServiceEntity> _services;

        public StatsService(IConfiguration config)
        {
            var certificate = new X509Certificate2(
                config["ASPNETCORE_Kestrel:Certificates:Default:Path"]!,
                config["ASPNETCORE_Kestrel:Certificates:Default:Password"]!);

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);

            HttpClient = new HttpClient(handler);
        }

        public async Task SetLimitForAsync(string serviceName, int limit)
        {
            var url = $"http://maps-stats-api:8080/api/limits/{serviceName}?limit={limit}";
            await HttpClient.PostAsync(url, null);
        }

        public async Task<IEnumerable<ServiceEntity>> GetAsync()
        {
            var url = "http://maps-stats-api:8080/api/limits";
            return await HttpClient.GetFromJsonAsync<IEnumerable<ServiceEntity>>(url)
                ?? new List<ServiceEntity>();
        }
    }
}
