using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Domain.Entities;
using WebUI.Services;

namespace WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly StatsService _statsService;
        public IEnumerable<ServiceEntity> _services;

        public IndexModel(StatsService statsService)
        {
            _statsService = statsService;
        }

        public async Task OnGet()
        {
            _services = await _statsService.GetAsync();
        }
    }
}
