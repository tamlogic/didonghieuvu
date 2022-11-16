using ManageEmployee.EnumList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.ViewModels
{
    public class SearchViewModel : PagingationRequestModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ExportExcel { get; set; }
        public int? Type { get; set; }
        public string? GoodType { get; set; }
        public string? CategoryTypesSearch { get; set; }
        public int? CustomerId { get; set; }
        public string? UserCode { get; set; }
        public int? GoodId { get; set; }
        public string? GoodCode { get; set; }
        public string? Account { get; set; }
        public string? Detail1 { get; set; }
        public string? PriceCode { get; set; }
        public string? MenuType { get; set; }
        public string? MenuWeb { get; set; }
        public string? Position { get; set; }
        public bool isCashier { get; set; } = false;
        public bool isHangChuaVe { get; set; } = false;
        public int Status { get; set; } = 1;
        public string? Warehouse { get; set; }
    }
    public class CustomersSearchViewModel : PagingationRequestModel
    {
        public int? JobId { get; set; }
        public int? StatusId { get; set; }
        public int? ExportExcel { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? Birthday { get; set; }
        public GenderEnum Gender { get; set; }
        public int? FromAge { get; set; }
        public int? ToAge { get; set; }
        public int? Area { get; set; }//0: mien bac; 1:mien nam

    }
}
