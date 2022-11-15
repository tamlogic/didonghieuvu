using ManageEmployee.Entities;
using ManageEmployee.Enum;
using ManageEmployee.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManageEmployee.Services
{
    public interface IUserRoleService
    {
        IEnumerable<UserRole> GetAll(int userId, List<string> listRole);
        UserRole GetById(int id);
        UserRole Create(UserRole userRole);
        UserRole Update(UserRole userRole);
        void Delete(int id);
        IEnumerable<UserRole> GetAll_Login();
    }

    public class UserRoleService : IUserRoleService
    {
        private ApplicationDbContext _context;

        public UserRoleService(ApplicationDbContext context)
        {
            _context = context;
        }



        public IEnumerable<UserRole> GetAll(int userId, List<string> listRole)
        {
            return _context.UserRoles.Where(x=> !listRole.Contains("ADMIN") ? x.UserCreated == userId : true).OrderBy(x=> x.Order);
        }
        public IEnumerable<UserRole> GetAll_Login()
        {
            return _context.UserRoles.OrderBy(x => x.Order);
        }
        public UserRole GetById(int id)
        {
            return _context.UserRoles.Find(id);
        }

        public UserRole Create(UserRole userRole)
        {
            // validation
            if (string.IsNullOrWhiteSpace(userRole.Title))
                throw new AppException(ResultErrorEnum.MODEL_MISS);

            _context.UserRoles.Add(userRole);
            _context.SaveChanges();

            return userRole;
        }

        public UserRole Update(UserRole userRoleParam)
        {
            var userRole = _context.UserRoles.Find(userRoleParam.Id);

            if (userRole == null)
                throw new AppException(ResultErrorEnum.MODEL_NULL);

            userRole.Title = userRoleParam.Title;
            userRole.Code = userRoleParam.Code;
            userRole.Note = userRoleParam.Note;
            userRole.Order = userRoleParam.Order;
            _context.UserRoles.Update(userRole);
            _context.SaveChanges();
            return userRole;
        }

        public void Delete(int id)
        {
            var userRole = _context.UserRoles.Find(id);
            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);
                _context.SaveChanges();
            }
        }
    }
}