using System;
namespace RoofSafety.Services.Abstract
{
	public interface IImageService
	{
		string? UploadImageToAzure(IFormFile file, bool shrink);
		string GetImageURL(string name);
		void UploadAzure(string filename, MemoryStream fileUploadStream);

    }
}

