using AutoMapper;
using Common.Helpers;

using ManageEmployee.Entities;
using ManageEmployee.Enum;
using ManageEmployee.EnumList;
using ManageEmployee.Helpers;
using ManageEmployee.Models;
using ManageEmployee.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ManageEmployee.Services
{
    public interface IUserService
    {
        UserMapper.Auth Authenticate(string username, string password);
        IEnumerable<UserActiveModel> GetAllUserActive();
        IEnumerable<String> GetAllUserName();
        IEnumerable<User> GetMany(Expression<Func<User, bool>> where);
        IEnumerable<User> Filter(UserMapper.FilterParams param);
        User GetById(int id);
        User GetByUserName(string username);
        User Create(User user, string password);
        User CreateExcel(User user, string password);
        User Update(User user, string password = null);
        void ResetPassword(User user, string password = null);
        bool CheckPassword(int id, string oldPassword);
        void UpdatePassword(PasswordModel passwordModel);
        void UpdateLastLogin(int id);
        void Delete(int id);
        Task<BaseResponseModel> CountFilter(UserMapper.FilterParams param);
        int Count();
        List<UserModel> GetForExcel(List<int> ids);
        Task<string> GetUserName();
    }

    public class UserService : IUserService
    {
        private ApplicationDbContext _context;
        private IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public UserMapper.Auth Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            //var user = _context.Users.SingleOrDefault(x => x.Username == username);
            var user = (from us in _context.Users
                        where us.Username == username && !us.IsDelete
                        select new UserMapper.Auth
                        {
                            PasswordHash = us.PasswordHash,
                            PasswordSalt = us.PasswordSalt,
                            //RoleName = usr != null ? usr.Title : "",
                            FullName = us.FullName,
                            Id = us.Id,
                            LastLogin = us.LastLogin,
                            Avatar = us.Avatar,
                            Username = us.Username,
                            Status = us.Status,
                            UserRoleIds = us.UserRoleIds,
                            Timekeeper =  us.Timekeeper ?? 0,
                            TargetId = us.TargetId ?? 0,
                            Language = us.Language,
                        }
            ).SingleOrDefault();

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;
            // authentication successful

            return user;
        }

        public IEnumerable<UserActiveModel> GetAllUserActive()
        {
            return _context.Users.Where(x => !x.IsDelete).Select(x => _mapper.Map<UserActiveModel>(x));
        }

        public User GetByUserName(string username)
        {
            return _context.Users.AsNoTracking().FirstOrDefault(x => x.Username.Equals(username) && !x.IsDelete);
        }

        public  IEnumerable<String> GetAllUserName()
        {
            return _context.Users.Where(x=>!x.IsDelete).Select(x=>x.Username);
        }
        public IEnumerable<User> GetMany(Expression<Func<User, bool>> where)
        {

            return _context.Users.Where(where);
        }

        public IEnumerable<User> Filter(UserMapper.FilterParams param)
        {
            string condition = "WHERE us.IsDelete = 0";
            if (!string.IsNullOrEmpty(param.Keyword))
            {
                condition += string.Format("AND (us.Username like N'%{0}%' " +
                                                    "or us.Code like N'%{0}%' " +
                                                    "or us.FullName like N'%{0}%' " +
                                                    "or us.Email like N'%{0}%'" +
                                                    "or us.Phone like N'%{0}%' )", param.Keyword);
            }

            if (param.WarehouseId != null && param.WarehouseId.Value != 0)
            {
                condition += string.Format("AND us.WarehouseId = {0} ", param.WarehouseId);
            }

            if (param.DepartmentId != null && param.DepartmentId.Value != 0)
            {
                condition += string.Format("AND us.DepartmentId = {0} ", param.DepartmentId);
            }

            if (param.PositionId != null && param.PositionId.Value != 0)
            {
                condition += string.Format("AND us.PositionId = {0} ", param.PositionId);
            }

            if (param.RequestPassword != null && param.RequestPassword.Value == true)
            {
                condition += string.Format("AND us.RequestPassword = {0} ", 1);
            }
            if (param.Quit != null && param.Quit.HasValue)
            {
                condition += string.Format("AND us.Quit = {0} ", param.Quit.Value ? 1 : 0);
            }

            if (param.Gender != GenderEnum.All)
            {
                condition += string.Format("AND us.Gender = {0} ", (short)param.Gender);
            }

            if (param.StartDate != null && param.EndDate != null)
            {
                condition += string.Format("AND BirthDay IS NULL OR (us.BirthDay >= '{0}' AND us.BirthDay <= '{1}') ",
                    param.StartDate?.ToString("MM/dd/yyyy"),
                    param.EndDate?.ToString("MM/dd/yyyy"));
            }
            else if (param.StartDate != null)
            {
                condition += string.Format("AND BirthDay IS NULL OR us.BirthDay >= '{0}'", param.StartDate?.ToString("MM/dd/yyyy"));
            }
            else if (param.EndDate != null)
            {
                condition += string.Format("AND BirthDay IS NULL OR us.BirthDay <= '{0}' ", param.EndDate?.ToString("MM/dd/yyyy"));
            }
            if (param.TargetId != 0)
            {
                condition += string.Format("AND us.TargetId = {0} ", param.TargetId);
            }

            if (param.Month.HasValue && param.Month.Value > 0 && param.Month.Value <= 12)
            {
                condition += string.Format("AND us.BirthDay IS NOT NULL AND Month(us.BirthDay) = {0} ", param.Month.Value);
            }

            string query = string.Format("SELECT * FROM Users us {0} ", condition);
            return _context.Users.FromSqlRaw(query).Skip(param.PageSize * (param.CurrentPage == 0 ? param.CurrentPage : (param.CurrentPage - 1)))
                .Take(param.PageSize);
        }

        public async Task<BaseResponseModel> CountFilter(UserMapper.FilterParams param)
        {
           
            var query = _context.Users.AsQueryable();
                        
            var user = _context.Users?.Find(param.UserId);

            if (user != null)
            {
                if (param.roles.Contains("ADMIN"))
                {

                }
                else if (param.roles.Contains("ADMIN_BRANCH"))
                {
                    //var departmentIds = _context.Departments.Where(x => x.BranchId == user.BranchId).Select(x => x.Id).ToArray();
                    //var warehouseIds = _context.Warehouses.Where(x => x.BranchId == user.BranchId).Select(x => x.Id).ToArray();
                    query = query.Where(x => x.BranchId == user.BranchId || x.BranchId == 0 || x.BranchId == null
                    //|| departmentIds.Contains(x.DepartmentId ?? 0)
                    //|| warehouseIds.Contains(x.WarehouseId ?? 0)
                    );
                }
                else if (param.roles.Contains("TRUONGPHONG"))
                {
                    query = query.Where(x => x.DepartmentId == user.DepartmentId);
                }
                else if (param.roles.Contains("NHANVIEN"))
                {
                    query = query.Where(x => x.Id == user.Id);
                }
            }

            if (param.Ids != null && param.Ids.Any())
            {
                query = query.Where(x => param.Ids.Contains(x.Id));
            }

            if (!string.IsNullOrEmpty(param.Keyword))
{
                param.Keyword = param.Keyword.RemoveAccents();

                query = query.Where(x => _context.RemoveAcents(x.Username ?? String.Empty).Contains(param.Keyword) ||
                x.Phone.Contains(param.Keyword));
            }

            if (param.WarehouseId != null && param.WarehouseId.Value != 0)
            { 
                query = query.Where(x => x.WarehouseId == param.WarehouseId.Value);
            } 

            if (param.DepartmentId != null && param.DepartmentId.Value != 0)
            {
                query = query.Where(x => x.DepartmentId == param.DepartmentId.Value);
            } 
            if (param.RequestPassword != null && param.RequestPassword.Value == true)
            {
                query = query.Where(x => x.RequestPassword == param.RequestPassword.Value);
            }
            if (param.Quit != null && param.Quit.HasValue)
            {
                query = query.Where(x => x.Quit == param.Quit.Value);
            }

            if (param.Gender != null && param.Gender != GenderEnum.All)
            {
                query = query.Where(x => x.Gender == param.Gender);
            }

            if (param.BirthDay.HasValue)
            {
                query = query.Where(x => x.BirthDay != null && x.BirthDay == param.BirthDay);
            }
            if (param.TargetId != 0)
            {
                query = query.Where(x => x.TargetId == param.TargetId);
            }
            if (param.Month.HasValue && param.Month.Value > 0 && param.Month.Value <= 12)
            {
                query = query.Where(x => x.BirthDay != null && x.BirthDay.Value.Month == param.Month);
            }

            if (param.StartDate != null && param.EndDate != null)
            {
                query = query.Where(x => x.BirthDay != null &&  x.BirthDay.Value >= param.StartDate && x.BirthDay.Value <= param.EndDate);
            }
            else if (param.StartDate != null)
            {
                query = query.Where(x => x.BirthDay != null && x.BirthDay.Value >= param.StartDate);
            }
            else if (param.EndDate != null)
            {
                query = query.Where(x => x.BirthDay != null && x.BirthDay.Value <= param.EndDate);
            }
            if (param.DegreeId != null && param.DegreeId > 0)
            {
                query = query.Where(x => string.IsNullOrEmpty(x.LiteracyDetail) ? false : x.LiteracyDetail.Contains(param.DegreeId.ToString()));
            }
            
            if (!string.IsNullOrEmpty(param.SortField))
            {
                switch (param.SortField)
                {
                    case "id":
                        query = param.isSort ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id);
                        break;
                    case "fullName":
                        query = param.isSort ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName);
                        break;
                    case "birthDay":
                        query = param.isSort ? query.OrderByDescending(x => x.BirthDay) : query.OrderBy(x => x.BirthDay);
                        break;
                }
            }

            var totalCount = await query.CountAsync();

            var result = new BaseResponseModel()
            {
                CurrenPage = param.CurrentPage,
                PageSize = param.PageSize,
                DataTotal = totalCount,
            };
            
            result.TotalItems = totalCount;

            query = query.Skip(param.PageSize * (param.CurrentPage == 0 ? param.CurrentPage : (param.CurrentPage - 1)))
                .Take(param.PageSize);

            var users = from coa in query join district in _context.Districts
                        on coa.DistrictId equals district.Id into empDistrict
                        from dis in empDistrict.DefaultIfEmpty()

                        join province in _context.Provinces
                        on coa.ProvinceId equals province.Id into empProvince
                        from pro in empProvince.DefaultIfEmpty()

                        join ward in _context.Wards
                        on coa.WardId equals ward.Id into empWard
                        from war in empWard.DefaultIfEmpty()

                        join districtNat in _context.Districts
                        on coa.NativeDistrictId equals districtNat.Id into empDistrictNat
                        from disNat in empDistrictNat.DefaultIfEmpty()

                        join provinceNat in _context.Provinces
                        on coa.NativeProvinceId equals provinceNat.Id into empProvinceNat
                        from proNat in empProvinceNat.DefaultIfEmpty()

                        join wardNat in _context.Wards
                        on coa.NativeWardId equals wardNat.Id into empWardNat
                        from warNat in empWardNat.DefaultIfEmpty()
                        where coa.IsDelete == false
                    select new UserModel()
                    {
                        Id = coa.Id,
                        Avatar = coa.Avatar,
                        FullName = coa.FullName ?? "",
                        Phone = coa.Phone ?? "",
                        Identify = coa.Identify,
                        Address = coa.Address,
                        BirthDay = coa.BirthDay,
                        Gender = coa.Gender,
                        RequestPassword = coa.RequestPassword,
                        Quit = coa.Quit,
                        TargetId = coa.TargetId ?? 0,
                        AddressFull = coa.Address + ", " + war.Name + ", " + dis.Name + ", " + pro.Name,
                        NativeAddressFull = warNat.Name + ", " + disNat.Name + ", " + proNat.Name,
                        Username = coa.Username ?? "",
                        Note = coa.Note,
                        UserRoleIds = coa.UserRoleIds,
                        Status = coa.Status,
                        PlaceOfPermanent = coa.PlaceOfPermanent,
                        Religion = coa.Religion,
                        EthnicGroup = coa.EthnicGroup,
                        UnionMember = coa.UnionMember ?? 0,
                        Nation = coa.Nation,
                        Literacy = coa.Literacy,
                        LiteracyDetail = coa.LiteracyDetail,
                        BankAccount = coa.BankAccount,
                        Bank = coa.Bank,
                        NoOfLeaveDate = coa.NoOfLeaveDate ?? 0,
                        ShareHolderCode = coa.ShareHolderCode,
                        Timekeeper = coa.Timekeeper ?? 0,
                        LicensePlates = coa.LicensePlates,
                    };


            var listItems = await users.ToListAsync();

            var listRole = _context.UserRoles?.ToDictionary(x => x.Id.ToString())
               ?? new Dictionary<string, UserRole>();

            List<string> roles = new List<string>();

            foreach (var item in listItems)
            {
                var roleIds = item.UserRoleIds?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[] { };
                
                roles.Clear();

                foreach (var roleId in roleIds)
                {
                    if (listRole.ContainsKey(roleId))
                    {
                        roles.Add(listRole[roleId].Title);
                    }
                }
                
                item.UserRoleName = string.Join(",", roles);
            }

            result.Data = listItems;
            return result;

        }
        public IEnumerable<UserModel> getListUserCommon()
        {
            return from coa in _context.Users
                   join district in _context.Districts
                   on coa.DistrictId equals district.Id into empDistrict
                   from dis in empDistrict.DefaultIfEmpty()

                   join province in _context.Provinces
                   on coa.ProvinceId equals province.Id into empProvince
                   from pro in empProvince.DefaultIfEmpty()

                   join ward in _context.Wards
                   on coa.WardId equals ward.Id into empWard
                   from war in empWard.DefaultIfEmpty()

                   join districtNat in _context.Districts
                   on coa.NativeDistrictId equals districtNat.Id into empDistrictNat
                   from disNat in empDistrictNat.DefaultIfEmpty()

                   join provinceNat in _context.Provinces
                   on coa.NativeProvinceId equals provinceNat.Id into empProvinceNat
                   from proNat in empProvinceNat.DefaultIfEmpty()

                   join wardNat in _context.Wards
                   on coa.NativeWardId equals wardNat.Id into empWardNat
                   from warNat in empWardNat.DefaultIfEmpty()

                   where coa.IsDelete == false

                   select new UserModel()
                   {
                       Id = coa.Id,
                       Avatar = coa.Avatar,
                       FullName = coa.FullName ?? "",
                       Phone = coa.Phone ?? "",
                       Identify = coa.Identify,
                       IdentifyCreatedDate = coa.IdentifyCreatedDate,
                       IdentifyCreatedPlace = coa.IdentifyCreatedPlace,
                       IdentifyExpiredDate = coa.IdentifyExpiredDate,
                       Email = coa.Email ?? "",
                       Address = coa.Address,
                       BirthDay = coa.BirthDay,
                       Gender = coa.Gender,
                       WarehouseId = coa.WarehouseId ?? 0,
                       DepartmentId = coa.DepartmentId ?? 0,
                       RequestPassword = coa.RequestPassword,
                       Quit = coa.Quit,
                       TargetId = coa.TargetId ?? 0,
                       AddressFull = coa.Address + ", " + war.Name + ", " + dis.Name + ", " + pro.Name,
                       NativeAddressFull = warNat.Name + ", " + disNat.Name + ", " + proNat.Name,
                             DistrictId = coa.DistrictId,
                             ProvinceId = coa.ProvinceId,
                             WardId = coa.WardId,
                             NativeDistrictId = coa.NativeDistrictId,
                             NativeProvinceId = coa.NativeProvinceId,
                             NativeWardId = coa.NativeWardId,


                             Username = coa.Username ?? "",
                             Language = coa.Language,
                             Note = coa.Note,
                             UserRoleIds = coa.UserRoleIds,
                             Facebook = coa.Facebook,
                             Salary = coa.Salary ?? 0,
                             SendSalaryDate = coa.SendSalaryDate,
                             Status = coa.Status,
                             PlaceOfPermanent = coa.PlaceOfPermanent,
                             Religion = coa.Religion,
                             EthnicGroup = coa.EthnicGroup,
                             UnionMember = coa.UnionMember ?? 0,
                             Nation = coa.Nation,
                             Literacy = coa.Literacy,
                             LiteracyDetail = coa.LiteracyDetail,
                             BankAccount = coa.BankAccount,
                             Bank = coa.Bank,
                             NoOfLeaveDate = coa.NoOfLeaveDate ?? 0,
                             ShareHolderCode = coa.ShareHolderCode,
                             PersonalTaxCode = coa.PersonalTaxCode,
                             SocialInsuranceCreated = coa.SocialInsuranceCreated,
                             SocialInsuranceCode = coa.SocialInsuranceCode,
                             LastLogin = coa.LastLogin,
                             Timekeeper = coa.Timekeeper ?? 0,
                             LicensePlates = coa.LicensePlates,
                             ContractTypeId = coa.ContractTypeId,
                         };
        }
        public User GetById(int id)
        {
            return _context.Users.AsNoTracking().Single(x => x.Id == id);
        }
        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException(ResultErrorEnum.USER_MISS_PASSWORD);



            if (_context.Users.Any(x => x.Username == user.Username && !x.IsDelete))
                throw new AppException(ResultErrorEnum.USER_USNEXIST);

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            try
            {
                user.Order = int.Parse(user.Username);
            }
            catch
            {
                user.Order = 0;
            }
            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }
        public User CreateExcel(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException(ResultErrorEnum.USER_MISS_PASSWORD);


            User userCheck = _context.Users.AsNoTracking().FirstOrDefault(x => x.Username == user.Username && !x.IsDelete);
            if (userCheck != null)
            {
                user.Id = userCheck.Id;
                Update(user, null);
                return user;
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }
        public User Update(User userParam, string password = null)
        {
            if (_context.Users.Any(x => x.Username == userParam.Username && !x.IsDelete && x.Id != userParam.Id))
                throw new AppException(ResultErrorEnum.USER_USNEXIST);

            var user = _context.Users.AsNoTracking().FirstOrDefault(x => x.Id == userParam.Id);
            if (user == null)
                throw new AppException(ResultErrorEnum.USER_EMPTY_OR_DELETE);

            if (string.IsNullOrWhiteSpace(userParam.FullName))
                userParam.FullName = user.FullName;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                userParam.PasswordHash = passwordHash;
                userParam.PasswordSalt = passwordSalt;
            }
            else
            {
                userParam.PasswordHash = user.PasswordHash;
                userParam.PasswordSalt = user.PasswordSalt;
            }
            userParam.UpdatedAt = DateTime.Now;
            if (userParam.Username != user.Username)
            {
                try
                {
                    user.Order = int.Parse(user.Username);
                }
                catch
                {
                    user.Order = 0;
                }
            }
            _context.Users.Update(userParam);
            _context.SaveChanges();
            return userParam;
        }

        public void ResetPassword(User userParam, string password = null)
        {

            var user = _context.Users.AsNoTracking().Single(x => x.Id == userParam.Id );
            var submitUser = new User();
            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            //submitUser = _mapper.Map<User>(userParam);
            submitUser.PasswordHash = user.PasswordHash;
            submitUser.PasswordSalt = user.PasswordSalt;
            submitUser.RequestPassword = userParam.RequestPassword;

            _context.Users.Update(submitUser);
            _context.SaveChanges();
        }

        public bool CheckPassword(int id, string oldPassword)
        {

            var user = _context.Users.AsNoTracking().Single(x => x.Id == id);
            if (user == null) return false;
            return VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt);
        }

        public void UpdatePassword(PasswordModel passwordModel)
        {

            var user = _context.Users.AsNoTracking().Single(x => x.Id == passwordModel.Id);
            // update password if provided
            if (!string.IsNullOrWhiteSpace(passwordModel.Password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(passwordModel.Password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _context.Users.Update(user);
                _context.SaveChanges();
            }
        }

        public void UpdateLastLogin(int userId)
        {
            var user = _context.Users.Find(userId);

            if (user == null)
                throw new AppException(ResultErrorEnum.USER_EMPTY_OR_DELETE);
            user.LastLogin = DateTime.Now;
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.IsDelete = true;
                _context.Users.Update(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public int Count()
        {
            return _context.Users.Count(x => !x.IsDelete);
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
        public List<UserModel> GetForExcel(List<int> ids)
        {
            var data = getListUserCommon().ToList();
            data = data.Where(x => ids.Contains(x.Id)).ToList();
            return data;
        }

        public async Task<string> GetUserName()
        {
            try
            {
                int stt = _context.Users.OrderByDescending(x => x.Order).FirstOrDefault().Order + 1;
                string userName = stt.ToString();
                while (userName.Length < 6)
                {
                    userName = "0" + userName;
                }
                return userName;
            }
            catch
            {
                return "00001";
            }
        }

    }
}