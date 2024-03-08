using DataAccess;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SchedulingSystemWeb.Pages.Availabilities
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        public IEnumerable<Booking> Bookings { get; set; }
        public IEnumerable<Availability> Availabilities { get; set; }

        public IEnumerable<Booking> ViewBookings { get; set; }
        public IEnumerable<Availability> ViewAvailabilities { get; set; }

        public DateTime CurrentDate { get; private set; }
        public List<DateTime> WeekDays { get; private set; } = new List<DateTime>();
        public List<DateTime> MonthDays { get; private set; }
        public string CurrentMonthName { get; private set; }

        public IndexModel(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            Bookings = new List<Booking>();
            Availabilities = new List<Availability>();
            SeedData();
        }
        public void SeedData()
        {
            // Temporary seeding of Availabilities and objBookingList
            Availabilities = new List<Availability>
            {
                new Availability
                {
                    Id = 1,
                    DayOfTheWeek = DayOfWeek.Friday,
                    StartTime = new DateTime(2024, 3, 15, 9, 0, 0),
                    EndTime = new DateTime(2024, 3, 15, 12, 0, 0),
                    Recurring = false
                },
                new Availability
                {
                    Id = 2,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 1, 9, 0, 0),
                    EndTime = new DateTime(2024, 4, 1, 12, 0, 0),
                    Recurring = false
                }
            };

            Bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    Subject = "Team Meeting",
                    Note = "Discuss project milestones",
                    StartTime = new DateTime(2024, 3, 14, 14, 0, 0),
                    Duration = 2,
                }
            };
        }


        public async Task OnGetAsync()
        {
            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");

            WeekDays = GetWeekDays(CurrentDate);
            MonthDays = GetMonthDays(CurrentDate);

            await FetchDataForCurrentViewAsync();
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

        public async Task<IActionResult> OnGetPreviousWeekAsync()
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetNextWeekAsync()
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetPreviousMonthAsync()
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(-1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            CurrentMonthName = CurrentDate.ToString("MMMM");
            MonthDays = GetMonthDays(CurrentDate);
            await FetchDataForCurrentViewAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetNextMonthAsync()
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetTodayWeekAsync()
        {
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();
            return RedirectToPage(); 
        }

        public async Task<IActionResult> OnGetTodayMonthAsync()
        {
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync(); 
            return RedirectToPage();
        }

        private async Task FetchDataForCurrentViewAsync()
        {
            // Adjust these lines to match your actual method names and parameters
            DateTime startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);


            //These will be used when the database is working, for now ignore them
            //ViewBookings = await _unitOfWork.Booking.GetAllAsync(
            //    b => b.StartTime >= startOfMonth && b.StartTime <= endOfMonth);

            //ViewAvailabilities = await _unitOfWork.Availability.GetAllAsync(
            //    a => a.StartTime >= startOfMonth && a.StartTime <= endOfMonth
            //    );

            // Filter bookings within the month.
            ViewBookings = Bookings.Where(b => b.StartTime.Date >= startOfMonth && b.StartTime.Date <= endOfMonth);

            // Filter availabilities within the month. Adjust this logic if your availabilities work differently.
            ViewAvailabilities = Availabilities.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
        }

    }
}