using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RoofSafety.Data;
using RoofSafety.Models;

namespace RoofSafety.Pages
{

    public class EquipmentLogModel : PageModel
    {
        private readonly dbcontext _context;
        public EquipmentLogModel(dbcontext context)
        {
            _context = context;
        }
        public string Error { get; set; }
        [FromQuery]
        public int InspectionID { get; set; }
        public List<EquipmentLogExcel> Equipment { get; set; }
        public string Title { get; set; }
        public void OnGet()
        {
            var ins = _context.Inspection.Where(i => i.id == InspectionID).FirstOrDefault();
            var build = _context.Building.Where(i => i.id == ins.BuildingID).FirstOrDefault();
            Title = ins.Status + " " + ins.InspectionDate.ToString("dd-MMM-yyyy") + " " + build?.BuildingName + "@" + build?.Address;

            Equipment = (from ine in _context.InspEquip join eqt in _context.EquipType on ine.EquipTypeID equals eqt.id where ine.InspectionID == InspectionID select new EquipmentLogExcel { Number = ine.id, EquipmentType_Desc = eqt.EquipTypeDesc, SerialNo = ine.SerialNo, Manufacturer=ine.Manufacturer, InspectionDate=ins.InspectionDate.ToString("dd-MMM-yyyy") }).ToList();
        }
    }
}
