using DataAccess;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.Entity.Infrastructure;
using System.Globalization;

namespace SchedulingSystemWeb.Pages.Student.Bookings
{
    public class IndexModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ICalendarService _calendarService;
        [BindProperty]
        public ApplicationUser _ProfUser { get; set; }

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

        public IndexModel(UnitOfWork unitOfWork, ICalendarService calendarService)
        {
            _unitOfWork = unitOfWork;
            _calendarService = calendarService;
            Bookings = new List<Booking>();
            Availabilities = new List<Availability>();
            //SeedData();
        }
        //public void SeedData()
        //{
        //    // Temporary test data of Availabilities and book list
        //    Availabilities = new List<Availability>
        //    {
        //        new Availability
        //        {
        //            Id = 1,
        //            DayOfTheWeek = DayOfWeek.Friday,
        //            StartTime = new DateTime(2024, 3, 15, 9, 0, 0),
        //            EndTime = new DateTime(2024, 3, 15, 12, 0, 0),
        //        },
        //        new Availability
        //        {
        //            Id = 2,
        //            DayOfTheWeek = DayOfWeek.Monday,
        //            StartTime = new DateTime(2024, 4, 1, 9, 0, 0),
        //            EndTime = new DateTime(2024, 4, 1, 12, 0, 0),
        //        }
        //    };

        //    Bookings = new List<Booking>
        //    {
        //        new Booking
        //        {
        //            Id = 1,
        //            Subject = "Team Meeting",
        //            Note = "Discuss project milestones",
        //            StartTime = new DateTime(2024, 3, 14, 14, 0, 0),
        //            Duration = 2,
        //        }
        //    };
        //}


        public async Task OnGetAsync(string UserId)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);
            _ProfUser = user;
            provider = _unitOfWork.ProviderProfile.Get(p=>p.User == user);
 

            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");

            WeekDays = _calendarService.GetWeekDays(CurrentDate);
            MonthDays = _calendarService.GetMonthDays(CurrentDate);

            Availabilities = _unitOfWork.Availability.GetAll().Where(p => p.ProviderProfileID == provider.Id);
            Bookings = _unitOfWork.Booking.GetAll().Where(p => p.ProviderProfileID == provider.Id);

            await FetchDataForCurrentViewAsync();
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
            MonthDays = _calendarService.GetMonthDays(CurrentDate);
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
            DateTime startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Filter bookings within the month.
            ViewBookings = Bookings.Where(b => b.StartTime.Date >= startOfMonth && b.StartTime.Date <= endOfMonth);
            // Filter availabilities within the month. Adjust this logic if your availabilities work differently.
            ViewAvailabilities = Availabilities.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
        }

    }
}
