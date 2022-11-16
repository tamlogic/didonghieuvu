using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.ViewModels
{
    public class UserRoleViewModel
    {
        public int? id { get; set; }
        public string title { get; set; }
        public string code { get; set; }
    }
}