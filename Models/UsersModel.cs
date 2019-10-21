using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace TherapistAPI.Models
{
   

    public class UsersModel : BaseModel
    {
        public int UserID { get; set; }

        //[Required]
        //[Display(Name = "First Name")]
        public string FirstName { get; set; }

        //[Required]
        //[Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string UserName { get; set; }

        //[Required]
        //[Display(Name = "Password")]
        public string Password { get; set; }

        public int RefID { get; set; }

        public int RefType { get; set; }

        //[Required]
        //[Display(Name = "Email Id")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        public string PostalCode { get; set; }

        public int Approve { get; set; }

        
        public int? Age { get; set; }

        
        public string InsuranceNo { get; set; }

        public DateTime ExpDate { get; set; }

        public DateTime DOB { get; set; }
        //public DateTime DateDOB { get; set; }

        //public DateTime DateExpdate { get; set; }
        

        // New Field

        public string AddrLine1 { get; set; }

        
        public string AddrLine2 { get; set; }

        
        public string City { get; set; }

      
        public string State { get; set; }

        
        public string Country { get; set; }

     
        public string ServiceID { get; set; }

        
        public string BankDetail { get; set; }

        public string RegistrationNo  { get; set; }

        public string ServiceArea { get; set; }


        public string Skill { get; set; }
       

        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; } = false;

        public bool Thursday { get; set; } = false;

        public bool Friday { get; set; } = false;

        public bool Saturday { get; set; } = false;

        public bool Sunday { get; set; } = false;

       
        public string FromTime { get; set; }
        
        public string ToTime { get; set; }

        public string Gender { get; set; }
        //[Required(ErrorMessage = "Please select file.")]
        //[Display(Name = "Browse File")]

        public UserTokenViewModel UserTokenInfo { get; set; }


        public string TherapistName { get; set; }
        public string PatientName { get; set; }

        public int BookingID { get; set; }

        public List<DocumentModel> Documents { get; set; }

        public List<ServicesModel> ServiceList { get; set; }

    }


    public class UserTokenViewModel
    {
        public string access_token { set; get; }
        public string expires_in { set; get; }
        public string refresh_token { set; get; }
    }

    public class ChangePasswordViewModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}