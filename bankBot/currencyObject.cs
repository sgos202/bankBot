using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bankBot
{
    public class currencyObject
    {
        public class RootObject
        {
            public string @base { get; set; }
            public DateTime date { get; set; }
            public KeyValuePair<string, string> rates { get; set; }
        }

    }
}