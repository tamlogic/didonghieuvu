using ManageEmployee.Entities;
using ManageEmployee.Enum;
using ManageEmployee.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ManageEmployee.Services
{
    public interface IWardService
    {
        IEnumerable<Ward> GetAll();
        IEnumerable<Ward> GetAllByDistrictId(int districtId);
        IEnumerable<Ward> GetAll(int currentPage, int pageSize);
        Ward GetById(int id);
        Ward Create(Ward param);
        void Update(Ward param);
        void Delete(int id);
        int Count(Expression<Func<Ward, bool>> where=null);
    }

    public class WardService : IWardService
    {
        private ApplicationDbContext _context;

        public WardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Ward> GetAll(int currentPage, int pageSize)
        {
            return _context.Wards
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.SortCode)
                .Skip(pageSize * (currentPage - 1))
                .Take(pageSize);
        }
        public IEnumerable<Ward> GetAll()
        {
            return _context.Wards
                .Where(x => !x.IsDeleted);
        }
        public IEnumerable<Ward> GetAllByDistrictId(int districtId)
        {
            return _context.Wards
                .Where(x => !x.IsDeleted && x.DistrictId == districtId)
                .OrderBy(x => x.SortCode);
        }

        public Ward GetById(int id)
        {
            return _context.Wards.Find(id);
        }

        public Ward Create(Ward param)
        {
            if (string.IsNullOrWhiteSpace(param.Name))
                throw new AppException(ResultErrorEnum.MODEL_MISS);

            _context.Wards.Add(param);
            _context.SaveChanges();

            return param;
        }

        public void Update(Ward param)
        {
            var ward = _context.Wards.Find(param.Id);

            if (ward == null)
                throw new AppException(ResultErrorEnum.MODEL_NULL);

            ward.Name = param.Name;
            ward.Type = param.Type;
            ward.DistrictId = param.DistrictId;
            ward.SortCode = param.SortCode;

            _context.Wards.Update(ward);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var ward = _context.Wards.Find(id);
            if (ward != null)
            {
                ward.IsDeleted = true;
                _context.Wards.Update(ward);
                _context.SaveChanges();
            }
        }

        public int Count(Expression<Func<Ward, bool>> where = null)
        {
            if(where != null)
            {
                return _context.Wards.Where(where).Count(x => !x.IsDeleted);
            }
            return _context.Wards.Count(x => !x.IsDeleted);

        }
    }
}