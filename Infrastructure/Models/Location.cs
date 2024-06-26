﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        public string LocationName { get; set; }
        
        public string? BuildingName { get; set; }

        [DisplayName("Room Number")]
        public string? RoomNumber { get; set; }

        public int LocationType { get; set; }

        public int ProfileId { get; set; }
    }
}
