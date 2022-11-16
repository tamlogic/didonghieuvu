using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.EnumList
{
    public enum GenderEnum : short
    {
        [Description("All")]
        All = -1,
        [Description("Male")]
        Male = 0,
        [Description("Female")]
        Female = 1,
        [Description("Other")]
        Other = 2,
    }
}
