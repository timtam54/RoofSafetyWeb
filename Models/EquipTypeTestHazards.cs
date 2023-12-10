using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class EquipTypeTestHazards
    {
        public int id { get; set; }
        public int HazardID { get; set; }
        public int EquipTypeTestID { get; set; }
        public Hazard? Hazard { get; set; }
        public EquipTypeTest? EquipTypeTest { get; set; }
    }

    public class EquipTypeTestHazardMatrix
    {
        public List<Hazard> Hazards { get; set; }
        public List<EquipTypeTestRpt> EquipTypeTests { get; set; }

        public List<EquipTypeTestHazards> EquipTypeTestHazards { get; set; }
    }
}
