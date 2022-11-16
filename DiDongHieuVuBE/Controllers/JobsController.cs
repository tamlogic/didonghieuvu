using System;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using ManageEmployee.Entities;
using ManageEmployee.EnumList;
using ManageEmployee.Helpers;
using ManageEmployee.Models;
using ManageEmployee.Services;
using ManageEmployee.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private IJobService _jobService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public JobsController(
            IJobService jobService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _jobService = jobService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet("list")]
        public IActionResult GetAll()
        {
            var jobs = _jobService.GetAll();
         
            return new JsonResult(new BaseResponseModel()
            {
                Data = jobs,
            });
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] PagingationRequestModel param)
        {
            var jobs = _jobService.GetAll(param.Page,param.PageSize,param.SearchText);
            var totalItems = _jobService.Count(param.SearchText);
            return new JsonResult(new BaseResponseModel
            {
                TotalItems = totalItems,
                Data = jobs,
                PageSize = param.PageSize,
                CurrenPage = param.Page
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var model = _jobService.GetById(id);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Job entity)
        {
            var message = await _jobService.Create(entity);
            if (string.IsNullOrEmpty(message))
                return Ok(
                new ObjectReturn
                {
                    message = message,
                    status = Convert.ToInt32(ErrorEnum.SUCCESS)
                });
            return Ok(new ObjectReturn
            {
                message = message,
                status = Convert.ToInt32(ErrorEnum.BAD_REQUEST)
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] Job entity)
        {
            var message = await _jobService.Update(entity);
            if (string.IsNullOrEmpty(message))
                return Ok(
                new ObjectReturn
                {
                    message = message,
                    status = Convert.ToInt32(ErrorEnum.SUCCESS)
                });
            return Ok(new ObjectReturn
            {
                message = message,
                status = Convert.ToInt32(ErrorEnum.BAD_REQUEST)
            });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _jobService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
           
        }
    }
}