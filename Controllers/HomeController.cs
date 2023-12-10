using Microsoft.AspNetCore.Mvc;
using RoofSafety.Models;
using RoofSafety.Services.Abstract;
using System.Diagnostics;
using RoofSafety.Data;

namespace RoofSafety.Controllers
{
    public class HomeController : Controller
    {

        public static string WidthImage = "410px";
        public static string WidthRow = "470px";//was 440px
        private readonly ILogger<HomeController> _logger;
        private readonly IImageService _imageservice;

        private readonly dbcontext _context;


            public HomeController(ILogger<HomeController> logger, IImageService imageservice, dbcontext context)
        {
            _context = context;

            _logger = logger;
            _imageservice = imageservice;
        }
        public IActionResult Delete(int id,int ieid)
        {
            InspPhoto ip = _context.InspPhoto.Find(id);
            if (ip != null)
            {
                _context.Remove(ip);
                _context.SaveChanges();
            }
            return RedirectToAction("Edit", "InspectionEquipments", new { id = ieid });
        }
        public IActionResult Index(int id)
        {
            ImageModel im = new ImageModel();
            im.InspEquipID = id;
            return View(im);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SavePicture(ImageModel imageModel)
        {
            try
            {
                if (imageModel.File == null || imageModel.File.FileName == null)
                    return View("Index");
               string? filename=  _imageservice.UploadImageToAzure(imageModel.File,false);
                //_imageservice.UploadImageToAzure(imageModel.File, false);
                InspPhoto ip = new InspPhoto();
                if (filename != null)
                {
                    ip.InspEquipID = imageModel.InspEquipID;
                    ip.description = imageModel.description;
                    ip.photoname = filename;
                    _context.Add(ip);
                    _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Index");
            }
            return RedirectToAction("Edit","InspectionEquipments",new {id=imageModel.InspEquipID });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}