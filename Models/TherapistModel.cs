using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TherapistAPI.Models
{
    public class TherapistModel : BaseModel
    {
        public int TherapistID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmgContName { get; set; }
        public string EmgEmail { get; set; }
        public string EmgPhone { get; set; }

        public string UserName { get; set; }
        public string Phone { get; set; }

    }
}