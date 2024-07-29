using FinalAssessment_Backend.ServiceInterface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalAssessment_Backend.Service
{
    public class ImageUploadService : IImageUploadService
    {

        private readonly IWebHostEnvironment _hostEnvironment;

        public ImageUploadService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public async Task<string> GetImageUrl(IFormFile file)
        {
            string folder = null;

            if (file != null && file.Length > 0)
            {

                string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName);

               /* if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    return BadRequest(new { message = "Only .jpg, .jpeg, and .png files are allowed." });
                }

                else if (user.UserImage.Length > (5 * 1024 * 1024)) // 5MB max file size
                {
                    return BadRequest(new { message = "File size should not exceed 5MB." });
                }*/

                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                string extension = Path.GetExtension(file.FileName);
                string uniqueFileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                folder = "user/images/" + uniqueFileName;
                string serverFolder = Path.Combine(wwwRootPath, folder);


                using (var fileStream = new FileStream(serverFolder, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

            }

            if (folder == null)
            {
                return null;
            }

            return folder;

        }
    }
}
