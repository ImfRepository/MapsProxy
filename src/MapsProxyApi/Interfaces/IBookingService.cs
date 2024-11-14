namespace MapsProxyApi.Interfaces
{
    public interface IBookingService
    {
        public Task<List<string>> GetAllServiceNames();
        public Task<int> BookRequestsFor(string serviceName);
    }
}
