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
using System.Linq;
using Newtonsoft.Json;
using ManageEmployee.Services;
using ManageEmployee.ViewModels;
using ManageEmployee.EnumList;
using ManageEmployee.Helpers;
using ManageEmployee.Models;
using ManageEmployee.Entities;
using ManageEmployee.Enum;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        private IUserRoleService _userRoleService;
        private ApplicationDbContext _context;

        public AuthController(
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration,
            IOptions<AppSettings> appSettings,
            IUserRoleService userRoleService,
            ApplicationDbContext context)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _appSettings = appSettings.Value;
            _userRoleService = userRoleService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("requestForgotPass")]
        public IActionResult RequestForgotPass([FromBody] ResetPasswordModel model)
        {
            try
            {
                if (!String.IsNullOrEmpty(model.Username))
                {
                    var user = _userService.GetByUserName(model.Username);
                    if (user != null)
                    {
                        user.RequestPassword = true;
                        _userService.Update(user, "123456");
                        return Ok(true);
                    }
                }
              
                return Ok(false);
           
            }
            catch(Exception ex)
            {
                return BadRequest(new
                {
                    msg = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthenticateModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
                    return Ok(
                       new ObjectReturn
                       {
                           message = ResultErrorEnum.USER_IS_NOT_EXIST,
                           status = Convert.ToInt32(ErrorEnum.USER_IS_NOT_EXIST)
                       });

                var user = _userService.GetByUserName(model.Username);
                if (user == null)
                    return Ok(
                        new ObjectReturn
                        {
                            message = ResultErrorEnum.USER_IS_NOT_EXIST,
                            status = Convert.ToInt32(ErrorEnum.USER_IS_NOT_EXIST)
                        });

                UserMapper.Auth checkUser = _userService.Authenticate(model.Username, model.Password);
                if (checkUser == null)
                    return Ok(
                        new ObjectReturn
                        {
                            message = ResultErrorEnum.ERROR_PASS,
                            status = Convert.ToInt32(ErrorEnum.ERROR_PASS)
                        });

                _userService.UpdateLastLogin(user.Id);

                IList<AuthRoleModel> listAuthRole = new List<AuthRoleModel>();
                List<string> listRole = user.UserRoleIds.Split(",").ToList();
                List<MenuCheckRole> listMenuCheckRole = new List<MenuCheckRole>();

                IList<string> roles = _userRoleService.GetAll_Login().ToList().Where(o => listRole.Contains( o.Id.ToString())).Select(x => x.Code).ToList();
                if (roles.Contains("ADMIN"))
                {
                    var menus = _context.Menus.ToList();
                    foreach (var menuCode in menus)
                    {
                        MenuCheckRole menu = new MenuCheckRole();
                        menu.MenuCode = menuCode.Code;
                        menu.Add = true;
                        menu.Edit = true;
                        menu.Delete = true;
                        menu.View = true;
                        listMenuCheckRole.Add(menu);
                    }
                }
                else
                {
                    var listMenuRole = _context.MenuRoles.Where(x => listRole.Contains(x.UserRoleId.ToString())).ToList();
                    var listMenuCode = listMenuRole.Select(x => x.MenuCode).Distinct().ToList();
                    foreach (string menuCode in listMenuCode)
                    {
                        MenuCheckRole menu = new MenuCheckRole();
                        menu.MenuCode = menuCode;
                        menu.Add = listMenuRole.FirstOrDefault(x => x.MenuCode == menuCode)?.Add ?? false;
                        menu.Edit = listMenuRole.FirstOrDefault(x => x.MenuCode == menuCode)?.Edit ?? false;
                        menu.Delete = listMenuRole.FirstOrDefault(x => x.MenuCode == menuCode)?.Delete ?? false;
                        menu.View = listMenuRole.FirstOrDefault(x => x.MenuCode == menuCode)?.View ?? false;
                        listMenuCheckRole.Add(menu);
                    }
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var authClaims = new List<Claim>
                    {
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim("FullName",  !String.IsNullOrEmpty(user.FullName)? user.FullName : ""),
                        new Claim("RoleName", JsonConvert.SerializeObject(roles)),
                    };

                var token = GetToken(authClaims);

                var tokenString = tokenHandler.WriteToken(token);

                // return basic user info and authentication token
                return Ok(
                        new ObjectReturn
                        {
                            data = new
                            {
                                Id = user.Id,
                                Username = user.Username,
                                Fullname = user.FullName,
                                Avatar = user.Avatar,
                                Timekeeper = user.Timekeeper,
                                Token = tokenString,
                                TargetId = user.TargetId,
                                RoleName = roles,
                                Menus = listMenuCheckRole,
                                UserRoleIds = user.UserRoleIds,
                            },
                            status = 200,
                            message = ResultErrorEnum.LOGIN_SUCCESS,
                        }); ;
            }
            catch (Exception ex)
            {
                return Ok(
                        new ObjectReturn
                        {
                            message = ex.Message,
                            status = 400
                        });
            }
        }
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        //private AuthRoleModel CreateAuthModel(AssignRole assignRole)
        //{
        //    AuthRoleModel authRoleModel = new AuthRoleModel();

        //    try
        //    {
        //        IList<string> ListRole = new List<string>();
        //        if (assignRole.AddRole)
        //        {
        //            ListRole.Add("add");
        //        }
        //        if (assignRole.EditRole)
        //        {
        //            ListRole.Add("edit");
        //        }
        //        if (assignRole.DeleteRole)
        //        {
        //            ListRole.Add("delete");
        //        }

        //        FunctionRole functionRole = _functionRoleService.Get(o => o.Id == assignRole.FunctionId);
        //        authRoleModel = new AuthRoleModel()
        //        {
        //            Name = functionRole.Code,
        //            Roles = ListRole
        //        };

        //    }
        //    catch (Exception ex){ }
        //    return authRoleModel;
        //}

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);
            try
            {
                // create user
                _userService.Create(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] PasswordModel model)
        {
            try
            {
                // check is current user
                if(!_userService.CheckPassword(model.Id, model.OldPassword))
                {
                    return Ok(new ObjectReturn
                    {
                        message = ErrorEnum.ERROR_PASS.ToString(),
                        status = Convert.ToInt32(ErrorEnum.ERROR_PASS)
                    });
                }
               
                _userService.UpdatePassword(model);
                return Ok(new ObjectReturn
                {
                    data = ErrorEnum.SUCCESS,
                    status = Convert.ToInt32(ErrorEnum.SUCCESS)
                });
            }
            catch (AppException ex)
            {
                return Ok(new ObjectReturn
                {
                    data = ex.Message,
                    status = Convert.ToInt32(ErrorEnum.BAD_REQUEST)
                });
            }
        }
    }
}
