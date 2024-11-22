namespace MapsProxyApi.Services
{
    public interface IProxyService
    {
        public Task<HttpResponseMessage> SendAsync(HttpContext context, string serviceName, string path, string query);
    }
}