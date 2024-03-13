using DataAccess;
using Infrastructure.Models;
//using Microsoft.AspNet.Identity;

//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystemWeb.Pages.Student
{
    public class ListOfProvidersModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public IEnumerable<ApplicationUser> objApplicationUserList;
        public ListOfProvidersModel(UnitOfWork unitofWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitofWork;
            objApplicationUserList = new List<ApplicationUser>();
            _userManager = userManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            objApplicationUserList = await _userManager.GetUsersInRoleAsync("TEACHER");
            //objApplicationUserList = _unitOfWork.ApplicationUser.GetAll(null);
            return Page();


        }
    }
}
