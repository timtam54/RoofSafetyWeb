using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class EquipTypeTestFail
    {
        public int id { get; set; }
        public int? EquipTypeTestID { get; set; }
        public string? FailReason { get; set; }
    }
}
