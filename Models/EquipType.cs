using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoofSafety.Models
{
    public class EquipType
    {
        public int id { get; set; }
        [Display(Name ="Equipment Type")]
        public string? EquipTypeDesc { get; set; }
        [NotMapped]
         public string? Tests { get; set; }
        [Display(Name = "Compliant Info")]
        public string? CompliantInfo { get; set; }
    }

}
