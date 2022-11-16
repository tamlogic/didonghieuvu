using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Security.Claims;
using System.Linq;
using System;
using System.Drawing;
using ManageEmployee.Services;
using ManageEmployee.Helpers;
using ManageEmployee.Entities;
using ManageEmployee.ViewModels;
using ManageEmployee.Enum;
using OfficeOpenXml;
using ManageEmployee.EnumList;
using Newtonsoft.Json;
using ManageEmployee.Models;

namespace ManageEmployee.Controllers
{
	[Authorize]
	[ApiController]
    [Route("api/[controller]")]

    public class UsersController : ControllerBase
	{
		private IUserService _userService;
		private IMapper _mapper;
		private readonly AppSettings _appSettings;
		private IFileService _fileService;
        private readonly IDistrictService _districtService;
        private readonly IWardService _wardService;
        private readonly IProvinceService _provinceService;
		private ApplicationDbContext _context;

		public UsersController(
			IUserService userService,
			IMapper mapper,
			IOptions<AppSettings> appSettings,
			IFileService fileService,
			IDistrictService districtService,
			IWardService wardService,
			IProvinceService provinceService,
			ApplicationDbContext context
			)
		{
			_userService = userService;
			_mapper = mapper;
			_appSettings = appSettings.Value;
			_fileService = fileService;
            _districtService = districtService;
            _wardService = wardService;
            _provinceService = provinceService;
			_context = context;
        }


		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] UserViewModel param)
		{
           string roles = "";
           int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
                userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
            List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

            return Ok(await _userService.CountFilter(new UserMapper.FilterParams()
            {
                BirthDay = param.Birthday,
                Gender = param.Gender,
                Keyword = param.SearchText,
                PositionId = param.Positionid,
                WarehouseId = param.Warehouseid,
                DepartmentId = param.Departmentid,
                RequestPassword = param.RequestPassword,
                Quit = param.Quit,
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                StartDate = param.StartDate,
                EndDate = param.EndDate,
                TargetId = param.Targetid,
                Month = param.Month,
                DegreeId = param.Degreeid ?? 0,
                CertificateId = param.Certificateid ?? 0,
                Ids = param.Ids,
                roles = listRole,
                UserId = userId
            }));
		}

		[HttpGet("{id}")]
		public IActionResult GetById(int id)
		{
			var user = _userService.GetById(id);
			user.PasswordHash = new byte[]{ };
			user.PasswordSalt = new byte[]{ };
			return Ok(user);
		}


		[HttpGet("get-total-reset-pass")]
		public IActionResult GetTotalResetPass()
		{
			var total = _userService.GetMany(x => !x.IsDelete && x.RequestPassword).Count();
			return Ok(total);
		}


        [HttpPut("{id}")]
        [HttpPost]
        public IActionResult Update([FromBody] UserModel model, int id = 0)
        {
            //if (model.BirthDayUnix > 0)
            //    model.BirthDay = DateHelpers.UnixTimeStampToDateTime(model.BirthDayUnix);

            //if (model.SendSalaryDateUnix > 0)
            //    model.SendSalaryDate = DateHelpers.UnixTimeStampToDateTime(model.SendSalaryDateUnix);

            // map model to entity and set id
            var user = _mapper.Map<User>(model);
            user.Id = id;

            try
            {
                if (HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    user.UpdatedBy = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                    user.CreatedBy = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                }

                if (user.Id != 0)
                {
                    var res = _userService.Update(user, model.Password);
                    return Ok(res);
                }
                else
                {
                    var res = _userService.Create(user, model.Password ?? "123456");
                    res.PasswordHash = new byte[] { };
                    res.PasswordSalt = new byte[] { };
                    return Ok(res);
                }
                // update user


            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { msg = ex.Message });
            }
        }


        [HttpPost("resetPassword/{id}")]
        public IActionResult ResetPassword(int id, [FromBody] UserModel model)
        {
            // map model to entity and set id
            model.RequestPassword = false;
            var user = _mapper.Map<User>(model);
            user.Id = id;

            try
            {

                if (user.Id != 0)
                {
                    _userService.ResetPassword(user, "123456");
                }
                // update user

                return Ok();
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
            _userService.Delete(id);
            return Ok();
        }


        [HttpPut("uploadAvatar/{id}")]
        public IActionResult UploadAvatar(int id, [FromForm] IFormFile file)
        {
            try
            {
                var user = _userService.GetById(id);
                if (user != null && !user.IsDelete)
                {
                    if (!string.IsNullOrEmpty(user.Avatar))
                    {
                        _fileService.DeleteFileUpload(user.Avatar);
                    }
                    var fileName = _fileService.Upload(file, "user_avatar");
                    user.Avatar = fileName;
                    _userService.Update(user);
                    return new JsonResult(new
                    {
                        id = id,
                        Avatar = fileName
                    });
                }
                else
                {
                    return BadRequest(new { msg = ResultErrorEnum.USER_EMPTY_OR_DELETE });
                }
            }
            catch (System.Exception ex)
            {

                return BadRequest(new { msg = ex.Message });
            }


        }


        //[HttpPost("export")]
        //public ActionResult Export([FromBody] List<int> ids, [FromQuery] bool allowImages = false)
        //{
        //	List<UserModel> datas = _userService.GetForExcel(ids);
        //	if (datas.Any())
        //	{
        //		string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/ThongTinNhanVien.xlsx");
        //		MemoryStream stream = new MemoryStream();

        //		var listDepartment = _context.Departments.Where(x => x.IsDelete == false).ToList();
        //		var listPosition = _context.Positions.Where(x => x.IsDelete == false).ToList();
        //		var listDegree = _context.Degrees.Where(x => x.IsDelete == false).ToList();
        //		var listMajor = _context.Majors.Where(x => x.IsDelete == false).ToList();

        //		using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        //		{
        //			using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
        //			{
        //				ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];

        //				sheet.DefaultColWidth = 10.0;
        //				sheet.DefaultRowHeight = 39.00D;
        //				int nRowBegin = 10, nCol = 32;
        //				int rowIdx = 11;

        //				foreach (UserModel lo in datas)
        //				{
        //					sheet.Cells.Style.WrapText = true;
        //					sheet.Cells[rowIdx, 1].Value = rowIdx - 10;
        //					try
        //					{
        //						if (!String.IsNullOrEmpty(lo.Avatar) && allowImages)
        //						{
        //							var pathAvatar = Path.Combine(Directory.GetCurrentDirectory(), lo.Avatar);
        //							System.Drawing.Image img = System.Drawing.Image.FromFile(pathAvatar);
        //							ExcelPicture pic = sheet.Drawings.AddPicture("Avatar" + rowIdx, new FileInfo(pathAvatar));
        //							pic.SetPosition(rowIdx - 1, 10,	1, 28);
        //							pic.SetSize(80, 80);
        //							sheet.Row(rowIdx).Height = 75;
        //						}
        //					}
        //					catch (Exception ex)
        //					{
        //						sheet.Cells[rowIdx, 2].Value = "Hình đã bị xóa";
        //					}
        //					sheet.Cells[rowIdx, 3].Value = lo.Code;
        //					sheet.Cells[rowIdx, 4].Value = lo.Username;
        //					sheet.Cells[rowIdx, 5].Value = lo.FullName;

        //					var pos = listPosition.FirstOrDefault(x => x.Id == lo.PositionId);
        //					if (pos != null)
        //						sheet.Cells[rowIdx, 6].Value = pos.Name;

        //					var dep = listDepartment.FirstOrDefault(x => x.Id == lo.DepartmentId);
        //					if (dep != null)
        //						sheet.Cells[rowIdx, 7].Value = dep.Name;
        //					var dep_excel = sheet.Cells[rowIdx, 7].DataValidation.AddListDataValidation();
        //					foreach (var type in listDepartment)
        //					{
        //						dep_excel.Formula.Values.Add(type.Name);
        //					}

        //					GenderEnum gioiTinh = lo.Gender;
        //					if (gioiTinh == GenderEnum.Male)
        //					{
        //						sheet.Cells[rowIdx, 8].Value = "x";
        //					}
        //					else if (gioiTinh == GenderEnum.Female)
        //					{
        //						sheet.Cells[rowIdx, 9].Value = "x";
        //					}
        //					sheet.Cells[rowIdx, 10].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("dd") : "";
        //					sheet.Cells[rowIdx, 11].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("MM") : "";
        //					sheet.Cells[rowIdx, 12].Value = lo.BirthDay.HasValue ? lo.BirthDay.Value.ToString("yyyy") : ""; 
        //					sheet.Cells[rowIdx, 13].Value = lo.PlaceOfPermanent + ", " + lo.NativeWardName + ", " + lo.NativeDistrictName + ", " + lo.NativeProvinceName;
        //					sheet.Cells[rowIdx, 14].Value = lo.Address;
        //					sheet.Cells[rowIdx, 15].Value = lo.WardName;
        //					sheet.Cells[rowIdx, 16].Value = lo.DistrictName;
        //					sheet.Cells[rowIdx, 17].Value = lo.ProvinceName;
        //					sheet.Cells[rowIdx, 18].Value = lo.EthnicGroup;
        //					sheet.Cells[rowIdx, 19].Value = lo.Religion;
        //					int unionMember = lo.UnionMember;
        //					if (unionMember == 0)
        //					{
        //						sheet.Cells[rowIdx, 20].Value = "x";
        //					}
        //					else if (unionMember == 1)
        //					{
        //						sheet.Cells[rowIdx, 21].Value = "x";
        //					}
        //					sheet.Cells[rowIdx, 22].Value = lo.Phone;
        //					sheet.Cells[rowIdx, 23].Value = lo.SocialInsuranceCode;
        //					sheet.Cells[rowIdx, 24].Value = lo.Identify;
        //					sheet.Cells[rowIdx, 25].Value = lo.IdentifyCreatedDate.HasValue ? lo.IdentifyCreatedDate.Value.ToString("dd/MM/yyyy") : "";
        //					sheet.Cells[rowIdx, 26].Value = lo.IdentifyCreatedPlace;
        //					sheet.Cells[rowIdx, 27].Value = lo.Literacy;

        //					if (!string.IsNullOrEmpty(lo.LiteracyDetail))
        //					{
        //						List<string> degrees = lo.LiteracyDetail.Split(',').ToList();
        //						lo.LiteracyDetail = "";
        //						degrees = degrees.Where(x => !string.IsNullOrEmpty(x)).ToList();
        //						foreach (string degree in degrees)
        //						{
        //							int degreeID = int.Parse(degree);
        //							var degr = listDegree.FirstOrDefault(x => x.Id == degreeID);
        //							if (degr != null)
        //								lo.LiteracyDetail += degr.Name + ", ";
        //						}
        //					}
        //					sheet.Cells[rowIdx, 28].Value = lo.LiteracyDetail;

        //					var degree_excel = sheet.Cells[rowIdx, 28].DataValidation.AddListDataValidation();
        //					foreach (var type in listDegree)
        //					{
        //						degree_excel.Formula.Values.Add(type.Name);
        //					}
        //					//
        //					if (!string.IsNullOrEmpty(lo.Specialize))
        //					{
        //						List<string> degrees = lo.Specialize.Split(',').ToList();
        //						lo.Specialize = "";
        //						degrees = degrees.Where(x => !string.IsNullOrEmpty(x)).ToList();
        //						foreach (string degree in degrees)
        //						{
        //							int degreeID = int.Parse(degree);
        //							var degr = listMajor.FirstOrDefault(x => x.Id == degreeID);
        //							if (degr != null)
        //								lo.Specialize += degr.Name + ", ";
        //						}

        //					}
        //					sheet.Cells[rowIdx, 29].Value = lo.Specialize;
        //					var major_excel = sheet.Cells[rowIdx, 29].DataValidation.AddListDataValidation();
        //					foreach (var type in listMajor)
        //					{
        //						major_excel.Formula.Values.Add(type.Name);
        //					}
        //					//
        //					sheet.Cells[rowIdx, 30].Value = lo.SendSalaryDate.HasValue ? lo.SendSalaryDate.Value.ToString("dd/MM/yyyy") : "";
        //					sheet.Cells[rowIdx, 31].Value = lo.Salary;
        //					sheet.Cells[rowIdx, 32].Value = lo.Note;
        //					rowIdx++;
        //				}
        //				rowIdx--;
        //				if (!allowImages)
        //                      {
        //					sheet.DeleteColumn(2);
        //                      }
        //				if (rowIdx >= nRowBegin)
        //				{
        //					//sheet.Cells[nRowBegin, 4, rowIdx, 6].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

        //					sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
        //					sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
        //					sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
        //					sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

        //					sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        //					sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        //					sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        //					sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        //				}
        //				package.SaveAs(stream);
        //			}
        //		}
        //		stream.Seek(0L, SeekOrigin.Begin);
        //		string fileName = string.Format("ThongtinNV_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
        //		Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        //		return this.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        //	}
        //	else
        //	{
        //		return BadRequest(new { msg = "Không có dữ liệu xuất file" });
        //	}
        //}

        [HttpPost("getallusername")]
        public ActionResult GetAllUserName()
        {
            var usernames = _userService.GetAllUserName();
            return new JsonResult(usernames); ;
        }

        [HttpGet("getAllUserActive")]
        public IActionResult GetAllUserActive()
        {
            var result = _userService.GetAllUserActive();
            return new JsonResult(new BaseResponseModel
            {
                Data = result,
            });
        }

        //[HttpPost("printPDF")]
        //public ActionResult CreateDocx(int id)
        //{
        //    User user = _userService.GetById(id);
        //    // Placeholders and sample data:
        //    (string placeholder, string data)[] placeholderData =
        //    {
        //        ("__%c_name%__", user.FullName),
        //        ("__%c_birthday%__", user.BirthDay?.ToString("dd/MM/yyyy")),
        //        ("__%c_phone%__", user.Nation),
        //        ("__%c_url%__", user.Specialize),
        //        ("__%c_address%__", user.Identify),
        //        ("__%to_name%__", user.PersonalTaxCode),
        //    };
        //    string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Pdf/The-Nhan-Vien.docx");

        //    XWPFDocument doc = new XWPFDocument(OPCPackage.Open(path));


        //    foreach ((string placeholder, string data)
        //    par in placeholderData)
        //    {
        //        foreach (XWPFParagraph p in doc.Paragraphs)
        //        {
        //            List<XWPFRun> runs = p.Runs.ToList();
        //            if (runs != null)
        //            {
        //                foreach (XWPFRun r in runs)
        //                {
        //                    String text = r.Text;
        //                    if (text != null && text.Contains(par.placeholder))
        //                    {
        //                        text = text.Replace(par.placeholder, par.data);
        //                        r.SetText(text, 0);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    ByteArrayOutputStream dataout = new ByteArrayOutputStream();
        //    doc.Write(dataout);
        //    dataout.Close();
        //    doc.Close();

        //    byte[] xwpfDocumentBytes = dataout.ToByteArray();

        //    string fileName = string.Format("ChamCong_{0}.docx", DateTime.Now.ToString("yyyyMMddHHmmss"));
        //    Stream stream = new MemoryStream(xwpfDocumentBytes);
        //    try
        //    {
        //        stream.Write(xwpfDocumentBytes); // doc should be a XWPFDocument
        //        stream.Seek(0L, SeekOrigin.Begin);
        //        return this.File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { msg = ex.Message });
        //    }

        //}

        [HttpGet("UserStatistics")]
        public ActionResult GetUserStatistics()
        {
            var currentActiveUser = _userService.GetAllUserActive()
                .Where(x => x.Quit == false).ToList();

            var birthDayOfUsers = currentActiveUser
                .GroupBy(x => x.BirthDay.HasValue ? x.BirthDay.Value.Month : -1)
                .Select(x => new
                {
                    Month = x.Key,
                    Male = x.Count(s => s.Gender == GenderEnum.Male),
                    Female = x.Count(s => s.Gender == GenderEnum.Female)
                })
                .ToDictionary(x => x.Month);

            List<object> distributeByMonth = new List<object>();

            for (int i = 1; i <= 12; i++)
            {
                distributeByMonth.Add(new
                {
                    Month = i,
                    Male = birthDayOfUsers.ContainsKey(i) ? birthDayOfUsers[i].Male : 0,
                    Female = birthDayOfUsers.ContainsKey(i) ? birthDayOfUsers[i].Female : 0,
                });
            }

            var result = new
            {
                TotalUsers = currentActiveUser.Count,
                TotalMale = currentActiveUser
                .Where(x => x.Gender == GenderEnum.Male)
                .Count(),
                TotalFemale = currentActiveUser
                .Where(x => x.Gender == GenderEnum.Female)
                .Count(),
                BirthDayOfUsers = distributeByMonth
            };

            return new JsonResult(new BaseResponseModel
            {
                Data = result,
            });
        }
        
        [HttpGet("get-user-name")]
        public async Task<IActionResult> GetUserName()
        {
            string result = await _userService.GetUserName();
            return Ok(new ObjectReturn
            {
                data = result,
                status = 200,
            });
        }
    }
}
