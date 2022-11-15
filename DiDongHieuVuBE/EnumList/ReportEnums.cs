using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.EnumList
{
    public class ReportEnums
    {
    }

    public enum AccountBalanceReportTypeEnum
    {
        inCotLuyKePhatSinh = 1,
        inCaChiTietCap2 = 2,
        khongInNhungDongKhongSoLieu = 4,
        inLuyKe_ChiTietCap2 = 3,
        inLuyKe_KhongSoLieu = 5,
        inChiTietCap2__KhongSoLieu = 6,
        inAll = 7
    }

    public enum ReportBookDetailTypeEnum
    {
        soCoDu_1_ben = 1,
        soCoDu_2_ben = 2,
        soCoNgoaiTe = 3,
        soCoHangTonKho = 4,
        soQuy = 5,
        soSoLuongTonKho = 6,

    }

    public enum AccountantBalanceEnumModelBase
    {
        Level_One = 1,
        Level_Two = 2,
        Level_Three = 3,
        Level_Four = 4,
        Level_Sum = 10
    }

    public static class ActionTypeEnumModelBase
    {
        public const string Du_Co = "du_co";
        public const string Du_Co_Am = "du_co_am";
        public const string Du_No = "du_no";
        public const string Du_No_Am = "du_no_am";
        public const string Tong_Du_No = "tong_du_no";
        public const string Tong_Du_Co = "tong_du_co";
        
        public const string Tong_Du_Co_Chi_Tiet = "tong_du_co_chi_tiet";
        public const string Tong_Du_No_Chi_Tiet = "tong_du_no_chi_tiet";
        public const string Tong_Du_Co_Cac_Chi_Tiet = "tong_du_co_cac_chi_tiet";
        public const string Tong_Du_Co_Exception = "tong_du_co_exception";
        public const string Loai_Tru_Du_No = "loai_tru_du_no";
    }

}
