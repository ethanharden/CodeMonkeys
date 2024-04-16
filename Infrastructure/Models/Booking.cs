using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Infrastructure.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public int? WNumber { get; set; }
        [Required]
        public string MeetingTitle {  get; set; }
        
        public string? Subject { get; set; }
        public string? Note { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public int Duration { get; set; }
        public List<string>? Attachment {  get; set; }

        [ForeignKey("ProviderId")]
        public int ProviderProfileID { get; set; }
        [ForeignKey("UserId")]
        public string User { get; set; }

        [ForeignKey("AvailabilityId")]
        public int objAvailability { get; set; }
        [ForeignKey("AvailabilityId")]
        public int? CategoryID { get; set; }
        public int LocationID { get; set; }
    }
}
