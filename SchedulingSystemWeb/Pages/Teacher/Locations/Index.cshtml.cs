using DataAccess;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchedulingSystemWeb.Pages.Teacher.Locations
{
    [Authorize(Roles = "TUTOR, TEACHER, ADVISOR, ADMIN")]
    public class IndexModel : PageModel
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEnumerable<Location> LocationList { get; set; }
        public ProviderProfile providerProfile { get; set; }
        [BindProperty]
        [Display(Name = "Remote Link")]
        public string RemoteLink { get; set; }
        public IEnumerable<LocationType> locationTypes { get; set; }
        public List<(Location, string campusName)> locationCompoundList { get; set; }

        [BindProperty]
        public bool Edit { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public IndexModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            locationCompoundList = new List<(Location, string)>();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Edit = false;
            await LoadData();
            return Page();
        }

        public async Task OnPostEdit()
        {
            Edit = true;
            await LoadData();
        }

        private async Task LoadData()
        {
            providerProfile = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User));
            LocationList = _unitOfWork.Location.GetAll().Where(l => l.ProfileId == providerProfile.Id);
            locationCompoundList.Clear();
            foreach (var loc in LocationList)
            {
                var locationTypeName = _unitOfWork.LocationType.Get(l => l.Id == loc.LocationType).LocationTypeName;
                locationCompoundList.Add((loc, locationTypeName));
            }
            RemoteLink = providerProfile.RemoteLink;
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
                existingProfile.RemoteLink = RemoteLink;
                _unitOfWork.ProviderProfile.Update(existingProfile);
                await _unitOfWork.CommitAsync();
                StatusMessage = "Virtual Meeting Link Updated";
            }

            Edit = false; 
            await LoadData();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var location = _unitOfWork.Location.Get(l => l.LocationId == id);
            if (location == null)
            {
                return NotFound();
            }

            _unitOfWork.Location.Delete(location);
            await _unitOfWork.CommitAsync();
            return RedirectToPage();
        }

    }
}
