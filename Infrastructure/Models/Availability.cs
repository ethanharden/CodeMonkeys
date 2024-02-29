using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Availability
    {
        public int Id { get; set; }

        [Required]
        public List<DayOfWeek> DaysOfWeek { get; set; } = new List<DayOfWeek>();
        [Required]
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }
        [Required]
        [DisplayName("End Time")]
        public DateTime EndTime { get; set; }
        [Required]
        public bool Recurring { get; set; }
        [DisplayName("End Date")]
        public DateTime? RecurringEndDate { get; set; }
        [Required]
        public bool isUnvailable { get; set; } //switched to unavailable so on screen we can ask "Is this Unavailability?" Which makes more sense

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Ensuring EndTime is after StartTime
        if (EndTime <= StartTime)
        {
            yield return new ValidationResult(
                "EndTime must be after StartTime",
                new[] { nameof(EndTime) });
        }

        // ensure end date is after current date
        if (IsRecurring && RecurringEndDate.HasValue && RecurringEndDate.Value.Date <= DateTime.Now.Date)
        {
            yield return new ValidationResult(
                "End Date must be a future date",
                new[] { nameof(RecurringEndDate) });
        }

        //ensure end date is after start date
        if (IsRecurring && RecurringEndDate.HasValue && RecurringEndDate.Value <= StartTime)
        {
            yield return new ValidationResult(
                "End Date must be after Start Date",
                new[] { nameof(RecurringEndDate) });
        }


        //Use this on the form for Creation, and edit pages
        //<form method="post">
        //    < div asp - validation - summary = "ModelOnly" ></ div >
        //    < button type = "submit" > Submit </ button >
        //</ form >

        //@section Scripts {
        //    @{ await Html.RenderPartialAsync("_ValidationScriptsPartial"); }
        //        }
        //    }
    }
}
