using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ManageEmployee.Helpers;
using ManageEmployee.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using ManageEmployee.ViewModels;

namespace ManageEmployee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportDownloadController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IFileService _fileService;

        public ReportDownloadController(IHostingEnvironment hostingEnvironment, IFileService fileService)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("DownloadReportFromFile")]
        public IActionResult DownloadFileDoc(string filename, string fileType)
        {
            try
            {
                return DownLoadFileExport(filename, @"ExportHistory\" + (fileType == "excel" ? "Excel" : "PDF"));
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        FileResult DownLoadFileExport(string pFileName, string pathRoot)
        {
            string filename = pFileName;
            string filepath = Path.Combine(_hostingEnvironment.ContentRootPath, pathRoot, pFileName);
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);

            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(filepath, out contentType);
            contentType = contentType ?? "application/octet-stream";
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(filedata, contentType);
        }
        [HttpPost("uploadImage")]
        public IActionResult uploadImage([FromForm] IFormFile file)
        {
            try
            {
                var fileName = _fileService.Upload(file, "Images", file.FileName);
                return Ok(new { imageUrl = fileName });
                //return BadRequest(new { msg = file.FileName });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }
        [HttpPost("deleteImages")]
        public IActionResult deleteImages([FromBody] List<DeleteImageModel> requests)
        {
            try
            {
                for (int i = 0; i < requests.Count; i++)
                {
                    _fileService.DeleteFileUpload(requests[i].imageUrl);
                }
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}
