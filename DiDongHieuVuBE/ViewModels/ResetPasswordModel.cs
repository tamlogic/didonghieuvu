using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.ViewModels
{
    public class ResetPasswordModel
    {
        [Required]
        public string Username { get; set; }
    }
}
