using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ManageEmployee.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Services;
using ManageEmployee.Entities;
using System.Linq;
using ManageEmployee.EnumList;
using System;
using System.Security.Claims;
using ManageEmployee.ViewModels;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]

    public class CompaniesController : ControllerBase
    {
        private ICompanyService _companyService;
        private IFileService _fileService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public CompaniesController(
            ICompanyService companyService,
            IFileService fileService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _companyService = companyService;
            _fileService = fileService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [HttpGet("get-company")]
        public IActionResult GetCompany()
        {
            var company = _companyService.GetCompany();
            return new JsonResult(new BaseResponseModel
            {
                Data = company,
            });
        }

        [HttpGet]
        public IActionResult GetLogCompany([FromQuery] PagingationRequestModel model)
        {
            var companyLogs = _companyService
                .GetAll(model.Page, model.PageSize);
            return new JsonResult(new BaseResponseModel
            {
                Data = companyLogs,
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var model = _companyService.GetById(id);
            return Ok(model);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Company model)
        {
            try
            {
                // Update userUpdated Fail.- reason fail convert to string
                //if (HttpContext.User.Identity is ClaimsIdentity identity)
                //{
                //    company.UserUpdated = Int32.Parse(identity.FindFirst(ClaimTypes.Name).Value);
                //}
                var companyResult = _companyService.Create(model);
                return Ok();
                // update user
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromBody] CompanyViewModel model)
        {
            var company = _companyService.GetById(model.Id);
            if(company == null)
            {
                return BadRequest(new { msg = "Company not found" });
            }
            try
            {
                // Update userUpdated Fail.- reason fail convert to string
                //if (HttpContext.User.Identity is ClaimsIdentity identity)
                //{
                //    company.UserUpdated = Int32.Parse(identity.FindFirst(ClaimTypes.Name).Value);
                //}

                #region "Update if exist field"

                if (!String.IsNullOrEmpty(model.Name))
                {
                    company.Name = model.Name;
                }

                if (!String.IsNullOrEmpty(model.Address))
                {
                    company.Address = model.Address;
                }

                if (!String.IsNullOrEmpty(model.Phone))
                {
                    company.Phone = model.Phone;
                }

                if (!String.IsNullOrEmpty(model.MST))
                {
                    company.MST = model.MST;
                }

                if (!String.IsNullOrEmpty(model.Email))
                {
                    company.Email = model.Email;
                }

                if (!String.IsNullOrEmpty(model.Fax))
                {
                    company.Fax = model.Fax;
                }

                if (!String.IsNullOrEmpty(model.WebsiteName))
                {
                    company.WebsiteName = model.WebsiteName;
                }

                if (!String.IsNullOrEmpty(model.NameOfCEO))
                {
                    company.NameOfCEO = model.NameOfCEO;
                }

                if (!String.IsNullOrEmpty(model.NoteOfCEO))
                {
                    company.NoteOfCEO = model.NoteOfCEO;
                }

                if (!String.IsNullOrEmpty(model.NameOfChiefAccountant))
                {
                    company.NameOfChiefAccountant = model.NameOfChiefAccountant;
                }

                if (!String.IsNullOrEmpty(model.NoteOfChiefAccountant))
                {
                    company.NoteOfChiefAccountant = model.NoteOfChiefAccountant;
                }

                if (!String.IsNullOrEmpty(model.NameOfTreasurer))
                {
                    company.NameOfTreasurer = model.NameOfTreasurer;
                }

                if (!String.IsNullOrEmpty(model.NameOfStorekeeper))
                {
                    company.NameOfStorekeeper = model.NameOfStorekeeper;
                }

                if (!String.IsNullOrEmpty(model.NameOfChiefSupplier))
                {
                    company.NameOfChiefSupplier = model.NameOfChiefSupplier;
                }

                if (!String.IsNullOrEmpty(model.NoteOfChiefSupplier))
                {
                    company.NoteOfChiefSupplier = model.NoteOfChiefSupplier;
                }

                if (!String.IsNullOrEmpty(model.AssignPerson))
                {
                    company.AssignPerson = model.AssignPerson;
                }

                if (model.CharterCapital != null)
                {
                    company.CharterCapital = (double)model.CharterCapital;
                }

                if (!String.IsNullOrEmpty(model.FileOfBusinessRegistrationCertificate))
                {
                    company.FileOfBusinessRegistrationCertificate = model.FileOfBusinessRegistrationCertificate;
                }

                if (!String.IsNullOrEmpty(model.FileLogo))
                {
                    company.FileLogo = model.FileLogo;
                }

                if (model.BusinessType != null)
                {
                    company.BusinessType = (int)model.BusinessType;
                }

                if (model.AccordingAccountingRegime != null)
                {
                    company.AccordingAccountingRegime = (int)model.AccordingAccountingRegime;
                }

                if (model.MethodCalcExportPrice != null)
                {
                    company.MethodCalcExportPrice = (int)model.MethodCalcExportPrice;
                }

                if (!String.IsNullOrEmpty(model.Note))
                {
                    company.Note = model.Note;
                }

                if (model.Quatity != null)
                {
                    company.Quatity = (int)model.Quatity;
                }

                if (model.UnitCost != null)
                {
                    company.UnitCost = (int)model.UnitCost;
                }

                if (model.Money != null)
                {
                    company.Money = (int)model.Money;
                }

                if (model.Currency != null)
                {
                    company.Currency = (int)model.Currency;
                }

                if (!String.IsNullOrEmpty(model.DayType))
                {
                    company.DayType = model.DayType;
                }

                if (!String.IsNullOrEmpty(model.DecimalUnit))
                {
                    company.DecimalUnit = model.DecimalUnit;
                }

                if (!String.IsNullOrEmpty(model.DecimalRate))
                {
                    company.DecimalRate = model.DecimalRate;
                }

                if (!String.IsNullOrEmpty(model.ThousandUnit))
                {
                    company.ThousandUnit = model.ThousandUnit;
                }

                if (model.SignDate != DateTime.MinValue)
                {
                    company.SignDate = model.SignDate;
                }

                #endregion

                var companyResult = _companyService.Update(company);
                return Ok(companyResult);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _companyService.Delete(id);
            return Ok();
        }
    }
}
