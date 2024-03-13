namespace RoofSafety.Models
{

        public class EquipmentLogExcel
    {
        public int Number { get; set; }
        public string EquipmentType_Desc { get; set; }

        public string Manufacturer { get; set; }

        public string WithdrawalDate { get; set; }

        public string SerialNo { get; set; }

        public string Status { get; set; }
        public string InspectionDate { get; set; }
        public string InspectionDue { get; set; }
    }
}
