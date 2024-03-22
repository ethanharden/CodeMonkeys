using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Models
{
    public class CustomerProfile
    {
        [Key]
        public int Id { get; set; }



        [ForeignKey("UserId")]
        public string User { get; set; }

        public int? WNumber {  get; set; }
        
    }
}
