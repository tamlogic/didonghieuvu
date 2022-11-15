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
    public class CustomersController : ControllerBase
    {
        private ICustomerService _customerService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public CustomersController(
            ICustomerService customerService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _customerService = customerService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet("list")]
        public IActionResult GetAll()
        {
            var jobs = _customerService.GetAll();
         
            return new JsonResult(new BaseResponseModel()
            {
                Data = jobs,
            });
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] PagingationRequestModel param)
        {
            var jobs = _customerService.GetAll(param.Page,param.PageSize,param.SearchText);
            return new JsonResult(new BaseResponseModel
            {
                TotalItems = 0,
                Data = jobs,
                PageSize = param.PageSize,
                CurrenPage = param.Page
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var model = _customerService.GetById(id);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Customer entity)
        {
            var message = await _customerService.Create(entity);
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
        public async Task<IActionResult> Update([FromBody] Customer entity)
        {
            var message = await _customerService.Update(entity);
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
                _customerService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
           
        }
    }
}