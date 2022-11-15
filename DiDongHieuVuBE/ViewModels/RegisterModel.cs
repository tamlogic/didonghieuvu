using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Models
{
    public class RegisterModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string? Address { get; set; }
        public string? Code { get; set; }

        public string? Identify { get; set; }
    }

    public class RegisterEmployerWebModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string TelephoneNumber { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public int EmployerFieldId { get; set; }
        public string? TaxCode { get; set; }
        public int LaborSize { get; set; }
        public int ProvinceId { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public int Area { get; set; }//0: mien bac; 1: mien nam
        public string? IndustryProduct { get; set; }// list Id job
    }
    public class RegisterLaborWebModel
    {
        [Required]
        public string FullName { get; set; }
        public string Email { get; set; }
        [Required]
        public string TelephoneNumber { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}