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
        public int? ID { get; set; }

        [FromQuery]

        public int? ClientID { get; set; }
        [BindProperty]
        public int? CliID { get; set; }
        public List<EquipmentLogExcel> Equipment { get; set; }
        public string Title { get; set; }

        public ActionResult OnPost()
        {
            if (InspectionID != null || ClientID!=null)
            {
                if (InspectionID != null)
                    
                    ID = InspectionID.Value;
                if (ClientID != null)
                    CliID = ClientID.Value;



                GetData();
                using (var wbook = new XLWorkbook())
                {
                    var ws = wbook.Worksheets.Add("EquipmentLog");

                    ws.Cell("A1").Value = Title + " - Equipment Log";
                    ws.Cell("A1").Style.Font.FontSize = 20;
                    ws.Cell("A1").Style.Font.FontName = "Arial";
                    ws.Cell("A1").Style.Font.Bold = true;


                    ws.Cell(2, 1).Value = "Building";
                    ws.Column(1).Width = 8;
                    FormatHeaderCell(ws.Cell(2, 1));

                    ws.Cell(2, 2).Value = "Qty";
                    ws.Column(2).Width = 40;
                    FormatHeaderCell(ws.Cell(2, 2));

                    ws.Cell(2, 3).Value = "Insp Date";
                    ws.Column(3).Width = 40;
                    FormatHeaderCell(ws.Cell(2, 3));


                    ws.Cell(2, 4).Value = "Item #";
                    ws.Column(4).Width = 8;
                    FormatHeaderCell(ws.Cell(2, 4));

                    ws.Cell(2, 5).Value = "Equipment Desc";
                    ws.Column(5).Width = 40;
                    FormatHeaderCell(ws.Cell(2, 5));

                    ws.Cell(2, 6).Value = "Manufacturer";
                    ws.Column(6).Width = 40;
                    FormatHeaderCell(ws.Cell(2, 6));

                    ws.Cell(2, 7).Value = "Withdrawal Date";
                    ws.Column(7).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 7));

                    ws.Cell(2, 8).Value = "Serial No";
                    ws.Column(8).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 8));

                    ws.Cell(2, 9).Value = "Status";
                    ws.Column(9).Width = 15;
                    FormatHeaderCell(ws.Cell(2,9));

                    ws.Cell(2,10).Value = "Inspection Date";
                    ws.Column(10).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 10));

                    ws.Cell(2,11).Value = "Due";
                    ws.Column(11).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 11));

                    ws.Cell(2, 12).Value = "Installer";
                    ws.Column(12).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 12));
                    //ws.Cell(2, 13).Value = "Qty";
                    //ws.Column(13).Width = 15;
                    FormatHeaderCell(ws.Cell(2, 13));
                    for (int i = 0; i < Equipment.Count(); i++)
                    {
                        ws.Cell(i + 3, 1).Value = Equipment[i].BuildingName;
                        FormatBodyCell(ws.Cell(i + 3, 1));
                        ws.Cell(i + 3, 2).Value = Equipment[i].Qty.ToString();
                        FormatBodyCell(ws.Cell(i + 3, 2));
                        ws.Cell(i + 3, 3).Value = Equipment[i].InspectionDate;
                        FormatBodyCell(ws.Cell(i + 3, 3));

                        ws.Cell(i + 3, 4).Value = (i + 4).ToString();
                        FormatBodyCell(ws.Cell(i + 3, 4));
                        ws.Cell(i + 3, 5).Value = Equipment[i].EquipmentType_Desc;
                        FormatBodyCell(ws.Cell(i + 3, 5));
                        ws.Cell(i + 3, 6).Value = Equipment[i].Manufacturer;
                        FormatBodyCell(ws.Cell(i + 3, 6));
                        ws.Cell(i + 3, 7).Value = Equipment[i].WithdrawalDate;
                        FormatBodyCell(ws.Cell(i + 3, 7));
                        ws.Cell(i + 3, 8).Value = Equipment[i].SerialNo;
                        FormatBodyCell(ws.Cell(i + 3, 8));
                        ws.Cell(i + 3, 9).Value = Equipment[i].Status;
                        FormatBodyCell(ws.Cell(i + 3, 9));
                        ws.Cell(i + 3, 10).Value = Equipment[i].InspectionDate;
                        FormatBodyCell(ws.Cell(i + 3, 10));
                        ws.Cell(i + 3, 11).Value = Equipment[i].InspectionDue;
                        FormatBodyCell(ws.Cell(i + 3,11));
                        ws.Cell(i + 3, 12).Value = Equipment[i].Installer;
                        FormatBodyCell(ws.Cell(i + 3, 12));
                        //ws.Cell(i + 3, 13).Value = Equipment[i].Qty;
                        //FormatBodyCell(ws.Cell(i + 3, 13));
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
            if (ClientID != null)
            {
                CliID = ClientID.Value;
                GetData();
            }
        }

        public class BuildingInsp
        {
            public int BuildingID { get; set; }
            public string BuildingName { get; set; }
          
            public int InspectionID { get; set; }
        }


            private void GetData()
        {
            
            if (CliID != null)
            {
                var bis = (from insp in _context.Inspection join bd in _context.Building on insp.BuildingID equals bd.id where bd.ClientID == CliID select new BuildingInsp { BuildingID = bd.id, BuildingName = bd.BuildingName, InspectionID = insp.id }).ToList();
                var bilast = bis.GroupBy(i => new { i.BuildingID, i.BuildingName }).Select(g => new BuildingInsp { BuildingID = g.Key.BuildingID, BuildingName = g.Key.BuildingName, InspectionID = g.Max(i => i.InspectionID) }).ToList();

                //   group bd by new { bd.id, bd.BuildingName } into bb select new BuildingLastInsp { BuildingID= bb.Key.id, BuildingName= bb.Key.BuildingName, LastInspectionID= bb.Max(ins.id) as LastInsp })
                //            var ins = _context.Inspection.Where(i => i.id == ID).FirstOrDefault();
                // var build = _context.Building.Where(i => i.id == ins.BuildingID).FirstOrDefault();
                var cli = _context.Client.Where(i => i.id == CliID).FirstOrDefault();
                Title = "All item for equipment for client " + cli.name;// ins.Status + " " + ins.InspectionDate.ToString("dd-MMM-yyyy") + " " + build?.BuildingName + "@" + build?.Address;

                var mm = (from bil in bilast join ineq in _context.InspEquip on bil.InspectionID equals ineq.InspectionID select ineq).ToList();
                Equipment = (from bil in bilast join ineq in _context.InspEquip on bil.InspectionID equals ineq.InspectionID join eqt in _context.EquipType on ineq.EquipTypeID equals eqt.id join insp in _context.Inspection on ineq.InspectionID equals insp.id join bd in _context.Building on bil.BuildingID equals bd.id select new EquipmentLogExcel { Qty = ineq.Qty ?? 1, Installer = ineq.Installer, InspDate = insp.InspectionDate, InspStatus = insp.Status, BuildingID = bil.BuildingID, BuildingName = bil.BuildingName, InspectionDue = (bd.InspFreqMonths == null) ? insp.InspectionDate.AddMonths(12).ToString("dd-MMM-yyyy") : insp.InspectionDate.AddMonths(bd.InspFreqMonths.Value).ToString("dd-MMM-yyyy"), WithdrawalDate = "", Status = "NA", InspEquipID = ineq.id, Number = ineq.id, EquipmentType_Desc = eqt.EquipTypeDesc, SerialNo = ineq.SerialNo, Manufacturer = ineq.Manufacturer, InspectionDate = insp.InspectionDate.ToString("dd-MMM-yyyy") }).ToList();
            }
            else
            {
                var ins = _context.Inspection.Where(i => i.id == ID).FirstOrDefault();
                var build = _context.Building.Where(i => i.id == ins.BuildingID).FirstOrDefault();
                Title = ins.Status + " " + ins.InspectionDate.ToString("dd-MMM-yyyy") + " " + build?.BuildingName + "@" + build?.Address;

                Equipment = (from ine in _context.InspEquip join eqt in _context.EquipType on ine.EquipTypeID equals eqt.id where ine.InspectionID == ID select new EquipmentLogExcel { Qty=ine.Qty??1, Installer=ine.Installer, BuildingID=ins.BuildingID, BuildingName=build.BuildingName, InspDate=ins.InspectionDate, InspStatus=ins.Status  ,InspectionDue = (build.InspFreqMonths==null)? ins.InspectionDate.AddMonths(12).ToString("dd-MMM-yyyy") : ins.InspectionDate.AddMonths(build.InspFreqMonths.Value).ToString("dd-MMM-yyyy"), WithdrawalDate = "", Status = "NA", InspEquipID = ine.id, Number = ine.id, EquipmentType_Desc = eqt.EquipTypeDesc, SerialNo = ine.SerialNo, Manufacturer = ine.Manufacturer, InspectionDate = ins.InspectionDate.ToString("dd-MMM-yyyy") }).ToList();
            }
            foreach (var item in Equipment)
            {

                var TestResult = (from iet in _context.InspEquipTypeTest join ett in _context.EquipTypeTest on iet.EquipTypeTestID equals ett.id where iet.InspEquipID == item.InspEquipID select iet).ToList();

                item.Status = ((TestResult.Count > 0)) ? "Fail" : "Pass";
            }
        }
    }
}
