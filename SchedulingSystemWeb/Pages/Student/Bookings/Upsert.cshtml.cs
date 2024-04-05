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
namespace SchedulingSystemWeb.Pages.Student.Bookings
{
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
        ApplicationUser user = new ApplicationUser();
        public ProviderProfile prov { get; set; }
        private IWebHostEnvironment _webhostenvironment;
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
            fullName = user.FirstName + " " + user.LastName;
            var locationID = availability.LocationId;
            location = _unitOfWork.Location.Get(l => l.LocationId == locationID);

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
            //newBooking.User = await _userManager.GetUserAsync(User);
            newBooking.Note = description;
            newBooking.MeetingTitle = title;
            newBooking.User = _userManager.GetUserId(User);
            newBooking.objAvailability = availability.Id;
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
            
            
            if (course == null)
            {
                newBooking.Subject = "";
            }
            _unitOfWork.Booking.Add(newBooking); //Reactivate when ready.
            _unitOfWork.Commit();
            await SendEmail(newBooking);
            var selectedCalendar = Request.Form["calendar"]; // For adding to google 
            if (selectedCalendar.Count > 0)
            {               
                    var calendarType = selectedCalendar[0];
                    
                     if (calendarType == "google") //GOOGLE-------------------------------------------------------------------------------
                    {
                        UserCredential credential;
                        string[] Scopes = { Google.Apis.Calendar.v3.CalendarService.Scope.CalendarEvents };
                        string ApplicationName = "Code Monkeys";
                        string CalendarId = "primary";
                        Google.Apis.Calendar.v3.Data.Event newEvent = new Google.Apis.Calendar.v3.Data.Event()
                        {
                            Summary = newBooking.Subject,
                            Location = _unitOfWork.Location.Get(l => l.LocationId == availability.LocationId).LocationName,
                            Description = newBooking.Note,
                            Start = new EventDateTime()
                            {
                                DateTime = newBooking.StartTime,
                                TimeZone = localTimeZone.DisplayName,
                            },
                            End = new EventDateTime()
                            {
                                DateTime = newBooking.StartTime.AddMinutes(newBooking.Duration),
                                TimeZone = localTimeZone.DisplayName,
                            },
                        };
                        using (var stream = new FileStream(@"wwwroot/secret/client_secret_328091863377-8nbrlurrfk4f51on47vvosc341ajlgk1.apps.googleusercontent.com (1).json", FileMode.Open, FileAccess.Read))
                        {
                            string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                            credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

                            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                                GoogleClientSecrets.Load(stream).Secrets,
                                Scopes,
                                "user",
                                CancellationToken.None,
                                new FileDataStore(credPath, true)).Result;
                            Console.WriteLine("Credential file saved to: " + credPath);
                        }
                        // Create Google Calendar API service.
                        var service = new Google.Apis.Calendar.v3.CalendarService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = credential,
                            ApplicationName = "CodeMonkeys"
                        });
                        EventsResource.InsertRequest request = service.Events.Insert(newEvent, CalendarId);
                        Google.Apis.Calendar.v3.Data.Event createdEvent = request.Execute();
                        Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);

                    }
            }
            return Redirect("/student/home/index");
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