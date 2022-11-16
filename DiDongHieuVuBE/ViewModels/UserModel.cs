using ManageEmployee.EnumList;

namespace ManageEmployee.ViewModels
{
    public class UserModel
    {
        public int Id { get; set; }
        public int? BranchId { get; set; }
        public int? WarehouseId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionDetailId { get; set; }
        public int? TargetId { get; set; }
        public int? SymbolId { get; set; }
        public string? Language { get; set; } = "";
        public string? Note { get; set; } = "";
        public string? FullName { get; set; }
        public string? Phone { get; set; } = "";
        public DateTime? BirthDay { get; set; }
        public GenderEnum Gender { get; set; }
        public string? Email { get; set; } = "";
        public string? Facebook { get; set; } = "";
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string? Address { get; set; } = "";
        #region CMND
        public string? Identify { get; set; } = "";
        public DateTime? IdentifyCreatedDate { get; set; }
        public string? IdentifyCreatedPlace { get; set; } = "";
        public DateTime? IdentifyExpiredDate { get; set; }
        public int? NativeProvinceId { get; set; }
        public int? NativeDistrictId { get; set; }
        public int? NativeWardId { get; set; }
        public string? PlaceOfPermanent { get; set; } = "";
        public string? Nation { get; set; } = ""; // Quốc gia
        public string? Religion { get; set; } = "";// Tôn giáo
        public string? EthnicGroup { get; set; } = ""; // Dân tộc
        public int UnionMember { get; set; } = 0; // Đoàn viên
        public string? LicensePlates { get; set; } = ""; // Biến số xe
        public bool isDemobilized { get; set; } = false; // xuất ngũ
        #endregion
        #region Trình độ
        public string? Literacy { get; set; } = "";
        public string? LiteracyDetail { get; set; } = "";
        public int? MajorId { get; set; }
        public string? CertificateOther { get; set; } = ""; // 
        #endregion
        #region lương, ngày phép
        public string? BankAccount { get; set; } = "";
        public string? Bank { get; set; } = "";
        public string? ShareHolderCode { get; set; } = "";
        public int? NoOfLeaveDate { get; set; }
        public DateTime? SendSalaryDate { get; set; }
        public int? ContractTypeId { get; set; }
        public double Salary { get; set; } = 0;
        public double SocialInsuranceSalary { get; set; } = 0;
        public double NumberWorkdays { get; set; } = 0;
        public int? DayOff { get; set; }
        #endregion
        #region Thuế
        public string PersonalTaxCode { get; set; } = "";
        public string SocialInsuranceCode { get; set; } = "";
        public DateTime? SocialInsuranceCreated { get; set; }
        #endregion

        public string? Username { get; set; } = "";
        public int Timekeeper { get; set; } = 0;
        public string Avatar { get; set; } = "";
        public string? UserRoleIds { get; set; } = "";
        public bool Status { get; set; } = false;
        public DateTime? LastLogin { get; set; }
        public bool RequestPassword { get; set; } = false;
        public bool Quit { get; set; } = false;
        public string? UserRoleName { get; set; }
        public string? AddressFull { get; set; }
        public string? NativeAddressFull { get; set; }
        public string? Password { get; set; }
    }

    public class UserMapper
    {
        public class Auth
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Username { get; set; }
            public string Avatar { get; set; }
            public bool Status { get; set; }
            public int PositionId { get; set; }
            public DateTime? LastLogin { get; set; }
            public string UserRoleIds { get; set; }
            public int CompanyId { get; set; }
            public int Timekeeper { get; set; } = 0;
            public byte[] PasswordHash { get; set; }
            public byte[] PasswordSalt { get; set; }
            public string Language { get; set; }
            public int TargetId { get; set; }
        }
        public class FilterParams
        {
            public string? Keyword { get; set; }
            public int? WarehouseId { get; set; }
            public int? PositionId { get; set; }
            public int? DepartmentId { get; set; }
            public Boolean? RequestPassword { get; set; }
            public Boolean? Quit { get; set; }
            public GenderEnum Gender { get; set; }
            public DateTime? BirthDay { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int CurrentPage { get; set; }
            public int PageSize { get; set; }
            public int TargetId { get; set; }
            public int? Month { get; internal set; }
            public int DegreeId { get; set; }
            public int CertificateId { get; set; }
            public bool isSort { get; set; } = false;
            public string? SortField { get; set; }
            public List<int> Ids { get; set; }
            public List<string> roles { get; set; }
            public int UserId { get; set; }

        }
    }

    public class User_SalaryModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public double Salary { get; set; }
        public int? ContractTypeId { get; set; }
        public int? SoNgayCong { get; set; }
        public int DepartmentId { get; set; }
        public string PositionName { get; set; }
        public double SalaryTotal { get; set; }
        public double SalaryContract { get; set; }
        public int? DayInOut { get; set; }
        public double SalaryReal { get; set; }
        public double ThueTNCN { get; set; }
        public double TamUng { get; set; }
        public double SalarySend { get; set; }
        public string SoThuTu { get; set; }
        public List<User_SalaryModel> listChild { get; set; }
        public List<SalarySocialModel> salarySocial { get; set; }
    }
    public class SalarySocialModel
    {
        public string Code { get; set; }
        public double ValueCompany { get; set; }
        public double ValueUser { get; set; }
    }
    public class ThongKeTongQuat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SoLuong { get; set; }
        public bool isBold { get; set; }
        public List<ThongKeTongQuat> listChildren { get; set; }
    }
    public class UserActiveModel
    {
        public int Id { get; set; }
        public int? BranchId { get; set; }
        public int? WarehouseId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionDetailId { get; set; }
        public int? TargetId { get; set; }
        public int? SymbolId { get; set; }
        public string? Language { get; set; } = "";
        public string? Note { get; set; } = "";
        public string? FullName { get; set; }
        public string? Phone { get; set; } = "";
        public DateTime? BirthDay { get; set; }
        public GenderEnum Gender { get; set; }
        public string? Email { get; set; } = "";
        public string? Facebook { get; set; } = "";
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }
        public int? WardId { get; set; }
        public string? Address { get; set; } = "";
        #region CMND
        public string? Identify { get; set; } = "";
        public DateTime? IdentifyCreatedDate { get; set; }
        public string? IdentifyCreatedPlace { get; set; } = "";
        public DateTime? IdentifyExpiredDate { get; set; }
        public int? NativeProvinceId { get; set; }
        public int? NativeDistrictId { get; set; }
        public int? NativeWardId { get; set; }
        public string? PlaceOfPermanent { get; set; } = "";
        public string? Nation { get; set; } = ""; // Quốc gia
        public string? Religion { get; set; } = "";// Tôn giáo
        public string? EthnicGroup { get; set; } = ""; // Dân tộc
        public int? UnionMember { get; set; } = 0; // Đoàn viên
        public string? LicensePlates { get; set; } = ""; // Biến số xe
        public bool isDemobilized { get; set; } = false; // xuất ngũ
        #endregion
        #region Trình độ
        public string? Literacy { get; set; } = "";
        public string? LiteracyDetail { get; set; } = "";
        public int? MajorId { get; set; }
        public string? CertificateOther { get; set; } = ""; // 
        #endregion
        #region lương, ngày phép
        public string? BankAccount { get; set; } = "";
        public string? Bank { get; set; } = "";
        public string? ShareHolderCode { get; set; } = "";
        public int? NoOfLeaveDate { get; set; }
        public DateTime? SendSalaryDate { get; set; }
        public int? ContractTypeId { get; set; }
        public double? Salary { get; set; } = 0;
        public double? SocialInsuranceSalary { get; set; } = 0;
        public double? NumberWorkdays { get; set; } = 0;
        public int? DayOff { get; set; }
        #endregion
        #region Thuế
        public string? PersonalTaxCode { get; set; } = "";
        public string? SocialInsuranceCode { get; set; } = "";
        public DateTime? SocialInsuranceCreated { get; set; }
        #endregion

        public string Username { get; set; } = "";
        public int? Timekeeper { get; set; } = 0;
        public string? Avatar { get; set; } = "";
        public string? UserRoleIds { get; set; } = "";
        public bool Status { get; set; } = false;
        public DateTime? LastLogin { get; set; }
        public bool RequestPassword { get; set; } = false;
        public bool Quit { get; set; } = false;
        public int Order { get; set; } = 0;
    }
}
