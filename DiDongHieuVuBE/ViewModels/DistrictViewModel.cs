using ManageEmployee.ViewModels;

namespace ManageEmployee.ViewModels
{
    public class DistrictViewModel
    {
     public class GetByProvince: PagingationRequestModel
        {
            public int? provinceid { get; set; }
        }
    }
}