namespace MapsProxyApi.Services
{
    public interface IProxyService
    {
        public Task<string> GetAsync(string serviceName, string path, string query);
    }
}