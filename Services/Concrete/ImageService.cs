using System;
using RoofSafety.Models;
using RoofSafety.Services.Abstract;
using RoofSafety.Options;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace RoofSafety.Services.Concrete
{
	public class ImageService:IImageService
	{

        private readonly AzureOptions _azureOptions;
		public ImageService(IOptions<AzureOptions> azureOptions)
		{
            _azureOptions = azureOptions.Value  ;
		}

        public string? UploadImageToAzure(IFormFile file,bool shrink)
        {
            string ext=Path.GetExtension(file.FileName);
            string filename = Guid.NewGuid().ToString()  + ext;// file.FileName;
            using MemoryStream fileUploadStream = new MemoryStream();  
            file.CopyTo(fileUploadStream);

            if (!shrink)
            {
                UploadAzure(filename, fileUploadStream);
                return filename;
            }
            /* System.Drawing.Image img = System.Drawing.Image.FromStream(fileUploadStream);
             resizeImage(img,(decimal) 0.2);// new Size(10, 10));
             using MemoryStream smallfileUploadStream = new MemoryStream();
             // img.CopyTo(smallfileUploadStream);
             img.Save(smallfileUploadStream, ImageFormat.Jpeg);

             UploadAzure(filename, smallfileUploadStream);*/
            return null;
        }

        private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, decimal nPercent)
        {
            //Get the image current width  
            int sourceWidth = imgToResize.Width;
            //Get the image current height  
            int sourceHeight = imgToResize.Height;
          /*  float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size  
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;*/
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

        public void UploadAzure(string filename, MemoryStream fileUploadStream)
        {
            fileUploadStream.Position = 0;
            BlobContainerClient blobContainreClient = new BlobContainerClient(_azureOptions.ConnectionString, _azureOptions.Container);
            BlobClient blobClient = blobContainreClient.GetBlobClient(filename);
            blobClient.Upload(fileUploadStream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "image/bitmap" }

            }, cancellationToken: default);


            System.Drawing.Image img = System.Drawing.Image.FromStream(fileUploadStream);
            resizeImage(img, (decimal)0.2);// new Size(10, 10));
            using MemoryStream smallfileUploadStream = new MemoryStream();
            // img.CopyTo(smallfileUploadStream);
            img.Save(smallfileUploadStream, ImageFormat.Jpeg);

            smallfileUploadStream.Position = 0;
            BlobContainerClient smallblobContainreClient = new BlobContainerClient(_azureOptions.ConnectionString, _azureOptions.Container);
            BlobClient smallblobClient = blobContainreClient.GetBlobClient(filename);
            smallblobClient.Upload(smallfileUploadStream, new BlobUploadOptions()
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = "image/bitmap" }

            }, cancellationToken: default);
            return;
        }

        public string GetImageURL(string UniqueName)
        {
            BlobContainerClient blobContainreClient = new BlobContainerClient(_azureOptions.ConnectionString, _azureOptions.Container);
            BlobClient blobClient = blobContainreClient.GetBlobClient(UniqueName);

            return blobClient.Uri.AbsoluteUri;
        }
    }
}

