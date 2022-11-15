using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.EnumList;
using ManageEmployee.Enum;

namespace ManageEmployee.Services
{
    public interface ICompanyService
    {
        List<Company> GetAll(int currentPage, int pageSize);
        Company GetCompany();
        Company GetById(int id);
        Company Create(Company param);
        Company Update(Company param);
        void Delete(int id);
    }

    public class CompanyService : ICompanyService
    {
        private ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }
        public Company GetCompany()
        {
            return _context.Companies.OrderByDescending(x => x.SignDate).FirstOrDefault();
        }
        public Company GetById(int id)
        {
            return _context.Companies.Find(id);
        }
        public Company Update(Company param)
        {
            // don't need compare last update and current last update in company logs
            //var lastUpdate = _context.CompanyLogs
            //    .Where(x => x.Id != param.Id)
            //    .OrderByDescending(x => x.UpdateAt)
            //    .AsNoTracking()
            //    .FirstOrDefault();

            //if (lastUpdate != null && param.UpdateAt.Date < lastUpdate.UpdateAt.Date)
            //{
            //    param = _mapper.Map(lastUpdate, param);
            //}

            var company = _context.Companies.FirstOrDefault();

            if (company == null)
                throw new AppException(ResultErrorEnum.MODEL_NULL);

            company.Name = param.Name;
            company.Address = param.Address;
            company.Phone = param.Phone;
            company.MST = param.MST;
            company.Email = param.Email;
            company.Fax = param.Fax;
            company.WebsiteName = param.WebsiteName;
            
            company.NameOfCEO = param.NameOfCEO;
            company.NoteOfCEO = param.NoteOfCEO;
            company.NameOfChiefAccountant = param.NameOfChiefAccountant;
            company.NoteOfChiefAccountant = param.NoteOfChiefAccountant;
            company.NameOfTreasurer = param.NameOfTreasurer;
            company.NameOfStorekeeper = param.NameOfStorekeeper;
            company.NameOfChiefSupplier = param.NameOfChiefSupplier;
            company.NoteOfChiefSupplier = param.NoteOfChiefSupplier;
            if (!String.IsNullOrEmpty(param.FileOfBusinessRegistrationCertificate))
            {
                company.FileOfBusinessRegistrationCertificate = param.FileOfBusinessRegistrationCertificate;
            }
            company.AssignPerson = param.AssignPerson;
            company.SignDate = param.SignDate;
            company.CharterCapital = param.CharterCapital;
            company.BusinessType = param.BusinessType;
            company.AccordingAccountingRegime = param.AccordingAccountingRegime;
            company.MethodCalcExportPrice = param.MethodCalcExportPrice;
            company.Note = param.Note;
            if (!String.IsNullOrEmpty(param.FileLogo))
            {
                company.FileLogo = param.FileLogo;
            }

            company.UpdateAt = param.UpdateAt;
            company.UserUpdated = param.UserUpdated;

            _context.Companies.Update(company);
            _context.SaveChanges();


            return company;
        }

        //

        public List<Company> GetAll(int currentPage, int pageSize)
        {
            List<Company> listCompany = _context.Companies
                .AsNoTracking()
                .OrderByDescending(x => x.SignDate)
                .Skip(pageSize * (currentPage - 1))
                // old .Skip(pageSize * currentPage)
                // =>> new .Skip(pageSize * (currentPage - 1))
                .Take(pageSize).ToList();
            return listCompany;
        }

        public Company Create(Company param)
        {
            var itemExist = _context
                .Companies
                .AsNoTracking()
                .SingleOrDefault(x => x.Id == param.Id);

            if (itemExist != null)
            {
                if (string.IsNullOrEmpty(param.FileOfBusinessRegistrationCertificate))
                {
                    param.FileOfBusinessRegistrationCertificate = itemExist.FileOfBusinessRegistrationCertificate;
                }

                if (string.IsNullOrEmpty(param.FileLogo))
                {
                    param.FileLogo = itemExist.FileLogo;
                }

                _context.Entry(param).State = EntityState.Detached;
                _context.Companies.Add(param);
            }
            else
            {
                param.Id = 0;
                _context.Companies.Add(param);
            }

            _context.SaveChanges();
            return param;
        }

        public void Delete(int id)
        {
            var companyLog = _context.Companies.Find(id);
            if (companyLog != null)
            {
                _context.Companies.Remove(companyLog);
                _context.SaveChanges();
            }
        }
    }
}