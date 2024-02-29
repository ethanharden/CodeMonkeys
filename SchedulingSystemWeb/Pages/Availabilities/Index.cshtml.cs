using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess;
using Infrastructure.Models;
using System.Collections.Generic;

namespace SchedulingSystemWeb.Pages.Availabilities
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;

        public IEnumerable<Availability> objAvailabilitiesList { get; set; }

        public IndexModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void OnGet()
        {
            objAvailabilitiesList = _unitOfWork.Availability.GetAll();
        }
    }
}
