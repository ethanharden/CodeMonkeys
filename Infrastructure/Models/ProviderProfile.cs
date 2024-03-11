﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class ProviderProfile
    {

        [Key]
        public int ProviderProfileID { get; set; }
        [Required]
        [Display(Name = "User")]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        public string? RemoteLink {  get; set; }
        public string? BookingPrompt { get; set; }
        public string? DepartmentString {  get; set; }
    }
}
