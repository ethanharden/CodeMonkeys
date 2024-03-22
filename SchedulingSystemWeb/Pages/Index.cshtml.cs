using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystemWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            if (User.IsInRole("STUDENT"))
            {
                RedirectToPage("/student/home/index");
            }
            else
            {
                RedirectToPage("/Teacher/Availabilities");
            }
        }
    }
}
