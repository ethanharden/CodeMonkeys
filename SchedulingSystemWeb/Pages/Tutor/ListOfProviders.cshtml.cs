using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace SchedulingSystemWeb.Pages.Tutor
{
    public class ListOfProvidersModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public List<ApplicationUser> objApplicationUserList;
        public Dictionary<string, IList<string>> UserRoles;
        public ListOfProvidersModel(UnitOfWork unitofWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitofWork;
            objApplicationUserList = new List<ApplicationUser>();
            _userManager = userManager;
            UserRoles = new Dictionary<string, IList<string>>(); // Initialize user roles dictionary
        }
        //public async Task<IActionResult> OnGetAsync(string search, string role)
        //{
        //    if (!string.IsNullOrEmpty(role))
        //    {
        //        objApplicationUserList.AddRange(await _userManager.GetUsersInRoleAsync(role));

        //    }
        //    else
        //    {
        //        objApplicationUserList.AddRange(await _userManager.GetUsersInRoleAsync("TUTOR"));
        //        objApplicationUserList.AddRange(await _userManager.GetUsersInRoleAsync("TEACHER"));
        //        objApplicationUserList.AddRange(await _userManager.GetUsersInRoleAsync("ADVISOR"));
        //    }
        //    if (!string.IsNullOrEmpty(search))
        //    {
        //        objApplicationUserList = objApplicationUserList.Where(u => u.FullName.ToLower().Contains(search.ToLower())).ToList();

        //    }

        //    foreach (var user in objApplicationUserList)
        //    {
        //        UserRoles[user.Id] = await _userManager.GetRolesAsync(user);
        //    }

        //    return Page();

        //}

        public async Task<IActionResult> OnPostAsync(string role, int department)
        {

            return RedirectToPage("/Student/Bookings/Index", new { role = role, department = department });
        }


    }
}
