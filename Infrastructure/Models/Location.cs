using System;
using System.Collections.Generic;
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

        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string AddressCity { get; set; }

        public string BuildingName { get; set; }

        public int RoomNumber { get; set; }
    }
}
