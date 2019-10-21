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
        therapistEntities entities = new therapistEntities();

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

                //if (model.BookingID == 0)
                //{
                //    SendMail mail = new SendMail();
                //    RegisterDL obj1 = new RegisterDL();
                //    var list = obj1.GetUserDetails(Convert.ToInt32(UserId), Convert.ToInt32(RefType));

                //    var getAllService = obj1.GetAllServices();
                //    string getServiceName = "";
                //    foreach (var item in model.BookingSerList)
                //    {
                //        getServiceName += getAllService.Where(x => x.ServiceID == item.ServiceID).FirstOrDefault().ServiceName + ",";
                //    }
                //    getServiceName = getServiceName.TrimEnd(',');

                //    string body = mail.createEmailBody("PatientBooking.html");
                //    body = body.Replace("{UserName}", list[0].FirstName + " " + list[0].LastName);
                //    body = body.Replace("{servicename}", getServiceName);

                //    mail.SendGeneralMail("Patient Booking", list[0].Email, body);
                //}

                int bookId = Convert.ToInt32(RefID);

                int BookingID = entities.Bookings.Where(x => x.PatientID == bookId).Select(x => x.BookingID).FirstOrDefault();


                responseData.message = "Booking details saved successfully";
                responseData.success = true;
                responseData.data = BookingID;
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
        [Route("GetAllPatientBooking")]
        public IHttpActionResult GetAllPatientBooking(BookingModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;
                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;

                BookingDL obj = new BookingDL();

                List<PatientBookingModel> list = obj.GetAllBooking(Convert.ToInt32(UserId), Convert.ToInt32(RefType), Convert.ToInt32(RefID), model);
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
        [Route("SavePatientBooking")]
        public IHttpActionResult SavePatientBooking(PatientBookingModel model)
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




                if (model.PatientBookingID == 0)
                {
                    PatientBooking pb = new PatientBooking();
                    pb.ServiceID = model.ServiceID;
                    pb.Address = model.Address;
                    pb.BookingDate = model.BookingDate;
                    pb.FromTime = model.FromTime;
                    pb.ToTime = model.ToTime;
                    pb.PatientID = model.PatientID;
                    pb.TherapistID = model.TherapistID;
                    pb.Status = "PENDING";
                    entities.PatientBookings.Add(pb);
                }
                else
                {

                    var getBooking = entities.PatientBookings.Where(x => x.PatientBookingID == model.PatientBookingID).FirstOrDefault();

                    getBooking.ServiceID = model.ServiceID;
                    getBooking.Address = model.Address;
                    getBooking.BookingDate = model.BookingDate;
                    getBooking.FromTime = model.FromTime;
                    getBooking.ToTime = model.ToTime;


                }


                entities.SaveChanges();
                if (model.PatientBookingID == 0)
                {
                    SendMail mail = new SendMail();
                    RegisterDL obj1 = new RegisterDL();
                    var list = obj1.GetUserDetails(Convert.ToInt32(UserId), Convert.ToInt32(RefType));

                    // var getAllService = obj1.GetAllServices();
                    string getServiceName = entities.Services.Where(x => x.ServiceID == model.ServiceID).Select(x => x.ServiceName).FirstOrDefault();

                    //foreach (var item in model.BookingSerList)
                    //{
                    //    getServiceName += getAllService.Where(x => x.ServiceID == item.ServiceID).FirstOrDefault().ServiceName + ",";
                    //}
                    //getServiceName = getServiceName.TrimEnd(',');

                    string body = mail.createEmailBody("PatientBooking.html");
                    body = body.Replace("{UserName}", list[0].FirstName + " " + list[0].LastName);
                    body = body.Replace("{servicename}", getServiceName);
                    body = body.Replace("{address}", model.Address);

                    mail.SendGeneralMail("Patient Booking", list[0].Email, body);
                }

                //int bookId = Convert.ToInt32(RefID);

                //int BookingID = entities.Bookings.Where(x => x.PatientID == bookId).Select(x => x.BookingID).FirstOrDefault();


                responseData.message = "Booking details saved successfully";
                responseData.success = true;
                //responseData.data = BookingID;
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
        [Route("GetPatientBooking")]
        public IHttpActionResult GetPatientBooking(PatientBookingModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                BookingDL obj = new BookingDL();

                PatientBookingModel booking = entities.PatientBookings.Where(x => x.PatientBookingID == model.PatientBookingID).Select(y => new PatientBookingModel
                {
                    PatientBookingID = y.PatientBookingID,
                    BookingDate = y.BookingDate,
                    Status = y.Status,
                    Address = y.Address,
                    FromTime = y.FromTime,
                    ToTime = y.ToTime,
                    PatientID = y.PatientID,
                    TherapistID = y.TherapistID,
                    ServiceID = y.ServiceID
                }).FirstOrDefault();

                if (booking == null)
                {
                    booking = new PatientBookingModel();
                    booking.ServiceList = new List<ServicesModel>();
                }

                booking.ServiceList = entities.Services.Where(x => x.ParServiceID == 0).Select(y => new ServicesModel
                {
                    ServiceID = y.ServiceID,
                    ServiceName = y.ServiceName

                }).ToList();


                booking.TherapistList = (from ep in entities.Therapists
                                         join e in entities.Users on ep.TherapistID equals e.RefID
                                         where e.RefType == 1 && e.Approve == 1
                                         select new TherapistModel
                                         {
                                             UserName = ep.FirstName + " " + ep.LastName,
                                             TherapistID = ep.TherapistID

                                         }).ToList();

                //             SELECT(T.FirstName + ' ' + T.LastName) as UserName,T.TherapistID,U.UserID FROM[therapistdb].[Therapist]
                //     T
                //INNER JOIN[therapistdb].[Users]
                //     U on T.TherapistID = U.RefID AND U.RefType = 1
                //WHERE U.Approve = 1
                //BookingModel list = obj.GetBooking(Convert.ToInt32(UserId), Convert.ToInt32(RefType), model.BookingID);

                //list.success = true;
                //list.message = "Get booking details Successfully";
                //list.code = 200;

                return Ok(booking);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [HttpPost]
        [Route("ApproveCancelBooking")]
        public IHttpActionResult ApproveCancelBooking(PatientBookingModel model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;
                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;

                BookingDL obj = new BookingDL();
                obj.ApproveCancelBooking(model);
                model.success = true;
                model.message = "Booking " + model.Status + " successfully";
                model.code = 200;

                //For Therapist
                if (model.Status == "APPROVE")
                {
                    SendMail mail = new SendMail();
                    RegisterDL obj1 = new RegisterDL();

                    var getBooking = entities.PatientBookings.Where(x => x.PatientBookingID == model.PatientBookingID).FirstOrDefault();

                    var getUserTherapist = entities.Therapists.Where(x => x.TherapistID == getBooking.TherapistID).FirstOrDefault();

                    var getUserPatient = entities.Patients.Where(x => x.PatientID == getBooking.PatientID).FirstOrDefault();

                    var getUserTherapistEmail = entities.Users.Where(x => x.RefID == getUserTherapist.TherapistID && x.RefType == 1).FirstOrDefault();

                    var getUserPatientEmail = entities.Users.Where(x => x.RefID == getUserPatient.PatientID && x.RefType == 2).FirstOrDefault();

                    var services = (from ep in entities.Services
                                    join e in entities.PatientBookings on ep.ServiceID equals e.ServiceID
                                    where e.PatientBookingID == model.PatientBookingID
                                    select new
                                    {
                                        ServiceName = ep.ServiceName
                                    }).ToList();

                    string getServiceName = "";
                    foreach (var item in services)
                    {
                        getServiceName += item.ServiceName + ",";
                    }
                    getServiceName = getServiceName.TrimEnd(',');


                    string bookingDate = getBooking.BookingDate?.DayOfWeek.ToString() + " " + getBooking.BookingDate?.ToString("MMMM") + " " + getBooking.BookingDate?.Day + " " + getBooking.BookingDate?.Year;



                    //For Therapist
                    string body = mail.createEmailBody("TherapistBookingConfirmation.html");
                    body = body.Replace("{PatientName}", getUserPatient.FirstName + " " + getUserPatient.LastName);
                    body = body.Replace("{UserName}", getUserTherapist.FirstName + " " + getUserTherapist.LastName);
                    body = body.Replace("{PatientAddress}", getBooking.Address);

                    body = body.Replace("{PatientServices}", getServiceName);
                    body = body.Replace("{Datetime}", bookingDate + " " + getBooking.FromTime + " to " + getBooking.ToTime);

                    mail.SendGeneralMail("Therapist Booking Confirmation", getUserTherapistEmail.Email, body);

                    //For Patient
                    string body1 = mail.createEmailBody("PatientBookingConfirmation.html");
                    body1 = body1.Replace("{PatientName}", getUserPatient.FirstName + " " + getUserPatient.LastName);
                    body1 = body1.Replace("{UserName}", getUserPatient.FirstName + " " + getUserPatient.LastName);
                    body1 = body1.Replace("{PatientServices}", getServiceName);
                    body1 = body1.Replace("{PatientAddress}", getBooking.Address);
                    body1 = body1.Replace("{TherapistName}", getUserTherapist.FirstName + " " + getUserTherapist.LastName);
                    body1 = body1.Replace("{Datetime}", bookingDate + " " + getBooking.FromTime + " to " + getBooking.ToTime);

                    mail.SendGeneralMail("Patient Booking Confirmation", getUserPatientEmail.Email, body1);
                }


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
        public IHttpActionResult CompleteBooking(PatientBookingModel model)
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
