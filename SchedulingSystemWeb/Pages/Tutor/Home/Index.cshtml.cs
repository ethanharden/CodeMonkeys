using DataAccess;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;

namespace SchedulingSystemWeb.Pages.Tutor.Home
{
    [Authorize(Roles = "TUTOR, TEACHER, ADVISOR")]
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ICalendarService _calendarService;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEnumerable<Booking> Bookings { get; set; }
        public IEnumerable<Booking> BookingsWithMe { get; set; }
        public IEnumerable<Availability> Availabilities { get; set; }

        public IEnumerable<Booking> ViewBookings { get; set; }
        public IEnumerable<Availability> ViewAvailabilities { get; set; }

        public List<ApplicationUser> Teachers { get; set; }

        public DateTime CurrentDate { get; private set; }
        public List<DateTime> WeekDays { get; private set; } = new List<DateTime>();
        public List<DateTime> MonthDays { get; private set; }
        public string CurrentMonthName { get; private set; }
        public IEnumerable<Booking> nextBookings { get; set; }
        public IEnumerable<Booking> nextAppointments { get; set; }

        public IEnumerable<Category> categoryList { get; set; }

        public List<Location> Locations { get; set; }
        public List<ProviderProfile> Providers { get; set; }


        public IndexModel(UnitOfWork unitOfWork, ICalendarService calendarService, UserManager<ApplicationUser> userManager)
        {
            _calendarService = calendarService;
            _unitOfWork = unitOfWork;
            Bookings = new List<Booking>();
            BookingsWithMe = new List<Booking>();
            Availabilities = new List<Availability>();
            Teachers = new List<ApplicationUser>();
            _userManager = userManager;
            Providers = new List<ProviderProfile>();
            Locations = new List<Location>();
        }

        public async Task OnGetAsync()
        {
            Load();
            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");

            WeekDays = _calendarService.GetWeekDays(CurrentDate);
            MonthDays = _calendarService.GetMonthDays(CurrentDate);

            int provId = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            string userId = _userManager.GetUserId(User);
            Availabilities = _unitOfWork.Availability.GetAll().Where(a => a.ProviderProfileID == provId);
            Bookings = _unitOfWork.Booking.GetAll().Where(p => p.User == userId);
            BookingsWithMe = _unitOfWork.Booking.GetAll().Where(a => a.ProviderProfileID == provId);

            nextBookings = _unitOfWork.Booking.GetAll().Where(b => b.StartTime.Date >= DateTime.Today && b.ProviderProfileID == provId).OrderBy(b => b.StartTime).Take(5).ToList();
            nextAppointments = _unitOfWork.Booking.GetAll().Where(b => b.StartTime.Date >= DateTime.Today && b.User == userId).OrderBy(b => b.StartTime).Take(5).ToList();

            int i = 0;
            foreach (var booking in Bookings) // Adds Teachers to List for names
            {
                Availability aval = _unitOfWork.Availability.Get(a => a.Id == booking.objAvailability);
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


        public void Load()
        {
            var tempProf = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            categoryList = _unitOfWork.Category.GetAll().Where(c => c.ProviderProfile == tempProf);
        }
        public async Task<IActionResult> OnGetPreviousWeekAsync()
        {
            Load();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetNextWeekAsync()
        {
            Load();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetPreviousMonthAsync()
        {
            Load();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(-1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            CurrentMonthName = CurrentDate.ToString("MMMM");
            MonthDays = _calendarService.GetMonthDays(CurrentDate);
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetNextMonthAsync()
        {
            Load();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetTodayWeekAsync()
        {
            Load();
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }
        public async Task<IActionResult> OnGetTodayMonthAsync()
        {
            Load();
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

        public bool IsAvailabilityBooked1(Availability availability)
        {
            return BookingsWithMe.Any(booking => booking.StartTime >= availability.StartTime && booking.StartTime < availability.EndTime);

        }
        private async Task FetchDataForCurrentViewAsync()
        {
            DateTime startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Filter bookings within the month.
            // Filter bookings within the month.
            ViewBookings = Bookings.Where(b => b.StartTime.Date >= startOfMonth && b.StartTime.Date <= endOfMonth);
            // Filter availabilities within the month. Adjust this logic if your availabilities work differently.
            ViewAvailabilities = Availabilities.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
        }

    }
}