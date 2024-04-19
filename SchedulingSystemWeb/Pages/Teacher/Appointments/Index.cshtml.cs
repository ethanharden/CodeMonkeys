using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchedulingSystemWeb.Pages.Teacher.Appointments
{
    [Authorize(Roles = "TUTOR, TEACHER, ADVISOR")]
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
        public List<string> filepath { get; set; }
        public Location location { get; set; }
        public async Task OnGet(int? id )
        {

            
            int provId = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            booking = _unitOfWork.Booking.Get(b => b.Id == id & b.ProviderProfileID ==provId);
            teacher = _unitOfWork.ApplicationUser.Get(u => u.Id == _userManager.GetUserId(User));
            student = _unitOfWork.ApplicationUser.Get(s => s.Id == booking.User);
            var aval = _unitOfWork.Availability.Get(i => i.Id == booking.objAvailability);
            location = _unitOfWork.Location.Get(l=> l.LocationId == aval.LocationId);
            filepath = new List<string>();
            if (booking.Attachment != null && booking.Attachment.Count() > 0)
            {
                string wwwroot = _webHostEnvironment.WebRootPath;
                foreach (var attachmentPath in booking.Attachment)
                {
                    // Parse the attachmentPath to remove the square brackets and quotes
                    string[] parts = attachmentPath.ToString().Trim(new char[] { '[', ']', '"' }).Split(',');

                    // Trim each part to remove leading and trailing whitespace
                    for (int i = 0; i < parts.Length; i++)
                    {
                        parts[i] = parts[i].Trim();
                    }

                    // Combine the parts to get the file path
                    filepath.Add( Path.Combine(wwwroot, parts[0]));

                    // Use filePath as needed
                }
            }

        }
        public async Task OnPost()
        {
            //Add calendar thinggy
        }
    }
}
