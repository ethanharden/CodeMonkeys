using DataAccess;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystemWeb.Pages.Teacher.Locations
{
    public class IndexModel : PageModel
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEnumerable<Location> LocationList { get; set; }
        public ProviderProfile providerProfile {  get; set; }

        public IndexModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public void OnGet()
        {
            providerProfile = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User));
            //LocationList = _unitOfWork.Location.GetAll().Where(l=>l.profileId == providerProfile.Id);
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingProfile = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User));
            if (existingProfile != null)
            {
                existingProfile.RemoteLink = providerProfile.RemoteLink;
            
                _unitOfWork.ProviderProfile.Update(existingProfile);
                await _unitOfWork.CommitAsync();
            }

            return RedirectToPage("./Index");
        }

    }
}
