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
using ManageEmployee.Entities;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WardsController : ControllerBase
    {
        private IWardService _WardService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public WardsController(
            IWardService WardService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _WardService = WardService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] PagingationRequestModel param)
        {
            var Wards = _WardService.GetAll(param.Page, param.PageSize).ToList();
            var totalItems = _WardService.Count();
            var model = _mapper.Map<IList<Ward>>(Wards);
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
            var Wards = _WardService.GetAll().ToList();
            var totalItems = _WardService.Count();
            var model = _mapper.Map<IList<Ward>>(Wards);
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
            var model = _WardService.GetById(id);
            return Ok(model);
        }

        [HttpGet("list/district/{districtId}")]
        public IActionResult GetListByProvinceId(int districtId)
        {
            var wards = _WardService.GetAllByDistrictId(districtId).ToList();
            var totalItems = _WardService.Count();
            var model = _mapper.Map<IList<Ward>>(wards);
            return new JsonResult(new BaseResponseModel
            {
                TotalItems = totalItems,
                Data = model,
                PageSize = 0,
                CurrenPage = 0
            });
        }

        [HttpPost]
        public IActionResult Create([FromBody] Ward model)
        {
            // map model to entity and set id
            var Ward = _mapper.Map<Ward>(model);

            try
            {
                // update user 
                _WardService.Create(Ward);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Ward Ward)
        {
            try
            {
         
                // update user 
                _WardService.Update(Ward);
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
            _WardService.Delete(id);
            return Ok();
        }
    }
}
