namespace ManageEmployee.Enum
{
    public class ResultErrorEnum
    {
        public const string AUTH_FAIL = "auth_f";
        public const string USER_MISS_PASSWORD = "auth_mp";
        public const string USER_MISS_USERNAME = "auth_mu";
        public const string USER_MISS_ADDRESSS = "auth_madr";
        public const string USER_NOT_FAULT = "auth_usf";
        public const string USER_EXIST = "auth_ex";
        public const string USER_USNEXIST = "Tên đăng nhập đã tồn tại";
        public const string USER_CODEEXIST = "Mã tài khoản đã tồn tại";

        public const string USER_EMPTY_OR_DELETE = "us_miss";
        public const string COMPANY_EMPTY_OR_DELETE = "cpm_miss";

        public const string MODEL_NULL = "Không tồn tại bản ghi";
        public const string MODEL_MISS = "Không tìm thấy dữ liệu";
        public const string WAREHOUSE_EXIST = "Kho đã tồn tại";
        public const string WAREHOUSE_USER_CONTAINT = "ware_us_containt";
        public const string OBJ_NULL = "OBJ_NULL";

        public const string STT_EXIST = "STT đã tồn tại";
        public const string CODE_EXIST = "Mã đã tồn tại";
        public const string NAME_EXIST = "Tên đã tồn tại";

        public const string USER_IS_NOT_EXIST = "Không tìm thấy username";
        public const string ERROR_PASS = "Mật khẩu sai";

        public const string Is_Used = "Mã đang được sử dụng";
        public const string LOGIN_SUCCESS = "Đăng nhập thành công";
        public const string SUCCESS = "Thành công";

        public const string CREATED_FAIL = "Tạo thất bại";
        public const string UPDATED_FAIL = "Cập nhật thất bại";
        public const string CREATED_SUCCESS = "Tạo thành công";
        public const string UPDATED_SUCCESS = "Cập nhật thành công";

        public const string DEPARTMENT_IS_USE = "Phòng ban đã được sử dụng";
        public const string DEPARTMENT_CODE_IS_EXIST = "Mã phòng ban đã tồn tại";
    }

    public enum FinalStandardTypeEnum
    {
        debitToCredit,
        creditToDebit
    }
    public class ChartOfAccountResultErrorEnum
    {
        public const string NOT_ACCOUNT_TEST = "Tài khoản này không có trong hệ thống tài khoản bạn";
        public const string ACCOUNT_GROUP_LINK = "Tài khoản này nằm trong Hệ thống tài khoản đồng bộ nên không được thêm tài khoản con";
        public const string ACCOUNT_EXIST = "Mã này đã có vui lòng nhập 1 Mã khác nha!!!";
        public const string ACCOUNT_LEDGER = "Tài khoản cha của mã này đã có Phát sinh vui lòng nhập 1 Mã khác";
        public const string ACCOUNT_LEDGER_DETAIL = "Đã có Phát sinh rồi không thể thêm chi tiết  được!!!";
        public const string ACCOUNT_PARENT_NOT_EXIST = "Tài khoản này chưa có Tài khoản Cha";
        public const string ACCOUNT_USING = "Tài khoản đã có chứng từ phát sinh không xóa được";

    }


}
