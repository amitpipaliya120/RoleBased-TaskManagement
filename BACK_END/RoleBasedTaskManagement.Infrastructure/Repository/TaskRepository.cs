using Microsoft.Data.SqlClient;
using RoleBasedTaskManagement.Application.Interfaces;
using RoleBasedTaskManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
namespace RoleBasedTaskManagement.Infrastructure.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDbConnectionRepository _dbConnection;
        public TaskRepository(IDbConnectionRepository dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public bool SaveTask(TaskModel task)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TasksMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TaskId", (object?)task.TaskId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Title", (object?)task.Title ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", (object?)task.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", (object?)task.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Priority", (object?)task.Priority ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@AssigneeId", (object?)task.AssigneeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ManagerId", (object?)task.ManagerId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Deadline", (object?)task.Deadline ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Flag", (object?)task.Flag ?? DBNull.Value);
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
        public bool DeleteTask(int id)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TasksMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TaskId", id);
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
        public TaskModel? GetTaskById(int id)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TasksMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TaskId", id);
                    cmd.Parameters.AddWithValue("@Flag", "GETBYID");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            return MapRowToTask(ds.Tables[0].Rows[0]);
                        }
                        return null;
                    }
                }
            }
        }
        public List<TaskModel> GetAllTasks()
        {
            var taskList = new List<TaskModel>();
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TasksMaster_CRUD", con))
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
                                taskList.Add(MapRowToTask(row));
                            }
                        }
                    }
                }
            }
            return taskList;
        }
        public List<TaskModel> GetTasksByAssigneeId(int assigneeId)
        {
            var taskList = new List<TaskModel>();
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TasksMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AssigneeId", assigneeId);
                    cmd.Parameters.AddWithValue("@Flag", "GET_BY_ASSIGNEE");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                taskList.Add(MapRowToTask(row));
                            }
                        }
                    }
                }
            }
            return taskList;
        }
        public List<TaskModel> GetTasksByManagerId(int managerId)
        {
            var taskList = new List<TaskModel>();
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TasksMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ManagerId", managerId);
                    cmd.Parameters.AddWithValue("@Flag", "GET_BY_MANAGER");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                taskList.Add(MapRowToTask(row));
                            }
                        }
                    }
                }
            }
            return taskList;
        }
        private static TaskModel MapRowToTask(DataRow row)
        {
            var task = new TaskModel();
            if (row.Table.Columns.Contains("TaskId") && row["TaskId"] != DBNull.Value)
                task.TaskId = Convert.ToInt32(row["TaskId"]);
            if (row.Table.Columns.Contains("Title") && row["Title"] != DBNull.Value)
                task.Title = row["Title"]?.ToString();
            if (row.Table.Columns.Contains("Description") && row["Description"] != DBNull.Value)
                task.Description = row["Description"]?.ToString();
            if (row.Table.Columns.Contains("Status") && row["Status"] != DBNull.Value)
                task.Status = row["Status"]?.ToString();
            if (row.Table.Columns.Contains("Priority") && row["Priority"] != DBNull.Value)
                task.Priority = row["Priority"]?.ToString();
            if (row.Table.Columns.Contains("AssigneeId") && row["AssigneeId"] != DBNull.Value)
                task.AssigneeId = Convert.ToInt32(row["AssigneeId"]);
            if (row.Table.Columns.Contains("ManagerId") && row["ManagerId"] != DBNull.Value)
                task.ManagerId = Convert.ToInt32(row["ManagerId"]);
            if (row.Table.Columns.Contains("Deadline") && row["Deadline"] != DBNull.Value)
                task.Deadline = Convert.ToDateTime(row["Deadline"]);
            if (row.Table.Columns.Contains("AssigneeName") && row["AssigneeName"] != DBNull.Value)
                task.AssigneeName = row["AssigneeName"]?.ToString();
            if (row.Table.Columns.Contains("ManagerName") && row["ManagerName"] != DBNull.Value)
                task.ManagerName = row["ManagerName"]?.ToString();
            return task;
        }
    }
}
