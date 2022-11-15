using AutoMapper;
using Common.Helpers;

using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Models;
using ManageEmployee.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.Services
{
    public interface IMenuService
    {
        IEnumerable<MenuViewModel> GetAll(Boolean isParent);
        Task<PagingResult<MenuViewModel>> GetAll(int pageIndex, int pageSize, string keyword, Boolean isParent,
            string codeParent, List<string> listRole, int userId, int? type = null);
        Task<string> Create(MenuViewModel request);
        MenuViewModel GetById(int id, List<string> listRole, int userId);
        Task<string> Update(MenuViewModel request, List<string> listRole, int userId);
        string Delete(int id);
        MenuCheckRole CheckRole(string MenuCode, List<string> roleCodes);
    }
    public class MenuService : IMenuService
    {
        private ApplicationDbContext _context;
        private IMapper _mapper;

        public MenuService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<MenuViewModel> GetAll(Boolean isParent)
        {

            var data = _context.Menus.WhereIf(isParent, x => x.IsParent).Select(x => new MenuViewModel
            {
                Id = x.Id,
                Name = x.Name,
                NameEN = x.NameEN,
                NameKO = x.NameKO,
                Code = x.Code,
                CodeParent = x.CodeParent,
                IsParent = x.IsParent
            }).ToList();
            return data;
        }

        public async Task<PagingResult<MenuViewModel>> GetAll(int pageIndex, int pageSize, string keyword,
            Boolean isParent, string codeParent, List<string> listRole, int userId, int? type = null)
        {
            try
            {
                if (pageSize <= 0)
                    pageSize = 20;

                if (pageIndex < 0)
                    pageIndex = 0;

                if (!keyword.IsNullOrEmpty())
                {
                    keyword = keyword.RemoveAccents();
                }
                var result = new PagingResult<MenuViewModel>()
                {
                    CurrentPage = pageIndex,
                    PageSize = pageSize,
                };
                List<int> menuIds = new List<int>();
                if (listRole.Contains("ADMIN_BRANCH"))
                {
                    int userRoleId = _context.UserRoles.FirstOrDefault(x => x.Code == "ADMIN_BRANCH")?.Id ?? 0;
                    menuIds = _context.MenuRoles.Where(x => x.UserRoleId == userRoleId).Select(x => x.MenuId ?? 0).Distinct().ToList();
                }
                var Menus =  _context.Menus
                                .WhereIf(!keyword.IsNullOrEmpty(), x => _context.RemoveAcents(x.Name).Contains(keyword) || _context.RemoveAcents(x.Code).Contains(keyword))
                                .WhereIf(!codeParent.IsNullOrEmpty(), x => x.CodeParent == codeParent)
                                .WhereIf(isParent, x => x.IsParent)
                                .WhereIf(listRole.Contains("ADMIN_BRANCH"), x => menuIds.Contains(x.Id))
                                .Select(x => new MenuViewModel
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    NameEN = x.NameEN,
                                    NameKO = x.NameKO,
                                    Code = x.Code,
                                    CodeParent = x.CodeParent,
                                    IsParent = x.IsParent
                                }).AsNoTracking();
                result.TotalItems = await Menus.CountAsync();
                result.Data = await Menus.Skip((pageIndex) * pageSize).Take(pageSize).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                return new PagingResult<MenuViewModel>()
                {
                    CurrentPage = pageIndex,
                    PageSize = pageSize,
                    TotalItems = 0,
                    Data = new List<MenuViewModel>()
                };
            }
        }

        public async Task<string> Create(MenuViewModel request)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var existCode = _context.Menus.Where(
                    x => x.Code.ToLower() == request.Code.ToLower()).FirstOrDefault();
                if (existCode != null)
                {
                    return ErrorMessages.MenuCodeAlreadyExist;
                }
                Menu menu = _mapper.Map<Menu>(request);
                _context.Menus.Add(menu);
                _context.SaveChanges();
                if (request.listItem != null)
                {
                    foreach (var item in request.listItem)
                    {
                        MenuRole menuRole = _mapper.Map<MenuRole>(item);
                        menuRole.MenuId = menu.Id;
                        menuRole.MenuCode = menu.Code;
                        _context.MenuRoles.Add(menuRole);
                    }
                }
                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();

                return string.Empty;
            }
            catch
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }

        public MenuViewModel GetById(int id, List<string> listRole, int userId)
        {
            try
            {
                var menu = _context.Menus.Find(id);
                if (menu != null)
                {
                    List<int> menuIds = new List<int>();
                    if (listRole.Contains("ADMIN_BRANCH"))
                    {
                        int userRoleId = _context.UserRoles.FirstOrDefault(x => x.Code == "ADMIN_BRANCH")?.Id ?? 0;
                        List<int> userRoleIds = _context.UserRoles.Where(x => x.UserCreated == userId).Select(x => x.Id).ToList();
                        menuIds = _context.MenuRoles.Where(x => userRoleIds.Contains(x.UserRoleId ?? 0)).Select(x => x.MenuId ?? 0).Distinct().ToList();
                    }
                    var menuRoles = _context.MenuRoles.Where(x => x.MenuId == id 
                    && (listRole.Contains("ADMIN_BRANCH") ? menuIds.Contains(x.UserRoleId ?? 0) : true)).Select(x => _mapper.Map<MenuRoleViewModel>(x)).ToList();

                    var menuView = _mapper.Map<MenuViewModel>(menu);
                    menuView.listItem = menuRoles;
                    return menuView;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string> Update(MenuViewModel request, List<string> listRole, int userId)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var menu = _context.Menus.Find(request.Id );
                if (menu == null)
                {
                    return ErrorMessages.DataNotFound;
                }
                
                var checkMenuCode = _context.Menus.Where(x => x.Code.ToLower() == request.Code.ToLower() && x.Id != menu.Id).FirstOrDefault();
                if (checkMenuCode != null && checkMenuCode.Id != menu.Id)
                {
                    return ErrorMessages.MenuCodeAlreadyExist;
                }
                menu.Name = request.Name;
                menu.NameEN = request.NameEN;
                menu.NameKO = request.NameKO;
                menu.Code = request.Code;
                menu.CodeParent = request.CodeParent;
                menu.IsParent = request.IsParent;
                _context.Menus.Update(menu);

                //
                List<int> menuIds = new List<int>();
                if (listRole.Contains("ADMIN_BRANCH"))
                {
                    int userRoleId = _context.UserRoles.FirstOrDefault(x => x.Code == "ADMIN_BRANCH")?.Id ?? 0;
                    List<int> userRoleIds = _context.UserRoles.Where(x => x.UserCreated == userId).Select(x => x.Id).ToList();
                    menuIds = _context.MenuRoles.Where(x => userRoleIds.Contains(x.UserRoleId ?? 0)).Select(x => x.MenuId ?? 0).Distinct().ToList();
                }
                var menuRoleDel = _context.MenuRoles.Where(x => x.MenuId == request.Id
                    && (listRole.Contains("ADMIN_BRANCH") ? menuIds.Contains(x.UserRoleId ?? 0) : true)).ToList();


                //List<MenuRole> menuRoleDel = _context.MenuRoles.Where(x => x.MenuId == request.Id).ToList();
                _context.MenuRoles.RemoveRange(menuRoleDel);
                if (request.listItem != null)
                {
                    foreach (var item in request.listItem)
                    {
                        MenuRole menuRole = _mapper.Map<MenuRole>(item);
                        menuRole.Id = 0;
                        menuRole.MenuId = menu.Id;
                        menuRole.MenuCode = menu.Code;
                        _context.MenuRoles.Add(menuRole);
                    }
                }

                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();
                return string.Empty;
            }
            catch
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }

        public string Delete(int id)
        {
            var menu = _context.Menus.Find(id);
            var menuRoles = _context.MenuRoles.Where(x => x.MenuId == id).ToList();
            if (menu != null)
            {
                var menuChild = _context.Menus.Where(x => x.CodeParent == menu.Code).ToList();
                if(menuChild.Count > 0)
                {
                    foreach(var item in menuChild)
                    {
                        item.CodeParent = null;
                        _context.Menus.Update(item);
                    }
                }
                _context.Menus.Remove(menu);
                _context.MenuRoles.RemoveRange(menuRoles);
                _context.SaveChanges();
            }
            return string.Empty;
        }
        public MenuCheckRole CheckRole(string MenuCode, List<string> roleCodes)
        {
            try
            {
                Menu menu = _context.Menus.FirstOrDefault(x => x.Code == MenuCode);
                var useRoleIds = _context.UserRoles.Where(x => roleCodes.Contains(x.Code)).Select(x => x.Id).ToArray();
                MenuCheckRole itemOut = new MenuCheckRole();
                var menuRoles = _context.MenuRoles.Where(x => x.MenuId == menu.Id && useRoleIds.Contains(x.UserRoleId ?? 0)).ToList();
                if (menuRoles.Count > 0)
                {
                    itemOut.Add = menuRoles.FirstOrDefault(x => x.Add == true)?.Add ?? false;
                    itemOut.Edit = menuRoles.FirstOrDefault(x => x.Edit == true)?.Edit ?? false;
                    itemOut.Delete = menuRoles.FirstOrDefault(x => x.Delete == true)?.Delete ?? false;
                    itemOut.View = menuRoles.FirstOrDefault(x => x.View == true)?.View ?? false;
                }
                return itemOut;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
