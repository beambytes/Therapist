using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using TherapistAPI.Models;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Net.Http;
using System.IO;
using System.Xml.Serialization;
//using TherapistAPI.Common;

namespace TherapistAPI.DataAccess
{
    public class BookingDL
    {
        SqlConnection conn = new
                        SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        Common objCommon = new Common();

        public BookingModel GetBooking(int userId, int refType, int bookingID)
        {
            DataSet DS = new DataSet();
            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_GetBooking]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@BookingID", bookingID);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            BookingModel model = new BookingModel();

            if (bookingID > 0)
            {
                List<BookingModel> m = objCommon.ConvertDataTable<BookingModel>(ds.Tables[2]);
                model = m[0];

                model.BookingSerList = objCommon.ConvertDataTable<BookingSerModel>(ds.Tables[3]);
                model.EHCbenefitsList = objCommon.ConvertDataTable<EHCbenefitsModel>(ds.Tables[4]);

                model.TherapistList = objCommon.ConvertDataTable<TherapistModel>(ds.Tables[5]);
            }
            model.User = objCommon.ConvertDataTable<UsersModel>(ds.Tables[0]);
            model.CurrentDateTime = DateTime.Now;
            model.ServiceList = objCommon.ConvertDataTable<ServicesModel>(ds.Tables[1]);

            return model;
        }

        public void SaveBooking(BookingModel model, string userId)
        {
            string BookingSerList = "";
            if (model.BookingSerList != null && model.BookingSerList.Count > 0)
            {
                BookingSerList = ObjectToXMLGeneric(model.BookingSerList);
                BookingSerList = BookingSerList.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            }
            else
            {
                model.BookingSerList = new List<BookingSerModel>();
            }

            string EHCbenefitsList = "";
            if (model.EHCbenefitsList != null && model.EHCbenefitsList.Count > 0)
            {
                EHCbenefitsList = ObjectToXMLGeneric(model.EHCbenefitsList);
                EHCbenefitsList = EHCbenefitsList.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
            }
            else
            {
                model.EHCbenefitsList = new List<EHCbenefitsModel>();
            }

            if (model.DOB == DateTime.MinValue)
            {
                model.DOB = null;
            }

            if (model.PlicyDOB == DateTime.MinValue)
            {
                model.PlicyDOB = null;
            }

            model.BookingDate = DateTime.Now;
            model.EnteredOn = DateTime.Now;
            model.EnteredBy = userId;
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_SaveBooking]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientID", model.PatientID);
            cmd.Parameters.AddWithValue("@TherapistID", model.TherapistID);
            //cmd.Parameters.AddWithValue("@BookingDate", model.BookingDate);
            cmd.Parameters.AddWithValue("@EnteredBy", model.EnteredBy);
            //cmd.Parameters.AddWithValue("@EnteredOn", model.EnteredOn);
            cmd.Parameters.AddWithValue("@PatientName", model.PatientName);
            cmd.Parameters.AddWithValue("@DOB", model.DOB);
            cmd.Parameters.AddWithValue("@InsuranceComp", model.InsuranceComp);
            cmd.Parameters.AddWithValue("@PolicyNo", model.PolicyNo);
            cmd.Parameters.AddWithValue("@PlicyDOB", model.PlicyDOB);
            cmd.Parameters.AddWithValue("@RelPat", model.RelPat);
            cmd.Parameters.AddWithValue("@AssBen", model.AssBen);
            cmd.Parameters.AddWithValue("@IsInvOtherHelthCare", model.IsInvOtherHelthCare);
            cmd.Parameters.AddWithValue("@BookingSerXML", BookingSerList);
            cmd.Parameters.AddWithValue("@ECHBenXML", EHCbenefitsList);
            cmd.Parameters.AddWithValue("@BookingID", model.BookingID);
            cmd.Parameters.AddWithValue("@Identity", 0);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }


        


        public DashboardModel GetDashboard(int userId, int RefType, int RefID)
        {
            DataSet DS = new DataSet();

            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_GetDashboard]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RefType", RefType);
            cmd.Parameters.AddWithValue("@RefID", RefID);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            DashboardModel model = new DashboardModel();


            List<DashboardModel> dashboard = objCommon.ConvertDataTable<DashboardModel>(ds.Tables[0]);
            model.Assign = dashboard[0].Assign;
            model.Completed = dashboard[0].Completed;
            model.Approve = dashboard[0].Approve;
            model.Pending = dashboard[0].Pending;
            model.BookingCount = dashboard[0].BookingCount;
            model.TherapistCount = dashboard[0].TherapistCount;
            model.PatientCount = dashboard[0].PatientCount;

            model.BookingList = objCommon.ConvertDataTable<PatientBookingModel>(ds.Tables[1]);
            return model;
        }

        public List<PatientBookingModel> GetAllBooking(int userId, int RefType, int RefID, BookingModel model1)
        {
            DataSet DS = new DataSet();

            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_GetBookingList]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RefType", RefType);
            cmd.Parameters.AddWithValue("@RefID", RefID);
            cmd.Parameters.AddWithValue("@Status", model1.Status);
            cmd.Parameters.AddWithValue("@TherapistName", model1.TherapistName);
            cmd.Parameters.AddWithValue("@PatientName", model1.PatientName);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            List<PatientBookingModel> model = objCommon.ConvertDataTable<PatientBookingModel>(ds.Tables[0]);
            return model;
        }

        public void ApproveCancelBooking(PatientBookingModel model)
        {

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_ApproveCancelBooking]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientBookingID", model.PatientBookingID);
            cmd.Parameters.AddWithValue("@TherapistID", model.TherapistID);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void CompleteBooking(PatientBookingModel model)
        {

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }

            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_CompleteBooking]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@PatientBookingID", model.PatientBookingID);
            cmd.Parameters.AddWithValue("@Status", model.Status);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public String ObjectToXMLGeneric<T>(T filter)
        {

            string xml = null;
            using (StringWriter sw = new StringWriter())
            {

                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(sw, filter);
                try
                {
                    xml = sw.ToString();

                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return xml;
        }
    }
}