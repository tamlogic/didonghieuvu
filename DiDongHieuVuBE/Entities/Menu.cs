using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        [StringLength(36)]
        public string? Code { get; set; }
        [StringLength(255)]
        public string? Name { get; set; }
        [StringLength(255)]
        public string? NameEN { get; set; }
        [StringLength(255)]
        public string? NameKO { get; set; }
        [StringLength(36)]
        public string? CodeParent { get; set; }
        [StringLength(255)]
        public string? Note { get; set; }
        public bool IsParent { get; set; }
    }
    public class MenuRole
    {
        public int Id { get; set; }
        public int? MenuId { get; set; }
        public int? UserRoleId { get; set; }
        public string? MenuCode { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
    }
}
