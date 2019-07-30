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
using System.Web.Http.ModelBinding;
using TherapistAPI.DataAccess;
using TherapistAPI.Models;


namespace TherapistAPI.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Register")]
    public class RegisterController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        private static Random random = new Random();

        [Route("Register")]
        public IHttpActionResult Register(UsersModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    responseData.message = ModelStateErrors(ModelState);
                    return Ok(responseData);
                }
                //DateTime a = DateTime.ParseExact(model.ExpDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                //DateTime b = Convert.ToDateTime(model.DOB);
                model.ServiceID = HttpContext.Current.Request.Form["ServiceID"];
                RegisterDL obj = new RegisterDL();
                int Id = obj.Register(model);


                // Upload Files
                HttpFileCollection files = HttpContext.Current.Request.Files;
                if (files.Count > 0)
                {
                    string path = HttpContext.Current.Server.MapPath("~/Uploads/" + model.RefType + "_" + Id);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    for (int iCnt = 0; iCnt <= files.Count - 1; iCnt++)
                    {
                        System.Web.HttpPostedFile hpf = files[iCnt];
                        if (hpf != null && !string.IsNullOrEmpty(hpf.FileName))
                        {
                            DocumentModel doc = new DocumentModel();
                            string fileName = RandomString(8) + Path.GetExtension(hpf.FileName);
                            string filePath = Path.Combine(path, fileName);
                            hpf.SaveAs(filePath);
                            doc.RefId = Id;
                            doc.RefType = model.RefType;
                            doc.DocPath = filePath;

                            obj.TransDocument(doc);
                        }
                    }

                };

                responseData.success = true;
                responseData.message = "Register successfully.";
                responseData.code = (int)HttpStatusCode.Created;
                return Ok(responseData);

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

        [HttpGet]
        public IHttpActionResult GetAllServices()
        {
            try
            {
                RegisterDL obj = new RegisterDL();

                var list = obj.GetAllServices();

                responseData.success = true;
                responseData.code = 200;
                responseData.message = "country list updated successfully";
                responseData.data = list;
                return Ok(responseData);
            }
            catch (Exception ex)
            {
                responseData.message = "server error";
                return Ok(responseData);
            }
        }

        [AllowAnonymous]
        [Route("Login")]
        public IHttpActionResult LoginUser(UsersModel model)
        {
            try
            {
                RegisterDL obj = new RegisterDL();

               
                UsersModel list = obj.UserLogin(model);

                responseData.success = true;
                responseData.code = 200;
                responseData.message = "Login successfully";
                UserTokenViewModel userToken = GenerateAccessTokenForUser(list);
                list.UserTokenInfo = userToken;
                responseData.success = true;
                responseData.message = "Login Successfully";
                responseData.data = list;
                responseData.code = 200;

                return Ok(responseData);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }





        #region private methods
        private string ModelStateErrors(ModelStateDictionary modelState)
        {
            return string.Join(" | ", modelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage));
        }
        #endregion

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
