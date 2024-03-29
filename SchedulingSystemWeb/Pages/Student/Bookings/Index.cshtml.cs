using DataAccess;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace SchedulingSystemWeb.Pages.Student.Bookings
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ICalendarService _calendarService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        [BindProperty]
        public ApplicationUser _ProfUser { get; set; }
        [BindProperty]
        public  string profFullName { get;set; }
        public IEnumerable<Booking> Bookings { get; set; }
        public IEnumerable<Availability> Availabilities { get; set; }
        [BindProperty]
        public ProviderProfile provider { get; set; }
        public IEnumerable<Booking> ViewBookings { get; set; }
        public IEnumerable<Availability> ViewAvailabilities { get; set; }

        public DateTime CurrentDate { get; private set; }
        public List<DateTime> WeekDays { get; private set; } = new List<DateTime>();
        public List<DateTime> MonthDays { get; private set; }
        public string CurrentMonthName { get; private set; }
        public Dictionary<string, IList<string>> SearchRoles;
        private readonly UserManager<ApplicationUser> _userManager;
        public List<ApplicationUser> objApplicationUserList;
        public new List<int> objProviderList;


        public IndexModel(IHttpContextAccessor httpContextAccessor, UnitOfWork unitOfWork, ICalendarService calendarService, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _calendarService = calendarService;
            Bookings = new List<Booking>();
            Availabilities = new List<Availability>();
            SearchRoles = new Dictionary<string, IList<string>>(); // Initialize user roles dictionary
            _userManager = userManager;
            objApplicationUserList = new List<ApplicationUser>();
            objProviderList = new List<int>();

        }


        public async Task OnGetAsync(string role, int? department, string? providerUserId)
        {
            string searchRole = HttpContext.Session.GetString("SearchRole");
            int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            
            if (string.IsNullOrEmpty(searchRole)) {
                HttpContext.Session.SetString("SearchRole", role);
                searchRole = HttpContext.Session.GetString("SearchRole");
            }
            if (!role.IsNullOrEmpty() && !string.IsNullOrEmpty(searchRole))
            {
                HttpContext.Session.SetString("SearchRole", role);
                searchRole = HttpContext.Session.GetString("SearchRole");
            }
            if (!searchDepartment.HasValue && department.HasValue)
            {
               HttpContext.Session.SetInt32("SearchDepartment", department.Value);
                searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            }

            objApplicationUserList.AddRange(await _userManager.GetUsersInRoleAsync(searchRole));

            if (objApplicationUserList.Count > 0)
            {
                foreach (var user in objApplicationUserList)
                {
                    objProviderList.Add(_unitOfWork.ProviderProfile.Get(p => p.User == user.Id).Id);
                }
            }
            Availabilities = new List<Availability>();
            Bookings = new List<Booking>();

            if (!providerUserId.IsNullOrEmpty())
            {
                HttpContext.Session.SetString("userProv", providerUserId);
                var provider = _unitOfWork.ProviderProfile.Get(p => p.User == providerUserId).Id;
                Availabilities = _unitOfWork.Availability.GetAll()
                                    .Where(a => a.ProviderProfileID == provider);
                Bookings = _unitOfWork.Booking.GetAll()
                                    .Where(a => a.ProviderProfileID == provider);
            }
            else
            {
                foreach (var p in objProviderList)
                {
                    var providerAvailabilities = _unitOfWork.Availability.GetAll().Where(a => a.ProviderProfileID == p);
                    Availabilities = Availabilities.Concat(providerAvailabilities);

                    var providerBookings = _unitOfWork.Booking.GetAll().Where(b => b.ProviderProfileID == p);
                    Bookings = Bookings.Concat(providerBookings);
                }
            }
            
           // Availabilities = _unitOfWork.Availability.GetAll() /*.Where(p => p.ProviderProfileID == provider.Id)*/
            //Bookings = _unitOfWork.Booking.GetAll()/*.Where(p => p.ProviderProfileID == provider.Id)*/;

            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");

            WeekDays = _calendarService.GetWeekDays(CurrentDate);
            MonthDays = _calendarService.GetMonthDays(CurrentDate);

           
            await FetchDataForCurrentViewAsync();
        }


        public async Task<IActionResult> OnGetPreviousWeekAsync()
        {
            string searchRole = HttpContext.Session.GetString("SearchRole");
            int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            string? searchProv = HttpContext.Session.GetString("userProv");
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetNextWeekAsync()
        {
            string searchRole = HttpContext.Session.GetString("SearchRole");
            int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            string? searchProv = HttpContext.Session.GetString("userProv");
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetPreviousMonthAsync()
        {
            string searchRole = HttpContext.Session.GetString("SearchRole");
            int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            string? searchProv = HttpContext.Session.GetString("userProv");
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(-1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            CurrentMonthName = CurrentDate.ToString("MMMM");
            MonthDays = _calendarService.GetMonthDays(CurrentDate);
            await FetchDataForCurrentViewAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetNextMonthAsync()
        {
            string searchRole = HttpContext.Session.GetString("SearchRole");
            int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            string? searchProv = HttpContext.Session.GetString("userProv");
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetTodayWeekAsync()
        {
            string searchRole = HttpContext.Session.GetString("SearchRole");
            int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            string? searchProv = HttpContext.Session.GetString("userProv");
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetTodayMonthAsync()
        {
            string searchRole = HttpContext.Session.GetString("SearchRole");
            int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            string? searchProv = HttpContext.Session.GetString("userProv");
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public bool IsAvailabilityBooked(Availability availability)
        {
            return Bookings.Any(booking => booking.StartTime >= availability.StartTime && booking.StartTime < availability.EndTime);
        }

        private async Task FetchDataForCurrentViewAsync()
        {
            DateTime startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Filter bookings within the month.
            ViewBookings = Bookings.Where(b => b.StartTime.Date >= startOfMonth && b.StartTime.Date <= endOfMonth);
            // Filter availabilities within the month. Adjust this logic if your availabilities work differently.
            ViewAvailabilities = Availabilities.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
        }

    }
}
