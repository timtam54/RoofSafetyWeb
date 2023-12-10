using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class Hazard
    {
        public int id { get; set; }

        [Display(Name = "Hazards If Non Compliant")]

        public string? Detail { get; set; }
    }
}
