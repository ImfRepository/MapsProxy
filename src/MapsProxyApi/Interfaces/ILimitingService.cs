namespace MapsProxyApi.Interfaces
{
    public interface ILimitingService
    {
        public Task<Dictionary<string, int>> GetAllAvailableRequests();
        public Task<int> GetAvailableRequestsTo(string service);
        public Task<bool> IsAvailableToUse(string service);
        public Task<bool> TryBookRequestsFor(string serviceName);
    }
}
