using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace try_cb_dotnet.Models
{
    public class FlightBookingModel
    {
        public string Token { get; set; }
        public List<FlightModel> _data { get; set; }
    }
}