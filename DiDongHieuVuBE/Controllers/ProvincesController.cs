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
    public class ProvincesController : ControllerBase
    {
        private IProvinceService _provinceService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;
        private ApplicationDbContext _context;

        public ProvincesController(
            IProvinceService provinceService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            ApplicationDbContext context)
        {
            _provinceService = provinceService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] PagingationRequestModel param)
        {
            var provinces = _provinceService.GetAll(param.Page, param.PageSize).ToList();
            var totalItems = _provinceService.Count();
            var model = _mapper.Map<IList<ProvinceModel>>(provinces);
            return new JsonResult(new BaseResponseModel
            {
                TotalItems = totalItems,
                Data = model,
                PageSize = param.PageSize,
                CurrenPage = param.Page
            });
        }

        [HttpGet("list")]
        public IActionResult GetList()
        {
            var provinces = _provinceService.GetAll().ToList();
            return new JsonResult(new BaseResponseModel
            {
                TotalItems = provinces.Count(),
                Data = provinces,
                PageSize = 0,
                CurrenPage = 0
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var province = _provinceService.GetById(id);
            return Ok(province);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ProvinceModel model)
        {
            // map model to entity and set id
            var province = _mapper.Map<Province>(model);

            try
            {
                // update user 
                _provinceService.Create(province);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProvinceModel model)
        {
            // map model to entity and set id
            var province = _mapper.Map<Province>(model);
            province.Id = id;

            try
            {
         
                // update user 
                _provinceService.Update(province);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _provinceService.Delete(id);
            return Ok();
        }

        [HttpGet("GetExcel")]
        public async Task<IActionResult> GetExcel()
        {
            string path_exc = @"D:\ath\data\excel\Danh sách chi tiết.xlsx";

            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path_exc))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                    try
                    {
                        string frmData = "";
                        int i = 6413;
                        while (sheet.Cells[i, 1].Value != null)
                        {
                            Province pro = new Province();
                            pro.Name = sheet.Cells[i, 1].Value.ToString();
                            pro.Code = sheet.Cells[i, 2].Value.ToString();
                            pro.Type = "";
                            pro.ZipCode = "";
                            _context.Provinces.Add(pro);
                            _context.SaveChanges();

                            while (sheet.Cells[i, 2].Value == pro.Code)
                            {
                                District dis = new District();
                                dis.Name = sheet.Cells[i, 3].Value.ToString();
                                dis.Code = sheet.Cells[i, 4].Value.ToString();
                                dis.ProvinceId = pro.Id;
                                dis.Type = "";
                                _context.Districts.Add(dis);
                                _context.SaveChanges();

                                while (sheet.Cells[i, 4].Value == dis.Code)
                                {
                                    if (sheet.Cells[i, 5].Value != null)
                                    {
                                        Ward ward = new Ward();
                                        ward.Name = sheet.Cells[i, 5].Value.ToString();
                                        ward.Code = sheet.Cells[i, 6].Value.ToString();
                                        ward.Type = sheet.Cells[i, 7].Value.ToString();
                                        ward.DistrictId = dis.Id;
                                        _context.Wards.Add(ward);
                                        _context.SaveChanges();
                                    }
                                    i++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                }
            }
            return Ok();
        }
    }
}
