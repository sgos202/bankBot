using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bankBot
{
    public class CurrencyObject
    {
        public class Rates
        {
            public string NZD { get; set; }
            public string AUD { get; set; }
            public string EUR { get; set; }
            public string JPY { get; set; }
            public string GBP { get; set; }
            public string CAD { get; set; }
            public string CNY { get; set; }
            public string USD { get; set; }

        }
        public class RatesObj
        {
            public Rates rates { get; set; }
            public string @base { get; set; }
        }
    }
}