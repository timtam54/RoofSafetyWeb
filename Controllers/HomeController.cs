using Microsoft.AspNetCore.Mvc;
using RoofSafety.Models;
using RoofSafety.Services.Abstract;
using System.Diagnostics;
using RoofSafety.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RoofSafety.Controllers
{
    [Authorize]
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

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            //return RedirectToPage("/Home/Login");
            return RedirectToAction("Login", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel ret)
        {
            try
            {
                var log = _context.Employee.Where(i => i.Email == ret.Email && i.Password == ret.password).FirstOrDefault();
                if (log == null)
                {
                    // return RedirectToPage("/AccessDenied");
                    return RedirectToAction("AccessDenied");// RedirectToPage("/Home/AccessDenied");
                }
                try
                {
                    var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name,ret.Email),
            new Claim("FullName", ret.Email),
            new Claim(ClaimTypes.Role, "Administrator"),
        };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        //IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                    //  _logger.LogInformation("User {Email} logged in at {Time}.",
                    //    user.Email, DateTime.UtcNow);
                }

                catch (Exception ex)
                {
                    Console.Write( ex.Message);
                }
                return  RedirectToAction("Index", "Inspections") ;
            }
            catch (Exception ex)
            {
                ret.Error = "Error:" + ex.Message;
                //return View(ret);
            }
            return View(ret);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            LoginModel ret = new LoginModel();
            return View(ret);
        }

            [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePicture(ImageModel imageModel)
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
                    ip.Description = imageModel.description;
                    ip.photoname = filename;
                    ip.SourceTable = "I";
                    _context.Add<InspPhoto>(ip);
                    
                    await _context.SaveChangesAsync();
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