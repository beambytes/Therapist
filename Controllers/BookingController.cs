using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ModelBinding;
using TherapistAPI.DataAccess;
using TherapistAPI.Models;


namespace TherapistAPI.Controllers
{
    //[EnableCors(methods: "*", headers: "*", origins: "*")]
    [Authorize]
    [RoutePrefix("api/Booking")]
    public class BookingController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        private static Random random = new Random();


        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet]
        [Route("GetBooking")]
        public IHttpActionResult GetBooking()
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                BookingDL obj = new BookingDL();
                BookingModel list = obj.GetBooking(Convert.ToInt32(UserId), Convert.ToInt32(RefType));

                list.success = true;
                list.message = "Get booking details Successfully";
                list.code = 200;

                return Ok(list);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("SaveBooking")]
        public IHttpActionResult SaveBooking(BookingModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;

                if (RefType == "1")
                {
                    model.TherapistID = Convert.ToInt32(RefID);
                }
                else if (RefType == "2")
                {
                    model.PatientID = Convert.ToInt32(RefID);
                }

                BookingDL obj = new BookingDL();
                obj.SaveBooking(model, UserId);

                responseData.message = "Booking details saved successfully";
                responseData.success = true;
                return Ok(responseData);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                responseData.success = false;
                return Ok(responseData);
            }
        }



        #region private methods
        private string ModelStateErrors(ModelStateDictionary modelState)
        {
            return string.Join(" | ", modelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage));
        }
        #endregion


    }
}
