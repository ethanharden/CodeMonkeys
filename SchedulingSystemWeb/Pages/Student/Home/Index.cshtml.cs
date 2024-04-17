using DataAccess;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.DotNet.Scaffolding.Shared;
using System.Drawing;
using System.Globalization;
using System.Security.Claims;

namespace SchedulingSystemWeb.Pages.Student.Home
{
    [Authorize(Roles = "STUDENT, TUTOR")]
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ICalendarService _calendarService;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEnumerable<Booking> Bookings { get; set; }
        public List<Availability> Availabilities { get; set; }

        public IEnumerable<Booking> ViewBookings { get; set; }
        public IEnumerable<Availability> ViewAvailabilities { get; set; }
        public IEnumerable<Category> ViewCategories { get; set; }
        public List<ApplicationUser> Teachers{get; set;}
        public DateTime CurrentDate { get; private set; }
        public List<DateTime> WeekDays { get; private set; } = new List<DateTime>();
        public List<DateTime> MonthDays { get; private set; }
        public string CurrentMonthName { get; private set; }
        public List<Location> Locations { get; set; }
        public List<ProviderProfile> Providers { get; set; }
        //public List<Category> categoryList { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager, UnitOfWork unitOfWork, ICalendarService calendarService)
        {
            _unitOfWork = unitOfWork;
            _calendarService = calendarService;
            Bookings = new List<Booking>();
            Availabilities = new List<Availability>();
            _userManager = userManager;
        }
       
        public async Task OnGetAsync()
        {
            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");

            WeekDays = _calendarService.GetWeekDays(CurrentDate);
            MonthDays = _calendarService.GetMonthDays(CurrentDate);

            var currentUser = await _userManager.GetUserAsync(User);
            string currentUserId = "";
            if (currentUser != null)
            {
                currentUserId = currentUser.Id;
            }

            Bookings = _unitOfWork.Booking.GetAll().Where(u=>u.User == currentUserId);
            Bookings = Bookings.ToList().OrderBy(obj => obj.StartTime); // Order bookings by date
            Providers = new List<ProviderProfile>();
            Locations = new List<Location>();
            Teachers = new List<ApplicationUser>();
            int i = 0;
            foreach (var booking in Bookings) // Adds Teachers to List for names
            {
                Availability aval = _unitOfWork.Availability.Get(a=> a.Id == booking.objAvailability);
                Locations.Add(_unitOfWork.Location.Get(l => l.LocationId == aval.LocationId));
                Providers.Add(_unitOfWork.ProviderProfile.Get(b => b.Id == booking.ProviderProfileID));
                Teachers.Add(_unitOfWork.ApplicationUser.Get(u => u.Id == Providers[i].User));
                i++;
            }
            await FetchDataForCurrentViewAsync();
        }

        public string GetUserName(string id)
        {
            return _unitOfWork.ApplicationUser.Get(i => i.Id == id).FullName;
        }
        public List<string> GetCategoryColors(List<int> categoryIds)
        {
            var categories = _unitOfWork.Category.GetAll().Where(c => categoryIds.Contains(c.Id)).ToList();
            return categories.Select(c => c.Color).ToList();
        }
        public string GetTextColor(string backgroundColor)
        {
            var color = ColorTranslator.FromHtml(backgroundColor);
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance > 0.5 ? "black" : "white";
        }

        public string GetBookingColors(int? bID)
        {
            return _unitOfWork.Category.Get(c => c.Id == bID).Color;
        }


        public async Task LoadAsync()
        {
            //    var tempProf = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            //    categoryList = _unitOfWork.Category.GetAll().Where(c => c.ProviderProfile == tempProf);
            //var currentUser = await _userManager.GetUserAsync(User);
            //var booking

        }


        public async Task<IActionResult> OnGetPreviousWeekAsync()
        {
            LoadAsync();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetNextWeekAsync()
        {
            LoadAsync();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetPreviousMonthAsync()
        {
            LoadAsync();
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
            LoadAsync();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetTodayWeekAsync()
        {
            LoadAsync();
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetTodayMonthAsync()
        {
            LoadAsync();
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        private async Task FetchDataForCurrentViewAsync()
        {
            DateTime startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            ViewBookings = Bookings.Where(b => b.StartTime.Date >= startOfMonth && b.StartTime.Date <= endOfMonth);
            ViewCategories = new List<Category>();
            foreach (var b in ViewBookings)
            {
                ViewCategories.Append(_unitOfWork.Category.Get(c => c.Id == b.CategoryID));
            }

        }
    }
}
