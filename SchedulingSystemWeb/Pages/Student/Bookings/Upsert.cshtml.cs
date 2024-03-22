using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace SchedulingSystemWeb.Pages.Student.Bookings
{
    public class UpsertModel : PageModel
    {
       private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public Availability availability = new Availability();
        public String startEndTime;
        public string fullName { get; set; }
        public Location location { get; set; }
        [BindProperty]
        public string course { get;set; }
        public Booking newBooking = new Booking();
        
        public static int? AId;
        [BindProperty]
        public string description { get; set; }
        ApplicationUser user = new ApplicationUser();
        private IWebHostEnvironment _webhostenvironment;
        public UpsertModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IWebHostEnvironment webhostenvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webhostenvironment = webhostenvironment;
        }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            AId = Id;
            availability = _unitOfWork.Availability.Get(a=> a.Id == Id);
            startEndTime = availability.StartTime + "-" + availability.EndTime;
            // user = availability.ProviderProfile.User;
            fullName = user.FirstName + " " + user.LastName;
            var locationID = availability.LocationId;
            location = _unitOfWork.Location.Get(l => l.LocationId == locationID);
            
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            availability = _unitOfWork.Availability.Get(a => a.Id == AId);
            newBooking.StartTime = availability.StartTime;
            newBooking.Duration = (int)(availability.EndTime - availability.StartTime).TotalMinutes;

            newBooking.Subject = course;
            newBooking.ProviderProfileID = availability.ProviderProfileID;
           // newBooking.User = await _userManager.GetUserAsync(User);
            newBooking.Note = description;
               // newBooking.objAvailability = availability;
                string webRootPath = _webhostenvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                //create unique identifier
                string fileName = Guid.NewGuid().ToString().ToString();
                //create path to /images/products folder
                var uploads = Path.Combine(webRootPath, @"bookingFiles\");
                //get and preserve extension type
                var extension = Path.GetExtension(files[0].FileName);
                //Create full path
                var fullPath = uploads + fileName + extension;
                //stream the binary files to the server
                using var fileStream = System.IO.File.Create(fullPath);
                files[0].CopyTo(fileStream);
                //associate real URL and save to DB
                newBooking.Attachment = @"\bookingFiles\" + fileName + extension;
            }
            else
            {
                newBooking.Attachment = "";
            }
            if (description == null)
            {
                newBooking.Note = "";
            }
            if (course == null)
            {
                newBooking.Subject = "";
            }
            _unitOfWork.Booking.Add(newBooking); //Reactivate when ready.
            _unitOfWork.Commit();   
            return RedirectToPage("/Student/Home/Index");
        }
    }
}
