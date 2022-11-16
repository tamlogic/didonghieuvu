
using Newtonsoft.Json;

namespace ManageEmployee.ViewModels
{
    public class BaseResponseModel
    {
        [JsonProperty("ti")]
        public int TotalItems { get; set; }

        [JsonProperty("p")]
        public int CurrenPage { get; set; }

        [JsonProperty("pz")]
        public int PageSize { get; set; }

        [JsonProperty("dt")]
        public object Data { get; set; }

        [JsonProperty("dts")]
        public object DataTotal { get; set; }

        [JsonProperty("nextStt")]
        public int NextStt { get; set; }
    }
}