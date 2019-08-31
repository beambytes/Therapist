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
        
        [HttpPost]
        [Route("GetBooking")]
        public IHttpActionResult GetBooking(BookingModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                BookingDL obj = new BookingDL();
                BookingModel list = obj.GetBooking(Convert.ToInt32(UserId), Convert.ToInt32(RefType), model.BookingID);

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


        [HttpPost]
        [Route("GetAllBooking")]
        public IHttpActionResult GetAllBooking(BookingModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;
                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;

                BookingDL obj = new BookingDL();
                List<BookingModel> list = obj.GetAllBooking(Convert.ToInt32(UserId), Convert.ToInt32(RefType), Convert.ToInt32(RefID), model);
                //list.success = true;
                //list.message = "Get dashboard details Successfully";
                //list.code = 200;
                return Ok(list);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [HttpPost]
        [Route("ApproveCancelBooking")]
        public IHttpActionResult ApproveCancelBooking(BookingModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;
                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;

                BookingDL obj = new BookingDL();
                obj.ApproveCancelBooking(model);
                model.success = true;
                model.message = "Booking "+ model.Status + " successfully";
                model.code = 200;
                return Ok(model);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }


        [HttpPost]
        [Route("CompleteBooking")]
        public IHttpActionResult CompleteBooking(BookingModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;
                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;

                BookingDL obj = new BookingDL();
                obj.CompleteBooking(model);
                model.success = true;
                model.message = "Booking " + model.Status + " successfully";
                model.code = 200;
                return Ok(model);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }



    }
}
