using Microsoft.EntityFrameworkCore;
using StatsApi.Data;
using StatsApi.Domain.Entities;
using System.Security.Cryptography.X509Certificates;

public class StatsService
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public StatsService(AppDbContext context, IConfiguration config)
    {
        _context = context;
        var certificate = new X509Certificate2(
        config["ASPNETCORE_Kestrel:Certificates:Default:Path"]!,
        config["ASPNETCORE_Kestrel:Certificates:Default:Password"]!);

        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(certificate);

        _httpClient = new HttpClient(handler);
    }


    public async Task<IEnumerable<ServiceEntity>> GetAllLimits()
    {
        var services = await _context.Services
            .AsNoTracking()
            .ToListAsync();

        var url = $"http://maps-proxy-api:8080/api/stats/available/";
        var notUsed = await _httpClient.GetFromJsonAsync<Dictionary<string, int>>(url);

        foreach (var service in services)
        {
            service.UsedTimes -= notUsed![service.Name];

            if (service.UsedTimes < 0)
                service.UsedTimes = 0;
        }
        return services;
    }

    public async Task<bool> SetLimitFor(string serviceName, int limit)
    {
        var service = await _context.Services
            .AsTracking()
            .Where(x => serviceName == x.Name)
            .SingleOrDefaultAsync();

        if (service == null)
            return false;

        service.MaxUses = limit;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task ResetAllLimits()
    {
        var services = await _context.Services
            .ExecuteUpdateAsync(x => x.SetProperty(x => x.UsedTimes, 0));
    }
}
