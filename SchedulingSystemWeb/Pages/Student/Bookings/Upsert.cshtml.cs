using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
//using Microsoft.Identity.Client;
using static System.Formats.Asn1.AsnWriter;
using System.Globalization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Graph;
using Microsoft.Extensions.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

using ClientCredential = Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential;
using static Google.Apis.Calendar.v3.CalendarService;
using System.Net.Http.Headers;
using Microsoft.Graph.Models;
using Azure.Identity;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.AspNetCore.Authorization;
using System;
namespace SchedulingSystemWeb.Pages.Student.Bookings
{
    [Authorize(Roles = "STUDENT, TUTOR")]
    public class UpsertModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public Availability availability = new Availability();
        public string startEndTime;
        public string fullName { get; set; }
        public Infrastructure.Models.Location location { get; set; }
        [BindProperty(SupportsGet = true)]
        public string course { get; set; }
        public Booking newBooking = new Booking();
        [BindProperty(SupportsGet = true)]
        public string title { get; set; }
        public static int? AId;
        [BindProperty(SupportsGet = true)]
        public string description { get; set; }
        public ApplicationUser user { get; set; }
        public List<string> ProvRole { get; set; }
        public ProviderProfile prov { get; set; }
        private IWebHostEnvironment _webhostenvironment;
        public IEnumerable<Category> AvCategories { get; set; }
        [BindProperty]
        public List<int> CategoryIds { get; set; } = new List<int>();
        public UpsertModel(UnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IWebHostEnvironment webhostenvironment, IHttpClientFactory httpClientFactory)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webhostenvironment = webhostenvironment;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync(int? Id)
        {
            AId = Id;
            
            availability = _unitOfWork.Availability.Get(a => a.Id == Id);
            startEndTime = availability.StartTime.TimeOfDay + "-" + availability.EndTime.TimeOfDay;
            prov = _unitOfWork.ProviderProfile.Get(u => u.Id == availability.ProviderProfileID);
            user = _unitOfWork.ApplicationUser.Get(u => u.Id == prov.User);
            ProvRole = (List<string>) await _userManager.GetRolesAsync(user);
            fullName = user.FirstName + " " + user.LastName;
            var locationID = availability.LocationId;
            location = _unitOfWork.Location.Get(l => l.LocationId == locationID);
            AvCategories = _unitOfWork.Category.GetAll().Where(c => availability.Category.Contains(c.Id));
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            availability = _unitOfWork.Availability.Get(a => a.Id == AId);
            newBooking.StartTime = availability.StartTime;
            newBooking.Duration = (int)(availability.EndTime - availability.StartTime).TotalMinutes;
            newBooking.Subject = course;
            newBooking.ProviderProfileID = availability.ProviderProfileID;
            newBooking.CategoryID = int.Parse(Request.Form["CategoryIds"]);
            //newBooking.User = await _userManager.GetUserAsync(User);
            newBooking.Note = description;
            //newBooking.MeetingTitle = title;
            newBooking.User = _userManager.GetUserId(User);
            newBooking.objAvailability = availability.Id;
            string webRootPath = _webhostenvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            newBooking.Attachment = new List<String>();
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    
                    // Create unique identifier
                    string fileName = Guid.NewGuid().ToString();
                    // Get and preserve extension type
                    var extension = Path.GetExtension(file.FileName);
                    // Create path to /images/products folder
                    var uploads = Path.Combine(webRootPath, @"bookingFiles\");
                    // Create full path
                    var fullPath = Path.Combine(uploads, fileName + extension);
                    // Stream the binary files to the server
                    using var fileStream = System.IO.File.Create(fullPath);
                    file.CopyTo(fileStream);
                    // Associate real URL and save to DB
                    newBooking.Attachment.Add(@"\bookingFiles\" + fileName + extension);
                    
                }
            }
            if (course == null)
            {
                newBooking.Subject = "";
            }
            _unitOfWork.Booking.Add(newBooking); //Reactivate when ready.
            _unitOfWork.Commit();
            await SendEmail(newBooking);
           
            
            if (User.IsInRole("STUDENT"))
            {
                return Redirect("/student/home/index");
            }
            else if (User.IsInRole("TUTOR"))
            {
                return Redirect("/tutor/home/index");
            }

            return Redirect("/Index");
        }
        public async Task SendEmail(Booking newBooking)
        {   
            ProviderProfile p = _unitOfWork.ProviderProfile.Get(p => p.Id == newBooking.ProviderProfileID);
            ApplicationUser Teacherinfo = _unitOfWork.ApplicationUser.Get(u => u.Id == p.User);
            ApplicationUser student = _unitOfWork.ApplicationUser.Get(b => b.Id == newBooking.User);

            var sendgridclient = new SendGridClient("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");
            var from = new SendGrid.Helpers.Mail.EmailAddress("CodemonkeysScheduling@outlook.com", "Code Monkeys");
            var to = new SendGrid.Helpers.Mail.EmailAddress(Teacherinfo.Email, Teacherinfo.FirstName + " " + Teacherinfo.LastName);
            var subject = "Booking Created";
            var plainText = student.FirstName + " " + student.LastName + " Created an appointment on " + newBooking.StartTime.ToShortDateString() + " at " + newBooking.StartTime.ToShortTimeString();
            var htmlContent = "<p>" + plainText + "</p>";
            var message = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
            var response = await sendgridclient.SendEmailAsync(message); //Teacher Message
            to = new SendGrid.Helpers.Mail.EmailAddress(student.Email, student.FirstName + " " + student.LastName);
            plainText = "You successfully made a booking with " + Teacherinfo.FirstName + " " + Teacherinfo.LastName + " on " + newBooking.StartTime.ToShortDateString() + " at " + newBooking.StartTime.ToShortTimeString();
            htmlContent = "<p>" + plainText + "</p>";
            message = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlContent);
            response = await sendgridclient.SendEmailAsync(message); //Student Message
        }
    }
}