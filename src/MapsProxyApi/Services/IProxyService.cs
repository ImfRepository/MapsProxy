namespace MapsProxyApi.Services
{
    public interface IProxyService
    {
        public Task<string> GetAsync(string service, string path, string query);
    }
}