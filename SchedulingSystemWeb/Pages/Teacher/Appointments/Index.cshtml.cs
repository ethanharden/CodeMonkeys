using DataAccess;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

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
        public Availability aval { get; set; }
        public CustomerProfile studprofile { get; set; }
        public async Task OnGet(int? id )
        {

            
            int provId = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            booking = _unitOfWork.Booking.Get(b => b.Id == id & b.ProviderProfileID == provId);
            teacher = _unitOfWork.ApplicationUser.Get(u => u.Id == _userManager.GetUserId(User));
            student = _unitOfWork.ApplicationUser.Get(s => s.Id == booking.User);
            aval = _unitOfWork.Availability.Get(i => i.Id == booking.objAvailability);
            location = _unitOfWork.Location.Get(l=> l.LocationId == aval.LocationId);
            studprofile = _unitOfWork.CustomerProfile.Get(d => d.User == student.Id);
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
        public async Task OnPost(int? id)
        {
            int provId = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            booking = _unitOfWork.Booking.Get(b => b.Id == id & b.ProviderProfileID == provId);
            teacher = _unitOfWork.ApplicationUser.Get(u => u.Id == _userManager.GetUserId(User));
            student = _unitOfWork.ApplicationUser.Get(s => s.Id == booking.User);
            aval = _unitOfWork.Availability.Get(i => i.Id == booking.objAvailability);
            location = _unitOfWork.Location.Get(l => l.LocationId == aval.LocationId);
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            UserCredential credential;
            string[] Scopes = { Google.Apis.Calendar.v3.CalendarService.Scope.CalendarEvents };
            string ApplicationName = "Code Monkeys";
            string CalendarId = "primary";
            Event newEvent = new()
            {
                Summary = booking.Subject,
                Location = _unitOfWork.Location.Get(l => l.LocationId == aval.LocationId).LocationName,
                Description = booking.Note,
                Start = new EventDateTime()
                {
                    DateTime = booking.StartTime,
                    TimeZone = localTimeZone.DisplayName,
                },
                End = new EventDateTime()
                {
                    DateTime = booking.StartTime.AddMinutes(booking.Duration),
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
}
