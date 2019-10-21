using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using TherapistAPI.Models;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Net.Http;
//using TherapistAPI.Common;

namespace TherapistAPI.DataAccess
{
    public class RegisterDL
    {
        SqlConnection conn = new
                        SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        Common objCommon = new Common();

        public List<ServicesModel> GetAllServices()
        {
            DataSet DS = new DataSet();
            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_GetAllServices]", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            List<ServicesModel> lstUsers = new List<ServicesModel>();
            lstUsers = objCommon.ConvertDataTable<ServicesModel>(ds.Tables[0]);
            return lstUsers;
        }

        public UsersModel UserLogin(UsersModel UserLogin)
        {
            DataSet DS = new DataSet();
            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_UserLogin]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmailId", UserLogin.Email);
            cmd.Parameters.AddWithValue("@Password", UserLogin.Password);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            List<UsersModel> lstUsers = new List<UsersModel>();
            lstUsers = objCommon.ConvertDataTable<UsersModel>(ds.Tables[0]);
            return lstUsers[0];
        }



        public int Register(UsersModel UserLogin)
        {
            int Id;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            //DateTime? DOB;
            //if (UserLogin.DOB == "")
            //{
            //    DOB = null;
            //}
            //else {
            //    DOB = DateTime.ParseExact(UserLogin.DOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            //}

            //DateTime? ExpDate;
            //if (UserLogin.ExpDate == "")
            //{
            //    ExpDate = null;
            //}
            //else
            //{
            //    ExpDate = DateTime.ParseExact(UserLogin.ExpDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            //}

            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_Register]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FirstName", UserLogin.FirstName);
            cmd.Parameters.AddWithValue("@LastName", UserLogin.LastName);
            cmd.Parameters.AddWithValue("@Email", UserLogin.Email);
            cmd.Parameters.AddWithValue("@Phone", UserLogin.Phone);
            cmd.Parameters.AddWithValue("@Password", UserLogin.Password);
            cmd.Parameters.AddWithValue("@RefType", UserLogin.RefType);
            cmd.Parameters.AddWithValue("@InsuranceNo", UserLogin.InsuranceNo);
            cmd.Parameters.AddWithValue("@Age", UserLogin.Age);

            cmd.Parameters.AddWithValue("@ExpDate", null);
            if (UserLogin.RefType == 1)
            {
                cmd.Parameters.AddWithValue("@DOB", null);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DOB", UserLogin.DOB);
            }
            
            //New Field
            cmd.Parameters.AddWithValue("@AddrLine1", UserLogin.AddrLine1);
            cmd.Parameters.AddWithValue("@AddrLine2", UserLogin.AddrLine2);
            cmd.Parameters.AddWithValue("@City", UserLogin.City);
            cmd.Parameters.AddWithValue("@State", UserLogin.State);
            cmd.Parameters.AddWithValue("@Country", UserLogin.Country);
            cmd.Parameters.AddWithValue("@PostalCode", UserLogin.PostalCode);
            cmd.Parameters.AddWithValue("@ServiceID", UserLogin.ServiceID);
            cmd.Parameters.AddWithValue("@BankDetail", UserLogin.BankDetail);
            cmd.Parameters.AddWithValue("@RegistrationNo", UserLogin.RegistrationNo);
            cmd.Parameters.AddWithValue("@Skill", UserLogin.Skill);
            cmd.Parameters.AddWithValue("@ServiceArea", UserLogin.ServiceArea);
            cmd.Parameters.AddWithValue("@Monday", UserLogin.Monday);
            cmd.Parameters.AddWithValue("@Tuesday", UserLogin.Tuesday);
            cmd.Parameters.AddWithValue("@Wednesday", UserLogin.Wednesday);
            cmd.Parameters.AddWithValue("@Thursday", UserLogin.Thursday);
            cmd.Parameters.AddWithValue("@Friday", UserLogin.Friday);
            cmd.Parameters.AddWithValue("@Saturday", UserLogin.Saturday);
            cmd.Parameters.AddWithValue("@Sunday", UserLogin.Sunday);
            cmd.Parameters.AddWithValue("@FromTime", UserLogin.FromTime);
            cmd.Parameters.AddWithValue("@ToTime", UserLogin.ToTime);
            cmd.Parameters.AddWithValue("@Gender", UserLogin.Gender);


            cmd.Parameters.Add("@id", SqlDbType.Int);
            cmd.Parameters["@id"].Direction = ParameterDirection.Output;
            conn.Open();
            cmd.ExecuteNonQuery();
            Id = (int)cmd.Parameters["@id"].Value;
            conn.Close();
            return Id;
        }



        public void UpdateProfile(UsersModel UserLogin, int UserID, int RefID, int RefType)
        {

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }



            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_UpdateProfile]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", UserID);
            cmd.Parameters.AddWithValue("@RefID", RefID);
            cmd.Parameters.AddWithValue("@FirstName", UserLogin.FirstName);
            cmd.Parameters.AddWithValue("@LastName", UserLogin.LastName);
            cmd.Parameters.AddWithValue("@Email", UserLogin.Email);
            cmd.Parameters.AddWithValue("@Phone", UserLogin.Phone);

            cmd.Parameters.AddWithValue("@RefType", RefType);
            cmd.Parameters.AddWithValue("@InsuranceNo", UserLogin.InsuranceNo);
            cmd.Parameters.AddWithValue("@Age", UserLogin.Age);
            if (RefType == 1)
            {
                cmd.Parameters.AddWithValue("@DOB", null);
                cmd.Parameters.AddWithValue("@ExpDate", null);
            }
            else
            {
                cmd.Parameters.AddWithValue("@DOB", UserLogin.DOB);
                cmd.Parameters.AddWithValue("@ExpDate", UserLogin.ExpDate);
            }

            //New Field
            cmd.Parameters.AddWithValue("@AddrLine1", UserLogin.AddrLine1);
            cmd.Parameters.AddWithValue("@AddrLine2", UserLogin.AddrLine2);
            cmd.Parameters.AddWithValue("@City", UserLogin.City);
            cmd.Parameters.AddWithValue("@State", UserLogin.State);
            cmd.Parameters.AddWithValue("@Country", UserLogin.Country);
            cmd.Parameters.AddWithValue("@ServiceID", UserLogin.ServiceID);
            cmd.Parameters.AddWithValue("@BankDetail", UserLogin.BankDetail);
            cmd.Parameters.AddWithValue("@Skill", UserLogin.Skill);
            cmd.Parameters.AddWithValue("@ServiceArea", UserLogin.ServiceArea);
            cmd.Parameters.AddWithValue("@Monday", UserLogin.Monday);
            cmd.Parameters.AddWithValue("@Tuesday", UserLogin.Tuesday);
            cmd.Parameters.AddWithValue("@Wednesday", UserLogin.Wednesday);
            cmd.Parameters.AddWithValue("@Thursday", UserLogin.Thursday);
            cmd.Parameters.AddWithValue("@Friday", UserLogin.Friday);
            cmd.Parameters.AddWithValue("@Saturday", UserLogin.Saturday);
            cmd.Parameters.AddWithValue("@Sunday", UserLogin.Sunday);
            cmd.Parameters.AddWithValue("@FromTime", UserLogin.FromTime);
            cmd.Parameters.AddWithValue("@ToTime", UserLogin.ToTime);
            cmd.Parameters.AddWithValue("@Gender", UserLogin.Gender);

            conn.Open();
            cmd.ExecuteNonQuery();

            conn.Close();
        }

        public void TransDocument(DocumentModel doc)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_TransDocument]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@RefId", doc.RefId);
            cmd.Parameters.AddWithValue("@RefType", doc.RefType);
            cmd.Parameters.AddWithValue("@DocPath", doc.DocPath);
            cmd.Parameters.AddWithValue("@DocName", doc.DocName);
            cmd.Parameters.AddWithValue("@MimeType", doc.MimeType);
            cmd.Parameters.AddWithValue("@FileContent", doc.FileContent);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public List<UsersModel> GetAllUsers()
        {
            DataSet DS = new DataSet();
            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_GetAllUsers]", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            List<UsersModel> lstUsers = new List<UsersModel>();
            lstUsers = objCommon.ConvertDataTable<UsersModel>(ds.Tables[0]);
            return lstUsers;
        }

        public List<UsersModel> GetUserDetails(int userID, int refType)
        {
            DataSet DS = new DataSet();
            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_GetUserDetails]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", userID);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            List<UsersModel> lstUsers = new List<UsersModel>();
            lstUsers = objCommon.ConvertDataTable<UsersModel>(ds.Tables[0]);
            lstUsers[0].ServiceList = new List<ServicesModel>();

            lstUsers[0].Documents = objCommon.ConvertDataTable<DocumentModel>(ds.Tables[1]);
            if (ds.Tables.Contains("Table2"))
            {
                lstUsers[0].ServiceList = objCommon.ConvertDataTable<ServicesModel>(ds.Tables[2]);
            }

            return lstUsers;
        }

        public void ApproveUser(int UserID)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_ApproveUser]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", UserID);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public void ChangePassword(ChangePasswordViewModel model, int UserID)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            SqlCommand cmd = new SqlCommand("[therapistdb].[SP_ChangePassword]", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", UserID);
            cmd.Parameters.AddWithValue("@OldPassword", model.OldPassword);
            cmd.Parameters.AddWithValue("@NewPassword", model.NewPassword);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }

        //public List<UserLoginModel> GetAutoSuggestedUsers(string UserSearchString, string SelectedUsers)
        //{
        //    DataSet DS = new DataSet();
        //    SqlCommand cmd = new SqlCommand("sp_GetAutoSuggestedUsers", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@UserSearchString", UserSearchString);
        //    cmd.Parameters.AddWithValue("@SelectedUsers", SelectedUsers);
        //    SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    adp.Fill(ds);
        //    List<UserLoginModel> lstUsers = new List<UserLoginModel>();
        //    lstUsers = objCommon.ConvertDataTable<UserLoginModel>(ds.Tables[0]);
        //    return lstUsers;
        //}

        //public List<Teams> GetAutoCompleteGameTeam(string SearchString)
        //{
        //    DataSet DS = new DataSet();
        //    SqlCommand cmd = new SqlCommand("sp_GetAutoCompleteGameTeam", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@SearchString", SearchString);
        //    SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    adp.Fill(ds);
        //    List<Teams> lstUsers = new List<Teams>();
        //    lstUsers = objCommon.ConvertDataTable<Teams>(ds.Tables[0]);
        //    return lstUsers;
        //}
        //public List<UserLoginModel> GetAutoSuggestedUsersBasedOnTeam(string UserSearchString, string SelectedUsers, int TeamId)
        //{
        //    DataSet DS = new DataSet();
        //    SqlCommand cmd = new SqlCommand("sp_GetAutoSuggestedUsersBasedOnTeam", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@UserSearchString", UserSearchString);
        //    cmd.Parameters.AddWithValue("@SelectedUsers", SelectedUsers);
        //    cmd.Parameters.AddWithValue("@TeamId", TeamId);
        //    SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    adp.Fill(ds);
        //    List<UserLoginModel> lstUsers = new List<UserLoginModel>();
        //    lstUsers = objCommon.ConvertDataTable<UserLoginModel>(ds.Tables[0]);
        //    return lstUsers;
        //}

        //public List<Teams> GetAutoSuggestUserTeams(string UserSearchString, int GameId, int UserId, string SelectedTeams)
        //{
        //    DataSet DS = new DataSet();
        //    SqlCommand cmd = new SqlCommand("sp_GetUserTeams", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@TeamSearchString", UserSearchString);
        //    cmd.Parameters.AddWithValue("@GameId", GameId);
        //    cmd.Parameters.AddWithValue("@UserId", UserId);
        //    cmd.Parameters.AddWithValue("@SelectedTeams", SelectedTeams);
        //    cmd.Parameters.AddWithValue("@Type", "AutoSuggestTeam");
        //    SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    adp.Fill(ds);
        //    List<Teams> lstTeams = new List<Teams>();
        //    lstTeams = objCommon.ConvertDataTable<Teams>(ds.Tables[0]);
        //    return lstTeams;
        //}

        //public List<Teams> GetUserTeams(int GameId, int UserId)
        //{
        //    DataSet DS = new DataSet();
        //    SqlCommand cmd = new SqlCommand("sp_GetUserTeams", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@GameId", GameId);
        //    cmd.Parameters.AddWithValue("@UserId", UserId);
        //    cmd.Parameters.AddWithValue("@Type", "GetUserTeam");
        //    SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    adp.Fill(ds);
        //    List<Teams> lstTeams = new List<Teams>();
        //    lstTeams = objCommon.ConvertDataTable<Teams>(ds.Tables[0]);
        //    return lstTeams;
        //}

        //public List<Sports> GetAutoCompleteSportTeam(string SearchString,string SelectedGames)
        //{
        //    DataSet DS = new DataSet();
        //    SqlCommand cmd = new SqlCommand("sp_GetAutoCompleteSportsName", conn);
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@SearchString", SearchString);
        //    cmd.Parameters.AddWithValue("@SelectedGames", SelectedGames);
        //    SqlDataAdapter adp = new SqlDataAdapter(cmd);
        //    DataSet ds = new DataSet();
        //    adp.Fill(ds);
        //    List<Sports> lstSports = new List<Sports>();
        //    lstSports = objCommon.ConvertDataTable<Sports>(ds.Tables[0]);
        //    return lstSports;
        //}


    }
}