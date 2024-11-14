using Microsoft.EntityFrameworkCore;
using MapsProxyApi.Data;
using MapsProxyApi.Interfaces;

namespace MapsProxyApi.Services
{
    public class BookingService : IBookingService
    {
        private readonly IDbContextFactory<AppDbContext> _factory;

        public BookingService(IDbContextFactory<AppDbContext> factory)
        {
            _factory = factory;
        }
            
        public async Task<List<string>> GetAllServiceNames()
        {
            using var context = await _factory.CreateDbContextAsync();
            return await context.Services.Select(s => s.Name).ToListAsync();
        }

        public async Task<int> BookRequestsFor(string serviceName)
        {
            using var context = await _factory.CreateDbContextAsync();
            var service = await context.Services
                .Where(x => serviceName == x.Name)
                .AsTracking()
                .FirstOrDefaultAsync();

            if (null == service)
                return 0;

            var bookedRequestAmount = Math.Min(service.MaxUses - service.UsedTimes, 50);

            service.UsedTimes += bookedRequestAmount;

            context.SaveChanges();

            return bookedRequestAmount;
        }
    }
}
