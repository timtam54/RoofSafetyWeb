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
        public int InspectionID = 54;
        public List<EquipmentLogExcel> Equipment { get; set; }
        public void OnGet()
        {
            Equipment = (from ine in _context.InspEquip join eqt in _context.EquipType on ine.EquipTypeID equals eqt.id where ine.InspectionID == InspectionID select new EquipmentLogExcel { Number = ine.id, EquipmentType_Desc = eqt.EquipTypeDesc, SerialNo = ine.SerialNo}).ToList();
        }
    }
}
