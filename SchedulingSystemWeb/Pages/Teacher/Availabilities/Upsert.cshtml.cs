using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess;
using Infrastructure.Models;
using System.Globalization;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;


namespace SchedulingSystemWeb.Pages.Availabilities
{
    public class UpsertModel : PageModel
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly ICalendarService _calendarService;
        private readonly IMemoryCache _memoryCache;

        public IEnumerable<Availability> Availabilities { get; set; }
        public IEnumerable<Booking> Bookings { get; set; }
        public IEnumerable<Availability> ViewAvailabilities { get; set; }
        public IEnumerable<Booking> ViewBookings { get; set; }
        public ProviderProfile providerProfileGet { get; set; }

        [BindProperty]
        public Availability objAvailability { get; set; }

        public DateTime CurrentDate { get; private set; }
        public List<DateTime> WeekDays { get; private set; } = new List<DateTime>();
        public List<DateTime> MonthDays { get; private set; }
        public string CurrentMonthName { get; private set; }
        [BindProperty]
        public List<DayOfWeek> SelectedDaysOfWeek { get; set; }

        public UpsertModel(UnitOfWork unitOfWork, ICalendarService calendarService, IMemoryCache memoryCache)
        {
            _calendarService = calendarService;
            _unitOfWork = unitOfWork;
            ViewAvailabilities = new List<Availability>();
            ViewBookings = new List<Booking>();
            _memoryCache = memoryCache;

        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");
            WeekDays = _calendarService.GetWeekDays(CurrentDate);
            MonthDays = _calendarService.GetMonthDays(CurrentDate);

           // providerProfileGet = _unitOfWork.ProviderProfile.Get(p => p.ProviderProfileID == 5);

            //Availabilities = _unitOfWork.Availability.GetAll().Where(p => p.ProviderProfile == providerProfileGet);
            //Bookings = _unitOfWork.Booking.GetAll().Where(p => p.ProviderProfile == providerProfileGet);



            if (providerProfileGet == null)
            {
                TempData["ErrorMessage"] = "Provider profile not found.";
                return Page();
            }

            if (id.HasValue && id != 0)
            {
                objAvailability = _unitOfWork.Availability.GetById(id.Value);
                if (objAvailability == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objAvailability = new Availability
                {
                    Id = 0,
                    StartTime = DateTime.Today.AddDays(1), // Set to tomorrow's date
                    EndTime = DateTime.Today.AddDays(1).AddHours(1), // Set to tomorrow's date + 1 hour
               //     ProviderProfile = providerProfileGet
                };
            }

            await FetchDataForCurrentViewAsync();
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage)).ToList();
                TempData["Errors"] = errorList;
                TempData["FormStatus"] = "Failed";
                TempData["ErrorMessage"] = "Please ensure all required fields are filled and valid.";
                return Page();
            }
            if (objAvailability == null)
            {
                return Page();
            }
            if (objAvailability.Id == 0)
            {
                // Creating new availabilities for each selected day
                foreach (var day in SelectedDaysOfWeek)
                {
                    var newAvailability = new Availability
                    {
                        // Assign values
                        DayOfTheWeek = day,
                        StartTime = objAvailability.StartTime,
                        EndTime = objAvailability.EndTime,
                      //  ProviderProfile = objAvailability.ProviderProfile,
                        LocationId = 1,
                    };

                    _unitOfWork.Availability.Add(newAvailability); //Getting an error here
                }
            }
            else
            {
                // Editing an existing availability
                var existingAvailability = _unitOfWork.Availability.GetById(objAvailability.Id);
                if (existingAvailability != null)
                {
                    // Update properties of existingAvailability with values from objAvailability
                    existingAvailability.StartTime = objAvailability.StartTime;
                    existingAvailability.EndTime = objAvailability.EndTime;
                    existingAvailability.DayOfTheWeek = objAvailability.DayOfTheWeek;
                    _unitOfWork.Availability.Update(existingAvailability);
                }
                else
                {
                    return NotFound();
                }
            }

            await _unitOfWork.CommitAsync();
            TempData["FormStatus"] = "Success";
            TempData["SuccessMessage"] = "Availability saved successfully.";
            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnGetPreviousWeekAsync(int? id)
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public async Task<IActionResult> OnGetNextWeekAsync(int? id)
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public async Task<IActionResult> OnGetPreviousMonthAsync(int? id)
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(-1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            CurrentMonthName = CurrentDate.ToString("MMMM");
            MonthDays = _calendarService.GetMonthDays(CurrentDate);
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public async Task<IActionResult> OnGetNextMonthAsync(int? id)
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public async Task<IActionResult> OnGetTodayWeekAsync(int? id)
        {
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public async Task<IActionResult> OnGetTodayMonthAsync(int? id)
        {
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public bool IsAvailabilityBooked(Availability availability)
        {
            return Bookings.Any(booking => booking.StartTime >= availability.StartTime && booking.StartTime < availability.EndTime);
        }

        private async Task FetchDataForCurrentViewAsync()
        {
            // Adjust these lines to match your actual method names and parameters
            DateTime startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);


            // Keys for cache
            string cacheKeyAvailabilities = "availabilities";
            string cacheKeyBookings = "bookings";

            //// Try to get data from cache
            //if (!_memoryCache.TryGetValue(cacheKeyAvailabilities, out IEnumerable<Availability> availabilities))
            //{
            //    // If not in cache, fetch from database and cache it
            //    availabilities = await _unitOfWork.Availability.GetAllAsync(a => true);
            //    _memoryCache.Set(cacheKeyAvailabilities, availabilities, TimeSpan.FromMinutes(30)); // Adjust expiration as needed
            //}

            //if (!_memoryCache.TryGetValue(cacheKeyBookings, out IEnumerable<Booking> bookings))
            //{
            //    bookings = await _unitOfWork.Booking.GetAllAsync(b => true);
            //    _memoryCache.Set(cacheKeyBookings, bookings, TimeSpan.FromMinutes(30)); // Adjust expiration as needed
            //}

            //Ensures ViewAvailabilities and ViewBookings are never null
            if (Bookings == null)
            {
                Bookings = Bookings ?? Enumerable.Empty<Booking>();
            }
            if (Availabilities == null)
            {
                Availabilities = Availabilities ?? Enumerable.Empty<Availability>();
            }

            ViewAvailabilities = Availabilities?.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
            ViewBookings = Bookings.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
        }
    }
}