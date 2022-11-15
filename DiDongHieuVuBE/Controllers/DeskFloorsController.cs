using AutoMapper;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Models;
using ManageEmployee.Services;
using ManageEmployee.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DeskFloorsController : ControllerBase
    {
        private IDeskFloorService _deskFloorService;
        private IMapper _mapper;

        public DeskFloorsController(
            IDeskFloorService deskFloorService,
            IMapper mapper)
        {
            _deskFloorService = deskFloorService;
            _mapper = mapper;
        }

        [HttpGet()]
        public IActionResult GetAll([FromQuery] DeskFLoorPagingationRequestModel param)
        {
            var result = _deskFloorService.GetAll(param);
            return new JsonResult(new BaseResponseModel
            {
                TotalItems = result.TotalItems,
                Data = result.DeskFloors,
                PageSize = result.PageSize,
                CurrenPage = result.pageIndex
            });
        }

        [HttpGet("getdeskfloor")]
        public IActionResult GetDeskFloor()
        {
            var deskFloors = _deskFloorService.GetAll();

            return new JsonResult(new BaseResponseModel
            {
                Data = deskFloors,
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var ward = _deskFloorService.GetById(id);
            var model = _mapper.Map<DeskFloor>(ward);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DeskFloor model)
        {
            try
            {
                var result = await _deskFloorService.Create(model);
                if (string.IsNullOrEmpty(result))
                    return Ok();
                return Ok(
                       new ObjectReturn
                       {
                           message = result,
                           status = 603
                       });
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] DeskFloor model, int id)
        {
            try
            {
                var result = await _deskFloorService.Update(model);
                if (string.IsNullOrEmpty(result))
                    return Ok();
                return Ok(
                       new ObjectReturn
                       {
                           message = result,
                           status = 603
                       });
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
            var result = _deskFloorService.Delete(id);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(
                   new ObjectReturn
                   {
                       message = result,
                       status = 603
                   });
        }
    }
}
