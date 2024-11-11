namespace MapsProxyApi.Services
{
    public interface IProxyService
    {
        public Task<string> Proxy(string token, string service, string path, string query);
    }
}