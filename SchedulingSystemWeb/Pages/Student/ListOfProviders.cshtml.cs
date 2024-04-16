using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SchedulingSystemWeb.Pages.Student
{
    [Authorize(Roles = "STUDENT, TUTOR")]
    public class ListOfProvidersModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        public Dictionary<string, IList<string>> UserRoles;
        public IEnumerable<Department> departmentList;
        public IList<IdentityRole> Roles;
        public IList<ProviderProfile> Providers;

        public ListOfProvidersModel(UnitOfWork unitofWork, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitofWork;      
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGetAsync(string search, string role)
        {
            departmentList = _unitOfWork.Department.GetAll();
            Roles = await _roleManager.Roles.ToListAsync();


            return Page();
        }

        public async Task<IActionResult> OnGetListProvidersAsync(string role, int department)
        {
            var providerProfiles = _unitOfWork.ProviderProfile
                .GetAll(pp => pp.DeparmentId == department /*&&  Condition to match the role */)
                .Select(pp => new { pp.Id, pp.userFullName })
                .ToList();

            return new JsonResult(providerProfiles);
        }

        public async Task<IActionResult> OnPostAsync(string role, int department, int locationTypeId)
        {
            TempData["SelectedRole"] = role;
            TempData["SelectedDepartment"] = department.ToString();
            TempData["SelectedLocationTypeId"] = locationTypeId;

            return RedirectToPage("/Student/Bookings/Index", new { role = role, department = department, locationTypeId = locationTypeId});
        }
        

    }
}
