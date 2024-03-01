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

        public IEnumerable<Availability> objAvailabilitiesList { get; set; }
        public string CalendarEvents { get; set; }

        public IndexModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void OnGet()
        {
            objAvailabilitiesList = _unitOfWork.Availability.GetAll();
            var events = objAvailabilitiesList.Select(a => new
            {
                title = a.isUnavailable ? "Unavailable" : "Available",
                start = a.StartTime,
                end = a.EndTime,
                allDay = false
            }).ToList();
            CalendarEvents = JsonSerializer.Serialize(events);
        }
    }
}
