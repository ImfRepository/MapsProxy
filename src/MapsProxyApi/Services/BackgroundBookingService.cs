using MapsProxyApi.Interfaces;

namespace MapsProxyApi.Services
{
    public class BackgroundBookingService : IHostedService
    {
        private readonly ILimitingService _limitingService;
        private readonly IBookingService _bookingService;
        private Timer? _timer = null;

        public BackgroundBookingService(ILimitingService limitingService,
            IBookingService bookingService)
        {
            _limitingService = limitingService;
            _bookingService = bookingService;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object? state) => BookNewRequests().Wait();

        private async Task BookNewRequests()
        {
            var serviceNames = (await _bookingService.GetAllServiceNames()).ToArray();
            Parallel.For(0, serviceNames.Length, async (int i) =>
            {
                if (0 < await _limitingService.GetAvailableRequestsTo(serviceNames[i]))
                {
                    await _limitingService.TryBookRequestsFor(serviceNames[i]);
                }
            });
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
