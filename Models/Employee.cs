using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class Employee
    {
        public int id { get; set; }
        [Display(Name = "Given Name")]
        public string? Given { get; set; }
        public string? Surname { get; set; }
        public bool? Inspector { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

    }



}
