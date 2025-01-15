
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using RoofSafety.Data;
using RoofSafety.Models;
using RoofSafety.Services.Abstract;


using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;


using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using Bold = DocumentFormat.OpenXml.Wordprocessing.Bold;
using Color = DocumentFormat.OpenXml.Wordprocessing.Color;
using FontSize = DocumentFormat.OpenXml.Wordprocessing.FontSize;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Break = DocumentFormat.OpenXml.Wordprocessing.Break;
using Drawing = DocumentFormat.OpenXml.Wordprocessing.Drawing;
using BottomBorder = DocumentFormat.OpenXml.Wordprocessing.BottomBorder;
using TopBorder = DocumentFormat.OpenXml.Wordprocessing.TopBorder;
using LeftBorder = DocumentFormat.OpenXml.Wordprocessing.LeftBorder;
using RightBorder = DocumentFormat.OpenXml.Wordprocessing.RightBorder;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using System.Drawing;
using DocumentFormat.OpenXml.Vml;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore.Query;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Drawing.Drawing2D;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.Extensions.Azure;
using static RoofSafety.Pages.EquipmentLogModel;
using System.Reflection.Metadata;
using System.Drawing.Imaging;
//using iText.Kernel.Pdf;
//using iText.Html2pdf.Attach.Impl.Layout;

//using iText.Html2pdf;


namespace RoofSafety.Controllers
{
    [Authorize]
    public class InspectionEquipmentsController : Controller
    {
        private readonly dbcontext _context;
        private readonly IImageService _imageservice;

        

        public InspectionEquipmentsController(dbcontext context, IImageService imageservice)
        {
            _context = context;
            _imageservice = imageservice;
        }

        public  class DescParID
        {
            public string Desc { get; set; }
            public int ID { get; set; }

        }
        public async Task<ActionResult> EquipForInspections(int id)
        {
            var yyy =await _context.InspEquip.Where(i => i.InspectionID == id).Include(i => i.EquipType).Include(i => i.Inspection).Include(i => i.EquipType).ToListAsync();

            SetOrderIfNull(yyy);
            ViewBag.InspectionID = id;
            DescParID xx = (from ie in _context.Inspection join bd in _context.Building on ie.BuildingID equals bd.id where ie.id == id select new DescParID { Desc = (bd.BuildingName), ID = ie.BuildingID }).FirstOrDefault();
            ViewBag.InspectionDesc = xx.Desc;
            ViewBag.BuildingID = xx.ID;
            return View("Index", yyy);
        }
        private static void SetOrderIfNull(List<InspEquipTest> xxx)
        {
            foreach (var x in xxx)
            {
                if (x.Ordr == null)
                    x.Ordr = x.id;
            }
        }
        private static void SetOrderIfNull(List<InspEquip> xxx)
        {
            foreach (var x in xxx)
            {
                if (x.Ordr == null)
                    x.Ordr = x.id;
            }
        }

        private static void AddImageToBodyWordDoc(WordprocessingDocument wordDoc, string relationshipId,decimal htw)
        {

            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L*5, Cy = Convert.ToInt64(792000L * 5 * htw )},
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L*5, Cy = Convert.ToInt64( 792000L*5* htw) }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            wordDoc.MainDocumentPart.Document.Body.AppendChild(//(new Paragraph(new Run(element)));

                 new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(element))
                 {
                     ParagraphProperties = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties()
                     {
                         Justification = new DocumentFormat.OpenXml.Wordprocessing.Justification()
                         {
                             Val = DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Center
                         }
                     }
                 });
        }

        private static void AddImageToBody(Body wordDoc, string relationshipId, decimal htw)
        {

            // Define the reference of the image.
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = 990000L * 5, Cy = Convert.ToInt64(792000L * 5 * htw) },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L * 5, Cy = Convert.ToInt64(792000L * 5 * htw) }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         )
                                         { Preset = A.ShapeTypeValues.Rectangle }))
                             )
                             { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U,
                         EditId = "50D07946"
                     });

            // Append the reference to body, the element should be in a Run.
            wordDoc.AppendChild(//(new Paragraph(new Run(element)));

                 new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(element))
                 {
                     ParagraphProperties = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties()
                     {
                         Justification = new DocumentFormat.OpenXml.Wordprocessing.Justification()
                         {
                             Val = DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Center
                         }
                     }
                 });
        }

        private static DocumentFormat.OpenXml.Wordprocessing.Header GeneratePicHeader(string relationshipId)
        {
            var element =
                new Drawing(
                    new DW.Inline(
                        new DW.Extent() { Cx = 990000L, Cy = 792000L/3 },
                        new DW.EffectExtent()
                        {
                            LeftEdge = 0L,
                            TopEdge = 0L,
                            RightEdge = 0L,
                            BottomEdge = 0L
                        },
                        new DW.DocProperties()
                        {
                            Id = (UInt32Value)1U,
                            Name = "RSS Logo"
                        },
                        new DW.NonVisualGraphicFrameDrawingProperties(
                            new A.GraphicFrameLocks() { NoChangeAspect = true }),
                        new A.Graphic(
                            new A.GraphicData(
                                new PIC.Picture(
                                    new PIC.NonVisualPictureProperties(
                                        new PIC.NonVisualDrawingProperties()
                                        {
                                            Id = (UInt32Value)0U,
                                            Name = "nis.png"
                                        },
                                        new PIC.NonVisualPictureDrawingProperties()),
                                    new PIC.BlipFill(
                                        new A.Blip(
                                            new A.BlipExtensionList(
                                                new A.BlipExtension()
                                                {
                                                    Uri =
                                                        "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                })
                                        )
                                        {
                                            Embed = relationshipId,
                                            CompressionState =
                                                A.BlipCompressionValues.Print
                                        },
                                        new A.Stretch(
                                            new A.FillRectangle())),
                                    new PIC.ShapeProperties(
                                        new A.Transform2D(
                                            new A.Offset() { X = 0L, Y = 0L },
                                            new A.Extents() { Cx = 990000L, Cy = 792000L/3 }),
                                        new A.PresetGeometry(
                                            new A.AdjustValueList()
                                        )
                                        { Preset = A.ShapeTypeValues.Rectangle }))
                            )
                            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                    )
                    {
                        DistanceFromTop = (UInt32Value)0U,
                        DistanceFromBottom = (UInt32Value)0U,
                        DistanceFromLeft = (UInt32Value)0U,
                        DistanceFromRight = (UInt32Value)0U,
                        EditId = "50D07946"
                    });

            var header = new DocumentFormat.OpenXml.Wordprocessing.Header();

            header.AppendChild(
    new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(element))
    {
        ParagraphProperties = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties()
        {
            Justification = new DocumentFormat.OpenXml.Wordprocessing.Justification()
            {
                Val = DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Right
            }
        }
    });
           // var paragraph = new Paragraph();
            //var run = new Run();

         //   run.Append(element);
          //  paragraph.Append(run);
          //  header.Append(paragraph);
            return header;
        }
        public static string Green = "#06AD29";//"#0BDA51";#008000
        public static string Yellow = "yellow";
        public static string Red = "#FF2C1A";//"#FF5733";//""
        public static string Blue = "#0052FF";//"#0096FF";

        [AllowAnonymous]
        public async Task<ActionResult> EquipForInspectionsAll(int id, string hpw, int scale=90)//0,1,2
        {
            InspectionRpt ret = new InspectionRpt();
            ret.InspItems = (from ins in _context.InspEquip join emp in _context.EquipType on ins.EquipTypeID equals emp.id where ins.InspectionID == id orderby ins.Ordr select emp.EquipTypeDesc).ToList();
          
            ret.Inspector = (from ins in _context.Inspection join emp in _context.Employee on ins.InspectorID equals emp.id where ins.id == id select emp.Given + " " + emp.Surname).FirstOrDefault();
            ret.Inspector2 = (from ins in _context.Inspection join emp in _context.Employee on ins.Inspector2ID equals emp.id where ins.id == id select emp.Given + " " + emp.Surname).FirstOrDefault();
            var insp = _context.Inspection.Where(i => i.id == id).FirstOrDefault();
            ret.InspDate = insp.InspectionDate;
            var building = _context.Building.Where(i => i.id == insp.BuildingID).FirstOrDefault(); 
            ret.NextDue = insp.InspectionDate.AddMonths((building.InspFreqMonths==null || building.InspFreqMonths==0) ?12:building.InspFreqMonths.Value);
            ret.Areas = insp.Areas;
            ret.id= insp.id;
            ret.Instrument = insp.TestingInstruments;
            ret.Tests = "Test";
            
            ret.Title = building.BuildingName;
            ret.Address = building.Address;
            ret.Items = (from ie in _context.InspEquip join et in _context.EquipType on ie.EquipTypeID equals et.id where ie.InspectionID == id select new InspEquipTest { Ordr=(ie.Ordr==null)?ie.id:ie.Ordr, ItemNo= ie.SerialNo ,Qty=(ie.Qty==null)?1:ie.Qty.Value, RequiredControls=ie.RequiredControls, Pass=true, Manufacturer =ie.Manufacturer, SNSuffix=ie.SNSuffix, SerialNo=ie.SerialNo, Rating=ie.Rating, Installer=ie.Installer ,EquipName = et.EquipTypeDesc,  Notes = ie.Notes, Location = ie.Location, id = ie.id, EquipType = et, ETID=et.id }).OrderBy(i=>i.Ordr).ToList();//.Include(i => i.EquipType).Include(i => i.Inspection).Include(i => i.EquipType)=efe
    
            int counter = 1;
            foreach (var ite in ret.Items.OrderBy(i => i.Ordr))
            {
                ite.ItemNo = counter.ToString();
                counter++;
            }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               // ret.Versions = (from vs in _context.Version join emp in _context.Employee on vs.AuthorID equals emp.id where vs.InspectionID == Convert.ToInt32(id) select new VersionRpt { id = vs.id, Information = vs.Information, Author = emp.Given + " " + emp.Surname, VersionNo = vs.VersionNo, VersionType = (vs.VersionType == "FD") ? "First Draft" : "Internal Review" }).ToList();
            ret.Versions = (from ins in _context.Inspection join emp in _context.Employee on ins.InspectorID equals emp.id where ins.BuildingID == Convert.ToInt32(insp.BuildingID) /*&& ins.id!=insp.id */select new VersionRpt { Author2=ins.Inspector2ID.ToString(), id = ins.id,NextDue= ins.InspectionDate.AddMonths((building.InspFreqMonths == null || building.InspFreqMonths == 0) ? 12 : building.InspFreqMonths.Value).ToString("dd-MM-yyyy"), Information = ins.InspectionDate.ToString("dd-MM-yyyy"), Author = emp.Given + " " + emp.Surname, VersionNo = ins.id, VersionType = (ins.Status == null) ? "New" : ((ins.Status == "A") ? "Active" : "Complete") }).ToList();
            //  ret.Versions = (from vs in _context.Version join emp in _context.Employee on vs.AuthorID equals emp.id where vs.InspectionID == Convert.ToInt32(id) select new VersionRpt { id = vs.id, Information = vs.Information, Author = emp.Given + " " + emp.Surname, VersionNo = vs.VersionNo, VersionType = (vs.VersionType == "FD") ? "First Draft" : "Internal Review" }).ToList();
            foreach (var item in ret.Versions)
            {
                var mm = _context.Employee.Where(i => i.id.ToString() == item.Author2).FirstOrDefault();
                if (mm != null)
                    item.Author2 = mm.Given + " " + mm.Surname; 
            }
            using (var client = new HttpClient())
            {
                if (insp.Photo != null)
                {
                    ret.Photo = _imageservice.GetImageURL(insp.Photo);
                    //try
                    //{
                    //    var bytes = await client.GetByteArrayAsync(ret.Photo);
                    //    string imgbase64 = "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
                    //    ret.Photo = imgbase64;
                    //}
                    //catch (Exception ex)
                    //{

                   // }
                }
                foreach (var item in ret.Items.OrderBy(i => i.Ordr))
                {
                    item.Pass = true;
                    item.TestResult = (from iet in _context.InspEquipTypeTest join ett in _context.EquipTypeTest on iet.EquipTypeTestID equals ett.id where iet.InspEquipID == item.id select new TestResult { Comment=iet.Comment, Test = ett.Test/*, PassFail = iet.Pass*/, Severity=ett.Severity,  EquipTypeTestID = iet.EquipTypeTestID, iettid=iet.id }).ToList();//.Include(i => i.EquipType).Include(i => i.Inspection).Include(i => i.EquipType)=efe//HazardIfNonCompliant = ett.HazardIfNonCompliant, 
                    List<int> HazardID = new List<int>();
                    foreach (var tr in item.TestResult)
                    {
                        if (tr.PassFail == 0)
                        {
                            item.Pass = false;
                            HazardID = HazardID.Concat(_context.EquipTypeTestHazards.Where(i => i.EquipTypeTestID == tr.EquipTypeTestID).Select(o => o.HazardID).ToList()).ToList();
                            tr.FailReason = (from /*iettf in _context.InspEquipTypeTestFail join*/ ettf in _context.EquipTypeTestFail /* on iettf.EquipTypeTestFailID equals ettf.id*/ where ettf.EquipTypeTestID == tr.EquipTypeTestID select ettf.FailReason).ToList();
                            var xxy = tr.FailReason;
                        }
                    }
                    item.Hazards = new List<string>();// = "";
                    foreach (var hz in _context.Hazard.Where(i => HazardID.Contains(i.id)).Select(i => i.Detail).ToList())
                    {
                        item.Hazards.Add(hz!);

                    }
                    item.Photos = _context.InspPhoto.Where(i => i.InspEquipID == item.id).ToList();
                    foreach (var phot in item.Photos)
                    {
                        phot.photoname = _imageservice.GetImageURL(phot.photoname);
                        //try { 
                        //var bytes = await client.GetByteArrayAsync(phot.photoname);
                        //string imgbase64 = "data:image/jpeg;base64," + Convert.ToBase64String(bytes);
                        //phot.photoname = imgbase64;
                        //}
                        //catch (Exception ex)
                        //{

                        //}
                    }
                }
            }
            ViewBag.InspectionID = id;
            DescParID xx = (from ie in _context.Inspection join bd in _context.Building on ie.BuildingID equals bd.id where ie.id == id select new DescParID { Desc = ( bd.BuildingName + " @ " + ie.InspectionDate.ToString("dd-MM-yyyy")), ID = ie.BuildingID }).FirstOrDefault();
            ViewBag.InspectionDesc = xx.Desc;
            ViewBag.BuildingID = xx.ID;
            if (hpw == "w")
            {
                using var memoryStream = new MemoryStream();
                try
                {
                    using var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);
                    try
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();// Document();
                        {
                            Body bodyintro = mainPart.Document.AppendChild(new Body());
                            Table tableInsp = new Table();
                            TableProperties props = new TableProperties();
                            tableInsp.AppendChild<TableProperties>(props);
                            TableLayout tl = new TableLayout() { Type = TableLayoutValues.Fixed };
                            props.TableLayout = tl;
                            FontSize fontSizeInsp = new FontSize() { Val = "42" };

                            TableProperties tblPropInsp = new TableProperties(
                               fontSizeInsp,
                                new TableBorders(
                                )
                            );
                            tableInsp.AppendChild<TableProperties>(tblPropInsp);

                            TableRow trInsp = new TableRow();
                            TableCell tcInspDteLbl = new TableCell();

                            tcInspDteLbl.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "4500" }));
                         
                            trInsp.Append(tcInspDteLbl);


                            //         TableCell tcInspDte = new TableCell();
                            //// TableCell tcInspDte = new TableCell();
                            // tcInspDte.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "6000" }));


                            // await InsertImage(tcInspDteLbl, wordDocument, mainPart, "https://rssblob.blob.core.windows.net/rssimage/d4a29b97-e3a6-4013-913d-505f936f73fe.jpg", 48);// "https://rssblob.blob.core.windows.net/rssimage/rsspnggreyvertical70.png",48);
                            await InsertImage(tcInspDteLbl, wordDocument, mainPart, "https://rssblob.blob.core.windows.net/rssimage/rsspnggreyvertical70.png",48);

                            TableCell tcInspDte2 = new TableCell();
                            tcInspDte2.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "6000" }));
                            var shading = new Shading()
                            {
                                Color = "Red",
                                Fill = "Red",
                                Val = ShadingPatternValues.Clear
                            };
                            tcInspDte2.Append(shading);
                            tcInspDte2.Append(ParaLeftSize("36", "Height Safety"));
                            tcInspDte2.Append(ParaLeftSize("36", "Audit Report"));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text("")))); tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text("")))); tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                            tcInspDte2.Append(ParaLeftSize("14", "Roof Safety Solutions"));
                            tcInspDte2.Append(ParaLeftSize("14", "38 Radius Loop Bayswater WA 6053"));
                            tcInspDte2.Append(ParaLeftSize("14", "08 9477 4884"));
                            tcInspDte2.Append(ParaLeftSize("14", "admin@RoofSafetySolutions.com.au"));
                            tcInspDte2.Append(ParaLeftSize("14", "www.RoofSafetySolutions.com.au"));


                           // tcInspDte2.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "9000" }));

                            trInsp.Append(tcInspDte2);

                            tableInsp.Append(trInsp);

                            bodyintro.AppendChild(tableInsp);
                            
                            Paragraph parabody2 = bodyintro.AppendChild(new Paragraph());
                            Run runp2 = parabody2.AppendChild(new Run());
                            RunProperties runPropertiesp2 = runp2.AppendChild(new RunProperties());
                            FontSize fontSizep24 = new FontSize() { Val = "36" };
                            runPropertiesp2.Append(fontSizep24);
                            
                            Paragraph paracover = bodyintro.AppendChild(new Paragraph());
                            Run runcover = paracover.AppendChild(new Run());
                            Break pgbrkcover = new Break();
                            pgbrkcover.Type = BreakValues.Page;
                            runcover.AppendChild(pgbrkcover);
                        }
                        Body body = mainPart.Document.AppendChild(new Body());
                        if (ret.Photo != null)
                        {

                            //{
                                Paragraph para = body.AppendChild(new Paragraph());
                                Run run = para.AppendChild(new Run());
                                ParagraphProperties paraProperties = new ParagraphProperties();
                                ParagraphBorders paraBorders = new ParagraphBorders();
                                BottomBorder bottom = new BottomBorder() { Val = BorderValues.Single, Color = "auto", Size = (UInt32Value)12U, Space = (UInt32Value)1U };
                                paraBorders.Append(bottom);
                                paraProperties.Append(paraBorders);
                                para.Append(paraProperties);
                                RunProperties runProperties = run.AppendChild(new RunProperties());
                                FontSize fontSize1 = new FontSize() { Val = "36" };
                                runProperties.Append(fontSize1);
                                Bold bold = new Bold();
                                bold.Val = OnOffValue.FromBoolean(true);
                                runProperties.AppendChild(bold);
                                run.AppendChild(new Text(xx.Desc));

                                run.AppendChild(new Break());
                            //}

                            string imgurl = ret.Photo.Replace("%0D%0A", "").TrimEnd();

                            await InsertImage(body, wordDocument, mainPart, imgurl);

                            Paragraph para22 = body.AppendChild(new Paragraph());
                            {

                                Run run2 = para22.AppendChild(new Run());
                                //RunProperties runProperties2 = run2.AppendChild(new RunProperties());
                                //FontSize fontSize24 = new FontSize() { Val = "24" };
                                //runProperties2.Append(fontSize24);
                                //run2.AppendChild(new Text("Roof Safety Solutions Pty Ltd"));
                                //run2.AppendChild(new Break());
                                //run2.AppendChild(new Text("38 Radius Loop, Bayswater WA 6053"));
                                //run2.AppendChild(new Break());
                                //run2.AppendChild(new Text("p: 08 9477 4884"));
                                //run2.AppendChild(new Break());
                                //run2.AppendChild(new Text("f: 08 9277 3009"));
                                //run2.AppendChild(new Break());
                                //run2.AppendChild(new Text("e: admin@roofsafetysolutions.com.au"));
                                //run2.AppendChild(new Break());

                                Break pgbrk = new Break();
                                pgbrk.Type = BreakValues.Page;
                                run2.AppendChild(pgbrk);
                            }
                        }

                        SectionProperties sectionProps = new SectionProperties();
                        PageMargin pageMargin = new PageMargin() { Top = 1008, Right = (UInt32Value)1008U, Bottom = 1008, Left = (UInt32Value)1008U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
                        sectionProps.Append(pageMargin);
                        mainPart.Document.Body?.Append(sectionProps);

                        Paragraph para2 = body.AppendChild(new Paragraph());
                        Run run3 = para2.AppendChild(new Run());
                        RunProperties runProperties3 = run3.AppendChild(new RunProperties());

                        AddHeader(run3, runProperties3, "Contents");
                        {

                            var newHeaderPart = mainPart.AddNewPart<HeaderPart>();
                            var imgPart = newHeaderPart.AddImagePart(ImagePartType.Png);
                            var imagePartID = newHeaderPart.GetIdOfPart(imgPart);
                            using (var client = new HttpClient())
                            {
                                try
                                {
                                   // var xxx = await client.GetByteArrayAsync("https://rssblob.blob.core.windows.net/rssimage/rsspnggreyvertical70.png");
                                   var bytes = await client.GetByteArrayAsync("https://rssblob.blob.core.windows.net/rssimage/roof_safety_logo.png");
                                    //var bytes = await client.GetByteArrayAsync(@"C:\RSS\RoofSafetyWeb\wwwroot\Image\rsslogo.png");// ret.Photo);
                                   // var bytes = await client.GetByteArrayAsync("https://www.roofsafetysolutions.com.au/wp-content/uploads/2020/06/roof_safety_logo.png");// ret.Photo);
                                    MemoryStream stream = new MemoryStream(bytes);
                                    imgPart.FeedData(stream);
                                    stream.Close();
                                    stream.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }

                            var rId = mainPart.GetIdOfPart(newHeaderPart);
                            var headerRef = new HeaderReference { Id = rId };
                            var sectionProps2 = sectionProps;// wordDocument.MainDocumentPart.Document.Body.Elements<SectionProperties>().LastOrDefault();
           
                            sectionProps2.RemoveAllChildren<HeaderReference>();
                            sectionProps2.Append(headerRef);
                            newHeaderPart.Header = GeneratePicHeader(imagePartID);
                        

                            Body body2 = mainPart.Document.AppendChild(new Body());
                            Paragraph parabody2 = body2.AppendChild(new Paragraph());
                            Run runp2 = parabody2.AppendChild(new Run());
                            RunProperties runPropertiesp2 = runp2.AppendChild(new RunProperties());
                            FontSize fontSizep24 = new FontSize() { Val = "36" };
                            runPropertiesp2.Append(fontSizep24);
                            Bold bold4 = new Bold();

                            bold4.Val = OnOffValue.FromBoolean(true);
                            runp2.AppendChild(bold4);
                            runp2.AppendChild(new Text("1. Version History"));
                            runp2.AppendChild(new Break());

                            runp2.AppendChild(new Text("2. Executive Summary"));
                            runp2.AppendChild(new Break());
                            runp2.AppendChild(new Text("3. Inspection Details"));
                            runp2.AppendChild(new Break());
                            runp2.AppendChild(new Text("4. Introduction"));
                            runp2.AppendChild(new Break());
                            runp2.AppendChild(new Text("5. Risk Assessment"));
                            runp2.AppendChild(new Break());
                            runp2.AppendChild(new Text("6. Inspection Report"));
                            runp2.AppendChild(new Break());
                            runp2.AppendChild(new Text("7. Summary"));
                            runp2.AppendChild(new Break());
                            runp2.AppendChild(new Text("8. Conclusion"));
                            runp2.AppendChild(new Break());
                            runp2.AppendChild(new Text("9. Disclaimer"));
                            runp2.AppendChild(new Break());

                            Break pgbrk2 = new Break();
                            pgbrk2.Type = BreakValues.Page;
                            runp2.AppendChild(pgbrk2);
                        }

                        Body body3 = mainPart.Document.AppendChild(new Body());
                        Paragraph parabody4 = body3.AppendChild(new Paragraph());
                        Run runVrsn = parabody4.AppendChild(new Run());
                        RunProperties runPropertiesVrsn = runVrsn.AppendChild(new RunProperties());

                        AddHeader(runVrsn, runPropertiesVrsn, "1. Version History");


                        Body body4 = mainPart.Document.AppendChild(new Body());

                        Run runVrsnDet = parabody4.AppendChild(new Run());
                        {

                            Table table = new Table();

                            FontSize fontSizep42 = new FontSize() { Val = "64" };

                            TableProperties tblProp = new TableProperties(
                               fontSizep42,
                                new TableBorders(
                                    new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 }
                                )
                            );

                            table.AppendChild<TableProperties>(tblProp);
                            TableRow tr = new TableRow();
                            table.Append(tr);

                            var rn = new Run(new Text("Version"));

                            tr.Append(CellFont("Version", 32, true));
                            tr.Append(CellFont("Status", 32, true));
                            tr.Append(CellFont("Author", 32, true));
                            tr.Append(CellFont("Reviewed", 32, true));
                            tr.Append(CellFont("Inspection Date", 32, true));
                            tr.Append(CellFont("Next Due", 32, true));

                            body4.AppendChild(table);
                            int iiii = 0;
                            foreach (var item in ret.Versions.OrderBy(i=>i.VersionNo))
                            {
                                iiii++;
                                TableRow trx = new TableRow();
                                table.Append(trx);
                                //trx.Append(CellFont(item.VersionNo?.ToString(), 28, false));
                                trx.Append(CellFont(iiii.ToString(), 28, false));
                                trx.Append(CellFont((item.VersionType), 28, false));
                                trx.Append(CellFont(item.Author, 28, false));
                                trx.Append(CellFont(item.Author2, 28, false));
                                trx.Append(CellFont((item.Information), 28, false));
                                trx.Append(CellFont((item.NextDue), 28, false));
                            }
                        }
                        Break pgbrk3 = new Break();
                        pgbrk3.Type = BreakValues.Page;
                        body3.AppendChild(pgbrk3);

                        Body body5 = mainPart.Document.AppendChild(new Body());

                        Paragraph parabody5 = body5.AppendChild(new Paragraph());

                        Run runExec = parabody5.AppendChild(new Run());
                        RunProperties runPropertiesExec = runExec.AppendChild(new RunProperties());

                        Break pgbrkeh = new Break();
                        pgbrkeh.Type = BreakValues.Page;
                        runPropertiesExec.AppendChild(pgbrkeh);

                        AddHeader(runExec, runPropertiesExec, "2. Executive Summary");

                        Body bodyeh = mainPart.Document.AppendChild(new Body());
                        Paragraph paraeh = bodyeh.AppendChild(new Paragraph());
                        Run runeh = paraeh.AppendChild(new Run());
                        RunProperties runPropertieseh = runeh.AppendChild(new RunProperties());
                        FontSize fontSizepeh = new FontSize() { Val = "26" };
                        runPropertieseh.Append(fontSizepeh);
                        //Bold bold4 = new Bold();

                        // bold4.Val = OnOffValue.FromBoolean(true);
                        // runp2.AppendChild(bold4);
                        runeh.AppendChild(new Text("Roof Safety Solutions were contracted to perform the routine audit of " + ret.Title + "."));
                        runeh.AppendChild(new Break()); runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("The following existing height safety equipment is installed on site:"));
                        runeh.AppendChild(new Break());
                        foreach (var itmdesc in ret.Items.GroupBy(i=>i.EquipName).Where(i=>!i.Key.ToLower().Contains("no safe access")).OrderBy(i=>i.Key))
                        {
                            runeh.AppendChild(new Text("- " + itmdesc.Key));// itmdesc.EquipName + " " + itmdesc.Manufacturer + " " + itmdesc.SerialNo); ;// ;//);
                                                                           runeh.AppendChild(new Break());
                        }
                        runeh.AppendChild(new Break());
                     //   runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("The inspection was carried out on the " + ret.InspDate.ToLongDateString() + " by height safety inspectors from Roof Safety Solutions Pty Ltd. The inspection identifies any risks and reports on compliance in relation to the Australian Standards, Acts and Regulations that form the basis for height safety in Australia."));
                        runeh.AppendChild(new Break()); runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("This report contains:"));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- An inventory of installed height safety equipment,"));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- Compliant items,"));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- Non-compliant items,"));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- Recommendations to rectify any non-compliant issues."));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("All recommendations have been made in the report to reduce risk of injury as outlined and required by:"));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- Work Health & Safety Act 2020 (WA)."));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- Work Health & Safety Regulations 2022 (WA) relevant to Fall Prevention."));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- Code of Practice: Managing the risk of falls at workplaces 2022."));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- AS 1657:2018 - Fixed platforms, walkways, stairways and ladders."));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- AS/NZS 1891.4:2009 - Industrial fall-arrest systems and devices –selection, use and maintenance."));
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("- AS/NZS 4488.2:1997 – Industrial rope access systems – selection use and maintenance."));

                        Break pgbrk4 = new Break();
                        pgbrk4.Type = BreakValues.Page;
                        runeh.AppendChild(pgbrk4);

                        Body body6 = mainPart.Document.AppendChild(new Body());
                        Paragraph parabody6 = body6.AppendChild(new Paragraph());
                        Run runExec6 = parabody6.AppendChild(new Run());
                        RunProperties runPropertiesExec6 = runExec6.AppendChild(new RunProperties());
                        AddHeader(runExec6, runPropertiesExec6, "3. Inspection Details");
                        {
                            Table tableInsp = new Table();
                            FontSize fontSizeInsp = new FontSize() { Val = "42" };

                            TableProperties tblPropInsp = new TableProperties(
                               fontSizeInsp,
                                new TableBorders(
                                )
                            );
                            // Append the TableProperties object to the empty table.
                            tableInsp.AppendChild<TableProperties>(tblPropInsp);
                            {
                                TableRow trInsp = new TableRow();

               
                                TableCell tcInspDteLbl = CellFont("Inspection Date:", 30, true);
                                trInsp.Append(tcInspDteLbl);


                                TableCell tcInspDte = CellFont(ret.InspDate.ToString("dd-MMM-yyyy"), 30, false);
                                trInsp.Append(tcInspDte);
                                tableInsp.Append(trInsp);


                            }
                            {
                                TableRow trInsp = new TableRow();


                                TableCell tcInspDteLbl = CellFont("Next Due:", 30, true);
                                trInsp.Append(tcInspDteLbl);


                                TableCell tcInspDte = CellFont(ret.NextDue.Value.ToString("dd-MMM-yyyy"), 30, false);
                                trInsp.Append(tcInspDte);
                                tableInsp.Append(trInsp);


                            }
                            {
                                
                                TableRow trInsp = new TableRow();
                                TableCell tcInspDteLbl = CellFont("Areas Inspected:",30,true);
                                trInsp.Append(tcInspDteLbl);


                                TableCell tcInspDte = CellFont(ret.Areas, 30, false);
                                trInsp.Append(tcInspDte);
                                tableInsp.Append(trInsp);
                            }
                            {
                                TableRow trInsp = new TableRow();

                                TableCell tcInspDteLbl = CellFont("Inspector:", 30, true);
                                trInsp.Append(tcInspDteLbl);


                                TableCell tcInspDte = CellFont(ret.Inspector, 30, false);
                                trInsp.Append(tcInspDte);
                                tableInsp.Append(trInsp);


                            }
                            {
                                TableRow trInsp = new TableRow();

                                TableCell tcInspDteLbl = CellFont("",30,false);
                                trInsp.Append(tcInspDteLbl);


                                TableCell tcInspDte = CellFont(ret.Inspector2, 30, false);
                                trInsp.Append(tcInspDte);
                                tableInsp.Append(trInsp);


                            }
                            {
                                TableRow trInsp = new TableRow();

                                TableCell tcInspDteLbl = CellFont("Testing Instrument:", 30, true);
                                trInsp.Append(tcInspDteLbl);


                                TableCell tcInspDte = CellFont(ret.Instrument, 30, false);
                                trInsp.Append(tcInspDte);
                                tableInsp.Append(trInsp);


                            }
                            {
                                TableRow trInsp = new TableRow();

                                TableCell tcInspDteLbl = CellFont("Building:", 30, true);
                                trInsp.Append(tcInspDteLbl);


                                TableCell tcInspDte = CellFont(ret.Address, 30, false);
                                trInsp.Append(tcInspDte);
                                tableInsp.Append(trInsp);


                            }


                            body6.AppendChild(tableInsp);
                        }
                     

                        { 
                        Body bodyintro = mainPart.Document.AppendChild(new Body());
                        Paragraph parabodyintro = bodyintro.AppendChild(new Paragraph());
                        Run runIntro = parabodyintro.AppendChild(new Run());
                        RunProperties runPropertiesIntr = runIntro.AppendChild(new RunProperties());

                            Break pgbrkintro = new Break();
                            pgbrkintro.Type = BreakValues.Page;
                            runPropertiesIntr.AppendChild(pgbrkintro);


                            AddHeader(runIntro, runPropertiesIntr, "4. Introduction");
                    }
                        {
                            Body bodyintro1 = mainPart.Document.AppendChild(new Body());
                            Paragraph paraintro1 = bodyintro1.AppendChild(new Paragraph());
                            Run runintro1 = paraintro1.AppendChild(new Run());
                            RunProperties runPropintro1 = runintro1.AppendChild(new RunProperties());
                            FontSize fontSizeintro1 = new FontSize() { Val = "30" };
                            runPropintro1.Append(fontSizeintro1);
                            //Bold bold4 = new Bold();

                            // bold4.Val = OnOffValue.FromBoolean(true);
                            // runp2.AppendChild(bold4);
                            runintro1.AppendChild(new Text("Roof Safety Solutions has been commissioned to:"));
                            runintro1.AppendChild(new Break()); runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("1. Supply an inventory of all height safety equipment installed on site."));
                            runintro1.AppendChild(new Break()); runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("2. Carry out an inspection of all existing assets, in accordance with:"));
                            runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("•  Work Health & Safety Act 2020 (WA)."));
                            runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("• Work Health & Safety Regulations 2022 (WA)"));
                            runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("• Code of Practice: Managing the risk of falls at workplaces 2022."));                          
                             runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("• AS 1657:2018 - Fixed platforms, walkways, stairways and ladders."));
                            runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("• AS/NZS 1891.4:2009 - Industrial fall-arrest systems and devices –selection, use and maintenance."));
                            runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("• AS/NZS 4488.2:1997 – Industrial rope access systems – selection use and maintenance."));
                            runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("• AS/NZS 1891.4:2009 - Industrial fall-arrest systems and devices –selection, use and maintenance."));


                            runintro1.AppendChild(new Break()); runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("3. Report on findings, identify existing hazards and provide suitable recommendations in cases of non-compliance."));

                        }
                        {
                            Body bodyintro = mainPart.Document.AppendChild(new Body());
                            Paragraph parabodyintro = bodyintro.AppendChild(new Paragraph());
                            Run runIntro = parabodyintro.AppendChild(new Run());
                            RunProperties runPropertiesIntr = runIntro.AppendChild(new RunProperties());

                            Break pgbrkintro = new Break();
                            pgbrkintro.Type = BreakValues.Page;
                            runPropertiesIntr.AppendChild(pgbrkintro);

                            AddHeader(runIntro, runPropertiesIntr, "5. Risk Assessment Matrix");
                        }
                        {
                            Body bodyintro1 = mainPart.Document.AppendChild(new Body());
                            Paragraph paraintro1 = bodyintro1.AppendChild(new Paragraph());
                            Run runintro1 = paraintro1.AppendChild(new Run());
                            RunProperties runPropintro1 = runintro1.AppendChild(new RunProperties());
                            FontSize fontSizeintro1 = new FontSize() { Val = "30" };
                            runPropintro1.Append(fontSizeintro1);
                            runintro1.AppendChild(new Text("This report will use the following risk assessment matrix to show the severity of any identified hazards."));


                            {

                                Body bodymatrix = mainPart.Document.AppendChild(new Body());
                                Paragraph paramatrix = bodymatrix.AppendChild(new Paragraph());
                                Run runmatrix = paramatrix.AppendChild(new Run());
                                RunProperties runPropmatrix = runmatrix.AppendChild(new RunProperties());
                                FontSize fontSizematrix = new FontSize() { Val = "24" };
                                runPropmatrix.Append(fontSizematrix);

                                Table table = new Table();
                                FontSize fontSizep42 = new FontSize() { Val = "42" };

                                TableProperties tblProp = new TableProperties(
                                   fontSizep42,
                                    new TableBorders(
                                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size =5},
                                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 },
                                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 5 }
                                    )
                                );
                                // Append the TableProperties object to the empty table.
                                
                                table.AppendChild<TableProperties>(tblProp);
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);


                                    TableCell tc1 = new TableCell();
                                    //{
                                    //    TableCellProperties cellProperties = new();
                                    //    cellProperties.Append(new Span { Val =2 });
                                    //    tc1.Append(cellProperties);
                                    //}
                                    tc1.Append(new Paragraph(new Run(new Text("      "))));
                                    tr.Append(tc1);

                                    TableCell tc2 = new TableCell();
                                    {
                                        TableCellProperties cellProperties = new();
                                        cellProperties.Append(new GridSpan { Val = 5 });
                                        tc2.Append(cellProperties);
                                    }
                                    tc2.Append(SetColor("#D3D3D3"));
                                    Paragraph parait = new Paragraph();
                                    ParagraphProperties pp = new ParagraphProperties();
                                    pp.Justification = new Justification() { Val = JustificationValues.Center };
                                    parait.Append(pp);
                                    Run runit = parait.AppendChild(new Run());
                                    RunProperties rpit = runit.AppendChild(new RunProperties());
   
                                    Bold bold = new Bold();
                                    rpit.AppendChild(bold);
                                    runit.AppendChild(new Text("Consequence"));
                                    tc2.Append(parait);                                   
                                    tr.Append(tc2);
                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);



                                    TableCell tc1 = new TableCell();
                                    tc1.Append(new Paragraph(new Run(new Text("      "))));
                                    tr.Append(tc1);

                                    TableCell tc2 = TableCellCentred("Injuries not requiring First Aid");
                                    tc2.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc2);

                                    TableCell tc3 = TableCellCentred("First Aid Treatment Case.");
                                    tc3.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc3);

                                    TableCell tc4 = TableCellCentred("Serious injury, medical treatment, or hospitalisation");
                                    tc4.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc4);

                                    TableCell tc5 = TableCellCentred("Multiple serious injuries causing hospitalisation.");
                                    tc5.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc5);

                                    TableCell tc6 = TableCellCentred("Death or multiple life-threatening injuries.");
                                    tc6.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc6);

                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();


                                    tc1.Append(new Paragraph(new Run(new Text("Likelihood"))));
                                    tr.Append(tc1);
                                    TableCell tc2 = TableCellCentred("Negligible");
                                    tr.Append(tc2);
                                    TableCell tc3 = TableCellCentred("Minor");
                                    tr.Append(tc3);
                                    TableCell tc4 = TableCellCentred("Moderate");
                                    tr.Append(tc4);
                                    TableCell tc5 = TableCellCentred("Major");
                                    tr.Append(tc5);
                                    TableCell tc6 = TableCellCentred("Catastrophic");
                                    tr.Append(tc6);

                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();


                                    tc1.Append(new Paragraph(new Run(new Text("Almost Certain"))));
                           
                                    tc1.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc1);
                                    TableCell tc2 = CenterAndColour("5", Blue);
           
                                    tr.Append(tc2);
                                    TableCell tc3 = CenterAndColour("10", Yellow);

                                    tr.Append(tc3);
                                    TableCell tc4 = CenterAndColour("15",Red);
                  
                                    tr.Append(tc4);
                                    TableCell tc5 = CenterAndColour("20", Red);
                 
                                    tr.Append(tc5);
                                    TableCell tc6 = CenterAndColour("25", Red);
             
                                    tr.Append(tc6);

                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();


                                    tc1.Append(new Paragraph(new Run(new Text("Likely"))));
                                    tc1.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc1);
                                    TableCell tc2 = CenterAndColour("4", Blue);

                                    tr.Append(tc2);
                                    TableCell tc3 = CenterAndColour("8", Yellow);
                                    tr.Append(tc3);
                                    TableCell tc4 = CenterAndColour("12", Yellow);
           
                                    tr.Append(tc4);
                                    TableCell tc5 = CenterAndColour("16", Red);
                                   
                                    tr.Append(tc5);
                                    TableCell tc6 = CenterAndColour("20", Red);
                                    
                                    tr.Append(tc6);

                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();
                                    tc1.Append(SetColor("#D3D3D3"));
                                    tc1.Append(new Paragraph(new Run(new Text("Occasionally"))));
                                    tr.Append(tc1);
                                    TableCell tc2 = CenterAndColour("3", Green);
                                    tr.Append(tc2);
                                    TableCell tc3 = CenterAndColour("6", Blue);
                                   
                                    tr.Append(tc3);
                                    TableCell tc4 = CenterAndColour("9", Yellow);
                                   
                                    tr.Append(tc4);
                                    TableCell tc5 = CenterAndColour("12", Yellow);
                                   
                                    tr.Append(tc5);
                                    TableCell tc6 = CenterAndColour("15", Red);
                                    
                                    tr.Append(tc6);

                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();
                                    tc1.Append(SetColor("#D3D3D3"));
                                    tc1.Append(new Paragraph(new Run(new Text("Unlikely"))));
                                    tr.Append(tc1);
                                    TableCell tc2 = CenterAndColour("2", Green);
                                  
                                    tr.Append(tc2);
                                    TableCell tc3 = CenterAndColour("4", Blue);
                                     tr.Append(tc3);
                                    TableCell tc4 = CenterAndColour("6", Blue);
                                    tr.Append(tc4);
                                    TableCell tc5 = CenterAndColour("8", Yellow);
                                    tr.Append(tc5);
                                    TableCell tc6 = CenterAndColour("10", Yellow);

                                    tr.Append(tc6);

                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();
                                    tc1.Append(SetColor("#D3D3D3"));
                                    tc1.Append(new Paragraph(new Run(new Text("Rare"))));
                                    tr.Append(tc1);
                        
                                    TableCell tc2 = CenterAndColour("1", Green);
                                    tr.Append(tc2);
                                    TableCell tc3 = CenterAndColour("3", Green); 
                                    tr.Append(tc3);
                                    TableCell tc4 = CenterAndColour("4", Green);
                                    tr.Append(tc4);
                                    TableCell tc5 = CenterAndColour("4", Blue);
                                    tr.Append(tc5);
                                    TableCell tc6 = CenterAndColour("5", Blue);
                                     tr.Append(tc6);

                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();


                                    tc1.Append(new Paragraph(new Run(new Text("Extreme (15-25)"))));
                                    tc1.Append(SetColor(Red));
                                    tr.Append(tc1);
                                    TableCell tc2 = new TableCell();
                                    {
                                        TableCellProperties cellProperties = new();
                                        cellProperties.Append(new GridSpan { Val = 5 });
                                        tc2.Append(cellProperties);
                                    }
                                    tc2.Append(new Paragraph(new Run(new Text("Immediate management action is required"))));
                                
                                    tr.Append(tc2);
                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();


                                    tc1.Append(new Paragraph(new Run(new Text("High (8-14)"))));
                                    tc1.Append(SetColor(Yellow));
                                    tr.Append(tc1);
                                    TableCell tc2 = new TableCell();
                                    {
                                        TableCellProperties cellProperties = new();
                                        cellProperties.Append(new GridSpan { Val = 5 });
                                        tc2.Append(cellProperties);
                                    }
                                    tc2.Append(new Paragraph(new Run(new Text("Management attention is required with the implementation of formal controls"))));
                                    
                                    tr.Append(tc2);
                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();


                                    tc1.Append(new Paragraph(new Run(new Text("Medium (4-7)"))));
                                    tc1.Append(SetColor(Blue));
                                    tr.Append(tc1);
                                    TableCell tc2 = new TableCell();
                                    {
                                        TableCellProperties cellProperties = new();
                                        cellProperties.Append(new GridSpan { Val = 5 });
                                        tc2.Append(cellProperties);
                                    }
                                    tc2.Append(new Paragraph(new Run(new Text("Requires active monitoring or action"))));

                                    tr.Append(tc2);
                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();


                                    tc1.Append(new Paragraph(new Run(new Text("Low (1-3)"))));
                                    tc1.Append(SetColor(Green));
                                    tr.Append(tc1);
                                    TableCell tc2 = new TableCell();
                                    {
                                        TableCellProperties cellProperties = new();
                                        cellProperties.Append(new GridSpan { Val = 5 });
                                        tc2.Append(cellProperties);
                                    }
                                    tc2.Append(new Paragraph(new Run(new Text("Does not require active management"))));

                                    tr.Append(tc2);
                                }
                                bodyintro1.AppendChild(table);
                            }
                            }
                        {
                            Body bodyintro2 = mainPart.Document.AppendChild(new Body());
                            Paragraph parabodyintro2 = bodyintro2.AppendChild(new Paragraph());
                            Run runIntro2 = parabodyintro2.AppendChild(new Run());
                            RunProperties runPropertiesIntr2 = runIntro2.AppendChild(new RunProperties());

                            Break pgbrkintro = new Break();
                            pgbrkintro.Type = BreakValues.Page;
                            runPropertiesIntr2.AppendChild(pgbrkintro);


                            AddHeader(runIntro2, runPropertiesIntr2, "6. Inspection Report");
                           
                            {
                                /////
                                
                                  int itemno = 0;
                                foreach (var item in ret.Items.OrderBy(i => i.Ordr))
                                {

                                    Body bodyintro = mainPart.Document.AppendChild(new Body());
                                  

                                    Table tableInsp = new Table();
                                    TableProperties props = new TableProperties();
                                    tableInsp.AppendChild<TableProperties>(props);
                                    TableLayout tl = new TableLayout() { Type = TableLayoutValues.Fixed };
                                    props.TableLayout = tl;
                                    FontSize fontSizeInsp = new FontSize() { Val = "42" };

                                    TableProperties tblPropInsp = new TableProperties(
                                       fontSizeInsp,
                                        new TableBorders(
                                        )
                                    );
                                    tableInsp.AppendChild<TableProperties>(tblPropInsp);
                                    itemno = itemno + 1;
                                    { //test
                                        TableRow trInsp = new TableRow();
                                        TableCell tcInspDteLbl = new TableCell();

                                        tcInspDteLbl.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2000" }));
                                  

                                        Bold boldinsp = new Bold();

                                        boldinsp.Val = OnOffValue.FromBoolean(true);

                                        Run runInsp = new Run(new Text(""));
                                        runInsp.AppendChild(boldinsp);
                                        tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                        trInsp.Append(tcInspDteLbl);


                                        TableCell tcInspDte = new TableCell();
                                        tcInspDte.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "4000" }));

                                        tcInspDte.Append(new Paragraph(new Run(new Text(""))));
                                      
                                        trInsp.Append(tcInspDte);

                                        TableCell tcInspDte2 = new TableCell();
                                        tcInspDte2.Append(new Paragraph(new Run(new Text(""))));
                                        tcInspDte2.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "4000" }));

                                        trInsp.Append(tcInspDte2);

                                        tableInsp.Append(trInsp);
                                    }
                                    {
                                        TableRow trInsp = new TableRow();
                                        TableCell tcInspDteLbl = CellFont("Item:", 26, true,"Navy");// new TableCell();

                                        //Bold boldinsp = new Bold();

                                        //boldinsp.Val = OnOffValue.FromBoolean(true);
                                        
                                        //Run runInsp = new Run(new Text(""));
                                        //runInsp.AppendChild(boldinsp);
                                        //tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                        trInsp.Append(tcInspDteLbl);


                                        TableCell tcInspDte = new TableCell();
                                        tcInspDte.Append(new Paragraph(new Run(new Text("1."+itemno.ToString()))));
                                        tcInspDte.Append(new GridSpan { Val =2 });
                                        trInsp.Append(tcInspDte);
                                        tableInsp.Append(trInsp);
                                    }
                                     {
                                        TableRow trInsp = new TableRow();
                                        TableCell tcInspDteLbl = CellFont("Risk:", 26, true, "Navy"); //new TableCell();

                                        //Bold boldinsp = new Bold();

                                        //boldinsp.Val = OnOffValue.FromBoolean(true);

                                        //{
                                        //    Paragraph paraxx = tcInspDteLbl.AppendChild(new Paragraph());

                                        //    ParagraphProperties paraPropsx = new ParagraphProperties();
                                        //    SpacingBetweenLines spacingx = new SpacingBetweenLines() { Before = "15", After = "0" };
                                        //    paraPropsx.SpacingBetweenLines = spacingx;
                                        //    paraxx.ParagraphProperties = paraPropsx;

                                        //    Run runInsp = paraxx.AppendChild(new Run());
                                        //    runInsp.AppendChild(new Text("Risk:"));
                                        //    runInsp.AppendChild(boldinsp);
                                        //}

                                        trInsp.Append(tcInspDteLbl);



                                        TableCell tcInspDte = new TableCell();
                                        {
                                            //TableCellProperties tcp = new TableCellProperties();
                                            //TableCellVerticalAlignment tcVA = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
                                            //tcp.Append(tcVA);
                                            //tcInspDte.Append(tcp);
                                        }

                                        TableCellProperties tcp = new TableCellProperties();
                                        TableCellVerticalAlignment tcVA = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };

                                        //append the TableCellVerticalAlignment instance
                                        tcp.Append(tcVA);
                                        tcInspDte.Append(tcp);

                                        string colour;
                                        if (item.Risk == "Maintained")
                                            colour = "#D3D3D3";
                                        else
                                            colour =RoofSafety.Models.InspEquipTest.getcolour(item.MaxRisk);
                                       tcInspDte.Append(SetColor(colour));
                                        tcInspDte.Append(new GridSpan { Val = 2 });

                                        Paragraph parax = tcInspDte.AppendChild(new Paragraph());

                                        ParagraphProperties ppp = new ParagraphProperties();
                                        ppp.Justification = new Justification() { Val = JustificationValues.Center };
                                        SpacingBetweenLines spacing = new SpacingBetweenLines() { Before = "15", After = "0" };
                                        ppp.SpacingBetweenLines = spacing;
                                        parax.ParagraphProperties = ppp;
                                        //parax.Append(pp);

                                        //ParagraphProperties paraProps = new ParagraphProperties();
                                        //
                                        //paraProps.SpacingBetweenLines = spacing;
                                        //parax.ParagraphProperties = paraProps;

                                        Run runx = parax.AppendChild(new Run());
                                        runx.AppendChild(new Text(item.Risk));



                                     //   tcInspDte.Append(new Paragraph(new Run(new Text(item.Risk))));
                                        trInsp.Append(tcInspDte);
                                        tableInsp.Append(trInsp);
                                    }
                                      {
                                          TableRow trInsp = new TableRow();
                                          TableCell tcInspDteLbl = CellFont("Description:", 26, true, "Navy"); //new TableCell();

                                          //Bold boldinsp = new Bold();

                                          //boldinsp.Val = OnOffValue.FromBoolean(true);

                                          //Run runInsp = new Run(new Text("Description:"));
                                          //runInsp.AppendChild(boldinsp);
                                          //tcInspDteLbl.Append(new Paragraph(runInsp));
                                          trInsp.Append(tcInspDteLbl);


                                          TableCell tcInspDte = new TableCell();

                                          tcInspDte.Append(new Paragraph(new Run(new Text(item.EquipName))));
                                        tcInspDte.Append(new GridSpan { Val = 2 });
                                        trInsp.Append(tcInspDte);
                                          tableInsp.Append(trInsp);
                                      }
                                      {
                                          TableRow trInsp = new TableRow();
                                          TableCell tcInspDteLbl = CellFont("Location:", 26, true, "Navy"); //new TableCell();

                                          //Bold boldinsp = new Bold();

                                          //boldinsp.Val = OnOffValue.FromBoolean(true);

                                          //Run runInsp = new Run(new Text("Location:"));
                                          //runInsp.AppendChild(boldinsp);
                                          //tcInspDteLbl.Append(new Paragraph(runInsp));
                                          trInsp.Append(tcInspDteLbl);


                                          TableCell tcInspDte = new TableCell();

                                          tcInspDte.Append(new Paragraph(new Run(new Text(item.Location))));
                                        tcInspDte.Append(new GridSpan { Val = 2 });
                                        trInsp.Append(tcInspDte);
                                          tableInsp.Append(trInsp);
                                      }
                                      {
                                          TableRow trInsp = new TableRow();
                                          TableCell tcInspDteLbl = CellFont("Result:", 26, true, "Navy"); //new TableCell();

                                          //Bold boldinsp = new Bold();

                                          //boldinsp.Val = OnOffValue.FromBoolean(true);

                                          //Run runInsp = new Run(new Text("Result:"));
                                          //runInsp.AppendChild(boldinsp);
                                          //tcInspDteLbl.Append(new Paragraph(runInsp));
                                          trInsp.Append(tcInspDteLbl);


                                          string text = item.Result;

                                          string colour;
                                          if (item.Result == "Compliant")
                                              colour = Green;// "#0BDA51";
                                          else
                                              colour =Red;
                                          TableCell tcInspDte = CellForeColor(text, colour);
                                          tcInspDte.Append(new GridSpan { Val = 2 });
                                          trInsp.Append(tcInspDte);
                                          tableInsp.Append(trInsp);
                                      }
                                      if (item.Result != "Compliant")
                                      {
                                        int ii = 0;
                                        foreach (var ep in item.Explanation.Eps)
                                        {
                                            TableRow trInsp = new TableRow();
                                          TableCell tcInspDteLbl = CellFont(((ii == 0) ? "Explanation:" : ""), 26, true, "Navy"); //new TableCell();

                                          //Bold boldinsp = new Bold();

                                          //boldinsp.Val = OnOffValue.FromBoolean(true);
                                     

                                          //Run runInsp = new Run(new Text((ii==0)?"Explanation:":""));
                                          //runInsp.AppendChild(boldinsp);
                                          //tcInspDteLbl.Append(new Paragraph(runInsp));
                                          trInsp.Append(tcInspDteLbl);

                                            ii++;
                                            {
                                                TableCell tcInspDte = new TableCell();
                                                Paragraph parait = new Paragraph();
                                                Run runit = parait.AppendChild(new Run());
                                                RunProperties rpit = runit.AppendChild(new RunProperties());
                                                DocumentFormat.OpenXml.Wordprocessing.Bold bold = new DocumentFormat.OpenXml.Wordprocessing.Bold();
                                                rpit.AppendChild(bold);
                                                runit.AppendChild(new Text(ep.ASTest));
                                                tcInspDte.Append(parait);
                                              
                                                trInsp.Append(tcInspDte);
                                            }
                                            {
                                                TableCell tcIn2 = new TableCell();

                                                ////this section makes it corrupt
                                                Paragraph parait = new Paragraph();
                                                Run runit = parait.AppendChild(new Run());
                                                RunProperties rpit = runit.AppendChild(new RunProperties());
                                                DocumentFormat.OpenXml.Wordprocessing.Italic italic = new DocumentFormat.OpenXml.Wordprocessing.Italic();
                                                italic.Val = OnOffValue.FromBoolean(true);
                                                rpit.AppendChild(italic);

                                                runit.AppendChild(new Text(ep.P2));
                                                tcIn2.Append(parait);

                                                // tcIn2.Append(new Paragraph(new Run(new Text(ep.P2))));
                                                trInsp.Append(tcIn2);
                                            }
                                                 
                                            
                                            //   tcInspDte.Append(tableIn);
                                            // tcInspDte.Append(new Paragraph(new Run(new Text(item.Result))));
                                            //
                                            
                                        tableInsp.Append(trInsp);
                                        }
                                    }
                                        {
                                            TableRow trInsp = new TableRow();
                                          TableCell tcInspDteLbl = new TableCell();

                                          Bold boldinsp = new Bold();

                                          boldinsp.Val = OnOffValue.FromBoolean(true);

                                          Run runInsp = new Run(new Text(""));
                                          runInsp.AppendChild(boldinsp);
                                          tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                          trInsp.Append(tcInspDteLbl);


                                          TableCell tcInspDte = new TableCell();
                                        tcInspDte.Append(new GridSpan { Val = 2 });
                                        tcInspDte.Append(new Paragraph(new Run(new Text(item.Notes))));
                                          trInsp.Append(tcInspDte);
                                          tableInsp.Append(trInsp);
                                      }
                                    if (item.Result == "Compliant")
                                      {
                                          TableRow trInsp = new TableRow();
                                        TableCell tcInspDteLbl = CellFont("Compliant:", 26, true, "Navy"); //new TableCell();
                                        //TableCell tcInspDteLbl = new TableCell();

                                        //  Bold boldinsp = new Bold();

                                        //  boldinsp.Val = OnOffValue.FromBoolean(true);

                                        //  Run runInsp = new Run(new Text("Compliant:"));
                                        //  runInsp.AppendChild(boldinsp);
                                        //  tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                          trInsp.Append(tcInspDteLbl);


                                          TableCell tcInspDte = new TableCell();
                                        tcInspDte.Append(new GridSpan { Val = 2 });
                                        tcInspDte.Append(new Paragraph(new Run(new Text(item.EquipType.CompliantInfo))));
                                          trInsp.Append(tcInspDte);
                                          tableInsp.Append(trInsp);
                                      }
                                      else
                                    {
                                        if (item.Hazards != null)
                                        {
                                            int cnt = 0;
                                            foreach (var hz in item.Hazards)
                                            {

                                                TableRow trInsp = new TableRow();
                                                TableCell tcInspDteLbl = CellFont(((cnt == 0) ? "Hazards:" : ""), 26, true, "Navy");// new TableCell();

                                                //Bold boldinsp = new Bold();

                                                //boldinsp.Val = OnOffValue.FromBoolean(true);

                                                //Run runInsp = new Run(new Text((cnt==0)?"Hazards:":""));
                                                cnt++;
                                                //runInsp.AppendChild(boldinsp);
                                                //tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                                trInsp.Append(tcInspDteLbl);


                                                TableCell tcInspDte = new TableCell();

                                                tcInspDte.Append(new Paragraph(new Run(new Text(hz))));
                                                tcInspDte.Append(new GridSpan { Val = 2 });
                                                trInsp.Append(tcInspDte);
                                                tableInsp.Append(trInsp);
                                            }
                                        }

                                          {
                                              TableRow trInsp = new TableRow();
                                            //  TableCell tcInspDteLbl = new TableCell();

                                             // Bold boldinsp = new Bold();

                                             // boldinsp.Val = OnOffValue.FromBoolean(true);

                                           //   Run runInsp = new Run(new Text("Required Controls:"));
                                           //   runInsp.AppendChild(boldinsp);
                                          //    tcInspDteLbl.Append(SetColor("Navy"));
                                            //  tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                          //    trInsp.Append(tcInspDteLbl);

                                              TableCell tcInspDteLbl = CellFont("Required Controls:",26,true, "navy");
                                              trInsp.Append(tcInspDteLbl);



                                              TableCell tcInspDte = new TableCell();

                                              tcInspDte.Append(new Paragraph(new Run(new Text(item.RequiredControls))));
                                            tcInspDte.Append(new GridSpan { Val = 2 });
                                            trInsp.Append(tcInspDte);
                                              tableInsp.Append(trInsp);
                                          }
                                      }
                                    foreach (var ph in item.Photos)
                                          {
                                              TableRow trInsp = new TableRow();

                                          TableCell tclbl = CellFont("Photo:", 26, true, "navy");// new TableCell();

                                          //tclbl.Append(new Paragraph(new Run(new Text("Photo"))));
                                          trInsp.Append(tclbl);

                                          TableCell tcInspDte = new TableCell();
                                        tcInspDte.Append(new GridSpan { Val = 2 });
                                        trInsp.Append(tcInspDte);
                                              tableInsp.Append(trInsp);

                                          string imgurlx = ph.photoname.Replace("%0D%0A", "").TrimEnd();
                                              //MainDocumentPart mainPart2 = wordDocument.AddMainDocumentPart();
                                              await InsertImage(tcInspDte,wordDocument, mainPart, imgurlx, scale);//50
                                          }
                                    
                                    bodyintro.AppendChild(tableInsp);

                                      // Body bodyintro = mainPart.Document.AppendChild(new Body());
                                      // Paragraph parabodyintro = bodyintro.AppendChild(new Paragraph());
                                      //Run runIntro = parabodyintro.AppendChild(new Run());
                                      // RunProperties runPropertiesIntr = runIntro.AppendChild(new RunProperties());
                                      Paragraph parabodyintro = bodyintro.AppendChild(new Paragraph());
                                      Run runIntro = parabodyintro.AppendChild(new Run());
                                      RunProperties runPropertiesIntr = runIntro.AppendChild(new RunProperties());

                                      
                                    Break pgbrkintro2 = new Break();
                                    pgbrkintro2.Type = BreakValues.Page;
                                    runPropertiesIntr.AppendChild(pgbrkintro2); 
                                }

                                /////
                            }
                       
                       


                                }

                                {
                                Body bodyintro = mainPart.Document.AppendChild(new Body());
                                Paragraph parabodyintro = bodyintro.AppendChild(new Paragraph());
                                Run runIntro = parabodyintro.AppendChild(new Run());
                                RunProperties runPropertiesIntr = runIntro.AppendChild(new RunProperties());

                                //Break pgbrkintro = new Break();
                                //pgbrkintro.Type = BreakValues.Page;
                                //runPropertiesIntr.AppendChild(pgbrkintro);


                                AddHeader(runIntro, runPropertiesIntr, "7. Summary");
                            }
                            
                        {

                            Body bodyintro1 = mainPart.Document.AppendChild(new Body());
                            Paragraph paraintro1 = bodyintro1.AppendChild(new Paragraph());
                            Run runintro1 = paraintro1.AppendChild(new Run());
                            RunProperties runPropintro1 = runintro1.AppendChild(new RunProperties());
                            FontSize fontSizeintro1 = new FontSize() { Val = "24" };
                            runPropintro1.Append(fontSizeintro1);

                            Table table = new Table();
                            FontSize fontSizep42 = new FontSize() { Val = "42" };

                            TableProperties tblProp = new TableProperties(
                               fontSizep42,
                                new TableBorders(
                                    new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                    new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size =3 }
                                )
                            );
                            // Append the TableProperties object to the empty table.
                            table.AppendChild<TableProperties>(tblProp);
                            TableRow tr = new TableRow();
                            table.Append(tr);
                            TableCell tc0 = new TableCell();
                            tc0.Append(new Paragraph(new Run(new Text("Item #"))));
                            tc0.Append(SetColor("#D3D3D3"));
                            tr.Append(tc0);

                            TableCell tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text("Equipment Name"))));
                            tc1.Append(SetColor("#D3D3D3"));
                            tr.Append(tc1);
                            TableCell tc2 = new TableCell();
                            tc2.Append(new Paragraph(new Run(new Text("Manufacturer"))));
                            tc2.Append(SetColor("#D3D3D3")); 
                            tr.Append(tc2);
                            TableCell tc3 = new TableCell();
                            tc3.Append(new Paragraph(new Run(new Text("Inspection Date"))));
                            tc3.Append(SetColor("#D3D3D3"));
                            tr.Append(tc3);
                            TableCell tc4 = new TableCell();
                            tc4.Append(new Paragraph(new Run(new Text("Status"))));
                            tc4.Append(SetColor("#D3D3D3"));
                            tr.Append(tc4);
                            TableCell tc5 = new TableCell();
                            tc5.Append(new Paragraph(new Run(new Text("Installer"))));
                            tc5.Append(SetColor("#D3D3D3"));
                            tr.Append(tc5);
                            TableCell tc6 = new TableCell();
                            tc6.Append(new Paragraph(new Run(new Text("Rating"))));
                            tc6.Append(SetColor("#D3D3D3"));
                            tr.Append(tc6);
                            TableCell tc7 = new TableCell();
                            tc7.Append(new Paragraph(new Run(new Text("Asset ID"))));
                            tc7.Append(SetColor("#D3D3D3"));
                            tr.Append(tc7);
                            bodyintro1.AppendChild(table);

                            
                            foreach (var item in ret.Items.OrderBy(i => i.Ordr))
                            {
                                string colour = item.Pass ? "Green" : "Red";
                                {
                                    TableRow trx = new TableRow();
                                    table.Append(trx);
                                    TableCell t0 = CellFont("1." + item.ItemNo?.ToString(), 24, false, "Black");
                                    trx.Append(t0);

                                    TableCell t1 = CellFont(item.EquipName?.ToString(), 24, false, "Black");
                                    trx.Append(t1);
                                    TableCell t2 = CellFont(item.Manufacturer?.ToString(), 24, false, "Black");
                              
                                    trx.Append(t2);
                                    TableCell t3 = CellFont(ret.InspDate.ToString("dd-MMM-yyyy"), 24, false, "Black");
                                    trx.Append(t3);

                                    TableCell t4 = CellFont((item.Pass) ? "Compliant" : "Non-Compliant", 24, false, colour);
                  
                                    trx.Append(t4);
                                    TableCell t5 = CellFont(item.Installer, 24, false, "Black");
 
                                    trx.Append(t5);
                                    TableCell t6 = CellFont(item.Rating, 24, false, "Black");
                                    trx.Append(t6);
                                    TableCell t7;
                                    if(item.SerialNos.Count()>1)
                                    t7 = CellFont(item.SerialNo + "1", 24, false, "Black");
                                    else
                                    t7 = CellFont(item.SerialNo , 24, false, "Black");
                                    trx.Append(t7);

                                }

                                ///////////////
                                if(item.SerialNos.Count() > 1)
                                {

                                    for (int i = 1; i < item.SerialNos.Count(); i++)
                                    {
                                        TableRow trx = new TableRow();
                                        table.Append(trx);
                                        TableCell t0 = CellFont("", 24, false, "Black");//item.ItemNo?.ToString() + "." + i.ToString()
                                        trx.Append(t0);
                                        TableCell t1 = CellFont(item.EquipName, 24, false, "Black");
                                        trx.Append(t1);
                                        TableCell t2 = CellFont(item.Manufacturer, 24, false, "Black");
                                        trx.Append(t2);

                                        TableCell t3 = CellFont(ret.InspDate.ToString("dd-MMM-yyyy"), 24, false, "Black");
                                        trx.Append(t3);

                                        TableCell t4 = CellFont(item.Result, 24, false, colour);
                                        trx.Append(t4);

                                        TableCell t5 = CellFont(item.Installer, 24, false, "Black");
                                        trx.Append(t5);
                                        TableCell t6 = CellFont(item.Rating, 24, false, "Black");
                                        trx.Append(t6);
                                        TableCell t7 = CellFont(item.SerialNos[i], 24, false, "Black");
                                        trx.Append(t7);
                                        
                                    }
                                }
                                /////
                            }
                        }

                        {
                            Body bodyintro = mainPart.Document.AppendChild(new Body());
                            Paragraph parabodyintro = bodyintro.AppendChild(new Paragraph());
                            Run runIntro = parabodyintro.AppendChild(new Run());
                            RunProperties runPropertiesIntr = runIntro.AppendChild(new RunProperties());

                            Break pgbrkintro = new Break();
                            pgbrkintro.Type = BreakValues.Page;
                            runPropertiesIntr.AppendChild(pgbrkintro);


                            AddHeader(runIntro, runPropertiesIntr, "8. Conclusion");
                        }
                        {
                            Body bodyintro1 = mainPart.Document.AppendChild(new Body());
                            Paragraph paraintro1 = bodyintro1.AppendChild(new Paragraph());
                            Run runintro1 = paraintro1.AppendChild(new Run());
                            RunProperties runPropintro1 = runintro1.AppendChild(new RunProperties());
                            FontSize fontSizeintro1 = new FontSize() { Val = "30" };
                            runPropintro1.Append(fontSizeintro1);
                            //Bold bold4 = new Bold();

                            // bold4.Val = OnOffValue.FromBoolean(true);
                            // runp2.AppendChild(bold4);
                            runintro1.AppendChild(new Text("All recommendations within this report will enable personnel to carry out their jobs in a safe manner while working on your roof and ensure the height safety systems are ready for everyday maintenance tasks."));
                            runintro1.AppendChild(new Break()); runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("Data plates have been updated to show inspection dates."));
                            runintro1.AppendChild(new Break()); runintro1.AppendChild(new Break()); 
                            runintro1.AppendChild(new Text("All height safety equipment has been tagged to reflect inspection dates and load ratings."));
                            runintro1.AppendChild(new Break()); runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("Any purlin fixed anchors have been re-tensioned to the sub structure."));
                            runintro1.AppendChild(new Break()); runintro1.AppendChild(new Break()); 
                            runintro1.AppendChild(new Text("Any friction mounted anchors have been subjected to load testing under the guidance given by the relevant Australian standard and its mechanical testing requirements for fall prevention anchors."));
                            runintro1.AppendChild(new Break()); runintro1.AppendChild(new Break());
                            runintro1.AppendChild(new Text("If you have any questions please do not hesitate to contact us on (08) 9447 4884 or email us at admin@roofsafetysolutions.com.au."));
                        }

                        {
                            Body bodyintro = mainPart.Document.AppendChild(new Body());
                            Paragraph parabodyintro = bodyintro.AppendChild(new Paragraph());
                            Run runIntro = parabodyintro.AppendChild(new Run());
                            RunProperties runPropertiesIntr = runIntro.AppendChild(new RunProperties());

                            Break pgbrkintro = new Break();
                            pgbrkintro.Type = BreakValues.Page;
                            runPropertiesIntr.AppendChild(pgbrkintro);


                            AddHeader(runIntro, runPropertiesIntr, "9. Disclaimer");
                        }
                        {
                            Body bodyintro1 = mainPart.Document.AppendChild(new Body());
                            Paragraph paraintro1 = bodyintro1.AppendChild(new Paragraph());
                            Run runintro1 = paraintro1.AppendChild(new Run());
                            RunProperties runPropintro1 = runintro1.AppendChild(new RunProperties());
                            FontSize fontSizeintro1 = new FontSize() { Val = "30" };
                            runPropintro1.Append(fontSizeintro1);
                            //Bold bold4 = new Bold();

                            // bold4.Val = OnOffValue.FromBoolean(true);
                            // runp2.AppendChild(bold4);
                            runintro1.AppendChild(new Text("This report was prepared solely for the purpose set out herein and it is not intended for any use. Whilst this report is accurate to the best of our knowledge and belief, we cannot guarantee completeness or accuracy of any descriptions or conclusions based on information supplied by other parties during site surveys, visits and interviews. Responsibility is disclaimed for any loss or damage, including but not limited to, any loss or damage suffered arising from the use of this report or suffered by any other person for any reason whatsoever. Recommendations to minimise the risk associated with roof safety are solely based on the assumption that all roofs have been built according to all current and relevant guidelines, acts, standards, codes of practice and regulations. No assessment or comment is made or implied in reference to the structural soundness of any structure or building. Structural soundness of all buildings should be referred to a qualified structural engineer prior to any installations. The certification procedure includes a visual inspection of the system to assess structural integrity and does not certify the system layout and or usage. If any component is damaged or subjected to a fall load, the system must not be used until checked and recertified by a competent person. It remains the building owner’s responsibility to ensure the system is used in accordance with current Occupational Health and Safety requirements and the system usage guidelines as noted by the system manufacturer. To ensure its contextual integrity, the report must be read in its entirety and should not be copied, distributed or referred to in part only. Information published in this report is confidential."));

                        }
                        wordDocument.MainDocumentPart?.Document.Save();

                        wordDocument.Save();
                        wordDocument.Dispose();
                        memoryStream.Close();
                        return File(memoryStream.ToArray(), "application/msword", ret.Title.Replace(" ","").Replace("'","")+".docx");
                        ///Task.FromResult(memoryStream.ToArray());
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        ;// wordDocument.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    memoryStream.Dispose();
                }
                // string Filepath = @"C:\temp\zoyeb.docx";
               /* using (var wordprocessingDocument = WordprocessingDocument.Create(Filepath, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = wordprocessingDocument.AddMainDocumentPart();
                    mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document();
                    Body body = mainPart.Document.AppendChild(new Body());
                    DocumentFormat.OpenXml.Wordprocessing.Paragraph para = body.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Paragraph());
                    DocumentFormat.OpenXml.Wordprocessing.Run run = para.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Run());
                    run.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Text("siddiq"));
                    wordprocessingDocument.MainDocumentPart.Document.Save();
                }


                string htmlContent = await Razor.Templating.Core.RazorTemplateEngine.RenderAsync("InspectionEquipments/Word", ret);
                
                string html = htmlContent;//"<strong>Hello</strong> World";
                using (MemoryStream mem = new MemoryStream())
                {
                    WordDocument doc = new WordDocument(mem);
                    
                    doc.Process(new HtmlParser(html));
                    doc.Save();

                    return File(mem.ToArray(), "application/msword", "sample.docx");
                }
               */
                            /*                using (MemoryStream generatedDocument = new MemoryStream())
                                            {
                                                using (WordprocessingDocument package =
                                                               WordprocessingDocument.Create(generatedDocument,
                                                               WordprocessingDocumentType.Document))
                                                {
                                                    MainDocumentPart mainPart = package.MainDocumentPart;
                                                    if (mainPart == null)
                                                    {
                                                        mainPart = package.AddMainDocumentPart();
                                                        new Document(new Body()).Save(mainPart);
                                                    }
                                                    HtmlConverter converter = new HtmlConverter(mainPart);
                                                    converter.ParseHtml(html);
                                                    mainPart.Document.Save();
                                                }
                                                return File(generatedDocument.ToArray(),
                                     "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                                     "filename.docx");
                                            }*/

                        }
                        if (hpw == "p")
            {
                string htmlContent = await Razor.Templating.Core.RazorTemplateEngine.RenderAsync("InspectionEquipments/EquipForInspectionsAll", ret);

                var workStream = new MemoryStream();

                using (var pdfWriter = new iText.Kernel.Pdf.PdfWriter(workStream))
                {
                    pdfWriter.SetCloseStream(false);
                    using (var document = iText.Html2pdf.HtmlConverter.ConvertToDocument(htmlContent, pdfWriter))
                    {
                    }
                }

                workStream.Position = 0;
                return new FileStreamResult(workStream, "application/pdf");

            }
            return View("Word", ret);
        }

        private static Paragraph ParaLeftSize(string fontsize,string content)
        {
            Paragraph para3 = new Paragraph();
            ParagraphProperties pp = new ParagraphProperties();
            pp.Justification = new Justification() { Val = JustificationValues.Right };
            para3.Append(pp);
            Run run2 = para3.AppendChild(new Run());

    


            RunProperties runProperties2 = run2.AppendChild(new RunProperties());

            Color color = new Color() { Val = "Red", ThemeColor = ThemeColorValues.Accent1, ThemeShade = "BF" };
            runProperties2.Append(color);

            FontSize fontSize24 = new FontSize() { Val = fontsize };
            runProperties2.Append(fontSize24);
            run2.AppendChild(new Text(content));
            return para3;
        }

        private static TableCell TableCellCentred(string txt)
        {
            TableCell tc2 = new TableCell();
            Paragraph parait = new Paragraph();
            ParagraphProperties pp = new ParagraphProperties();
            pp.Justification = new Justification() { Val = JustificationValues.Center };
            parait.Append(pp);
            Run runit = parait.AppendChild(new Run());
            RunProperties rpit = runit.AppendChild(new RunProperties());
            runit.AppendChild(new Text(txt));
            tc2.Append(parait);
            return tc2;
        }

        private static TableCell CenterAndColour(string txt, string clr)
        {
            TableCell tc2 = new TableCell();
            Paragraph pp = new Paragraph(new Run(new Text(txt)))
            {
                ParagraphProperties = new ParagraphProperties()
                {
                    Justification = new Justification()
                    {
                        Val = JustificationValues.Center
                    }
                }
            };
            tc2.Append(pp);
            tc2.Append(SetColor(clr));
            return tc2;
        }

        private static TableCell CellForeColor(string text, string colour)
        {
            TableCell tcInspDte = new TableCell();
            DocumentFormat.OpenXml.Wordprocessing.RunProperties rprp =
            new DocumentFormat.OpenXml.Wordprocessing.RunProperties();
            // Add the Color object for your run into the RunProperties
            rprp.Color = new Color() { Val = colour };
            
            Run rnrn = new Run(new Text(text));
            rnrn.RunProperties = rprp;
            tcInspDte.Append(new Paragraph(rnrn));
            return tcInspDte;
        }

        private static TableCell CellFont(string vall,int fontsize, bool bold,string colour="black",bool FullPage=false)
        {

            TableCell tc1 = new TableCell();
            var paragraph = new DocumentFormat.OpenXml.Wordprocessing.Paragraph();
            var runXX = new DocumentFormat.OpenXml.Wordprocessing.Run();
            var text = new DocumentFormat.OpenXml.Wordprocessing.Text(vall);

            RunProperties runProperties1 = new RunProperties();
            FontSize fs1 = new FontSize() { Val = fontsize.ToString() };
            runProperties1.Append(fs1);

            if (bold)
            {
                Bold bold3 = new Bold();


                bold3.Val = OnOffValue.FromBoolean(true);
                runProperties1.Append(bold3);
            }

            // DocumentFormat.OpenXml.Wordprocessing.RunProperties rprp =
            // new DocumentFormat.OpenXml.Wordprocessing.RunProperties();

            runProperties1.Color = new Color() { Val = colour };

           
            


            runXX.Append(runProperties1);
          
            if (FullPage)
            {
                for (int i = 0; i < 12; i++)
                {
                    runXX.Append(new Break());
                }
                
            }
            runXX.Append(text);
            if (FullPage)
            {
                for (int i = 0; i < 12; i++)
                {
                    runXX.Append(new Break());
                }

            }
            paragraph.Append(runXX);
            tc1.Append(paragraph);
            return tc1;
        }
   /*     private static TableCellProperties SetForeColor(string col)
        {
            var tcp = new TableCellProperties();
            var shading = new Shading()
            {
                Color = col,
               // Fill = col,
                Val = ShadingPatternValues.Clear
            };

            tcp.Append(shading);
            return tcp;
        }*/

        private static TableCellProperties SetColor(string col)
        {
            var tcp = new TableCellProperties();
            var shading = new Shading()
            {
                Color = col,
                Fill = col,
                Val = ShadingPatternValues.Clear
            };
            
            tcp.Append(shading);
            return tcp;
        }

        private static void AddHeader(Run run3, RunProperties runProperties3, string Text)
        {
            FontSize fontSizeBlue = new FontSize() { Val = "48" };
            runProperties3.Append(fontSizeBlue);
            Bold bold3 = new Bold();

            bold3.Val = OnOffValue.FromBoolean(true);
            Color color = new Color() { Val = "365F91", ThemeColor = ThemeColorValues.Accent1, ThemeShade = "BF" };
            runProperties3.Append(color);

            runProperties3.AppendChild(bold3);
           
            run3.AppendChild(new Text(Text));
            run3.AppendChild(new Break());
        }

        private static async Task InsertImage(Body Body, WordprocessingDocument wordDocument, MainDocumentPart mainPart, string imgurl)
        {    ImagePart imagePart = wordDocument.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
           
            using (var client = new HttpClient())
            {
                var bytes2 = await client.GetByteArrayAsync(imgurl);// ret.Photo);
                MemoryStream stream2 = new MemoryStream(bytes2);
                Image img = Image.FromStream(stream2);
                int wSize = img.Width;
                int hSize = img.Height;
                stream2.Close();
                stream2.Dispose();

             //   var bytes = await client.GetByteArrayAsync(imgurl);// ret.Photo);
                MemoryStream stream = new MemoryStream(bytes2);
                //MemoryStream stream2 = new MemoryStream();
                //stream.CopyTo(stream2);
                //Image img = Image.FromStream(stream2);
               


                imagePart.FeedData(stream);
                stream.Close();
                stream.Dispose();
                AddImageToBody(Body, mainPart.GetIdOfPart(imagePart), (decimal)hSize/(decimal)wSize);
            }
        }

        //private static async Task InsertImageFile(TableCell tc1, WordprocessingDocument wordDocument, MainDocumentPart mainPart, string imgurl)
        //{
        //    ImagePart imagePart = wordDocument.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
        //    //ImagePart imageP = document.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);

        //    //try
        //    using (var client = new HttpClient())
        //    {
        //                        var bytes = await client.GetByteArrayAsync(imgurl);


        //        MemoryStream stream2 = new MemoryStream(bytes);
        //        //MemoryStream stream2 = new MemoryStream();
        //        //using (FileStream fs = System.IO.File.OpenRead("C:\\RSS\\RoofSafetyWeb\\rsspnggreyvertical20.jpg"))
        //        //{
        //        //    fs.CopyTo(stream2);
        //        //}
        //        Image img = Image.FromStream(stream2);
        //        //img.Width = img.Width / 2;
        //        //img.Height = img.Height / 2;
                
        //        int wSize = img.Width;
        //        int hSize = img.Height;
        //        imagePart.FeedData(stream2);
        //        stream2.Close();
        //        stream2.Dispose();


        //        //MemoryStream stream = stream2;// new MemoryStream(bytes);
                
                
        //       // stream.Close();
        //       // stream.Dispose();
        //        AddImageToCell(tc1, mainPart.GetIdOfPart(imagePart), (decimal)hSize / (decimal)wSize,3);
        //    }
        //}

        private static System.Drawing.Image ResizeImage(System.Drawing.Image imgToResize,System.Drawing.Size size)
        {
            // Get the image current width
            int sourceWidth = imgToResize.Width;
            // Get the image current height
            int sourceHeight = imgToResize.Height;
            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            // Calculate width and height with new desired size
            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);
            nPercent = Math.Min(nPercentW, nPercentH);
            // New Width and Height
            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }


        public static byte[] convertImageToBytes(Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }


        private static async Task InsertImage(TableCell tc1,WordprocessingDocument wordDocument, MainDocumentPart mainPart, string imgurl, int wdth)
        {
            ImagePart imagePart = wordDocument.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
            using (var client = new HttpClient())
            {
                byte[] bytes = await client.GetByteArrayAsync(imgurl);
                MemoryStream stream2 = new MemoryStream(bytes);
                Image img = Image.FromStream(stream2);
                //get the image orientation or EXIF property of the image

               
                byte oi;
                PropertyItem[] propItems = img.PropertyItems;

                // Find the orientation property (ID 0x0112).
                PropertyItem orientationItem = propItems.FirstOrDefault(p => p.Id == 0x0112);

                if (orientationItem != null)
                {
                    // The orientation value is stored in the first byte.
                    oi= orientationItem.Value[0];
                }
                else
                {
                    // If the orientation property is not found, return 1 (normal orientation).
                    oi= 1;
                }
                if (oi==6)
                {
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);

                    using (var ms = new MemoryStream())
                    {
                        img.Save(ms, ImageFormat.Png); // You can change the format if needed
                        bytes= ms.ToArray();
                    }
                   // bytes = await client.GetByteArrayAsync(img);
                    stream2 = new MemoryStream(bytes);

                    // return img;
                }
                int wSize = img.Width;
                int hSize = img.Height;
                //rezieimage
                //                var imgsmall = ResizeImage(img,new System.Drawing.Size(wSize/4,hSize/4));
                //convert image to byte
                //              var bytessmall = convertImageToBytes(imgsmall);
                stream2.Close();
                stream2.Dispose();
                MemoryStream stream = new MemoryStream(bytes);// bytes);
                imagePart.FeedData(stream);
                stream.Close();
                stream.Dispose();
                AddImageToCell(tc1, mainPart.GetIdOfPart(imagePart), (decimal)hSize / (decimal)wSize,  wdth);
            }
        }

        public static int GetImageOrientation(string imagePath)
        {
            using (Image image = Image.FromFile(imagePath))
            {
                // Get the PropertyItems property from image.
                PropertyItem[] propItems = image.PropertyItems;

                // Find the orientation property (ID 0x0112).
                PropertyItem orientationItem = propItems.FirstOrDefault(p => p.Id == 0x0112);

                if (orientationItem != null)
                {
                    // The orientation value is stored in the first byte.
                    return orientationItem.Value[0];
                }
                else
                {
                    // If the orientation property is not found, return 1 (normal orientation).
                    return 1;
                }
            }
        }

        private static void AddImageToCell(TableCell cell, string relationshipId)
        {
            var element =
              new Drawing(
                new DW.Inline(
                  new DW.Extent() { Cx = 990000L, Cy = 792000L },
                  new DW.EffectExtent()
                  {
                      LeftEdge = 0L,
                      TopEdge = 0L,
                      RightEdge = 0L,
                      BottomEdge = 0L
                  },
                  new DW.DocProperties()
                  {
                      Id = (UInt32Value)1U,
                      Name = "Picture 1"
                  },
                  new DW.NonVisualGraphicFrameDrawingProperties(
                      new A.GraphicFrameLocks() { NoChangeAspect = true }),
                  new A.Graphic(
                    new A.GraphicData(
                      new PIC.Picture(
                        new PIC.NonVisualPictureProperties(
                          new PIC.NonVisualDrawingProperties()
                          {
                              Id = (UInt32Value)0U,
                              Name = "New Bitmap Image.jpg"
                          },
                          new PIC.NonVisualPictureDrawingProperties()),
                        new PIC.BlipFill(
                          new A.Blip(
                            new A.BlipExtensionList(
                              new A.BlipExtension()
                              {
                                  Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                              })
                           )
                          {
                              Embed = relationshipId,
                              CompressionState =
                              A.BlipCompressionValues.Print
                          },
                          new A.Stretch(
                            new A.FillRectangle())),
                          new PIC.ShapeProperties(
                            new A.Transform2D(
                              new A.Offset() { X = 0L, Y = 0L },
                              new A.Extents() { Cx = 990000L, Cy = 792000L }),
                            new A.PresetGeometry(
                              new A.AdjustValueList()
                            )
                            { Preset = A.ShapeTypeValues.Rectangle }))
                    )
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U
                });

            cell.Append(new Paragraph(new Run(element)));
        }
        private static void AddImageToCell(TableCell cell, string relationshipId, decimal htw,int wdth)
        {
            var element =
              new Drawing(
                new DW.Inline(
                  new DW.Extent() { Cx = wdth * 49500L, Cy = Convert.ToInt64(htw* wdth * 39600L) },
                  new DW.EffectExtent()
                  {
                      LeftEdge = 0L,
                      TopEdge = 0L,
                      RightEdge = 0L,
                      BottomEdge = 0L
                  },
                  new DW.DocProperties()
                  {
                      Id = (UInt32Value)1U,
                      Name = "Picture 1"
                  },
                  new DW.NonVisualGraphicFrameDrawingProperties(
                      new A.GraphicFrameLocks() { NoChangeAspect = true }),
                  new A.Graphic(
                    new A.GraphicData(
                      new PIC.Picture(
                        new PIC.NonVisualPictureProperties(
                          new PIC.NonVisualDrawingProperties()
                          {
                              Id = (UInt32Value)0U,
                              Name = "New Bitmap Image.jpg"
                          },
                          new PIC.NonVisualPictureDrawingProperties()),
                        new PIC.BlipFill(
                          new A.Blip(
                            new A.BlipExtensionList(
                              new A.BlipExtension()
                              {
                                  Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"
                              })
                           )
                          {
                              Embed = relationshipId,
                              CompressionState =
                              A.BlipCompressionValues.Print
                          },
                          new A.Stretch(
                            new A.FillRectangle())),
                          new PIC.ShapeProperties(
                            new A.Transform2D(
                              new A.Offset() { X = 0L, Y = 0L },
                              new A.Extents() { Cx = wdth * 49500L, Cy = Convert.ToInt64(htw * wdth * 39600L) }),
                            new A.PresetGeometry(
                              new A.AdjustValueList()
                            )
                            { Preset = A.ShapeTypeValues.Rectangle }))
                    )
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U
                });
            cell.Append(
             new DocumentFormat.OpenXml.Wordprocessing.Paragraph(new DocumentFormat.OpenXml.Wordprocessing.Run(element))
             {
                 ParagraphProperties = new DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties()
                 {
                     Justification = new DocumentFormat.OpenXml.Wordprocessing.Justification()
                     {
                         Val = DocumentFormat.OpenXml.Wordprocessing.JustificationValues.Center
                     }
                 }
             });
        }


        public async Task<IActionResult> Index(int InspectionID)
        {
            var xxx = _context.InspEquip.Where(i => i.InspectionID == InspectionID).Include(i => i.EquipType).Include(i=>i.Inspection);
            var yyy = await xxx.ToListAsync();
            return View(yyy);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.InspEquip == null)
            {
                return NotFound();
            }
            var inspectionEquipment = await _context.InspEquip
                .FirstOrDefaultAsync(m => m.id == id);
            if (inspectionEquipment == null)
            {
                return NotFound();
            }

            return View(inspectionEquipment);
        }

        public IActionResult Create(int? id)
        {
            ViewBag.EquipmentTypeID = (from xx in _context.EquipType select new SelectListItem() { Value = xx.id.ToString(), Text = xx.EquipTypeDesc }).ToList();
            InspEquip ret = new InspEquip();
            ret.InspectionID = id.Value;
           
         
            return View(ret);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( InspEquip inspEquip)
        {
            var xx = ModelState.Values.SelectMany(i => i.Errors);
            //if (ModelState.IsValid)
            {
                inspEquip.id = 0;
                _context.Add(inspEquip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EquipForInspections),new { id = inspEquip.InspectionID });
            }
            return View(inspEquip);
        }

        public async Task<IActionResult> Down(int? id, int Ordr)
        {
            var xxx = await _context.InspEquip.Where(i => i.InspectionID == id).ToListAsync();
            SetOrderIfNull(xxx);
            int? counter = OrdinalAsc(Ordr, xxx);
            if (counter != null)
            {
                if (counter + 1 < xxx.Count())
                {
                    var ss = xxx.OrderBy(i => i.Ordr).ToList()[counter.Value];
                    var tt = xxx.OrderBy(i => i.Ordr).ToList()[counter.Value + 1];

                    int ssid = ss.id;
                    int ssOrdr = ss.Ordr.Value;

                    int ttid = tt.id;
                    int ttOrdr = tt.Ordr.Value;

                    xxx.Where(i => i.id == ss.id).FirstOrDefault().Ordr = ttOrdr;
                    xxx.Where(i => i.id == tt.id).FirstOrDefault().Ordr = ssOrdr;
                    _context.SaveChanges();
                }
            }
            return RedirectToAction(nameof(EquipForInspections), new { id = id });
        }
        public async Task<IActionResult> Up(int? id,int Ordr)
        {
            var xxx = await _context.InspEquip.Where(i => i.InspectionID == id).ToListAsync();
            SetOrderIfNull(xxx);
            int? counter = Ordinal(Ordr, xxx);
            if (counter != null)
            {
                if (counter+1 < xxx.Count())
                {
                    var ss = xxx.OrderByDescending(i => i.Ordr).ToList()[counter.Value];
                    var tt = xxx.OrderByDescending(i => i.Ordr).ToList()[counter.Value + 1];

                    int ssid = ss.id;
                    int ssOrdr = ss.Ordr.Value;

                    int ttid = tt.id;
                    int ttOrdr = tt.Ordr.Value;

                    xxx.Where(i => i.id == ss.id).FirstOrDefault().Ordr = ttOrdr;
                    xxx.Where(i => i.id == tt.id).FirstOrDefault().Ordr = ssOrdr;
                    _context.SaveChanges();
                }
            }
            return RedirectToAction(nameof(EquipForInspections), new { id = id });
        }

        private static int? Ordinal(int? Ordr, List<InspEquip> xxx)
        {
            int counter = 0;
            foreach (var item in xxx.OrderByDescending(i => i.Ordr))
            {
                if (item.Ordr == Ordr)
                {
                   return counter;
                }
                counter++;
            }
            return null;
        }
        private static int? OrdinalAsc(int? Ordr, List<InspEquip> xxx)
        {
            int counter = 0;
            foreach (var item in xxx.OrderBy(i => i.Ordr))
            {
                if (item.Ordr == Ordr)
                {
                    return counter;
                }
                counter++;
            }
            return null;
        }
      
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.InspEquip == null)
            {
                return NotFound();
            }

            var inspectionEquipment = await _context.InspEquip.FindAsync(id);
        
            var Photos =  _context.InspPhoto.Where(i => i.InspEquipID == id).ToList();
            foreach (var item in Photos)
            {
                item.photoname = _imageservice.GetImageURL(item.photoname);
            }
            if (inspectionEquipment == null)
            {
                return NotFound();
            }

            ViewBag.EquipmentTypeID = (from xx in _context.EquipType select new SelectListItem() { Value = xx.id.ToString(), Text = xx.EquipTypeDesc }).ToList();
            inspectionEquipment.Photos = Photos;

            return View(inspectionEquipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InspEquip inspEquip)
        {
            if (id != inspEquip.id)
            {
                return NotFound();
            }

           // if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspEquip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InspectionEquipmentExists(inspEquip.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(EquipForInspections),new {id=inspEquip.InspectionID });
            }
            return View(inspEquip);
        }

        // GET: InspectionEquipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.InspEquip == null)
            {
                return NotFound();
            }

            var inspectionEquipment = await _context.InspEquip
                .FirstOrDefaultAsync(m => m.id == id);
            if (inspectionEquipment == null)
            {
                return NotFound();
            }

            return View(inspectionEquipment);
        }

        // POST: InspectionEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.InspEquip == null)
            {
                return Problem("Entity set 'dbcontext.InspectionEquipment'  is null.");
            }
            var inspectionEquipment = await _context.InspEquip.FindAsync(id);
            int inspid = inspectionEquipment.InspectionID;
            if (inspectionEquipment != null)
            {
                _context.InspEquip.Remove(inspectionEquipment);
            }            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EquipForInspectionsAll), new { id = inspid  });
        }

        private bool InspectionEquipmentExists(int id)
        {
          return _context.InspEquip.Any(e => e.id == id);
        }
    }
}
