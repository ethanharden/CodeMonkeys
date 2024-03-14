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
        public IEnumerable<Booking> Bookings { get; set; }
        public IEnumerable<Availability> ViewAvailabilities { get; set; }
        public IEnumerable<Booking> ViewBookings { get; set; }

        public Availability objAvailability { get; set; }
        AvailabilityGroup availabilityGroup = null;

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
            // Temporary seeding of Availabilities and Bookings
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
                },
                new Availability
                {
                    Id = 3,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 10, 13, 0, 0),
                    EndTime =   new DateTime(2024, 4, 10, 14, 0, 0),
                },
                new Availability
                {
                    Id = 4,
                    DayOfTheWeek = DayOfWeek.Monday,
                    StartTime = new DateTime(2024, 4, 12, 13, 0, 0),
                    EndTime =   new DateTime(2024, 4, 12, 14, 0, 0),
                }
            };

            Bookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    Subject = "Team Meeting",
                    Note = "Discuss project milestones",
                    StartTime = new DateTime(2024, 3, 15, 9, 0, 0),
                    Duration = 3,
                    objAvailability = Availabilities.FirstOrDefault(a => a.Id == 1)
                },

                new Booking
                {
                    Id = 2,
                    Subject = "Client Consultation",
                    Note = "Initial project discussion",
                    StartTime = new DateTime(2024, 4, 1, 9, 0, 0),
                    Duration = 1,
                    objAvailability = Availabilities.FirstOrDefault(a => a.Id == 1),
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
            await FetchDataForCurrentViewAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            AvailabilityGroup availabilityGroup = null;
            if (Request.Form["Recurring"].FirstOrDefault() == "on") // Check if recurring is checked
            {
                // Create an AvailabilityGroup to assign to new Availabilities
                availabilityGroup = new AvailabilityGroup
                {
                    // TODO: Set properties like RecurringEndDate and RecurringType
                };
            }

           

            if (objAvailability.Id == 0) 
            {            
                if (Request.Form["Recurring"].FirstOrDefault() == "on") // Check if recurring is checked
                {
                    // Create or find an AvailabilityGroup to assign to new Availabilities
                    availabilityGroup = new AvailabilityGroup
                    {
                        // Set properties like RecurringEndDate and RecurringType
                    };
                }

                foreach (var dateTime in GetRecurringDates(objAvailability.StartTime, Request.Form["RecurringEndDate"]))
                {
                    foreach (var day in SelectedDaysOfWeek) // Creating new availabilities for each selected day
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

                    existingAvailability.AvailabilityGroup = objAvailability.AvailabilityGroup;

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

        public async Task<IActionResult> OnGetPreviousWeekAsync(int? id)
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(-7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();

            if (id.HasValue)
            {
                objAvailability = Availabilities.FirstOrDefault(a => a.Id == id.Value);
                if (objAvailability == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objAvailability = new Availability();
            }

            return RedirectToPage(new { id = objAvailability?.Id });
        }
      
        public async Task<IActionResult> OnGetNextWeekAsync(int? id)
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddDays(7);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();

            if (id.HasValue)
            {
                objAvailability = Availabilities.FirstOrDefault(a => a.Id == id.Value);
                if (objAvailability == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objAvailability = new Availability();
            }

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

            if (id.HasValue)
            {
                objAvailability = Availabilities.FirstOrDefault(a => a.Id == id.Value);
                if (objAvailability == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objAvailability = new Availability();
            }

            return RedirectToPage(new { id = objAvailability?.Id });
        }

        public async Task<IActionResult> OnGetNextMonthAsync(int? id)
        {
            CurrentDate = ((DateTime?)TempData["CurrentDate"] ?? DateTime.Today).AddMonths(1);
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();

            if (id.HasValue)
            {
                objAvailability = Availabilities.FirstOrDefault(a => a.Id == id.Value);
                if (objAvailability == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objAvailability = new Availability();
            }

            return RedirectToPage(new { id = objAvailability?.Id });
        }

        public async Task<IActionResult> OnGetTodayWeekAsync(int? id)
        {
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "weekly";
            await FetchDataForCurrentViewAsync();

            if (id.HasValue)
            {
                objAvailability = Availabilities.FirstOrDefault(a => a.Id == id.Value);
                if (objAvailability == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objAvailability = new Availability();
            }

            return RedirectToPage(new { id = objAvailability?.Id });
        }

        public async Task<IActionResult> OnGetTodayMonthAsync(int? id)
        {
            CurrentDate = DateTime.Today;
            TempData["CurrentDate"] = CurrentDate;
            TempData["ActiveTab"] = "monthly";
            await FetchDataForCurrentViewAsync();

            if (id.HasValue)
            {
                objAvailability = Availabilities.FirstOrDefault(a => a.Id == id.Value);
                if (objAvailability == null)
                {
                    return NotFound();
                }
            }
            else
            {
                objAvailability = new Availability();
            }

            return RedirectToPage(new { id = objAvailability?.Id });
        }

        public bool IsAvailabilityBooked(Availability availability)
        {
            return Bookings.Any(booking => booking.StartTime >= availability.StartTime && booking.StartTime < availability.EndTime);
        }
        private IEnumerable<DateTime> GetRecurringDates(DateTime startDate, string recurringEndDate)
        {
            DateTime end = Convert.ToDateTime(recurringEndDate);
            List<DateTime> dates = new List<DateTime>();
            // calculate dates based on the recurring pattern

            return dates;
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

            //I may need to add View bookings?
            //ViewBookings = await _unitOfWork.Availability.GetAllAsync(
            //    a => a.StartTime >= startOfMonth && a.StartTime <= endOfMonth
            //    );


            // TEMP
            ViewAvailabilities = Availabilities.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
            ViewBookings = Bookings.Where(a => a.StartTime.Date >= startOfMonth && a.StartTime.Date <= endOfMonth);
        }

    }
}