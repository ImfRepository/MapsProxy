using System.Collections.Concurrent;
using MapsProxyApi.Interfaces;

namespace MapsProxyApi.Services
{
    public class RecordingService : IRecordingService
    {
        private readonly ConcurrentDictionary<string, int> _availibleAmounts;
        private readonly IBookingService _bookingService;

        public RecordingService(IBookingService bookingService)
        {
            _bookingService = bookingService;
            _availibleAmounts = new ConcurrentDictionary<string, int>();
            Init().Wait();
        }

        public Task<int> GetAvailableRequestAmountTo(string service)
        {
            return Task.FromResult(GetOrInit(service));
        }

        public async Task<bool> TryRecordTheRequestTo(string service)
        {
            if (0 >= GetOrInit(service))
                return false;

            _availibleAmounts[service] -= 1;

            if (0 >= GetOrInit(service))
                _availibleAmounts[service] = await _bookingService.BookRequestsFor(service);

            return true;
        }

        private async Task Init()
        {
            var serviceNames = await _bookingService.GetAllServiceNames();
            foreach (var serviceName in serviceNames)
            {
                GetOrInit(serviceName);
                _availibleAmounts[serviceName] = await _bookingService.BookRequestsFor(serviceName);
            }
        }

        private int GetOrInit(string service)
        {
            return _availibleAmounts.GetOrAdd(service, 0);
        }
    }
}
