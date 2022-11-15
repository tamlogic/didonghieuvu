using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ManageEmployee.EnumList
{
    public enum LanguageEnum
    {
        [Description("Tiếng Hàn Quốc")]
        Korea = 1,
        [Description("Tiếng Việt Nam")]
        VietNam = 2,
        [Description("Tiếng Anh")]
        English = 3
    }
    public enum SocialEnum
    {
        [Description("Facebook")]
        Facebook = 1,
        [Description("Instagram")]
        Instagram = 2,
        [Description("Twitter")]
        Twitter = 3,
        [Description("Youtube")]
        Youtube = 4,
        [Description("Whatsapp")]
        Whatsapp = 5,
        [Description("Printerest")]
        Printerest = 6,
    }
}
