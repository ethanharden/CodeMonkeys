﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class LocationType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LocationTypeName { get; set; }

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        [DisplayName("City")]
        public string? AddressCity { get; set; }

    }
}
