using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoofSafety.Models;
using RoofSafety.Data;
using RoofSafety.Services.Concrete;
using RoofSafety.Services.Abstract;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RSSAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly dbcontext _context;
       private readonly IImageService _imageservice;
        public ImageController(dbcontext context, IImageService imageservice)
        {
            _context = context;
            _imageservice = imageservice;
        }

       
        // POST: api/Image
        // To protect from overposting aFttacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ImageFile>> PostImage(ImageFile imagefile)
        {
            if (_context.InspPhoto == null)
            {
                return Problem("Entity set 'dbContext.Image'  is null.");
            }

            byte[] bytes = Convert.FromBase64String(imagefile.byteBase64!);

            MemoryStream ms = new MemoryStream(bytes);
            _imageservice.UploadAzure(imagefile.FileName!,ms);
            if (imagefile.ParentTable == 1)
            {
                InspPhoto inspPhoto = new InspPhoto();
                inspPhoto.description = imagefile.ContentType;
                inspPhoto.photoname = imagefile.FileName!;
                inspPhoto.InspEquipID = imagefile.ParentID;
                _context.InspPhoto.Add(inspPhoto);
                /*
                InspPhoto inspPhotoS = new InspPhoto();
                inspPhotoS.description = imagefile.ContentType;
                inspPhotoS.photoname = imagefile.FileName!;
                inspPhotoS.InspEquipID = imagefile.ParentID;
                _context.InspPhoto.Add(inspPhoto);
                */
                await _context.SaveChangesAsync();
            }
            else if (imagefile.ParentTable == 0)
            {
                var inspect = _context.Inspection.Where(i=>i.id==imagefile.ParentID).FirstOrDefault();
                inspect!.Photo=imagefile.FileName;
                await _context.SaveChangesAsync();
            }
            return imagefile;// CreatedAtAction("GetEquipType", new { id = inspPhoto.id }, inspPhoto);
        }


        private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, decimal nPercent)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
            
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height  
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return (System.Drawing.Image)b;
        }

        public class ImageFile
        {
            public string? byteBase64 { get; set; }
            public string? ContentType { get; set; }
            public string? FileName { get; set; }
            public int ParentTable { get; set; }
            public int ParentID { get; set; }
        }
        // DELETE: api/Image/5
       
    }
}
