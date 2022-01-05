using CEMVC.Core.BLL.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CEMVC.FrontEnd.Web.Models.Project
{
    public class ProjectEventAggregateModel
    {
        public string Id { get; set; }
        public DateTime DateSent { get { return LastEvent.GetTime(); } }
        public string Subject { get; set; }

        IEnumerable<ProjectEvent> events;
        public ProjectEvent LastEvent { get; private set; }
            
        public IEnumerable<ProjectEvent> Events
        {
            get { return events; }
            set
            {
                events = value;
                LastEvent = value.OrderBy(e => e.TimeStamp).LastOrDefault();
            }
        }

        public static IEnumerable<ProjectEventAggregateModel> FromBOCollection(IEnumerable<ISendgridEventModel> docs)
        {
            return docs.GroupBy(d => d.MessageId).Select(g => new ProjectEventAggregateModel
            {
                Id = g.Key,
                Subject = g.FirstOrDefault(e => e.Event == "delivered")?.Subject,
                Events = g.OrderBy(e => e.Timestamp).Select(e => new ProjectEvent(e.Event, e.Timestamp, e.TimestampInt, e.Email, e.Invoice)).ToArray()
            }
            );
        }

    }
    [Serializable]
    public class ProjectEvent
    {
        public DateTimeOffset Time { private get; set; }
        public string Name { get; set; }
        public object Data { get; set; }
        public string TimeStamp { get; set; }
        public DateTime GetTime() { return Time.DateTime; }

        public ProjectEvent(string name, string timestamp, int timeStampSec, string email, string invoice_data)
        {
            Name = name;
            TimeStamp = timestamp;
            Time = DateTimeOffset.FromUnixTimeSeconds(timeStampSec);
            var t = string.IsNullOrEmpty(invoice_data) ? null : (JObject)JsonConvert.DeserializeObject(invoice_data);
            Data = new { email, invoice = t?.GetValue("number")?.ToString() };
        }
    }
}