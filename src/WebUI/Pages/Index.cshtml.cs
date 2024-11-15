using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using WebUI.Data;
using WebUI.Domain.Entities;

namespace WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private static HttpClient HttpClient;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public IList<ServiceEntity> _services;

        public IndexModel(IDbContextFactory<AppDbContext> contextFactory, IConfiguration config)
        {
            _contextFactory = contextFactory;
            var certificate = new X509Certificate2(
                config["ASPNETCORE_Kestrel:Certificates:Default:Path"]!,
                config["ASPNETCORE_Kestrel:Certificates:Default:Password"]!);

            var handler = new HttpClientHandler();
            handler.ClientCertificates.Add(certificate);

            HttpClient = new HttpClient(handler);
        }

        public async Task OnGet()
        {
            await RequestData();
        }

        public async Task OnPostAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            _services = await context.Services
                .AsTracking()
                .ToListAsync();

            foreach (var service in _services)
            {
                service.UsedTimes = 0;
            }

            await context.SaveChangesAsync();
        }

        private async Task RequestData()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            _services = await context.Services
                .AsNoTracking()
                .ToListAsync();

            foreach (var service in _services)
            {
                var url = $"http://maps-proxy-api:8080/api/stats/{service.Name}";
                var notUsed = await HttpClient.GetFromJsonAsync<int>(url);
                service.UsedTimes -= notUsed;

                if (service.UsedTimes < 0)
                    service.UsedTimes = 0;
            }
        }
    }
}
