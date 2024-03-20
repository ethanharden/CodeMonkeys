using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Interfaces;

namespace SchedulingSystemWeb.Pages.Student.Bookings
{
    public class UpsertModel : PageModel
    {
       private readonly UnitOfWork _unitOfWork;

        public UpsertModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void OnGet()
        {

        }
    }
}
