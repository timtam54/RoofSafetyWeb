using System.ComponentModel.DataAnnotations;

namespace RoofSafety.Models
{
    public class Inspection
    {
        public int id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime InspectionDate { get; set; }
        public string? Areas { get; set; }
        public Building? Building { get; set; }
        [Display(Name = "Building")]
        public int BuildingID { get; set; }
        [Display(Name = "Inspector")]
        public int? InspectorID { get; set; }
        public Employee? Inspector { get; set; }
        public string? TestingInstruments { get; set; }
        public string? Photo { get; set; }
        public string? Status { get; set; }
    }

    //https://localhost:7017/api/inspections
    public class InpsectionView
    {
        public int id { get; set; }
        public string? Areas { get; set; }
        public string? ClientName { get; set; }
        public string? Address { get; set; }
        public string? TestingInstruments { get; set; }
        public string? Inspector { get; set; }
        public DateTime InspDate { get; set; }
        public string? Photo { get; set; }
        public string? Status { get; set; }
    }

}
