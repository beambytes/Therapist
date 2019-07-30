using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TherapistAPI.Models
{
    public class ServicesModel
    {
        public int ServiceID { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public int ParServiceID { get; set; }
    }
}