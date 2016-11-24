using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bankBot.dataModel
{
    public class Branch
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "location")]
        public string location { get; set; }

        [JsonProperty(PropertyName = "manager")]
        public string manager { get; set; }

        [JsonProperty(PropertyName = "weekDayHours")]
        public string weekDayHours { get; set; }

        [JsonProperty(PropertyName = "satHours")]
        public string satHours { get; set; }

        [JsonProperty(PropertyName = "sunHours")]
        public string sunHours { get; set; }
    }
}