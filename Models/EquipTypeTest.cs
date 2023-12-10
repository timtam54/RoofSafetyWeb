using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace RoofSafety.Models
{
    public class EquipTypeTest
    {
        public int id { get; set; }
        public int? EquipTypeID { get; set; }
        public string? Test { get; set; }
        public int? Severity { get; set; }

        public EquipType? EquipType { get; set; }
    }

    public class EquipTypeTestRpt
    {
        public int id { get; set; }
        public int? EquipTypeID { get; set; }
        public string? EquipTypeName { get; set; }
        public string? Test { get; set; }
       // public int? Severity { get; set; }

    }

    public class EquipTypeTestAll
    {
        public List<EquipTypeTest>? Tests { get; set; }
        public int? ETID { get; set; }
        public int? ETCopyID { get; set; }

    }

    public class EquipTypeTestMatrix
    {


             public List<EquipType> EquipTypes { get; set; }
        public List<EquipTypeTest> EquipTypeTests { get; set; }

       public List<string?> Tests { get; set; }
    }
}
