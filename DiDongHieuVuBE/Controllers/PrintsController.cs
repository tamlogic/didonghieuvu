using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using ManageEmployee.Services;
using ManageEmployee.Helpers;
using ManageEmployee.ViewModels;
using ManageEmployee.Models;
using ManageEmployee.Entities;
using OfficeOpenXml;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PrintsController : ControllerBase
    {
        private IPrintService _printService;

        public PrintsController(
            IPrintService printService)
        {
            _printService = printService;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] PagingationRequestModel param)
        {
            var data = _printService.GetAll().ToList();
            return new JsonResult(new BaseResponseModel
            {
                TotalItems = data.Count(),
                Data = data.Skip(param.PageSize * (param.Page - 1))
                 .Take(param.PageSize),
                PageSize = param.PageSize,
                CurrenPage = param.Page
            });
        }

        [HttpGet("list")]
        public IActionResult GetList()
        {
            var result = _printService.GetAll().ToList();
            return Ok(new ObjectReturn
            {
                data = result,
                status = 200,
            });
        }

        [HttpGet("get-page-print")]
        public IActionResult GetPagePrint()
        {
            var result = _printService.GetPagePrint();
            return Ok(new ObjectReturn
            {
                data = result,
                status = 200,
            });
        }
      
        [HttpPut]
        public IActionResult Update([FromBody] PagePrintViewModel model)
        {
            try
            {
                _printService.Update(model);
                return Ok(new ObjectReturn
                {
                    data = "",
                    status = 200,
                });
            }
            catch (AppException ex)
            {
                return Ok(new ObjectReturn
                {
                    data = ex.Message.ToString(),
                    status = 200,
                });
            }
        }
    }
}
