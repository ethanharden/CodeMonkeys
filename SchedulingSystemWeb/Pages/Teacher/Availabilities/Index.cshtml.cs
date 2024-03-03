using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess;
using Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SchedulingSystemWeb.Pages.Availabilities
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        public IEnumerable<Availability> AvailabilitiesList { get; set; }
        public string CalendarEvents { get; set; }

        public IndexModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void OnGet()
        {
            AvailabilitiesList = _unitOfWork.Availability.GetAll(includes: "ProviderProfile,ProviderProfile.User");

            var events = AvailabilitiesList.Select(a => new
            {
                title = $"Availability for {a.ProviderProfile.User.UserName}", // Adjust according to actual data model
                start = a.StartTime.ToString("s"), // ISO format
                end = a.EndTime.ToString("s"),
                allDay = false
            }).ToList();
            CalendarEvents = System.Text.Json.JsonSerializer.Serialize(events);
        }
    }

}
