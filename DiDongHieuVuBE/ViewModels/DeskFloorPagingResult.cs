using ManageEmployee.Entities;
using System.Collections.Generic;

namespace ManageEmployee.ViewModels
{
    public class DeskFLoorPagingResult
    {
        public int pageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<DeskFloorModel> DeskFloors { get; set; } = new List<DeskFloorModel>();
    }
    public class DeskFLoorPagingationRequestModel : PagingationRequestModel
    {
        public int? FloorId { get; set; }
        public bool? IsFloor { get; set; }
    }
}
