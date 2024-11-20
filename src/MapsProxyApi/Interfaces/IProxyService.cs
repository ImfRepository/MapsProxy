namespace MapsProxyApi.Services
{
    public interface IProxyService
    {
        public Task<HttpResponseMessage> GetAsync(string serviceName, string path, string query);
    }
}