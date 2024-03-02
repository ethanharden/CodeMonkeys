using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class ProviderProfile
    {

        [Key]
        public string ProviderProfileID { get; set; }

        public string RemoteLink {  get; set; }
        public string BookingPrompt { get; set; }


        [ForeignKey("UserId")]
        public User? User { get; set; }

    }
}
