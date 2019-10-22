using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using TherapistAPI.DataAccess;
using TherapistAPI.Models;


namespace TherapistAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        private static Random random = new Random();
        therapistEntities entities = new therapistEntities();

        [HttpPost]
        [Route("GetUsers")]
        public IHttpActionResult GetAllUsers(UsersModel model)
        {
            try
            {
                RegisterDL obj = new RegisterDL();
                var list = obj.GetAllUsers(model);
                return Ok(list);

            }
            catch (Exception ex)
            {
                responseData.success = false;
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [HttpPost]
        [Route("GetUserDetails")]
        public IHttpActionResult GetUserDetails(UsersModel model)
        {
            try
            {
                RegisterDL obj = new RegisterDL();
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;


                var list = obj.GetUserDetails(model.UserID == 0 ? Convert.ToInt32(UserId) : model.UserID, Convert.ToInt32(RefType));
                return Ok(list);

            }
            catch (Exception ex)
            {
                responseData.success = false;
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [HttpPost]
        [Route("ApproveUser")]
        public IHttpActionResult ApproveUser(UsersModel model)
        {
            try
            {
                RegisterDL obj = new RegisterDL();
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;
                obj.ApproveUser(model.UserID);
                responseData.success = true;
                responseData.code = 200;
                responseData.message = "User approve successfully";

                var list = obj.GetUserDetails(model.UserID, Convert.ToInt32(RefType));
                string subject;
                SendMail mail = new SendMail();
                if (list[0].RefType == 2) // Patient
                {
                    subject = "Patient Approval";
                    string body = mail.createEmailBody("PatientApproval.html");
                    body = body.Replace("{UserName}", list[0].FirstName + " " + list[0].LastName);

                    mail.SendGeneralMail(subject, list[0].Email, body);
                   

                }
                else {
                    subject = "Therapist Approval";
                    string body = mail.createEmailBody("TherapistApproval.html");
                    body = body.Replace("{UserName}", list[0].FirstName + " " + list[0].LastName);

                    mail.SendGeneralMail(subject, list[0].Email, body);
                }

                return Ok(responseData);
            }
            catch (Exception ex)
            {
                responseData.success = false;
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

       

        [HttpPost]
        [Route("ChangePassword")]
        public IHttpActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                RegisterDL obj = new RegisterDL();
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                obj.ChangePassword(model, Convert.ToInt32(UserId));
                responseData.success = true;
                responseData.code = 200;
                responseData.message = "Password changed successfully";
                return Ok(responseData);

            }
            catch (Exception ex)
            {
                responseData.success = false;
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [Route("UpdateProfile")]
        public IHttpActionResult UpdateProfile(UsersModel model)
        {
            try
            {

                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;

                model.ServiceID = HttpContext.Current.Request.Form["ServiceID"];
                model.ServiceArea = HttpContext.Current.Request.Form["ServiceArea"];
                RegisterDL obj = new RegisterDL();
                obj.UpdateProfile(model,Convert.ToInt32(UserId),Convert.ToInt32(RefID), Convert.ToInt32(RefType));


                // Upload Files
                HttpFileCollection files = HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    //string path = HttpContext.Current.Server.MapPath("~/Uploads/" + model.RefType + "_" + RefID);
                    //if (!Directory.Exists(path))
                    //{
                    //    Directory.CreateDirectory(path);
                    //}

                    for (int iCnt = 0; iCnt <= files.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = files[iCnt];
                        if (hpf != null && !string.IsNullOrEmpty(hpf.FileName))
                        {
                            DocumentModel doc = new DocumentModel();
                            Stream str = hpf.InputStream;
                            BinaryReader Br = new BinaryReader(str);
                            //string fileName = RandomString(8) + Path.GetExtension(hpf.FileName);
                            //string filePath = Path.Combine(path, fileName);
                            //hpf.SaveAs(filePath);
                            Byte[] FileDet = Br.ReadBytes((Int32)str.Length);
                            string base64String = Convert.ToBase64String(FileDet, 0, FileDet.Length);

                            doc.RefId = Convert.ToInt32(RefID);
                            doc.RefType = Convert.ToInt32(RefType);
                            //doc.DocPath = filePath;
                            doc.DocName = hpf.FileName;
                            doc.FileContent = base64String;
                            string ext = Path.GetExtension(hpf.FileName).ToUpper();
                            if (ext == ".PDF")
                            {
                                doc.MimeType = "application/pdf";
                            }
                            else if (ext == ".JPEG" || ext == ".JPG")
                            {
                                doc.MimeType = "image/jpeg";
                            }
                            else if (ext == ".PNG")
                            {
                                doc.MimeType = "image/png";
                            }
                            obj.TransDocument(doc);
                        }
                    }
                };

                responseData.success = true;
                responseData.message = "Profile Update successfully.";
                responseData.code = (int)HttpStatusCode.Created;
                return Ok(responseData);

            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [Route("GetTherapistCalender")]
        public IHttpActionResult GetTherapistCalender()
        {
            try
            {
                return Ok(entities.TherapistCalenders.ToList());

            }
            catch (Exception ex)
            {
                responseData.success = false;
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [HttpPost]
        [Route("SaveTherapistCalender")]
        public IHttpActionResult SaveTherapistCalender(TherapistCalender model)
        {
            try
            {
                RegisterDL obj = new RegisterDL();
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;

                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;

                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;

                TherapistCalender tc = new TherapistCalender();
                tc.Day = model.Day;
                tc.EnteredOn = DateTime.Now;
                tc.FromTime = model.FromTime;
                tc.ToTime = model.ToTime;
                tc.TherapistID = Convert.ToInt32(RefID);
                entities.TherapistCalenders.Add(tc);
                entities.SaveChanges();

                responseData.success = true;
                responseData.message = "Save calender successfully.";
                return Ok(responseData);
            }
            catch (Exception ex)
            {
                responseData.success = false;
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        [HttpPost]
        [Route("DeleteTherapistCalender")]
        public IHttpActionResult DeleteTherapistCalender(TherapistCalender model)
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;
                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;


                TherapistCalender deptDelete = entities.TherapistCalenders.Find(model.TherapistCalenderID);
                entities.TherapistCalenders.Remove(deptDelete);
                entities.SaveChanges();

                ResponseData responseData = new ResponseData();

                responseData.success = true;
                responseData.message = "Record delete successfully";
                responseData.code = 200;

                

                return Ok(model);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //[HttpGet]
        //[Route("CountryList")]
        //public IHttpActionResult CountryList()
        //{
        //    try
        //    {
        //        GeneralBusiness generalBusiness = new GeneralBusiness(false);
        //        var listCountries = generalBusiness.GetAllCountries();

        //        responseData.success = true;
        //        responseData.code = 200;
        //        responseData.message = "country list updated successfully";
        //        responseData.data = listCountries;
        //        return Ok(responseData);
        //    }
        //    catch (Exception ex)
        //    {
        //        responseData.message = "server error";
        //        return Ok(responseData);
        //    }
        //}
        //[HttpGet]
        //[Route("StateList/{countryId}")]
        //public IHttpActionResult StateList(Guid countryId)
        //{
        //    try
        //    {
        //        GeneralBusiness generalBusiness = new GeneralBusiness(false);
        //        var listStates = generalBusiness.GetStatesByCountry(countryId);

        //        responseData.success = true;
        //        responseData.code = 200;
        //        responseData.message = "states list updated successfully";
        //        responseData.data = listStates;
        //        return Ok(responseData);
        //    }
        //    catch (Exception ex)
        //    {
        //        responseData.message = "server error";
        //        return Ok(responseData);
        //    }
        //}
        //[HttpGet]
        //[Route("CityList/{stateId}")]
        //public IHttpActionResult CityList(Guid stateId)
        //{
        //    try
        //    {
        //        GeneralBusiness generalBusiness = new GeneralBusiness(false);
        //        var listCities = generalBusiness.GetCitiesByState(stateId);

        //        responseData.success = true;
        //        responseData.code = 200;
        //        responseData.message = "cities list updated successfully";
        //        responseData.data = listCities;
        //        return Ok(responseData);
        //    }
        //    catch (Exception ex)
        //    {
        //        responseData.message = "server error";
        //        return Ok(responseData);
        //    }
        //}

        /// <summary>
        /// Create access token and refresh token for user login with facebook or google
        /// </summary>
        /// <param name="objUser">User Model Object</param>
        /// <returns>Return Json object with access_token and refresh_token</returns>
        private UserTokenViewModel GenerateAccessTokenForUser(UsersModel userVM)
        {
            var tokenExpirationTimeSpan = TimeSpan.FromDays(1);
            // var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);
            //identity.AddClaim(new Claim(ClaimTypes.Role, userVM.UserType));
            //identity.AddClaim(new Claim(ClaimTypes.Email, userVM.EmailID));
            //identity.AddClaim(new Claim(ClaimTypes.Name, Convert.ToString(userVM.UserID)));

            ClaimsIdentity oAuthIdentity =
            new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, Convert.ToString(userVM.RefType)));
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Sid, Convert.ToString(userVM.RefID)));
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Convert.ToString(userVM.UserID)));
            oAuthIdentity.AddClaim(new Claim(ClaimTypes.Email, userVM.Email));

            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, new AuthenticationProperties());
            var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(tokenExpirationTimeSpan);
            var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
            AuthenticationTokenCreateContext rtokenContext = new AuthenticationTokenCreateContext(HttpContext.Current.GetOwinContext(), Startup.OAuthOptions.AccessTokenFormat, ticket);

            AuthenticationTokenProvider rtokenProvider = new AuthenticationTokenProvider();
            var refresh_token = rtokenProvider.CreateRefreshToken(rtokenContext);
            UserTokenViewModel userToken = new UserTokenViewModel();
            userToken.access_token = accessToken;
            userToken.expires_in = tokenExpirationTimeSpan.TotalSeconds.ToString();
            userToken.refresh_token = refresh_token;
            return userToken;
        }

    }
}
