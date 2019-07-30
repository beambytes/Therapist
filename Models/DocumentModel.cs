using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TherapistAPI.Models
{
    public class DocumentModel :BaseModel
    {
        public int DocID { get; set; }
        public int RefId { get; set; }
        public int RefType { get; set; }
        public string DocPath { get; set; }
        
    }
}