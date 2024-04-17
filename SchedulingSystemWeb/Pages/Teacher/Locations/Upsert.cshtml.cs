using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystemWeb.Pages.Teacher.Locations
{
    [Authorize(Roles = "TUTOR, TEACHER, ADVISOR, ADMIN")]
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        [BindProperty]
        public Location objLocation { get; set; }
        public IEnumerable<LocationType> typeList { get; set; }

        public UpsertModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            typeList = new List<LocationType>();
        }

        public IActionResult OnGet(int? locationId)
        {
            typeList = _unitOfWork.LocationType.GetAll();

            if (locationId.HasValue)
            {
                objLocation = _unitOfWork.Location.GetById(locationId.Value);
                if (objLocation == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objLocation = new Location();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }


            objLocation.ProfileId = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            if(objLocation.LocationId == 0)// Adding a new
            {
                _unitOfWork.Location.Add(objLocation);
            }
            else// editing existing
            {
                _unitOfWork.Location.Update(objLocation);
            }

            await _unitOfWork.CommitAsync();
            return RedirectToPage("./Index");
        }
    }
}
