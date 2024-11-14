namespace MapsProxyApi.Interfaces
{
    public interface IRecordingService
    {
        public Task<int> GetAvailableRequestAmountTo(string service);
        public Task<bool> TryRecordTheRequestTo(string service);
    }
}
