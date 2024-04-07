using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystemWeb.Pages.Teacher.Availabilities
{
    public class DeleteModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        public DeleteModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Availability availability { get; set; }
        public void OnGet(int? id)
        {
            availability = _unitOfWork.Availability.Get(i=> i.Id == id);
        }
        public async Task<IActionResult> OnPost(int? id ) 
        {
            availability = _unitOfWork.Availability.Get(i => i.Id == id);
            _unitOfWork.Availability.Delete(availability);
            _unitOfWork.Commit();
            return Redirect("/Teacher/Availabilities");
        }
    }
}
