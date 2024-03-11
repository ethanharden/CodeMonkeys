// DataAccess/CalendarService.cs
using Infrastructure.Interfaces;
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess
{
    public class CalendarService : ICalendarService
    {
        private readonly UnitOfWork _unitOfWork;

        public CalendarService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<DateTime> GetWeekDays(DateTime currentDate)
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

        public List<DateTime> GetMonthDays(DateTime currentDate)
        {
            var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            var monthDays = new List<DateTime>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                monthDays.Add(new DateTime(currentDate.Year, currentDate.Month, day));
            }

            return monthDays;
        }

        public string GetCurrentMonthName(DateTime currentDate)
        {
            return currentDate.ToString("MMMM", CultureInfo.CurrentCulture);
        }

    }
}
