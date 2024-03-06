using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SchedulingSystemWeb.Pages.Availabilities
{
    public class IndexModel : PageModel
    {
        public DateTime CurrentDate { get; private set; }
        public List<DateTime> WeekDays { get; private set; } = new List<DateTime>();
        public List<DateTime> MonthDays { get; private set; }
        public string CurrentMonthName { get; private set; }

        public void OnGet()
        {
            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");

            WeekDays = GetWeekDays(CurrentDate);
            MonthDays = GetMonthDays(CurrentDate);
        }

        private List<DateTime> GetWeekDays(DateTime currentDate)
        {
            var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
            var startDate = currentDate.Date;

            while (startDate.DayOfWeek != firstDayOfWeek)
            {
                startDate = startDate.AddDays(-1);
            }

            var weekDays = new List<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                weekDays.Add(startDate.AddDays(i));
            }

            return weekDays;
        }

        private List<DateTime> GetMonthDays(DateTime currentDate)
        {
            var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            var monthDays = new List<DateTime>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                monthDays.Add(new DateTime(currentDate.Year, currentDate.Month, day));
            }

            return monthDays;
        }

        public IActionResult OnGetPreviousWeek()
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            CurrentMonthName = CurrentDate.ToString("MMMM");
            WeekDays = GetWeekDays(CurrentDate);
            MonthDays = GetMonthDays(CurrentDate);
            return Page();
        }
        public IActionResult OnGetToday()
        {
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            return RedirectToPage();
        }

        public IActionResult OnGetNextWeek()
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            CurrentMonthName = CurrentDate.ToString("MMMM");
            WeekDays = GetWeekDays(CurrentDate);
            MonthDays = GetMonthDays(CurrentDate);
            return Page();
        }

        public IActionResult OnGetPreviousMonth()
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(-1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            CurrentMonthName = CurrentDate.ToString("MMMM");
            MonthDays = GetMonthDays(CurrentDate);
            return Page();
        }

        public IActionResult OnGetNextMonth()
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            CurrentMonthName = CurrentDate.ToString("MMMM");
            MonthDays = GetMonthDays(CurrentDate);
            return Page();
        }

        public IActionResult OnGetTodayMonth()
        {
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            return RedirectToPage();
        }
    }
}

        //ignore this for now

        //private readonly UnitOfWork _unitOfWork;
        //public IEnumerable<Availability> AvailabilitiesList { get; set; }
        //public string CalendarEvents { get; set; }

        //public IndexModel(UnitOfWork unitOfWork)
        //{
        //    _unitOfWork = unitOfWork;
        //}

        //public void OnGet()
        //{
        //    AvailabilitiesList = _unitOfWork.Availability.GetAll(includes: "ProviderProfile,ProviderProfile.User");

        //    var events = AvailabilitiesList.Select(a => new
        //    {
        //        title = $"Availability for {a.ProviderProfile.User.UserName}", // Adjust according to actual data model
        //        start = a.StartTime.ToString("s"), // ISO format
        //        end = a.EndTime.ToString("s"),
        //        allDay = false
        //    }).ToList();
        //    CalendarEvents = System.Text.Json.JsonSerializer.Serialize(events);
        //}
