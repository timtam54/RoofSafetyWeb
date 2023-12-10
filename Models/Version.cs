using System.ComponentModel.DataAnnotations;

namespace RoofSafety.Models
{
    public class Version
    {
        public int id { get; set; }
        public int? VersionNo { get; set; }
        public string? VersionType { get; set; }
        public int? AuthorID { get; set; }
        public string? Information { get; set; }
        [Display(Name = "Inspection")]
        public int InspectionID { get; set; }
    }

    public class VersionRpt
    {
        public int id { get; set; }
        public int? VersionNo { get; set; }
        public string? VersionType { get; set; }
        public string? Author { get; set; }
        public string? Information { get; set; }
        public int InspectionID { get; set; }
    }
}
