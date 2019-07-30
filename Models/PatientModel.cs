using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TherapistAPI.Models
{
    public class PatientModel : BaseModel
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string InsuranceNo { get; set; }
        public DateTime ExpDate { get; set; }
        public DateTime DOB { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }

    }
}