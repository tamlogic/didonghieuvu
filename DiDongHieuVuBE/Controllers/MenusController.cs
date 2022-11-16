using AutoMapper;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Models;
using ManageEmployee.Services;
using ManageEmployee.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : ControllerBase
    {
        private IMenuService _menuService;
        private IMapper _mapper;
        private IFileService _fileService;

        public MenusController(
            IMenuService menuService,
            IMapper mapper, IFileService fileService)
        {
            _menuService = menuService;
            _mapper = mapper;
            _fileService = fileService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MenuPagingationRequestModel param)
        {
            string roles = "";
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
                userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
            }
            List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

            return Ok(await _menuService.GetAll(param.Page, param.PageSize, param.SearchText, param.isParent, param.CodeParent, listRole, userId));
        }

        [HttpGet("list")]
        public IActionResult GetMenu([FromQuery] MenuPagingationRequestModel param)
        {
            string roles = "";
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
                userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
            }
            List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

            var results = _menuService.GetAll(param.isParent);

            return new JsonResult(new BaseResponseModel
            {
                Data = results,
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            string roles = "";
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
                userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
            }
            List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

            var model = _menuService.GetById(id, listRole, userId);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MenuViewModel model)
        {
            try
            {
                var result = await _menuService.Create(model);
                if (string.IsNullOrEmpty(result))
                    return Ok();
                return Ok(new { code = 400, msg = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] MenuViewModel model)
        {
            try
            {
                string roles = "";
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
                    userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
                }
                List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);


                var result = await _menuService.Update(model, listRole, userId);
                if (string.IsNullOrEmpty(result))
                    return Ok();
                return Ok(new { code = 400, msg = result });
            }
            catch (AppException ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _menuService.Delete(id);
                if (string.IsNullOrEmpty(result))
                    return Ok();
                return BadRequest(new { msg = result });
            }
            catch (AppException ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
        [HttpGet("check-role")]
        public IActionResult CheckRole(string MenuCode)
        {
            try
            {
                string roles = "";
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
                }
                List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

                var result = _menuService.CheckRole(MenuCode, listRole);
                return Ok(new ObjectReturn
                {
                    status = 200,
                    data = result,
                });
            }
            catch (AppException ex)
            {
                return Ok(new ObjectReturn
                {
                    status = 400,
                    message = ex.Message.ToString(),
                });
            }
        }
    }
}
