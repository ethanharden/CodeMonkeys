using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystemWeb.Pages.Teacher.Categories
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();

        public IndexModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public void OnGet()
        {
            var prof = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            Categories = _unitOfWork.Category.GetAll().Where(c => c.ProviderProfile == prof);
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var category = _unitOfWork.Category.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Delete(category);
            await _unitOfWork.CommitAsync();

            return RedirectToPage();
        }
    }
}
