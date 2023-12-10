using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class InspEquipTypeTestFail
    {
        public int id { get; set; }
        public int InspEquipTypeTestID { get; set; }
        public int EquipTypeTestFailID { get; set; }
    }
}
