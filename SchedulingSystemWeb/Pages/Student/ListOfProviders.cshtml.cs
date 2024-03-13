using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystemWeb.Pages.Student
{
    public class ListOfProvidersModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;

        public IEnumerable<ApplicationUser> objApplicationUserList;
        public ListOfProvidersModel(UnitOfWork unitofWork)
        {
            _unitOfWork = unitofWork;
            objApplicationUserList = new List<ApplicationUser>();
        }
        public IActionResult OnGet()
        {
            objApplicationUserList = _unitOfWork.ApplicationUser.GetAll(null);
            return Page();
        }
    }
}
