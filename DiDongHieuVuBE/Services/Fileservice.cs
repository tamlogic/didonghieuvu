using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.IO;

namespace ManageEmployee.Services
{
    public interface IFileService
    {
        string Upload(IFormFile file, string folder = "",string fileNameUpload="");
        bool DeleteFileUpload(string filePath);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment env;
        public FileService(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public string Upload(IFormFile file, string folder = "Images", string fileNameUpload=null)
        {
            try
            {
                var uploadDirecotroy = "Uploads/";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), uploadDirecotroy);
                {
                    uploadDirecotroy += folder;
                    uploadPath = Path.Combine(uploadPath, folder);
                }
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var fileName =(! String.IsNullOrEmpty(fileNameUpload) ? fileNameUpload:(Guid.NewGuid().ToString() + Path.GetExtension(file.FileName))) ;
                var filePath = Path.Combine(uploadPath, fileName);

                using (var strem = File.Create(filePath))
                {
                    file.CopyTo(strem);
                }

                return Path.Combine(uploadDirecotroy, fileName).Replace("\\","/");
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }

      
        public bool DeleteFileUpload(string filePath)
        {
            try
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                if (File.Exists(uploadPath))
                {
                    File.Delete(uploadPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}