using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bankBot.dataModel
{
    public class branch
    {
        public string ID { get; set; }

        public string name { get; set; }

        public string location { get; set; }

        public string manager { get; set; }

        public string weekdayOpening { get; set; }

        public string weekdayClosing { get; set; }
    }
}