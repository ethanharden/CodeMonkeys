using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Availability
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ProviderId")]
        public int ProviderProfileID { get; set; }

        public string ProviderFullName { get; set; }

        [ForeignKey("AvailabilityGroupId")]
        public int? AvailabilityGroupID { get; set; }

        [Required]
        public int LocationId { get; set; }

        [Required]
        public DayOfWeek DayOfTheWeek { get; set; }

        [Required]
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }
        [Required]
        [DisplayName("End Time")]
        public DateTime EndTime { get; set; }
        
    }
}
