// Infrastructure.Interfaces/ICalendarService.cs
using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ICalendarService
    {
        List<DateTime> GetWeekDays(DateTime currentDate);
        List<DateTime> GetMonthDays(DateTime currentDate);
        string GetCurrentMonthName(DateTime currentDate);


    }
}
