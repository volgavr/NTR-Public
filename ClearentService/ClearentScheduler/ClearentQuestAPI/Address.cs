using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearentScheduler.ClearentQuestAPI
{
    public struct Address
    {
        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("first-name")]
        public string FirstName { get; set; }

        [JsonProperty("last-name")]
        public string LastName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("street")]
        public string Street { get; set; }

        [JsonProperty("street2")]
        public string Street2 { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }
    }
}
