using ManageEmployee.EnumList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.Models
{
    public class SliderModel
    {
        public int Id { get; set; }
        public LanguageEnum Type { get; set; }
        public string Name { get; set; }
        public string Img { get; set; }
        public DateTime? CreateAt { get; set; }
    }
}
