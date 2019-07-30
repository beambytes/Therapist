using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TherapistAPI.Models
{
    public class BookingModel : BaseModel
    {
        public int BookingID { get; set; }
        public int PatientID { get; set; }
        public int TherapistID { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime BookingFTime { get; set; }
        public DateTime BookingTTime { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Chekout { get; set; }
        public string Note { get; set; }
        public decimal Amount { get; set; }
        public string Addressline1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public int ServiceID { get; set; }
        public string Status { get; set; }
        public DateTime CurrentDateTime { get; set; }
        
        public int IsPannic { get; set; }
        public char Gender { get; set; }
        public DateTime DOB { get; set; }
        public string EmergencyName { get; set; }
        public string EmergPhone { get; set; }
        public string Occupation { get; set; }
        public string Employer { get; set; }
        public string FamilyDocor { get; set; }
        public string RefProf { get; set; }
        public string RefSour { get; set; }
        public string ReferredTo { get; set; }
        public string MarkEmail { get; set; }
        public string PatientName { get; set; }
        public string InsuranceComp { get; set; }
        public string PolicyNo { get; set; }
        public string IDNo { get; set; }
        public string Year { get; set; }
        public DateTime? PlicyDOB { get; set; }
        public string RelPat { get; set; }
        public string AssBen { get; set; }
        
        public string PolicyHolderName { get; set; }

        public List<UsersModel> User { get; set; }
        public List<TherapistModel> Therapist { get; set; }
        public List<ServicesModel> ServiceList { get; set; }

        public List<BookingSerModel> BookingSerList { get; set; }
        public List<EHCbenefitsModel> EHCbenefitsList { get; set; }
    }

    public class BookingSerModel
    {
        public int BookingSerID { get; set; }
        public int ServiceID { get; set; }
        public string Description { get; set; }
    }

    public class EHCbenefitsModel
    {
        public int ECHBenID { get; set; }
        public string Type { get; set; }
        public string YearlyMax { get; set; }
        public string InitiMax { get; set; }
        public string SubSeqMax { get; set; }
        public string Pay { get; set; }
        public string RefRequired { get; set; }
        public string Description { get; set; }
        //public int BookingID { get; set; }
    }



    public enum EHC
    {
        [Description("EHC benefits for Physiotherapy")]
        EHCBP = 1,
        [Description("EHC benefits for Chiro")]
        EHCBC = 2,
        [Description("EHC benefits for Massage")]
        EHCBM = 3,
        [Description("EHC benefits for Acupuncture")]
        EHCBA = 4,
        [Description("EHC benefits for Orthotics")]
        EHCBO = 5,
        [Description("EHC benefits for Compression stockings")]
        EHCBCS = 6,
    }

}