using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess;
using Infrastructure.Models;

namespace SchedulingSystemWeb.Pages.Availabilities
{
    public class UpsertModel : PageModel
    {
        //private readonly UnitOfWork _unitOfWork;

        //[BindProperty]
        //public Availability objAvailability { get; set; }


        //public UpsertModel(UnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //    objAvailability = new Availability();

        //}

        //public IActionResult OnGet(int? id)
        //{
        //    //edit?
        //    if (id != 0)
        //    {
        //        objAvailability = _unitOfWork.Availability.GetById(id);
        //    }
        //    if (objAvailability == null) //nothing in DB
        //    {
        //        return NotFound(); // prebuilt system page
        //    }

        //    return Page();
        //}
        //public async Task<IActionResult> OnPostAsync(List<string> DaysOfWeek)
        //{
            
        //    objAvailability.DaysOfWeek = DaysOfWeek.Select(day => Enum.Parse<DayOfWeek>(day)).ToList();

        //    if (!ModelState.IsValid)
        //    {
        //        TempData["error"] = "Data Incomplete";
        //        return Page(); 
        //    }

        //    if (objAvailability.Id == 0)
        //    {
        //        _unitOfWork.Availability.Add(objAvailability);
        //        TempData["Success"] = "Availability added Successfully";
        //    }
        //    else
        //    {
        //        _unitOfWork.Availability.Update(objAvailability);
        //        TempData["Success"] = "Availability updated Successfully";
        //    }

        //    _unitOfWork.CommitAsync();
        //    return RedirectToPage("./Index");
        //}

    }

}
