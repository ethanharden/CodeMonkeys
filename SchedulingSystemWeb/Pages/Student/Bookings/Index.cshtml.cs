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
using Microsoft.EntityFrameworkCore;

namespace SchedulingSystemWeb.Pages.Student.Bookings
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ICalendarService _calendarService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
      
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
        
        public List<ApplicationUser> ApplicationUserList;

        public new List<int> ProviderList;
        public IEnumerable<Department> departmentList;
        public IList<IdentityRole> Roles;

        public IndexModel(UnitOfWork unitOfWork, ICalendarService calendarService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _calendarService = calendarService;
            _userManager = userManager;
            _roleManager = roleManager;

            InitializeCollections();
        }

        private void InitializeCollections()
        {
            Bookings = new List<Booking>();
            Availabilities = new List<Availability>();
            SearchRoles = new Dictionary<string, IList<string>>();
            ApplicationUserList = new List<ApplicationUser>();
            ProviderList = new List<int>();
        }


        public async Task OnGetAsync(string role, int? department, string? providerUserId)
        {
            InitializeSessionStates(role, department, providerUserId);
            await LoadDataAsync();

            SetupDateAndViewData();
            await FetchDataForCurrentViewAsync();
        }


        private void InitializeSessionStates(string role, int? department, string? providerUserId)
        {
            HttpContext.Session.SetString("SearchRole", role ?? HttpContext.Session.GetString("SearchRole") ?? string.Empty);
           
             if (department.HasValue)
            {
                HttpContext.Session.SetInt32("SearchDepartment", department.Value);
            }
            if (!string.IsNullOrEmpty(providerUserId))
            {
                HttpContext.Session.SetString("SearchProvId", providerUserId);
            }
        }
        private async Task LoadDataAsync()
        {
            departmentList = _unitOfWork.Department.GetAll();
            Roles = await _roleManager.Roles.ToListAsync();

            var searchRole = HttpContext.Session.GetString("SearchRole");
            var usersInRole = await _userManager.GetUsersInRoleAsync(searchRole);

            //var providerList = usersInRole.Select(user => _unitOfWork.ProviderProfile.Get(p => p.User == user.Id).Id);       
            //FilterAvailabilitiesAndBookings(providerList);
        }

        private void FilterAvailabilitiesAndBookings(IEnumerable<int> providerList)
        {
            var providerUserId = HttpContext.Session.GetString("SearchProvId");
            if (!string.IsNullOrEmpty(providerUserId))
            {
                var providerId = _unitOfWork.ProviderProfile.Get(p => p.User == providerUserId).Id;
                Availabilities = _unitOfWork.Availability.GetAll().Where(a => a.ProviderProfileID == providerId);
                Bookings = _unitOfWork.Booking.GetAll().Where(b => b.ProviderProfileID == providerId);
            }
            else
            {
                Availabilities = providerList.SelectMany(id => _unitOfWork.Availability.GetAll().Where(a => a.ProviderProfileID == id));
                Bookings = providerList.SelectMany(id => _unitOfWork.Booking.GetAll().Where(b => b.ProviderProfileID == id));
            }
        }

        private void SetupDateAndViewData()
        {
            CurrentDate = TempData["CurrentDate"] as DateTime? ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM", CultureInfo.CurrentCulture);
            WeekDays = _calendarService.GetWeekDays(CurrentDate);
            MonthDays = _calendarService.GetMonthDays(CurrentDate);
        }

        public async Task<IActionResult> OnGetPreviousWeekAsync()
        {
            //string searchRole = HttpContext.Session.GetString("SearchRole");
            //int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            //string? searchProv = HttpContext.Session.GetString("SearchProvId");
            //CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            //TempData["CurrentDate"] = CurrentDate;
            //TempData["ActiveTab"] = "weekly";
            //await FetchDataForCurrentViewAsync();

            await LoadDataAsync();
            AdjustDateAndRedirect(-7);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetNextWeekAsync()
        {
            //string searchRole = HttpContext.Session.GetString("SearchRole");
            //int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            //string? searchProv = HttpContext.Session.GetString("SearchProvId");
            //CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(7);
            //TempData["CurrentDate"] = CurrentDate;
            //TempData["ActiveTab"] = "weekly";
            //await FetchDataForCurrentViewAsync();

            await LoadDataAsync();
            AdjustDateAndRedirect(7);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetPreviousMonthAsync()
        {
            //string searchRole = HttpContext.Session.GetString("SearchRole");
            //int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            //string? searchProv = HttpContext.Session.GetString("SearchProvId");
            //CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(-1);
            //TempData["CurrentDate"] = CurrentDate;
            //TempData["ActiveTab"] = "monthly";
            //CurrentMonthName = CurrentDate.ToString("MMMM");
            //MonthDays = _calendarService.GetMonthDays(CurrentDate);
            //await FetchDataForCurrentViewAsync();
            
            await LoadDataAsync();
            AdjustDateAndRedirect(0, -1);
            return Page();
        }

        public async Task<IActionResult> OnGetNextMonthAsync()
        {
            //string searchRole = HttpContext.Session.GetString("SearchRole");
            //int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            //string? searchProv = HttpContext.Session.GetString("SearchProvId");
            //CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(1);
            //TempData["CurrentDate"] = CurrentDate;
            //TempData["ActiveTab"] = "monthly";
            //await FetchDataForCurrentViewAsync();
            await LoadDataAsync();
            AdjustDateAndRedirect(0, 1);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetToday()
        {
            await LoadDataAsync();

            TempData["CurrentDate"] = DateTime.Today;
            return RedirectToPage();
        }

        //public async Task<IActionResult> OnGetTodayWeekAsync() { 

        //    string searchRole = HttpContext.Session.GetString("SearchRole");
        //    int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
        //    string? searchProv = HttpContext.Session.GetString("SearchProvId");
        //    CurrentDate = DateTime.Today;
        //    TempData["CurrentDate"] = CurrentDate;
        //    TempData["ActiveTab"] = "weekly";
        //    await FetchDataForCurrentViewAsync();
        //    return RedirectToPage();
        //}

        //public async Task<IActionResult> OnGetTodayMonthAsync()
        //{
        //    Load();
        //    string searchRole = HttpContext.Session.GetString("SearchRole");
        //    int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
        //    string? searchProv = HttpContext.Session.GetString("SearchProvId");
        //    CurrentDate = DateTime.Today;
        //    TempData["CurrentDate"] = CurrentDate;
        //    TempData["ActiveTab"] = "monthly";
        //    await FetchDataForCurrentViewAsync();
        //    return RedirectToPage();
        //}

        private void AdjustDateAndRedirect(int days = 0, int months = 0)
        {
            var date = TempData["CurrentDate"] as DateTime? ?? DateTime.Today;
            date = date.AddDays(days).AddMonths(months);
            TempData["CurrentDate"] = date;
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
