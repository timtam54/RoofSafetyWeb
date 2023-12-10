using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace RoofSafety.Models
{
    public class InspEquip
    {
        public int id { get; set; }
        [Display(Name = "Type")]
        public int EquipTypeID { get; set; }
        [Display(Name = "Inspection")]
        public int InspectionID { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
        public Inspection? Inspection { get; set; }
        public EquipType? EquipType { get; set; }
        public string? Manufacturer { get; set; }
        public string? Installer { get; set; }
        public string? Rating { get; set; }
        public string? SerialNo { get; set; }
        public int? SNSuffix { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public List<InspPhoto>? Photos { get; set; }
        public string? RequiredControls { get; set; }
        public int? Qty { get; set; }

        
    }
    public class InspEquipView
    {
        public int id { get; set; }
        [Display(Name = "Type")]
        public string? EquipDesc { get; set; }
        public int InspectionID { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }


        public string? Manufacturer { get; set; }
        public string? Installer { get; set; }
        public string? Rating { get; set; }
        public string? SerialNo { get; set; }
        public DateTime? WithdrawalDate { get; set; }
        public string? Photos { get; set; }
        public string? RequiredControls { get; set; }
        public int? Qty { get; set; }
    }


}
