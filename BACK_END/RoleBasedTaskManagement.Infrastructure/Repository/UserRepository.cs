using Microsoft.Data.SqlClient;
using RoleBasedTaskManagement.Application.Interfaces;
using RoleBasedTaskManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
namespace RoleBasedTaskManagement.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionRepository _dbConnection;
        public UserRepository(IDbConnectionRepository dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public bool SaveUser(UserModel user)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_UsersMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", (object?)user.UserId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Name", (object?)user.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object?)user.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PasswordHash", (object?)user.PasswordHash ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RoleId", (object?)user.RoleId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TeamId", (object?)user.TeamId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ManagerId", (object?)user.ManagerId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", (object?)user.IsActive ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Flag", (object?)user.Flag ?? DBNull.Value);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            string code = ds.Tables[0].Rows[0]["Code"]?.ToString() ?? "";
                            return code == "200" || code == "201";
                        }
                        return false;
                    }
                }
            }
        }
        public bool DeleteUser(int id)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_UsersMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", id);
                    cmd.Parameters.AddWithValue("@Flag", "DELETE");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            string code = ds.Tables[0].Rows[0]["Code"]?.ToString() ?? "";
                            return code == "200";
                        }
                        return false;
                    }
                }
            }
        }
        public UserModel? GetUserById(int id)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_UsersMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", id);
                    cmd.Parameters.AddWithValue("@Flag", "GETBYID");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            return MapRowToUser(ds.Tables[0].Rows[0]);
                        }
                        return null;
                    }
                }
            }
        }
        public UserModel? GetUserByEmail(string email)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_UsersMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Flag", "GET_BY_EMAIL");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            return MapRowToUser(ds.Tables[0].Rows[0]);
                        }
                        return null;
                    }
                }
            }
        }
        public List<UserModel> GetAllUsers()
        {
            var userList = new List<UserModel>();
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_UsersMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Flag", "GETALL");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                userList.Add(MapRowToUser(row));
                            }
                        }
                    }
                }
            }
            return userList;
        }
        public List<UserModel> GetUsersByFlag(UserModel model)
        {
            var userList = new List<UserModel>();
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_UsersMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Flag", model.Flag);
                    cmd.Parameters.AddWithValue("@ManagerId", (object?)model.ManagerId ?? DBNull.Value);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                userList.Add(MapRowToUser(row));
                            }
                        }
                    }
                }
            }
            return userList;
        }
        private static UserModel MapRowToUser(DataRow row)
        {
            var user = new UserModel();
            if (row.Table.Columns.Contains("UserId") && row["UserId"] != DBNull.Value)
                user.UserId = Convert.ToInt32(row["UserId"]);
            if (row.Table.Columns.Contains("Name") && row["Name"] != DBNull.Value)
                user.Name = row["Name"]?.ToString();
            if (row.Table.Columns.Contains("Email") && row["Email"] != DBNull.Value)
                user.Email = row["Email"]?.ToString();
            if (row.Table.Columns.Contains("PasswordHash") && row["PasswordHash"] != DBNull.Value)
                user.PasswordHash = row["PasswordHash"]?.ToString();
            if (row.Table.Columns.Contains("RoleId") && row["RoleId"] != DBNull.Value)
                user.RoleId = Convert.ToInt32(row["RoleId"]);
            if (row.Table.Columns.Contains("RoleName") && row["RoleName"] != DBNull.Value)
                user.RoleName = row["RoleName"]?.ToString();
            if (row.Table.Columns.Contains("TeamId") && row["TeamId"] != DBNull.Value)
                user.TeamId = Convert.ToInt32(row["TeamId"]);
            if (row.Table.Columns.Contains("ManagerId") && row["ManagerId"] != DBNull.Value)
                user.ManagerId = Convert.ToInt32(row["ManagerId"]);
            if (row.Table.Columns.Contains("ManagerName") && row["ManagerName"] != DBNull.Value)
                user.ManagerName = row["ManagerName"]?.ToString();
            if (row.Table.Columns.Contains("IsActive") && row["IsActive"] != DBNull.Value)
                user.IsActive = Convert.ToBoolean(row["IsActive"]);
            return user;
        }
    }
}
