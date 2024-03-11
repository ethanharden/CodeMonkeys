using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess;
using Infrastructure.Models;
using System.Globalization;
using Infrastructure.Interfaces;

namespace SchedulingSystemWeb.Pages.Availabilities
{
    public class UpsertModel : PageModel
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly ICalendarService _calendarService;
        public IEnumerable<Availability> Availabilities { get; set; }

        public IEnumerable<Availability> ViewAvailabilities { get; set; }

        public Availability objAvailability { get; set; }

        public DateTime CurrentDate { get; private set; }
        public List<DateTime> WeekDays { get; private set; } = new List<DateTime>();
        public List<DateTime> MonthDays { get; private set; }
        public string CurrentMonthName { get; private set; }
        [BindProperty]
        public List<DayOfWeek> SelectedDaysOfWeek { get; set; }

        public UpsertModel(UnitOfWork unitOfWork, ICalendarService calendarService)
        {
            _calendarService = calendarService;
            _unitOfWork = unitOfWork;
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
                },
                new Availability
                {
                    Id = 2,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 1, 9, 0, 0),
                    EndTime = new DateTime(2024, 4, 1, 12, 0, 0),
                }
            };
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");

            WeekDays = _calendarService.GetWeekDays(CurrentDate);
            MonthDays = _calendarService.GetMonthDays(CurrentDate);

            await FetchDataForCurrentViewAsync();

            if (id.HasValue)
            {
                //TEMP
                objAvailability = Availabilities.FirstOrDefault(a => a.Id == id.Value);

                //objAvailability = _unitOfWork.Availability.GetById(id.Value);
                if (objAvailability == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objAvailability = new Availability();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (objAvailability.Id == 0) // Creating new availabilities for each selected day
            {
                foreach (var day in SelectedDaysOfWeek)
                {
                    var newAvailability = new Availability
                    {
                        // Assign values
                        DayOfTheWeek = day,
                        StartTime = objAvailability.StartTime,
                        EndTime = objAvailability.EndTime,
                    };

                    // Save to database
                    //TEMP
                    //_unitOfWork.Availability.Add(newAvailability);
                }
            }
            else // Editing an availability
            {
                var existingAvailability = Availabilities.FirstOrDefault(a => a.Id == objAvailability.Id);
                // var existingAvailability = _unitOfWork.Availability.GetById(objAvailability.Id);
                if (existingAvailability != null)
                {
                    // Update properties of `existingAvailability` with values from `objAvailability`
                    existingAvailability.StartTime = objAvailability.StartTime;
                    existingAvailability.EndTime = objAvailability.EndTime;

                    //Temp
                    //_unitOfWork.Availability.Update(existingAvailability);             
                }
                else
                {
                    return NotFound();
                }
            }

            //Temp
            //await _unitOfWork.CommitAsync();
            return RedirectToPage("./Index");
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
            // Adjust these lines to match your actual method names and parameters
            DateTime startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            //TEMP
            //These will be used when the database is working, for now ignore them
            //Later i will want to also filter availabilities based on provider
            //ViewAvailabilities = await _unitOfWork.Availability.GetAllAsync(
            //    a => a.StartTime >= startOfMonth && a.StartTime <= endOfMonth
            //    );

            // TEMP
            ViewAvailabilities = Availabilities.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
        }

    }
}