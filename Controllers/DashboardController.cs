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
    [RoutePrefix("api/Dashboard")]
    public class DashboardController : ApiController
    {
        private ResponseData responseData = new ResponseData();
        
        [HttpGet]
        [Route("GetDashboard")]
        public IHttpActionResult GetDashboard()
        {
            try
            {
                var UserId = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
                var RefType = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name)).Value;
                var RefID = ((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Sid)).Value;

                BookingDL    obj = new BookingDL();
                DashboardModel list = obj.GetDashboard(Convert.ToInt32(UserId), Convert.ToInt32(RefType),Convert.ToInt32(RefID));

                list.success = true;
                list.message = "Get dashboard details Successfully";
                list.code = 200;
                
                return Ok(list);
            }
            catch (Exception ex)
            {
                responseData.message = ex.Message != null ? ex.Message.ToString() : "server error";
                return Ok(responseData);
            }
        }
        

    }
}
