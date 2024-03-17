using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
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

        public int? InspectionID { get; set; }
        [BindProperty]
        public int ID { get; set; }
        public List<EquipmentLogExcel> Equipment { get; set; }
        public string Title { get; set; }

        public ActionResult OnPost()
        {
            if (InspectionID != null)
            {
                ID = InspectionID.Value;
                GetData();

                using (var wbook = new XLWorkbook())
                {
                    var ws = wbook.Worksheets.Add("EquipmentLog");

                    ws.Cell("A1").Value = Title + " - Equipment Log";
                    ws.Cell("A1").Style.Font.FontSize = 20;
                    ws.Cell("A1").Style.Font.FontName = "Arial";
                    ws.Cell("A1").Style.Font.Bold = true;


                    ws.Cell(2, 1).Value = "Item #";
                    ws.Column(1).Width = 8;
                    FormatHeaderCell(ws.Cell(2, 1));

                    ws.Cell(2, 2).Value = "Equipment Desc";
                    ws.Column(2).Width = 40;
                    FormatHeaderCell(ws.Cell(2, 2));

                    ws.Cell(2, 3).Value = "Manufacturer";
                    ws.Column(3).Width = 40;
                    FormatHeaderCell(ws.Cell(2, 3));

                    ws.Cell(2, 4).Value = "Withdrawal Date";
                    ws.Column(4).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 4));

                    ws.Cell(2, 5).Value = "Serial No";
                    ws.Column(5).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 5));

                    ws.Cell(2, 6).Value = "Status";
                    ws.Column(6).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 6));

                    ws.Cell(2, 7).Value = "Inspection Date";
                    ws.Column(7).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 7));

                    ws.Cell(2, 8).Value = "Due";
                    ws.Column(8).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 8));

                    for (int i = 0; i < Equipment.Count(); i++)
                    {
                        ws.Cell(i + 3, 1).Value = (i + 1).ToString();
                        FormatBodyCell(ws.Cell(i + 3, 1));
                        ws.Cell(i + 3, 2).Value = Equipment[i].EquipmentType_Desc;
                        FormatBodyCell(ws.Cell(i + 3, 2));
                        ws.Cell(i + 3, 3).Value = Equipment[i].Manufacturer;
                        FormatBodyCell(ws.Cell(i + 3, 3));
                        ws.Cell(i + 3, 4).Value = Equipment[i].WithdrawalDate;
                        FormatBodyCell(ws.Cell(i + 3, 4));
                        ws.Cell(i + 3, 5).Value = Equipment[i].SerialNo;
                        FormatBodyCell(ws.Cell(i + 3, 5));
                        ws.Cell(i + 3, 6).Value = Equipment[i].Status;
                        FormatBodyCell(ws.Cell(i + 3, 6));
                        ws.Cell(i + 3, 7).Value = Equipment[i].InspectionDate;
                        FormatBodyCell(ws.Cell(i + 3, 7));
                        ws.Cell(i + 3, 8).Value = Equipment[i].InspectionDue;
                        FormatBodyCell(ws.Cell(i + 3,8));
                    }

                    for (int i = 1; i <= 8; i++)
                    {
                        ws.Cell(Equipment.Count() + 2,i).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell(Equipment.Count() + 2, i).Style.Border.BottomBorderColor = XLColor.Black;
                    }
                    using (var stream = new System.IO.MemoryStream())
                    {
                        wbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Title + ".xlsx");
                    }
                }
            }
            return Page();
        }
        static void FormatBodyCell(IXLCell cell)
        {
            
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorderColor = XLColor.Black;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorderColor = XLColor.Black;
            }
        private static void FormatHeaderCell(IXLCell cell)
        {
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontColor = XLColor.Navy;
            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.BottomBorderColor = XLColor.Black;
            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.TopBorderColor = XLColor.Black;
            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.LeftBorderColor = XLColor.Black;
            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.RightBorderColor = XLColor.Black;
        }

        [HttpGet]//("excel")]
        
        public IActionResult CreateExcelSheet()
        {
            try
            {
                using (var wbook = new XLWorkbook())
                {
                    var ws = wbook.Worksheets.Add("Sheet1");
                    ws.Cell("A1").Value = "1";

                    using (var stream = new System.IO.MemoryStream())
                    {
                        wbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Opportunities.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        public void OnGet()
        {
                if (InspectionID != null)
            {
                ID = InspectionID.Value;
                GetData();
            }
        }

        private void GetData()
        {
            var ins = _context.Inspection.Where(i => i.id == ID).FirstOrDefault();
            var build = _context.Building.Where(i => i.id == ins.BuildingID).FirstOrDefault();
            Title = ins.Status + " " + ins.InspectionDate.ToString("dd-MMM-yyyy") + " " + build?.BuildingName + "@" + build?.Address;

            Equipment = (from ine in _context.InspEquip join eqt in _context.EquipType on ine.EquipTypeID equals eqt.id where ine.InspectionID == ID select new EquipmentLogExcel { InspectionDue="", WithdrawalDate="", Status="NA", InspEquipID = ine.id, Number = ine.id, EquipmentType_Desc = eqt.EquipTypeDesc, SerialNo = ine.SerialNo, Manufacturer = ine.Manufacturer, InspectionDate = ins.InspectionDate.ToString("dd-MMM-yyyy") }).ToList();

            foreach (var item in Equipment)
            {

                var TestResult = (from iet in _context.InspEquipTypeTest join ett in _context.EquipTypeTest on iet.EquipTypeTestID equals ett.id where iet.InspEquipID == item.InspEquipID select iet).ToList();

                item.Status = ((TestResult.Count > 0)) ? "Fail" : "Pass";
            }
        }
    }
}
