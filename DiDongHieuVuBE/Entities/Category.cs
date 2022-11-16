using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [StringLength(36)]
        public string? Code { get; set; }
        [StringLength(36)]
        public string? Name { get; set; }
        public int Type { get; set; } = 0;// 6: goodErrorStatus
        [StringLength(36)]
        public string? Note { get; set; }
        public bool IsDeleted { get; set; } = false;
        //
        public int? NumberItem { get; set; }
        public bool isPublish { get; set; } = false;
        [StringLength(200)]
        public string? Icon { get; set; }
        [StringLength(200)]
        public string? Image { get; set; }
        [StringLength(36)]
        public string? CodeParent { get; set; }
        // 
        [StringLength(36)]
        public string? NameEnglish { get; set; }
        [StringLength(36)]
        public string? NameKorea { get; set; }
        public bool isEnableDelete { get; set; } = false;
    }
}
