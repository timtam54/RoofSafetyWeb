using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Azure;

using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis.RulesetToEditorconfig;
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
using HtmlToOpenXml;
using MariGold.OpenXHTML;
using System.Text;
using iText.Kernel.Geom;
using iText.Svg.Renderers.Path.Impl;
using DocumentFormat.OpenXml.Spreadsheet;
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
            
//            var yyy = await xxx.ToListAsync();
            SetOrderIfNull(yyy);
            ViewBag.InspectionID = id;
            DescParID xx = (from ie in _context.Inspection join bd in _context.Building on ie.BuildingID equals bd.id where ie.id == id select new DescParID { Desc = (bd.BuildingName + " @ " + ie.InspectionDate.ToString("dd-MM-yyyy")), ID = ie.BuildingID }).FirstOrDefault();
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
        public async Task<ActionResult> EquipForInspectionsAll(int id, string hpw)//0,1,2
        {
            InspectionRpt ret = new InspectionRpt();
            ret.InspItems = (from ins in _context.InspEquip join emp in _context.EquipType on ins.EquipTypeID equals emp.id where ins.InspectionID == id orderby ins.Ordr select emp.EquipTypeDesc).ToList();
          
            ret.Inspector = (from ins in _context.Inspection join emp in _context.Employee on ins.InspectorID equals emp.id where ins.id == id select emp.Given + " " + emp.Surname).FirstOrDefault();
            var insp = _context.Inspection.Where(i => i.id == id).FirstOrDefault();
            ret.InspDate = insp.InspectionDate;
            ret.Areas = insp.Areas;
            ret.id= insp.id;
            ret.Instrument = insp.TestingInstruments;
            ret.Tests = "Test";
            
            ret.Title = (from bd in _context.Building where bd.id == insp.BuildingID select bd.BuildingName).FirstOrDefault();
            ret.Items = (from ie in _context.InspEquip join et in _context.EquipType on ie.EquipTypeID equals et.id where ie.InspectionID == id select new InspEquipTest { Ordr=(ie.Ordr==null)?ie.id:ie.Ordr, ItemNo= ie.SerialNo ,Qty=(ie.Qty==null)?1:ie.Qty.Value, RequiredControls=ie.RequiredControls, Pass=true, Manufacturer =ie.Manufacturer, SNSuffix=ie.SNSuffix, SerialNo=ie.SerialNo, Rating=ie.Rating, Installer=ie.Installer ,EquipName = et.EquipTypeDesc,  Notes = ie.Notes, Location = ie.Location, id = ie.id, EquipType = et, ETID=et.id }).OrderBy(i=>i.Ordr).ToList();//.Include(i => i.EquipType).Include(i => i.Inspection).Include(i => i.EquipType)=efe
    
            int counter = 1;
            foreach (var ite in ret.Items.OrderBy(i => i.Ordr))
            {
                ite.ItemNo = counter.ToString();
                counter++;
            }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               // ret.Versions = (from vs in _context.Version join emp in _context.Employee on vs.AuthorID equals emp.id where vs.InspectionID == Convert.ToInt32(id) select new VersionRpt { id = vs.id, Information = vs.Information, Author = emp.Given + " " + emp.Surname, VersionNo = vs.VersionNo, VersionType = (vs.VersionType == "FD") ? "First Draft" : "Internal Review" }).ToList();
            ret.Versions = (from ins in _context.Inspection join emp in _context.Employee on ins.InspectorID equals emp.id where ins.BuildingID == Convert.ToInt32(insp.BuildingID) /*&& ins.id!=insp.id */select new VersionRpt { id = ins.id, Information = ins.InspectionDate.ToString("dd-MM-yyyy"), Author = emp.Given + " " + emp.Surname, VersionNo = ins.id, VersionType = (ins.Status == null) ? "New" : ((ins.Status == "A") ? "Active" : "Complete") }).ToList();
            //  ret.Versions = (from vs in _context.Version join emp in _context.Employee on vs.AuthorID equals emp.id where vs.InspectionID == Convert.ToInt32(id) select new VersionRpt { id = vs.id, Information = vs.Information, Author = emp.Given + " " + emp.Surname, VersionNo = vs.VersionNo, VersionType = (vs.VersionType == "FD") ? "First Draft" : "Internal Review" }).ToList();
           
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
                        mainPart.Document = new Document();
                        {
                            Body bodycover = mainPart.Document.AppendChild(new Body());
                            Shape sp = new Shape();
                            string StyleString = "position:absolute;margin-left:0;margin-top:63pt;width:540pt;height:6pt;z-index:-251658240;visibility:visible;mso-wrap-style:square;mso-width-percent:0;mso-height-percent:0;mso-wrap-distance-left:0;mso-wrap-distance-top:0;mso-wrap-distance-right:0;mso-wrap-distance-bottom:0;mso-position-horizontal:absolute;mso-position-horizontal-relative:page;mso-position-vertical:absolute;mso-position-vertical-relative:page;mso-width-percent:0;mso-height-percent:0;mso-width-relative:page;mso-height-relative:page;v-text-anchor:top";
                            //, CoordinateSize = "6858000,76200", OptionalString = "_x0000_s1026", AllowInCell = false,
                            ////FillColor = "#00afef", Stroked = false, EdgePath = "m,l,76200r6858000,l6858000,,,e", 
                            /////EncodedPackage = "UEsDBBQABgAIAAAAIQC2gziS/gAAAOEBAAATAAAAW0NvbnRlbnRfVHlwZXNdLnhtbJSRQU7DMBBF\n90jcwfIWJU67QAgl6YK0S0CoHGBkTxKLZGx5TGhvj5O2G0SRWNoz/78nu9wcxkFMGNg6quQqL6RA\n0s5Y6ir5vt9lD1JwBDIwOMJKHpHlpr69KfdHjyxSmriSfYz+USnWPY7AufNIadK6MEJMx9ApD/oD\nOlTrorhX2lFEilmcO2RdNtjC5xDF9pCuTyYBB5bi6bQ4syoJ3g9WQ0ymaiLzg5KdCXlKLjvcW893\nSUOqXwnz5DrgnHtJTxOsQfEKIT7DmDSUCaxw7Rqn8787ZsmRM9e2VmPeBN4uqYvTtW7jvijg9N/y\nJsXecLq0q+WD6m8AAAD//wMAUEsDBBQABgAIAAAAIQA4/SH/1gAAAJQBAAALAAAAX3JlbHMvLnJl\nbHOkkMFqwzAMhu+DvYPRfXGawxijTi+j0GvpHsDYimMaW0Yy2fr2M4PBMnrbUb/Q94l/f/hMi1qR\nJVI2sOt6UJgd+ZiDgffL8ekFlFSbvV0oo4EbChzGx4f9GRdb25HMsYhqlCwG5lrLq9biZkxWOiqY\n22YiTra2kYMu1l1tQD30/bPm3wwYN0x18gb45AdQl1tp5j/sFB2T0FQ7R0nTNEV3j6o9feQzro1i\nOWA14Fm+Q8a1a8+Bvu/d/dMb2JY5uiPbhG/ktn4cqGU/er3pcvwCAAD//wMAUEsDBBQABgAIAAAA\nIQC6yjCfpAMAACgKAAAOAAAAZHJzL2Uyb0RvYy54bWysVlFvmzAQfp+0/2DxOCkFUiAhajqt7TJN\n6rZKYz/AARPQwGa2E9JN+++7M5Cadsm6aS9g44/z3ff57nzxel9XZMekKgVfOv6Z5xDGU5GVfLN0\nviSrydwhSlOe0UpwtnTumXJeX758cdE2CzYVhagyJgkY4WrRNkun0LpZuK5KC1ZTdSYaxmExF7Km\nGqZy42aStmC9rtyp50VuK2TWSJEypeDrTbfoXBr7ec5S/SnPFdOkWjrgmzZPaZ5rfLqXF3SxkbQp\nyrR3g/6DFzUtOWx6MHVDNSVbWT4xVZepFErk+iwVtSvyvEyZiQGi8b1H0XwuaMNMLECOag40qf9n\nNv24u5OkzJbO1CGc1iDRSjKGhC+I2Z/4SFLbqAVgPzd3EsNUza1IvypYcEcrOFGAIev2g8jAGN1q\nYYjZ57LGPyFksjf83x/4Z3tNUvgYzcO554FMKazNItAXt3bpYvg53Sr9jgljiO5ule7ky2BkyM/6\nEBKwkdcVKPnKJR5pyWC5xw8wfwQryGFPEPJgC4h5jq1zC2bskCP2AgvY+3Xcw9ACn7QaWcA/Wp1Z\nYO+Yn5C5z4k7HsGOxOyPBYlAZY9EYXge9Rl4YNsfa3IKORbmFNLWBvY9sbstzmmkrcypI2YLc5Rr\n31bkj/L5tjRPTgXky2bICFoMSZLueZ8lMCIUy3MCqmDaNEJhSmLSQN4lJt/BCOBw1YL7IzjQj/Dz\nPkefwqcjODCL8PAo/HwEB9oQPjsKD0ZwYAThsQ3vYuijltAKsAkkPggHbSDxYQdoBAlSD60gAVJN\ndWioRtIMMTAkrVWXiqEs4WotdiwRBqcflTTY+WG14k9Rh0IDyGF9eDfG2lCxMKy/Qw9Fc7A3vDu7\nnchjDDiBUZtSewgf2bPKrRJVma3KqsKAldysrytJdhTbqvdm9XbVEz+CVeYAcYG/AbcmVvwd6n3P\nMFZ+0yZ/xP408K6m8WQVzWeTYBWEk3jmzSeeH1/FkRfEwc3qJ0ruB4uizDLGb0vOhpbtB89rif3l\noWu2pmmjvnE4Dc1pGnn/KEgsGr8LUootz8zJKRjN3vZjTcuqG7tjjw0NEPbwNkSYJop9s2u0a5Hd\nQw+VAs4p6AXXKxgUQn53SAtXlaWjvm2pZA6p3nO4C8R+EABMm0kQzqYwkfbK2l6hPAVTS0c7UAVw\neK1hBr9sG1luCtipS3Mu3kDvzkvssca/zqt+AtcRE0F/dcL7jj03qIcL3uUvAAAA//8DAFBLAwQU\nAAYACAAAACEAu18l7d0AAAAJAQAADwAAAGRycy9kb3ducmV2LnhtbEyPzU7DMBCE70i8g7VI3KhN\nkaoQ4lSlEkKIip/SB3DjbRI1XhvbbcPbsz3BbXZnNftNNR/dII4YU+9Jw+1EgUBqvO2p1bD5erop\nQKRsyJrBE2r4wQTz+vKiMqX1J/rE4zq3gkMolUZDl3MopUxNh86kiQ9I7O18dCbzGFtpozlxuBvk\nVKmZdKYn/tCZgMsOm/364DR8rz7sZnx/Xvn78Lh8Ca+yjW87ra+vxsUDiIxj/juGMz6jQ81MW38g\nm8SggYtk3k5nLM62KhSrLau7QoGsK/m/Qf0LAAD//wMAUEsBAi0AFAAGAAgAAAAhALaDOJL+AAAA\n4QEAABMAAAAAAAAAAAAAAAAAAAAAAFtDb250ZW50X1R5cGVzXS54bWxQSwECLQAUAAYACAAAACEA\nOP0h/9YAAACUAQAACwAAAAAAAAAAAAAAAAAvAQAAX3JlbHMvLnJlbHNQSwECLQAUAAYACAAAACEA\nusown6QDAAAoCgAADgAAAAAAAAAAAAAAAAAuAgAAZHJzL2Uyb0RvYy54bWxQSwECLQAUAAYACAAA\nACEAu18l7d0AAAAJAQAADwAAAAAAAAAAAAAAAAD+BQAAZHJzL2Rvd25yZXYueG1sUEsFBgAAAAAE\nAAQA8wAAAAgHAAAAAA==\n" ;
                            //https://stackoverflow.com/questions/42898907/how-to-add-a-shape-in-word-document-using-openxml-c
                            sp.Style = StyleString;
                            sp.FillColor = "Red";
                            bodycover.AppendChild(sp);

/*                            Table table = new Table();
                            FontSize fontSizep42 = new FontSize() { Val = "42" };


                            TableRow tr = new TableRow();

                            TableCell t3 = CellFont("Roof Safety Solutions", 30, false, "white",true);
                            t3.Append(SetColor("Navy"));
                            t3.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "50" }));

                            tr.Append(t3);
                            TableCell t2;// 
                            if (ret.Photo == null)
                            {
                                t2 = CellFont("Height and Safety Audit Report for " + xx.Desc, 26, false, "black", true);
                                t2.Append(SetColor("RoyalBlue"));
                            }
                            else
                            {
                                t2 = new TableCell();
                                string imgurl = ret.Photo.Replace("%0D%0A", "").TrimEnd();
                                await InsertImage(t2, wordDocument, mainPart, imgurl);
                            }
                            t3.Append(new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "30" }));

                            tr.Append(t2);
                            table.Append(tr);
                            bodycover.AppendChild(table);*/

                            Paragraph paracover = bodycover.AppendChild(new Paragraph());
                            Run runcover = paracover.AppendChild(new Run());
                            Break pgbrkcover = new Break();
                            pgbrkcover.Type = BreakValues.Page;
                            runcover.AppendChild(pgbrkcover);
                        }
                        Body body = mainPart.Document.AppendChild(new Body());
                        {
                            
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
                            run.AppendChild(new Text("Height and Safety Audit Report for " + xx.Desc));

                            run.AppendChild(new Break());
                        }
                        if (ret.Photo != null)
                        {
                            string imgurl = ret.Photo.Replace("%0D%0A", "").TrimEnd();

                            await InsertImage(body,wordDocument, mainPart, imgurl);
                        }
                        Paragraph para2 = body.AppendChild(new Paragraph());
                        {
                            
                            Run run2 = para2.AppendChild(new Run());
                            RunProperties runProperties2 = run2.AppendChild(new RunProperties());
                            FontSize fontSize24 = new FontSize() { Val = "24" };
                            runProperties2.Append(fontSize24);
                            run2.AppendChild(new Text("Roof Safety Solutions Pty Ltd"));
                            run2.AppendChild(new Break());
                            run2.AppendChild(new Text("38 Radius Loop, Bayswater WA 6053"));
                            run2.AppendChild(new Break());
                            run2.AppendChild(new Text("p: 08 9477 4884"));
                            run2.AppendChild(new Break());
                            run2.AppendChild(new Text("f: 08 9277 3009"));
                            run2.AppendChild(new Break());
                            run2.AppendChild(new Text("e: admin@roofsafetysolutions.com.au"));
                            run2.AppendChild(new Break());
                            Break pgbrk = new Break();
                            pgbrk.Type = BreakValues.Page;
                            run2.AppendChild(pgbrk);
                        }
                        SectionProperties sectionProps = new SectionProperties();
                        PageMargin pageMargin = new PageMargin() { Top = 1008, Right = (UInt32Value)1008U, Bottom = 1008, Left = (UInt32Value)1008U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
                        sectionProps.Append(pageMargin);
                        mainPart.Document.Body?.Append(sectionProps);
                        Run run3 = para2.AppendChild(new Run());
                        RunProperties runProperties3 = run3.AppendChild(new RunProperties());

                        AddHeader(run3, runProperties3, "Contents");
                        {

                            var newHeaderPart = mainPart.AddNewPart<HeaderPart>();
                            var imgPart = newHeaderPart.AddImagePart(ImagePartType.Png);
                            var imagePartID = newHeaderPart.GetIdOfPart(imgPart);
                            using (var client = new HttpClient())
                            {
                                var bytes = await client.GetByteArrayAsync("https://www.roofsafetysolutions.com.au/wp-content/uploads/2020/06/roof_safety_logo.png");// ret.Photo);
                                MemoryStream stream = new MemoryStream(bytes);
                                imgPart.FeedData(stream);
                                stream.Close();
                                stream.Dispose();
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
                            tr.Append(CellFont("Inspection Date", 32, true));

                            body4.AppendChild(table);
                            foreach (var item in ret.Versions)
                            {
                                TableRow trx = new TableRow();
                                table.Append(trx);
                                trx.Append(CellFont(item.VersionNo?.ToString(), 28, false));
                                trx.Append(CellFont((item.VersionType), 28, false));
                                trx.Append(CellFont(item.Author, 28, false));
                                trx.Append(CellFont((item.Information), 28, false));
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
                        FontSize fontSizepeh = new FontSize() { Val = "30" };
                        runPropertieseh.Append(fontSizepeh);
                        //Bold bold4 = new Bold();

                        // bold4.Val = OnOffValue.FromBoolean(true);
                        // runp2.AppendChild(bold4);
                        runeh.AppendChild(new Text("Roof Safety Solutions were contracted to perform the routine audit of " + ret.Title + "."));
                        runeh.AppendChild(new Break()); runeh.AppendChild(new Break());
                        runeh.AppendChild(new Text("The following existing height safety equipment is installed on site:"));
                        runeh.AppendChild(new Break());
                        foreach (var itmdesc in ret.Items.OrderBy(i=>i.Ordr))
                        {
                            runeh.AppendChild(new Text("- " + itmdesc.EquipName + " " + itmdesc.Manufacturer + " " + itmdesc.SerialNo )); runeh.AppendChild(new Break());
                        }
                        runeh.AppendChild(new Break());
                        runeh.AppendChild(new Break());
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
                        runeh.AppendChild(new Text("- Code of Practice – Prevention of Falls at Workplaces 2020 (WA)."));
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

                                TableCell tcInspDteLbl = CellFont("Testing Instrument:", 30, true);
                                trInsp.Append(tcInspDteLbl);


                                TableCell tcInspDte = CellFont(ret.Instrument, 30, false);
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
                            runintro1.AppendChild(new Text("• Code of Practice – Prevention of Falls at Workplaces 2020 (WA)."));                          
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
                                        new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                        new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size =3},
                                        new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                        new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                        new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 },
                                        new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 3 }
                                    )
                                );
                                // Append the TableProperties object to the empty table.
                                table.AppendChild<TableProperties>(tblProp);
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();
                                    tc1.Append(new Paragraph(new Run(new Text("      "))));
                                    tr.Append(tc1);
                     
                                    TableCell tc2 = new TableCell();
                                    tc2.Append(new Paragraph(new Run(new Text("  not requiring First Aid"))));
                                    tc2.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc2);
                                    TableCell tc3 = new TableCell();
                                    tc3.Append(new Paragraph(new Run(new Text("First Aid Treatment Case."))));
                                    tc3.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc3);
                                    TableCell tc4 = new TableCell();
                                    tc4.Append(new Paragraph(new Run(new Text("Serious injury, medical treatment, or hospitalisation"))));
                                    tc4.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc4);
                                    TableCell tc5 = new TableCell();
                                    tc5.Append(new Paragraph(new Run(new Text("Multiple serious injuries causing hospitalisation."))));
                                    tc5.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc5);
                                    TableCell tc6 = new TableCell();
                                    tc6.Append(new Paragraph(new Run(new Text("Death or multiple life-threatening injuries."))));
                                    tc6.Append(SetColor("#D3D3D3"));
                                    tr.Append(tc6);
                                   
                                }
                                {
                                    TableRow tr = new TableRow();
                                    table.Append(tr);
                                    TableCell tc1 = new TableCell();


                                    tc1.Append(new Paragraph(new Run(new Text("Likelihood"))));
                                    tr.Append(tc1);
                                    TableCell tc2 = new TableCell();
                                    tc2.Append(new Paragraph(new Run(new Text("Negligible"))));
                                    tr.Append(tc2);
                                    TableCell tc3 = new TableCell();
                                    tc3.Append(new Paragraph(new Run(new Text("Minor"))));
                                    tr.Append(tc3);
                                    TableCell tc4 = new TableCell();
                                    tc4.Append(new Paragraph(new Run(new Text("Moderate"))));
                                    tr.Append(tc4);
                                    TableCell tc5 = new TableCell();
                                    tc5.Append(new Paragraph(new Run(new Text("Major"))));
                                    tr.Append(tc5);
                                    TableCell tc6 = new TableCell();
                                    tc6.Append(new Paragraph(new Run(new Text("Catastrophic"))));
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

                                    TableCell tc5 = CenterAndColour("4", "##0096FF");

                                    tr.Append(tc5);
                                    TableCell tc6 = CenterAndColour("5", "##0096FF");
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
                                        TableCell tcInspDteLbl = new TableCell();

                                        Bold boldinsp = new Bold();

                                        boldinsp.Val = OnOffValue.FromBoolean(true);

                                        Run runInsp = new Run(new Text("Item:"));
                                        runInsp.AppendChild(boldinsp);
                                        tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                        trInsp.Append(tcInspDteLbl);


                                        TableCell tcInspDte = new TableCell();
                                        tcInspDte.Append(new Paragraph(new Run(new Text("1."+itemno.ToString()))));
                                        tcInspDte.Append(new GridSpan { Val =2 });
                                        trInsp.Append(tcInspDte);
                                        tableInsp.Append(trInsp);
                                    }
                                     {
                                        TableRow trInsp = new TableRow();
                                        TableCell tcInspDteLbl = new TableCell();

                                        Bold boldinsp = new Bold();

                                        boldinsp.Val = OnOffValue.FromBoolean(true);

                                        {
                                            Paragraph paraxx = tcInspDteLbl.AppendChild(new Paragraph());

                                            ParagraphProperties paraPropsx = new ParagraphProperties();
                                            SpacingBetweenLines spacingx = new SpacingBetweenLines() { Before = "15", After = "0" };
                                            paraPropsx.SpacingBetweenLines = spacingx;
                                            paraxx.ParagraphProperties = paraPropsx;

                                            Run runInsp = paraxx.AppendChild(new Run());
                                            runInsp.AppendChild(new Text("Risk:"));
                                            runInsp.AppendChild(boldinsp);
                                        }
                                       // Run runInsp = new Run(new Text("Risk:"));
                                       // 
                                        //tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                        trInsp.Append(tcInspDteLbl);



                                        TableCell tcInspDte = new TableCell();
                                        {
                                            //TableCellProperties tcp = new TableCellProperties();
                                            //TableCellVerticalAlignment tcVA = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
                                            //tcp.Append(tcVA);
                                            //tcInspDte.Append(tcp);
                                        }

                                        TableCellProperties tcp = new TableCellProperties();

                                        

                                        //set the alignment to "Center"
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

                                        ParagraphProperties paraProps = new ParagraphProperties();
                                        SpacingBetweenLines spacing = new SpacingBetweenLines() {Before="15", After = "0" };
                                        paraProps.SpacingBetweenLines = spacing;
                                        parax.ParagraphProperties = paraProps;

                                        Run runx = parax.AppendChild(new Run());
                                        runx.AppendChild(new Text(item.Risk));



                                     //   tcInspDte.Append(new Paragraph(new Run(new Text(item.Risk))));
                                        trInsp.Append(tcInspDte);
                                        tableInsp.Append(trInsp);
                                    }
                                      {
                                          TableRow trInsp = new TableRow();
                                          TableCell tcInspDteLbl = new TableCell();

                                          Bold boldinsp = new Bold();

                                          boldinsp.Val = OnOffValue.FromBoolean(true);

                                          Run runInsp = new Run(new Text("Description:"));
                                          runInsp.AppendChild(boldinsp);
                                          tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                          trInsp.Append(tcInspDteLbl);


                                          TableCell tcInspDte = new TableCell();

                                          tcInspDte.Append(new Paragraph(new Run(new Text(item.EquipName))));
                                        tcInspDte.Append(new GridSpan { Val = 2 });
                                        trInsp.Append(tcInspDte);
                                          tableInsp.Append(trInsp);
                                      }
                                      {
                                          TableRow trInsp = new TableRow();
                                          TableCell tcInspDteLbl = new TableCell();

                                          Bold boldinsp = new Bold();

                                          boldinsp.Val = OnOffValue.FromBoolean(true);

                                          Run runInsp = new Run(new Text("Location:"));
                                          runInsp.AppendChild(boldinsp);
                                          tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                          trInsp.Append(tcInspDteLbl);


                                          TableCell tcInspDte = new TableCell();

                                          tcInspDte.Append(new Paragraph(new Run(new Text(item.Location))));
                                        tcInspDte.Append(new GridSpan { Val = 2 });
                                        trInsp.Append(tcInspDte);
                                          tableInsp.Append(trInsp);
                                      }
                                      {
                                          TableRow trInsp = new TableRow();
                                          TableCell tcInspDteLbl = new TableCell();

                                          Bold boldinsp = new Bold();

                                          boldinsp.Val = OnOffValue.FromBoolean(true);

                                          Run runInsp = new Run(new Text("Result:"));
                                          runInsp.AppendChild(boldinsp);
                                          tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
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
                                          TableCell tcInspDteLbl = new TableCell();

                                          Bold boldinsp = new Bold();

                                          boldinsp.Val = OnOffValue.FromBoolean(true);
                                     

                                          Run runInsp = new Run(new Text((ii==0)?"Explanation:":""));
                                          runInsp.AppendChild(boldinsp);
                                          tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
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
                                          TableCell tcInspDteLbl = new TableCell();

                                          Bold boldinsp = new Bold();

                                          boldinsp.Val = OnOffValue.FromBoolean(true);

                                          Run runInsp = new Run(new Text("Compliant:"));
                                          runInsp.AppendChild(boldinsp);
                                          tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                          trInsp.Append(tcInspDteLbl);


                                          TableCell tcInspDte = new TableCell();
                                        tcInspDte.Append(new GridSpan { Val = 2 });
                                        tcInspDte.Append(new Paragraph(new Run(new Text(item.EquipType.CompliantInfo))));
                                          trInsp.Append(tcInspDte);
                                          tableInsp.Append(trInsp);
                                      }
                                      else
                                    {
                                        {
                                              TableRow trInsp = new TableRow();
                                              TableCell tcInspDteLbl = new TableCell();

                                              Bold boldinsp = new Bold();

                                              boldinsp.Val = OnOffValue.FromBoolean(true);

                                              Run runInsp = new Run(new Text("Hazards:"));
                                              runInsp.AppendChild(boldinsp);
                                              tcInspDteLbl.Append(new Paragraph(runInsp));//.AppendChild(boldinsp)
                                              trInsp.Append(tcInspDteLbl);


                                              TableCell tcInspDte = new TableCell();
                                              string ss="";
                                              if(item.Hazards != null)
                                              {
                                                  foreach (var hz in item.Hazards)
                                                  {
                                                      ss = ss + @hz;
                                                  }
                                              }
                                              tcInspDte.Append(new Paragraph(new Run(new Text(ss))));
                                            tcInspDte.Append(new GridSpan { Val = 2 });
                                            trInsp.Append(tcInspDte);
                                              tableInsp.Append(trInsp);
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

                                              TableCell tcInspDteLbl = CellForeColor("Required Controls:", "navy");
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

                                          TableCell tclbl = new TableCell();

                                          tclbl.Append(new Paragraph(new Run(new Text("Photo"))));
                                          trInsp.Append(tclbl);

                                          TableCell tcInspDte = new TableCell();
                                        tcInspDte.Append(new GridSpan { Val = 2 });
                                        trInsp.Append(tcInspDte);
                                              tableInsp.Append(trInsp);

                                          string imgurlx = ph.photoname.Replace("%0D%0A", "").TrimEnd();
                                              //MainDocumentPart mainPart2 = wordDocument.AddMainDocumentPart();
                                              await InsertImage(tcInspDte,wordDocument, mainPart, imgurlx);
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
                                    TableCell t7 = CellFont(item.SerialNo, 24, false, "Black");
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
                        return File(memoryStream.ToArray(), "application/msword", "RSSInspection_"+id.ToString()+".docx");
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


            runProperties3.AppendChild(bold3);
            runProperties3.Append(color);
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



        private static async Task InsertImage(TableCell tc1,WordprocessingDocument wordDocument, MainDocumentPart mainPart, string imgurl)
        {
            ImagePart imagePart = wordDocument.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);
            //ImagePart imageP = document.MainDocumentPart.AddImagePart(ImagePartType.Jpeg);

            //try
            using (var client = new HttpClient())
            {
                var bytes = await client.GetByteArrayAsync(imgurl);// ret.Photo);

                
                MemoryStream stream2 = new MemoryStream(bytes);
                Image img = Image.FromStream(stream2);
                int wSize = img.Width;
                int hSize = img.Height;
                stream2.Close();
                stream2.Dispose();


                MemoryStream stream = new MemoryStream(bytes);
                //int iWidth = 0;
                //int iHeight = 0;
                //using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(stream))
                //{
                //    iWidth = bmp.Width;
                //    iHeight = bmp.Height;
                //}
                //int cx = (int)Math.Round((decimal)iWidth * 9525);
                //int cy = (int)Math.Round((decimal)iHeight * 9525);
                imagePart.FeedData(stream);
                stream.Close();
                stream.Dispose();
                AddImageToCell(tc1, mainPart.GetIdOfPart(imagePart),(decimal)hSize/(decimal)wSize);
            }
        }
        private static void AddImageToCell(TableCell cell, string relationshipId, decimal htw)
        {
            var element =
              new Drawing(
                new DW.Inline(
                  new DW.Extent() { Cx = 5*990000L, Cy = Convert.ToInt64(htw* 5 * 792000L) },
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
                              new A.Extents() { Cx = 5 * 990000L, Cy = Convert.ToInt64(htw * 5 * 792000L) }),
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

         //   cell.Append(new Paragraph(new Run(element)));
            cell.Append(//(new Paragraph(new Run(element)));

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
