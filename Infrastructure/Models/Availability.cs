using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Availability
    {
        int DaysOfWeek {get; set;}   
        DateTime StartTime { get; set; }
        DateTime EndTime { get; set; }
        bool Recurring { get; set; }
        DateTime RecurringEndDate? { get; set; }
        bool Unvailable { get; set; } //switched to unavailable so on screen we can ask "Is this Unavailability?" Which makes more sense
    }
}
