using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Domain.Entities;
using WebUI.Services;

namespace WebUI.Pages
{
    public class ChangeLimitsModel : PageModel
    {
        private readonly StatsService _statsService;
        public IEnumerable<ServiceEntity> _services;

        public ChangeLimitsModel(StatsService statsService)
        {
            _statsService = statsService;
        }

        public async Task OnGet()
        {
            _services = await _statsService.GetAsync();
        }

        public async Task OnPostAsync(string serviceName, int limit)
        {
            await _statsService.SetLimitForAsync(serviceName, limit);
            _services = await _statsService.GetAsync();
        }
    }
}
