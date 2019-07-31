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

        public BookingModel GetBooking(int userId, int refType)
        {
            DataSet DS = new DataSet();
            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_GetBooking]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userId);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            BookingModel model = new BookingModel();

            model.User = objCommon.ConvertDataTable<UsersModel>(ds.Tables[0]);
            model.CurrentDateTime = DateTime.Now;
            model.ServiceList = objCommon.ConvertDataTable<ServicesModel>(ds.Tables[1]);
            return model;
        }

        public void SaveBooking(BookingModel model,string userId)
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
            cmd.Parameters.AddWithValue("@BookingDate", model.BookingDate);
            cmd.Parameters.AddWithValue("@EnteredBy", model.EnteredBy);
            cmd.Parameters.AddWithValue("@EnteredOn", model.EnteredOn);
            cmd.Parameters.AddWithValue("@PatientName", model.PatientName);
            cmd.Parameters.AddWithValue("@InsuranceComp", model.InsuranceComp);
            cmd.Parameters.AddWithValue("@PolicyNo", model.PolicyNo);
            cmd.Parameters.AddWithValue("@RelPat", model.RelPat);
            cmd.Parameters.AddWithValue("@AssBen", model.AssBen);
            cmd.Parameters.AddWithValue("@IsInvOtherHelthCare", model.IsInvOtherHelthCare);
            cmd.Parameters.AddWithValue("@BookingSerXML", BookingSerList);
            cmd.Parameters.AddWithValue("@ECHBenXML", EHCbenefitsList);
            cmd.Parameters.AddWithValue("@Identity", 0);
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