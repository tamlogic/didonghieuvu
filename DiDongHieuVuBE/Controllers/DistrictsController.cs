using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using ManageEmployee.Services;
using ManageEmployee.Helpers;
using ManageEmployee.Entities;
using ManageEmployee.Models;
using ManageEmployee.ViewModels;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DistrictsController : ControllerBase
    {
        private IDistrictService _districtService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public DistrictsController(
            IDistrictService districtService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _districtService = districtService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] DistrictViewModel.GetByProvince param)
        {
            var districts = new List<District>();
            var totalItems =0;
            if (param.provinceid != null && param.provinceid != 0)
            {
                districts = _districtService.GetMany(x => !x.IsDeleted && x.ProvinceId == param.provinceid.Value, param.Page, param.PageSize).ToList();
                totalItems = _districtService.Count(x => !x.IsDeleted && x.ProvinceId == param.provinceid.Value);
            }
            else
            {
                districts = _districtService.GetAll(param.Page, param.PageSize).ToList();
                totalItems =  _districtService.Count();
            }
         
            var model = _mapper.Map<IList<DistrictModel>>(districts);
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
            var districts = _districtService.GetAll().ToList();
            var totalItems = _districtService.Count();
            var model = _mapper.Map<IList<District>>(districts);
            return new JsonResult(new BaseResponseModel
            {
                TotalItems = totalItems,
                Data = model,
                PageSize = 0,
                CurrenPage = 0
            });
        }

        [HttpGet("list/province/{provinceId}")]
        public IActionResult GetListByProvinceId(int provinceId)
        {
            var districts = _districtService.GetAllByProvinceId(provinceId).ToList();
            var totalItems = _districtService.Count();
            var model = _mapper.Map<IList<District>>(districts);
            return new JsonResult(new BaseResponseModel
            {
                TotalItems = totalItems,
                Data = model,
                PageSize = 0,
                CurrenPage = 0
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var district = _districtService.GetById(id);
            var model = _mapper.Map<District>(district);
            return Ok(model);
        }

        [HttpPost]
        public IActionResult Create([FromBody] DistrictModel model)
        {
            // map model to entity and set id
            var district = _mapper.Map<District>(model);

            try
            {
                // update user 
                _districtService.Create(district);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] DistrictModel model)
        {
            // map model to entity and set id
            var district = _mapper.Map<District>(model);
            district.Id = id;

            try
            {
                // update user 
                _districtService.Update(district);
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
            _districtService.Delete(id);
            return Ok();
        }
    }
}
