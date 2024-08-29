namespace RoofSafety.Models
{

        public class EquipmentLogExcel
    {
        public int InspEquipID { get; set; }
        public int Number { get; set; }
        public string EquipmentType_Desc { get; set; }

        public string Manufacturer { get; set; }

        public string? Installer { get; set; }

        public string WithdrawalDate { get; set; }

        public string SerialNo { get; set; }

        public string Status { get; set; }
        public string InspectionDate { get; set; }
        public string InspectionDue { get; set; }

        public int BuildingID { get; set; }

        public string? BuildingName { get; set; }

        public string? InspStatus { get; set; }

        public DateTime? InspDate { get; set; }

        public int? Qty { get; set; }
    }
}
