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
using Microsoft.Graph.Models;

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
        public IEnumerable<Availability> ViewWeekAvailabilities { get; set; }

        public DateTime CurrentDate { get; private set; }
        public List<DateTime> WeekDays { get; private set; } = new List<DateTime>();
        public List<DateTime> MonthDays { get; private set; }
        public string CurrentMonthName { get; private set; }
        public Dictionary<string, IList<string>> SearchRoles;
        
        public IList<ApplicationUser> ApplicationUserList;

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
            //string currentTab = HttpContext.Request.Query["tab"].ToString().ToLower();
            //if (string.IsNullOrEmpty(currentTab) || (currentTab != "weekly" && currentTab != "monthly"))
            //{
            //    currentTab = "weekly"; // Default tab
            //}
            //ViewData["CurrentTab"] = currentTab;

            ViewData["ActiveTab"] = TempData["ActiveTab"] as string ?? "weekly";
            TempData.Keep("ActiveTab");
            int? provProfID = null;

            if (!providerUserId.IsNullOrEmpty())
            {
                provProfID = _unitOfWork.ProviderProfile.Get(p => p.User == providerUserId).Id;
            }

            
            InitializeSessionStates(role, department, provProfID);
            await LoadDataAsync(provProfID);

            SetupDateAndViewData();
            await FetchDataForCurrentViewAsync();
        }
        //public async Task<IActionResult> OnPostAsync(string role, int department, int providerUserId)
        //{
        //    TempData["SelectedRole"] = role;
        //    TempData["SelectedDepartment"] = department.ToString();

        //    return RedirectToPage("/Student/Bookings/Index", new { role = role, department = department, providerUserId = providerUserId });
        //}


        private void InitializeSessionStates(string role, int? department, int? providerUserId)
        {
            HttpContext.Session.SetString("SearchRole", role ?? HttpContext.Session.GetString("SearchRole") ?? string.Empty);
           
             if (department.HasValue)
            {
                HttpContext.Session.SetInt32("SearchDepartment", department.Value);
            }
            if (providerUserId != null)
            {
                HttpContext.Session.SetInt32("SearchProvId", providerUserId.Value);
                HttpContext.Session.SetInt32("provChecker", 1);
            }
            else
            {
                HttpContext.Session.SetInt32("provChecker", 0);
            }
        }
        private async Task LoadDataAsync(int? providerUserId)
        {
            departmentList = _unitOfWork.Department.GetAll();
            Roles = await _roleManager.Roles.ToListAsync();

            var searchRole = HttpContext.Session.GetString("SearchRole");
            ApplicationUserList = await _userManager.GetUsersInRoleAsync(searchRole);

            //var providerList = usersInRole.Select(user => _unitOfWork.ProviderProfile.Get(p => p.User == user.Id).Id);
            if (providerUserId != null)
            {
                ProviderList.Add(_unitOfWork.ProviderProfile.Get(p => p.Id == providerUserId).Id);
            }
            foreach(var u in ApplicationUserList)
            {
                ProviderList.Add(_unitOfWork.ProviderProfile.Get(p => p.User == u.Id).Id);
            }

            FilterAvailabilitiesAndBookings(ProviderList);
        }

        private void FilterAvailabilitiesAndBookings(IEnumerable<int> providerList)
        {
            if (HttpContext.Session.GetInt32("provChecker") == 1)
            {
                var providerUserId = HttpContext.Session.GetInt32("SearchProvId");
                if (providerUserId != null)
                {
                    Availabilities = _unitOfWork.Availability.GetAll().Where(a => a.ProviderProfileID == providerUserId);
                    Bookings = _unitOfWork.Booking.GetAll().Where(b => b.ProviderProfileID == providerUserId);
                }
                else
                {
                    Availabilities = new List<Availability>();
                    Bookings = new List<Booking>();

                    foreach (var p in providerList)
                    {
                        var AList = Availabilities.ToList();
                        AList.AddRange(_unitOfWork.Availability.GetAll().Where(a => a.ProviderProfileID == p).ToList());
                        Availabilities = AList;

                        var BList = Bookings.ToList();
                        BList.AddRange(_unitOfWork.Booking.GetAll().Where(b => b.ProviderProfileID == p));
                        Bookings = BList;
                    }
                }
            }
            
            else
            {
                Availabilities = new List<Availability>();
                Bookings = new List<Booking>();

                foreach ( var p in providerList)
                {
                    var AList = Availabilities.ToList();
                    AList.AddRange(_unitOfWork.Availability.GetAll().Where(a => a.ProviderProfileID == p).ToList());
                    Availabilities = AList;

                    var BList = Bookings.ToList();
                    BList.AddRange(_unitOfWork.Booking.GetAll().Where(b => b.ProviderProfileID == p));
                    Bookings = BList;
                }

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

        public async Task<IActionResult> OnGetPreviousWeekAsync(int? providerUserId)
        {
            //string searchRole = HttpContext.Session.GetString("SearchRole");
            //int? searchDepartment = HttpContext.Session.GetInt32("SearchDepartment");
            //string? searchProv = HttpContext.Session.GetString("SearchProvId");
            //CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            //TempData["CurrentDate"] = CurrentDate;
            //TempData["ActiveTab"] = "weekly";
            //await FetchDataForCurrentViewAsync();

            await LoadDataAsync(providerUserId);
            AdjustDateAndRedirect(-7);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetNextWeekAsync(int? providerUserId)
        {
            await LoadDataAsync(providerUserId);
            AdjustDateAndRedirect(7);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetPreviousMonthAsync(int? providerUserId)
        {
            TempData["ActiveTab"] = "monthly";
            await LoadDataAsync(providerUserId);
            AdjustDateAndRedirect(0, -1);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetNextMonthAsync(int? providerUserId)
        {
            TempData["ActiveTab"] = "monthly";
            await LoadDataAsync(providerUserId);
            AdjustDateAndRedirect(0, 1);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetToday(int? providerUserId)
        {
            await LoadDataAsync(providerUserId);

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
