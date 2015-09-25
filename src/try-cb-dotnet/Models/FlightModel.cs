using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace try_cb_dotnet.Models
{
    public class FlightModel
    {
        /*
            name: 'string',
            flight: 'string',
            date:'string',
            sourceairport:'string',
            destinationairport:'string',
            bookedon:'string'
        */

        public string name { get; set; }
        public string date { get; set; }
        public string sourceairport { get; set; }
        public string destinationairport { get; set; }
        public string bookedon { get; set; }
    }
}