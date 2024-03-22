﻿using System;
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
        public int WNumber { get; set; } 
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Note { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public int Duration { get; set; }
        public string Attachment {  get; set; }

        [ForeignKey("ProviderId")]
        public int ProviderProfileID { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [ForeignKey("AvailabilityId")]
        public Availability objAvailability { get; set; }
    }
}
