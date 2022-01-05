using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearentScheduler.ClearentQuestAPI
{
    [Serializable]
    public class Page
    {
        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("size")]
        public int Size { get; set; }

        [JsonProperty("sort")]
        public Sort Sort { get; set; }

        [JsonProperty("first")]
        public bool IsFirst { get; set; }

        [JsonProperty("last")]
        public bool IsLast { get; set; }

        [JsonProperty("total-pages")]
        public int TotalPages { get; set; }

        [JsonProperty("number-of-elements")]
        public int NumberOfElements { get; set; }

        [JsonProperty("total-elements")]
        public int TotalElements { get; set; }
    }

    public struct Sort
    {
        [JsonProperty("field")]
        public KeyValuePair<string, string> field { get; set; }
    }
}
