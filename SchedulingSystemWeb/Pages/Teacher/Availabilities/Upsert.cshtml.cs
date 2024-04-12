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
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        public IEnumerable<RecurringType> RecurringTypes { get; set; }
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

        [BindProperty]
        public bool recurringCheckbox { get; set; }

        [BindProperty]
        public DateTime? RecurringEndDate { get; set; }
        [BindProperty]
        public int SelectedRecurringTypeId { get; set; }
        public IEnumerable<Category> AllCategories { get; set; }
        [BindProperty]
        public List<int> CategoryIds { get; set; } = new List<int>();


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
            Load();
            CurrentDate = (DateTime?)TempData["CurrentDate"] ?? DateTime.Today;
            TempData.Keep("CurrentDate");
            CurrentMonthName = CurrentDate.ToString("MMMM");
            WeekDays = _calendarService.GetWeekDays(CurrentDate);
            MonthDays = _calendarService.GetMonthDays(CurrentDate);


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
                objAvailability.Category = CategoryIds;
            }

            Availabilities = _unitOfWork.Availability.GetAll().Where(p => p.ProviderProfileID == objAvailability.ProviderProfileID);
            Bookings = _unitOfWork.Booking.GetAll().Where(p => p.ProviderProfileID == objAvailability.ProviderProfileID);

            await FetchDataForCurrentViewAsync();
            return Page();
        }

        public void Load()
        {
            RecurringTypes = _unitOfWork.RecurringType.GetAll();
            var tempProf = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id;
            AllCategories = _unitOfWork.Category.GetAll().Where(c => c.ProviderProfile == tempProf);
        }


        public async Task<IActionResult> OnPostAsync()
        {
            Load();
            if (!CategoryIds.Any())
            {
                ModelState.AddModelError("CategoryValidation", "At least one category must be selected.");
            }

            bool multipleDaysSelected = Request.Form["multipleDaysCheckbox"].FirstOrDefault() == "on";

            if (!multipleDaysSelected || (SelectedDaysOfWeek == null || !SelectedDaysOfWeek.Any()))
            {
                SelectedDaysOfWeek = new List<DayOfWeek> { MeetingDate.DayOfWeek };
            }

            ModelState.Remove("SelectedRecurringTypeId");
            ModelState.Remove("objAvailability.ProviderFullName");
            ModelState.Remove("objAvailability.Category");
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
            DayOfWeek firstDay = MeetingDate.DayOfWeek;

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
                    var datesForAvailabilities = CalculateWeeksForRecurring(startTime, RecurringEndDate.Value, daysBetween);

                    //for each week or biweekly
                    foreach (var date in datesForAvailabilities)
                    {
                        // Creating new availabilities for each selected day
                        foreach (var day in SelectedDaysOfWeek)
                        {
                            int diffInDay = (int)day - (int)firstDay;
                            if (diffInDay < 0)
                            {
                                diffInDay += 7;
                            }
                            DateTime weekStart = startTime.AddDays(diffInDay);
                            DateTime weekEnd = endTime.AddDays(diffInDay);

                            var newAvailability = new Availability
                            {
                                // Assign values
                                DayOfTheWeek = day,
                                StartTime = weekStart,
                                EndTime = weekEnd,
                                ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id,
                                ProviderFullName = _unitOfWork._ProviderProfile.Get(p => p.Id == objAvailability.ProviderProfileID).userFullName,
                                LocationId = 1,
                                Category = CategoryIds,
                        };

                            _unitOfWork.Availability.Add(newAvailability);
                            await _unitOfWork.CommitAsync();
                            avIds.Add(newAvailability.Id);
                        }
                        startTime = startTime.AddDays(daysBetween);
                        endTime = endTime.AddDays(daysBetween);

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
                        int diffInDay = (int)day - (int)firstDay;
                        if (diffInDay < 0)
                        {
                            diffInDay += 7;
                        }
                        DateTime weekStart = startTime.AddDays(diffInDay);
                        DateTime weekEnd = endTime.AddDays(diffInDay);
                        Availability newAvailability = new Availability
                        {
                            // Assign values
                            DayOfTheWeek = day,
                            StartTime = weekStart,
                            EndTime = weekEnd,
                            ProviderProfileID = _unitOfWork.ProviderProfile.Get(p => p.User == _userManager.GetUserId(User)).Id,
                            ProviderFullName = _unitOfWork._ProviderProfile.Get(p=>p.Id == objAvailability.ProviderProfileID).userFullName,
                            LocationId = 1,
                            Category = CategoryIds,
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
            Load();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public async Task<IActionResult> OnGetNextWeekAsync(int? id)
        {
            Load();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public async Task<IActionResult> OnGetPreviousMonthAsync(int? id)
        {
            Load();
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
            Load();
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public async Task<IActionResult> OnGetTodayWeekAsync(int? id)
        {
            Load();
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();

            return RedirectToPage(new { id = objAvailability?.Id });
        }
        public async Task<IActionResult> OnGetTodayMonthAsync(int? id)
        {
            Load();
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
        private List<DateTime> CalculateWeeksForRecurring(DateTime start, DateTime end, int daysBetween)
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
        //public void CreateCategory(string categoryName, string color)
        //{
        //    var newCategory = new Category
        //    {
        //        Name = categoryName,
        //        Color = color
        //    };
        //    if(newCategory.Name == "")
        //    {
        //        newCategory.Name = "New Category";
        //    }

        //    _unitOfWork.Category.Add(newCategory);
        //    _unitOfWork.Commit();
        //}

        private async Task FetchDataForCurrentViewAsync()
        {
            DateTime startOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            ViewAvailabilities = Availabilities?.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
            ViewBookings = Bookings?.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth).ToList() ?? new List<Booking>();

        }
    }
}