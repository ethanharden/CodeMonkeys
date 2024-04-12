using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Models;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using DataAccess;

namespace SchedulingSystemWeb.Pages.Teacher.Categories
{
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public Category objCategory { get; set; }

        public UpsertModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            objCategory.ProviderProfile = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;

            _unitOfWork.Category.Add(objCategory);
            await _unitOfWork.CommitAsync();

            return RedirectToPage("./Index");
        }
    }
}
