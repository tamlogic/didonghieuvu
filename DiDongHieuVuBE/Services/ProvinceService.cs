using ManageEmployee.Entities;
using ManageEmployee.Enum;
using ManageEmployee.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ManageEmployee.Services
{
    public interface IProvinceService
    {
        IEnumerable<Province> GetAll();
        IEnumerable<Province> GetAll(int currentPage, int pageSize);
        Province GetById(int id);
        Province Create(Province param);
        void Update(Province param);
        void Delete(int id);
        int Count();
    }

    public class ProvinceService : IProvinceService
    {
        private ApplicationDbContext _context;

        public ProvinceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Province> GetAll(int currentPage, int pageSize)
        {
            if (pageSize == 0)
            {
                return _context.Provinces
                              .Where(x => !x.IsDeleted)
                              .OrderBy(x => x.SortCode)
                              .ThenBy(x => x.Name);
            }
            return _context.Provinces
                 .Where(x => !x.IsDeleted)
                 .OrderBy(x => x.SortCode)
                 .ThenBy(x => x.Name)
                 .Skip(pageSize * (currentPage - 1))
                 .Take(pageSize);
        }
        public IEnumerable<Province> GetAll()
        {
            return _context.Provinces
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SortCode);
        }

        public Province GetById(int id)
        {
            return _context.Provinces.Find(id);
        }

        public Province Create(Province param)
        {
            if (string.IsNullOrWhiteSpace(param.Name))
                throw new AppException(ResultErrorEnum.MODEL_MISS);

            _context.Provinces.Add(param);
            _context.SaveChanges();

            return param;
        }

        public void Update(Province param)
        {
            var province = _context.Provinces.Find(param.Id);

            if (province == null)
                throw new AppException(ResultErrorEnum.MODEL_NULL);

            province.Name = param.Name;
            province.Type = param.Type;
            province.ZipCode = param.ZipCode;
            province.SortCode = param.SortCode;

            _context.Provinces.Update(province);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var province = _context.Provinces.Find(id);
            if (province != null)
            {
                province.IsDeleted = true;
                _context.Provinces.Update(province);
                _context.SaveChanges();
            }
        }

        public int Count()
        {
            return _context.Provinces.Count(x => !x.IsDeleted);

        }
    }
}