using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using static System.Formats.Asn1.AsnWriter;
using System.Globalization;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
namespace SchedulingSystemWeb.Pages.Student.Bookings
{
    public class UpsertModel : PageModel
    {
       private readonly UnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public Availability availability = new Availability();
        public string startEndTime;
        public string fullName { get; set; }
        public Location location { get; set; }
        [BindProperty(SupportsGet = true)]
        public string course { get;set; }
        public Booking newBooking = new Booking();
        [BindProperty(SupportsGet = true)]
        public string title { get; set; }
        public static int? AId;
        [BindProperty(SupportsGet = true)]
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
            startEndTime = availability.StartTime.TimeOfDay+ "-" + availability.EndTime.TimeOfDay;
            ProviderProfile prov = _unitOfWork.ProviderProfile.Get(u => u.Id == availability.ProviderProfileID);
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
            var selectedCalendar = Request.Form["calendar"];
            if (selectedCalendar.Count > 0)
            {
                var calendarType = selectedCalendar[0];
                if (calendarType == "outlook")
                {
                    
                }
                else if (calendarType == "google")
                {
                    UserCredential credential;
                    string[] Scopes = { Google.Apis.Calendar.v3.CalendarService.Scope.CalendarEvents };
                    string ApplicationName = "Code Monkeys";
                    string CalendarId = "primary";

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

                    //Availability availability = _unitOfWork.Availability.Get(a => a.Id == newBooking.objAvailability);
                    // Define event
                    Event newEvent = new Event()
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

                    // Add event
                    EventsResource.InsertRequest request = service.Events.Insert(newEvent, CalendarId);
                    Event createdEvent = request.Execute();
                    Console.WriteLine("Event created: {0}", createdEvent.HtmlLink);
                }

            }
            
            return Redirect("/student/home/index");
        }
    }
}
