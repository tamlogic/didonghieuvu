using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.ViewModels
{
    public class PagingResult<T> where T:class
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
