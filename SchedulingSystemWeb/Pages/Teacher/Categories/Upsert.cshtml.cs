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

        public IActionResult OnGet(int? id)
        {
            if (id.HasValue)
            {
                objCategory = _unitOfWork.Category.GetById(id.Value);
                if (objCategory == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objCategory = new Category();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            objCategory.ProviderProfile = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            if (objCategory.Id == 0)// Adding a new
            {              
                _unitOfWork.Category.Add(objCategory);
            }
            else// editing existing
            {              
                _unitOfWork.Category.Update(objCategory);
            }
            await _unitOfWork.CommitAsync();

            return RedirectToPage("./Index");
        }

    }
}
