using ManageEmployee.Services;
using ManageEmployee.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManageEmployee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] FileModel model)
        {
            try
            {
                var img = _fileService.Upload(HttpContext.Request.Form.Files[0], model.folderName);
                return Ok(new { url = img });
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}
