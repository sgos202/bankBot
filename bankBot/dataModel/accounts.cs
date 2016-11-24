using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bankBot.dataModel
{
    public class Accounts
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string userName { get; set; }

        [JsonProperty(PropertyName = "userPassword")]
        public string userPassword { get; set; }
    }
}