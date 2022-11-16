using System;
using System.ComponentModel.DataAnnotations;
using ManageEmployee.EnumList;
using Microsoft.AspNetCore.Http;

namespace ManageEmployee.ViewModels
{
    public class UserViewModel : PagingationRequestModel
    {
        public int? Warehouseid { get; set; }
        public int? Positionid { get; set; }
        public int? Departmentid { get; set; }
        public Boolean? RequestPassword { get; set; }
        public Boolean? Quit { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Targetid { get; set; }
        public int? TypeOfWork { get; set; }
        public int? Month { get; set; }
        public int? Degreeid { get; set; }
        public int? Certificateid { get; set; }
        public List<int> Ids { get; set; } = new List<int>();
    }
}