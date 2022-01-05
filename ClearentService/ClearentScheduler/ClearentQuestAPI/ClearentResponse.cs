using Newtonsoft.Json;
using System;

namespace ClearentScheduler.ClearentQuestAPI
{
    [Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ClearentResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("exchange-id")]
        public string ExchangeId { get; set; }

        [JsonProperty("links")]
        public Link[] Links { get; set; }

        [JsonProperty("payload")]
        public Payload Payload { get; set; }

        [JsonProperty("page")]
        public Page Page { get; set; }

    }

    public struct Link
    {
        [JsonProperty("rel")]
        public string rel { get; set; }

        [JsonProperty("href")]
        public string href { get; set; }

        [JsonProperty("id")]
        public string id { get; set; }
    }
}
