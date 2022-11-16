using Microsoft.VisualBasic;
using System;

namespace ManageEmployee.Entities
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public bool IsDelete { get; set; }
        public long CreatedBy { get; set; } = 0;
        public long UpdatedBy { get; set; } = 0;
    }
}