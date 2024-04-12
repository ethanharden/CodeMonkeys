using DataAccess;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystem.Pages.Admin.Users
{
    [Authorize(Roles = "ADMIN")]
    public class UserIndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserIndexModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
        public Dictionary<string, List<string>> UserRoles { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }

        public async Task OnGetAsync(bool success = false, string message = null)
        {
            Success = Success;
            Message = message;
            UserRoles = new Dictionary<string, List<string>>();
            ApplicationUsers = _unitOfWork.ApplicationUser.GetAll();
            foreach (var user in ApplicationUsers)
            {
                var userRole = await _userManager.GetRolesAsync(user);
                UserRoles.Add(user.Id, userRole.ToList());
            }
        }

        public async Task<IActionResult> OnPostLockUnlock(string id)
        {
            var user = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == id);
            if (user == null)
            {
                return Page(); // Or handle the error as appropriate
            }

            if (user.LockoutEnd == null || user.LockoutEnd <= DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(100);
                user.LockoutEnabled = true;
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
                user.LockoutEnabled = false;
            }

            _unitOfWork.ApplicationUser.Update(user);
            await _unitOfWork.CommitAsync();
            return RedirectToPage(new { success = true, message = "User updated successfully" });
        }
    }
}
