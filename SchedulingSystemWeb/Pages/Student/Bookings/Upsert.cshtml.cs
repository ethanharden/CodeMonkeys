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
        public String fullName;
        public Location location;
        public string course;
        public Booking newBooking = new Booking();
        public string description;
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
            availability = _unitOfWork.Availability.Get(a=> a.Id == Id);
            startEndTime = availability.StartTime + "-" + availability.EndTime;
             user = availability.ProviderProfile.User;
            fullName = user.FullName;
            var locationID = availability.LocationId;
            Location location = _unitOfWork.Location.Get(l => l.LocationId == locationID);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
          
          
                newBooking.Subject = course;
                newBooking.ProviderProfile.User = user;
                //newBooking.User =
                newBooking.Note = description;
                newBooking.objAvailability = availability;
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
            //_unitOfWork.Booking.Add(newBooking); Reactivate when ready.
            return RedirectToPage("././Home/Index");
        }
    }
}
