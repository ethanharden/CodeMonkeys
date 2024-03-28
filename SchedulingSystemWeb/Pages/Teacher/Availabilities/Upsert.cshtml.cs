using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess;
using Infrastructure.Models;
using System.Globalization;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SchedulingSystemWeb.Pages.Availabilities
{
    public class UpsertModel : PageModel
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly ICalendarService _calendarService;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEnumerable<Availability> Availabilities { get; set; }
        public IEnumerable<Booking> Bookings { get; set; }
        public IEnumerable<Availability> ViewAvailabilities { get; set; }
        public IEnumerable<Booking> ViewBookings { get; set; }
        public ProviderProfile providerProfileGet { get; set; }

        [BindProperty]
        public Availability objAvailability { get; set; }

        [BindProperty]
        public DateTime MeetingDate { get; set; }

        [BindProperty]
        public TimeSpan MeetingStartTime { get; set; }

        [BindProperty]
        public TimeSpan MeetingEndTime { get; set; }

        public DateTime CurrentDate { get; private set; }
        public List<DateTime> WeekDays { get; private set; } = new List<DateTime>();
        public List<DateTime> MonthDays { get; private set; }
        public string CurrentMonthName { get; private set; }
        [BindProperty]
        public List<DayOfWeek> SelectedDaysOfWeek { get; set; }
        public List<RecurringType> RecurringTypes { get; set; }

        [BindProperty]
        public bool recurringCheckbox { get; set; }

        [BindProperty]
        public DateTime? RecurringEndDate { get; set; }
        [BindProperty]
        public int SelectedRecurringTypeId { get; set; }


        public UpsertModel(UnitOfWork unitOfWork, ICalendarService calendarService, UserManager<ApplicationUser> userManager)
        {
            _calendarService = calendarService;
            _unitOfWork = unitOfWork;
            ViewAvailabilities = new List<Availability>();
            ViewBookings = new List<Booking>();
            _userManager = userManager;

        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");
            WeekDays = _calendarService.GetWeekDays(CurrentDate);
            MonthDays = _calendarService.GetMonthDays(CurrentDate);
            RecurringTypes = _unitOfWork.RecurringType.GetAll().ToList();

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
                    ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id,
                };
            }

            Availabilities = _unitOfWork.Availability.GetAll().Where(p => p.ProviderProfileID == objAvailability.ProviderProfileID);
            Bookings = _unitOfWork.Booking.GetAll().Where(p => p.ProviderProfileID == objAvailability.ProviderProfileID);

            await FetchDataForCurrentViewAsync();
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (SelectedDaysOfWeek == null || !SelectedDaysOfWeek.Any())
            {
                ModelState.AddModelError("SelectedDaysOfWeek", "Please select at least one day of the week.");
                return Page();
            }

            ModelState.Remove("SelectedRecurringTypeId");
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
            DateTime startTime = MeetingDate.Add(MeetingStartTime);
            DateTime endTime = MeetingDate.Add(MeetingEndTime);

            // Ensure the end time is after the start time
            if (endTime <= startTime)
            {
                ModelState.AddModelError("", "End time must be after start time.");
                return Page(); // Return with error
            }

            if (objAvailability.Id == 0)
            {

                if (recurringCheckbox && RecurringEndDate != null)//recurring checkbox is checked
                {
                    var recurringType = _unitOfWork.RecurringType.GetById(SelectedRecurringTypeId);
                    var daysBetween = recurringType.DaysBetween;
                    var avIds = new List<int>();
                    var datesForAvailabilities = CalculateDatesForRecurring(startTime, RecurringEndDate.Value, daysBetween); //needs to return a list of dates
                    
                    //for each week or biweekly
                    foreach (var date in datesForAvailabilities)
                    {

                        // Creating new availabilities for each selected day
                        foreach (var day in SelectedDaysOfWeek)
                        {
                            var newAvailability = new Availability
                            {
                                // Assign values
                                DayOfTheWeek = day,
                                StartTime = startTime,
                                EndTime = endTime,
                                ProviderProfileID = objAvailability.ProviderProfileID,
                                LocationId = 1,
                            };

                            _unitOfWork.Availability.Add(newAvailability);
                            await _unitOfWork.CommitAsync();
                            avIds.Add(newAvailability.Id);
                        }
                    }
                    var availabilityGroup = new AvailabilityGroup
                    {
                        RecurringType = SelectedRecurringTypeId,
                        RecurringEndDate = RecurringEndDate,
                        AvailabilityIDList = avIds,
                    };
                    
                    _unitOfWork.AvailabilityGroup.Add(availabilityGroup);
                    await _unitOfWork.CommitAsync();

                    foreach (var availabilityId in avIds)
                    {
                        var availabilityToUpdate = _unitOfWork.Availability.GetById(availabilityId);
                        if (availabilityToUpdate != null)
                        {
                            availabilityToUpdate.AvailabilityGroupID = availabilityGroup.Id;
                            _unitOfWork.Availability.Update(availabilityToUpdate);
                        }
                    }
                    await _unitOfWork.CommitAsync();
                }
                else 
                {
                    foreach (var day in SelectedDaysOfWeek)
                    {

                        var newAvailability = new Availability
                        {
                            // Assign values
                            DayOfTheWeek = day,
                            StartTime = startTime,
                            EndTime = endTime,
                            ProviderProfileID = objAvailability.ProviderProfileID,
                            LocationId = 1,
                        };
                        _unitOfWork.Availability.Add(newAvailability);
                    }                 
                    await _unitOfWork.CommitAsync();
                }
            }
            else
            {
                // Editing an existing availability
                var existingAvailability = _unitOfWork.Availability.GetById(objAvailability.Id);
                if (existingAvailability != null)
                {
                    // Update properties of existingAvailability with values from objAvailability
                    existingAvailability.StartTime = startTime;
                    existingAvailability.EndTime = endTime;
                    existingAvailability.DayOfTheWeek = startTime.DayOfWeek;
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

        private List<DateTime> CalculateDatesForRecurring(DateTime start, DateTime end, int daysBetween)
        {
            var dates = new List<DateTime>();
            int multiplier = 1;
            for (var dt = start; dt <= end; dt = dt.AddDays(daysBetween))
            {   
                dates.Add(dt.AddDays(daysBetween));
                multiplier *= daysBetween;
            }
            return dates;
        }

        private async Task FetchDataForCurrentViewAsync()
        {
            DateTime startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            ViewAvailabilities = Availabilities?.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
            ViewBookings = Bookings?.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth).ToList() ?? new List<Booking>();

        }
    }
}