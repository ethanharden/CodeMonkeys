using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystemWeb.Pages.Teacher.Appointments
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public IndexModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userman , IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userman;
            _webHostEnvironment = webHostEnvironment;   

        }
        //Public Variables
        public Booking booking { get; set; }
        public ApplicationUser student { get; set; }
        public ApplicationUser teacher { get; set; }
        public string filepath { get; set; }

        public async Task OnGet(int? id )
        {

            
            int provId = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            booking = _unitOfWork.Booking.Get(b => b.Id == id & b.ProviderProfileID ==provId);
            teacher = _unitOfWork.ApplicationUser.Get(u => u.Id == _userManager.GetUserId(User));
            student = _unitOfWork.ApplicationUser.Get(s => s.Id == booking.User);
            if (booking.Attachment != null)
            {
                string wwwroot = _webHostEnvironment.WebRootPath;
                filepath = Path.Combine(wwwroot, booking.Attachment);
            }
            
        }
    }
}
