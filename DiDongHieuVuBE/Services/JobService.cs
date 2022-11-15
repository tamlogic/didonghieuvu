using ManageEmployee.Entities;
using ManageEmployee.Enum;
using ManageEmployee.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ManageEmployee.Services
{
    public interface IJobService
    {
        IEnumerable<Job> GetAll();
        IEnumerable<Job> GetAll(int currentPage, int pageSize,string keyword);
        Job GetById(int id);
        Task<string> Create(Job param);
        Task<string> Update(Job param);
        int Count(string keyword);
        int Count(Expression<Func<Job, bool>> where = null);

        string Delete(int id);
    }
    public class JobService : IJobService
    {
        private ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Job> GetAll()
        {
            return _context.Jobs
                .Where(x => !x.IsDelete)
                .OrderBy(x => x.Name);
        }

        public IEnumerable<Job> GetAll(int currentPage, int pageSize,string keyword)
        {

            var query = _context.Jobs
                .Where(x => !x.IsDelete);
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                         x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                         x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                         x.Description.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                         x.Description.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                         x.Description.Trim().ToLower().Equals(keyword.Trim().ToLower())
                );
            }


            return query
                .Skip(pageSize * currentPage)
                .Take(pageSize);
        }

        public Job GetById(int id)
        {
            return _context.Jobs.Find(id);
        }

        public async Task<string> Create(Job param)
        {
            {
                if (_context.Jobs.Where(u => u.Name == param.Name).Any())
                {
                    throw new AppException(ResultErrorEnum.CODE_EXIST);
                }

                if (string.IsNullOrWhiteSpace(param.Name))
                    throw new AppException(ResultErrorEnum.MODEL_MISS);

                _context.Jobs.Add(param);
                _context.SaveChanges();

                return string.Empty;
            }
        }

        public async Task<string> Update(Job param)
        {
            var job = _context.Jobs.Find(param.Id);

            if (job == null)
                throw new AppException(ResultErrorEnum.MODEL_NULL);

            job.Name = param.Name;
            job.Description = param.Description;
            job.Status = param.Status;
            job.CompanyId = param.CompanyId;
            job.Color = param.Color;


            job.UpdatedAt = DateTime.Now;
            job.UpdatedBy = param.UpdatedBy;

            _context.Jobs.Update(job);
            _context.SaveChanges();

            return string.Empty;
        }

        public int Count(Expression<Func<Job, bool>> where = null)
        {
            if (where != null)
            {
                return _context.Jobs.Where(where).Count(x => !x.IsDelete);
            }
            return _context.Jobs.Count(x => !x.IsDelete);

        }
        public int Count(string keyword)
        {
            var query = _context.Jobs.Where(x => !x.IsDelete);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                         x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                         x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                         x.Description.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                         x.Description.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                         x.Description.Trim().ToLower().Equals(keyword.Trim().ToLower())
                );
            }
            return query.Count();

        }

        public string Delete(int id)
        {
            try
            {
                 _context.Database.BeginTransactionAsync();
                var job = _context.Jobs.Find(id);
                if (job != null)
                {
                    _context.Jobs.Remove(job);
                }
                 _context.SaveChanges();
                _context.Database.CommitTransaction();
                return string.Empty;
            }
            catch
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }
    }
}
