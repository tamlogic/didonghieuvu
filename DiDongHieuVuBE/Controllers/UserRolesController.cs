using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Services;
using ManageEmployee.Helpers;
using ManageEmployee.Entities;
using ManageEmployee.Enum;
using ManageEmployee.ViewModels;
using Newtonsoft.Json;
using ManageEmployee.Models;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserRolesController : ControllerBase
    {
        private IUserRoleService _userRoleService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UserRolesController(
            IUserRoleService userRoleService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userRoleService = userRoleService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public IActionResult GePaging([FromQuery] PagingationRequestModel param)
        {
            string roles = "";
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
                userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
            }
            List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

            var data = _userRoleService.GetAll(userId, listRole);
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
        public IActionResult GetAll()
        {
            string roles = ""; 
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
                userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
            }
            List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

            var userRoles = _userRoleService.GetAll(userId, listRole);
            //if (!listRole.Contains("ADMIN"))
            //{
            //    userRoles = userRoles.Where(o => listRole.Contains(o.Code.ToString())).ToList();
            //}
            //if (listRole.Contains("ADMIN_BRANCH"))
            //{
            //    userRoles = userRoles.Where(o => o.Code != "ADMIN_BRANCH").ToList();
            //}
            return new JsonResult(new BaseResponseModel
            {
                Data = userRoles,
            });
        }

        [HttpPost]
        [HttpPut("{id}")]
        public IActionResult Save([FromBody] UserRole userRole)
        {
            if (userRole == null)
            {
                return BadRequest(new { msg = ResultErrorEnum.MODEL_NULL });
            }
            if (String.IsNullOrEmpty(userRole.Title))
            {
                return BadRequest(new { msg = ResultErrorEnum.MODEL_MISS });
            }
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
            }

            userRole.UserCreated = userId;

            try
            {
                if (userRole.Id != null && userRole.Id != 0)
                {
                    userRole= _userRoleService.Update(userRole);
                }
                else
                {
                    userRole = _userRoleService.Create(userRole);
                }
                return Ok(userRole);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _userRoleService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {msg=ex.Message });
            }
           
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var data =  _userRoleService.GetById(id);
                return Ok(new ObjectReturn { data = data,
                    status = 200,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }

        }
    }
}
