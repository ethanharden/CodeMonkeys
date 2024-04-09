using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SchedulingSystem.Pages.Admin.Users
{
    public class UpdateModel : PageModel
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UpdateModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        [BindProperty]
        public ApplicationUser AppUser { get; set; }
        public List<string> UsersRoles { get; set; }
        public List<string> AllRoles { get; set; }
        public List<string> OldRoles { get; set; }
        public IEnumerable<SelectListItem> DepartmentList { get; set; }
        [BindProperty]
        public string Department { get; set; }
        public async Task OnGet(string id)
        {
            AppUser = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            var roles = await _userManager.GetRolesAsync(AppUser);
            UsersRoles = roles.ToList();
            OldRoles = roles.ToList();
            AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            DepartmentList = _unitOfWork.Department.GetAll().Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Id.ToString(),
            });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var newRoles = Request.Form["roles"];
            UsersRoles = newRoles.ToList();
            var OldRoles = await _userManager.GetRolesAsync(AppUser);  //ones in DB
            var rolesToAdd = new List<string>();
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == AppUser.Id);

            user.FirstName = AppUser.FirstName;
            user.LastName = AppUser.LastName;
            user.Email = AppUser.Email;
            user.PhoneNumber = AppUser.PhoneNumber;
            _unitOfWork.ApplicationUser.Update(user);
            _unitOfWork.Commit();
            if (newRoles != OldRoles)
            {
                //update their roles
                if (_unitOfWork.ProviderProfile.Get(i => i.User == user.Id) != null) // Teachers need profile deleted
                {
                    var profile = _unitOfWork.ProviderProfile.Get(i => i.User == user.Id);
                    if (_unitOfWork.Availability.GetAll(a => a.ProviderProfileID == profile.Id) != null) //Deletes all availibilities
                    {
                        foreach(var aval in _unitOfWork.Availability.GetAll(a => a.ProviderProfileID == profile.Id))
                        {
                            _unitOfWork.Availability.Delete(aval);
                        }
                    }
                    if (_unitOfWork.Booking.GetAll(b=> b.User == user.Id || b.ProviderProfileID == profile.Id) != null) // Deletes all bookings (for tutors) & Regular bookings
                    {
                        foreach (var booking in (_unitOfWork.Booking.GetAll(b => b.User == user.Id || b.ProviderProfileID == profile.Id)))
                        {
                            _unitOfWork.Booking.Delete(booking);
                        }
                    }
                    _unitOfWork.ProviderProfile.Delete(profile);
                    _unitOfWork.Commit();
                    
                }
                if (_unitOfWork.CustomerProfile.Get(i => i.User == user.Id) != null) // Student need profile deleted
                {
                    var profile = _unitOfWork.CustomerProfile.Get(i => i.User == user.Id);
                    if (_unitOfWork.Booking.GetAll(b => b.User == user.Id) != null) // Deletes all bookings
                    {
                        foreach (var booking in (_unitOfWork.Booking.GetAll(b => b.User == user.Id)))
                        {
                            _unitOfWork.Booking.Delete(booking);
                        }
                    }
                    _unitOfWork.CustomerProfile.Delete(profile);
                    _unitOfWork.Commit();
                }

                foreach (var r in UsersRoles)
                {
                    if (!OldRoles.Contains(r)) //new Role
                    {
                        rolesToAdd.Add(r);
                    }
                }

                foreach (var r in OldRoles)
                {
                    if (!UsersRoles.Contains(r))  //remove
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user, r);
                    }
                }
                var result1 = await _userManager.AddToRolesAsync(user, rolesToAdd.AsEnumerable());
                if (rolesToAdd.Contains("STUDENT")) // Add student profile
                {
                    var newprofile = new CustomerProfile();
                    newprofile.User = user.Id;
                   
                    _unitOfWork.CustomerProfile.Add(newprofile);
                    _unitOfWork.Commit();
                }
                if (rolesToAdd.Contains("TEACHER") || rolesToAdd.Contains("ADVISOR") || rolesToAdd.Contains("TUTOR")) // Add Teacher profile
                {
                    var newprofile = new ProviderProfile();
                    newprofile.User = user.Id;
                    if (Department != null) { newprofile.DeparmentId = Int32.Parse(Department); }
                    newprofile.userFullName = user.FullName;
                    _unitOfWork.ProviderProfile.Add(newprofile);

                    _unitOfWork.Commit();
                }
            }
            return RedirectToPage("./UserIndex", new { sucess = true, message = "Update Successful" });
        }
    }
}