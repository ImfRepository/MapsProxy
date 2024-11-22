using System.Collections.Concurrent;
using MapsProxyApi.Interfaces;

namespace MapsProxyApi.Services
{
    public class LimitingService : ILimitingService
    {
        private readonly ConcurrentDictionary<string, int> _availibleAmounts;
        private readonly IBookingService _bookingService;

        public LimitingService(IBookingService bookingService)
        {
            _bookingService = bookingService;
            _availibleAmounts = new ConcurrentDictionary<string, int>();
            Init().Wait();
        }

        public Task<Dictionary<string, int>> GetAvailableRequests()
        {
            return Task.FromResult(_availibleAmounts
                .Where(x => 0 < x.Value)
                .ToDictionary());
        }

        public Task<int> GetAvailableRequestsTo(string service)
        {
            return GetOrInit(service);
        }

        public async Task<bool> IsAvailableToUse(string serviceName)
        {
            if(serviceName.Contains("C01_Belarus_WGS84"))
                return true;

            if (0 >= await GetOrInit(serviceName))
                return false;

            _availibleAmounts[serviceName] -= 1;

            // last available request tries to book new
            if (0 >= await GetOrInit(serviceName))
                _availibleAmounts[serviceName] = await _bookingService.BookRequestsFor(serviceName);

            return true;
        }

        public async Task<bool> TryBookRequestsFor(string serviceName)
        {
            await GetOrInit(serviceName);
            var amount = await _bookingService.BookRequestsFor(serviceName);
            _availibleAmounts[serviceName] += amount;
            return amount != 0;
        }

        private async Task Init()
        {
            var serviceNames = await _bookingService.GetAllServiceNames();
            foreach (var serviceName in serviceNames)
            {
                await TryBookRequestsFor(serviceName);
            }
        }

        private Task<int> GetOrInit(string service)
        {
            return Task.FromResult(_availibleAmounts.GetOrAdd(service, 0));
        }
    }
}
