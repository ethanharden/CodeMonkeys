using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class AvailabilityGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public IEnumerable<int> AvailabilityIDList { get; set; }


        [ForeignKey("RecurringTypeId")]
        public int RecurringType { get; set; }

        [Required]

        [DisplayName("End Date")]
        public DateTime? RecurringEndDate { get; set; }
    }
}
