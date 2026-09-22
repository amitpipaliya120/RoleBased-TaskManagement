using Microsoft.Data.SqlClient;
using RoleBasedTaskManagement.Application.Interfaces;
using RoleBasedTaskManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
namespace RoleBasedTaskManagement.Infrastructure.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IDbConnectionRepository _dbConnection;
        public CommentRepository(IDbConnectionRepository dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public bool SaveComment(CommentModel comment)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_CommentsMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CommentId", (object?)comment.CommentId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TaskId", (object?)comment.TaskId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserId", (object?)comment.UserId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CommentText", (object?)comment.CommentText ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Flag", (object?)comment.Flag ?? DBNull.Value);
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
        public bool DeleteComment(int id)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_CommentsMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CommentId", id);
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
        public List<CommentModel> GetCommentsByTaskId(int taskId)
        {
            var commentList = new List<CommentModel>();
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_CommentsMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TaskId", taskId);
                    cmd.Parameters.AddWithValue("@Flag", "GET_BY_TASKID");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                commentList.Add(new CommentModel
                                {
                                    CommentId = Convert.ToInt32(row["CommentId"]),
                                    TaskId = Convert.ToInt32(row["TaskId"]),
                                    UserId = Convert.ToInt32(row["UserId"]),
                                    UserName = row.Table.Columns.Contains("UserName") && row["UserName"] != DBNull.Value ? row["UserName"].ToString() : null,
                                    CommentText = row["CommentText"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            return commentList;
        }
    }
}
