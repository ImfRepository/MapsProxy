namespace MapsProxyApi.Interfaces
{
    public interface ILimitingService
    {
        public Task<int> GetAvailableRequestsTo(string service);
        public Task<bool> IsAvailableToUse(string service);
        public Task<bool> TryBookRequestsFor(string serviceName);
    }
}
