namespace ManageEmployee.ViewModels
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? NameEN { get; set; }
        public string? NameKO { get; set; }
        public string? CodeParent { get; set; }
        public string? Note { get; set; }
        public bool IsParent { get; set; }
        public List<MenuRoleViewModel> listItem { get; set; }
    }
    public class MenuRoleViewModel
    {
        public int Id { get; set; }
        public int? MenuId { get; set; }
        public int? UserRoleId { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
    }
    public class MenuCheckRole
    {
        public string? MenuCode { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
    }
    public class MenuPagingationRequestModel : PagingationRequestModel
    {
        public string? CodeParent { get; set; }
        public Boolean isParent { get; set; }
    }
}
