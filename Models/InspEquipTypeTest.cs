using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class InspEquipTypeTest
    {
        public int id { get; set; }
        public int InspEquipID { get; set; }

        [Display(Name = "Test")]
        public int EquipTypeTestID { get; set; }

       // public int Pass { get; set; }

        public string? Comment { get; set; }

        //public string? Reason { get; set; }

        public EquipTypeTest? EquipTypeTest { get; set; }

    }


    public class InspEquipTypeTestRpt
    {
        public int? Severity { get; set; }
        public int EquipTypeTestID { get; set; }
        public int id { get; set; }
        public int InspEquipID { get; set; }

        public string? EquipTypeTest { get; set; }

       // public int Pass { get; set; }

        public string? Comment { get; set; }
        public List<string>? Reason { get; set; }

       
    }
}
