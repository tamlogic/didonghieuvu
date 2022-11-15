using ManageEmployee.Entities;
using ManageEmployee.Enum;
using ManageEmployee.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ManageEmployee.Services
{
    public interface IDistrictService
    {
        IEnumerable<District> GetAll();
        IEnumerable<District> GetAllByProvinceId(int provinceId);
        IEnumerable<District> GetAll(int currentPage, int pageSize);
        IEnumerable<District> GetMany(Expression<Func<District, bool>> where, int currentPage, int pageSize);
        IEnumerable<District> GetMany(Expression<Func<District, bool>> where);
        District GetById(int id);
        District Create(District param);
        void Update(District param);
        void Delete(int id);
        int Count(Expression<Func<District, bool>> where = null);
    }

    public class DistrictService : IDistrictService
    {
        private ApplicationDbContext _context;

        public DistrictService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<District> GetAll(int currentPage, int pageSize)
        {
            return _context.Districts
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SortCode)
                .Skip(pageSize * (currentPage - 1))
                .Take(pageSize);
        }
        public IEnumerable<District> GetAll()
        {
            return _context.Districts
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SortCode);
        }

        public IEnumerable<District> GetAllByProvinceId(int provinceId)
        {
            return _context.Districts
                .Where(x => !x.IsDeleted && x.ProvinceId == provinceId)
                .OrderBy(x => x.SortCode);
        }

        public IEnumerable<District> GetMany(Expression<Func<District, bool>> where, int currentPage, int pageSize)
        {
            if (pageSize == 0)
            {
                return _context.Districts
                     .Where(where)
                     .OrderBy(x => x.SortCode).ThenBy(x=>x.Name);
            }
            return _context.Districts
                .Where(where)
                .OrderBy(x => x.SortCode)
                .ThenBy(x => x.Name)
                .Skip(pageSize * currentPage)
                .Take(pageSize);
        }

        public IEnumerable<District> GetMany(Expression<Func<District, bool>> where)
        {
            return _context.Districts
                .Where(where);
        }


        public District GetById(int id)
        {
            return _context.Districts.Find(id);
        }

        public District Create(District param)
        {
            if (string.IsNullOrWhiteSpace(param.Name))
                throw new AppException(ResultErrorEnum.MODEL_MISS);

            _context.Districts.Add(param);
            _context.SaveChanges();

            return param;
        }

        public void Update(District param)
        {
            var district = _context.Districts.Find(param.Id);

            if (district == null)
                throw new AppException(ResultErrorEnum.MODEL_NULL);

            district.Name = param.Name;
            district.Type = param.Type;
            district.ProvinceId = param.ProvinceId;
            district.SortCode = param.SortCode;
            _context.Districts.Update(district);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var district = _context.Districts.Find(id);
            if (district != null)
            {
                district.IsDeleted = true;
                _context.Districts.Update(district);
                _context.SaveChanges();
            }
        }

        public int Count(Expression<Func<District, bool>> where = null)
        {
            if (where != null)
            {
                return _context.Districts.Where(where).Count(x => !x.IsDeleted);
            }
            return _context.Districts.Count(x => !x.IsDeleted);
        }
    }
}