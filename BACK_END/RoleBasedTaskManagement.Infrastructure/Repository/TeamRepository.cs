using Microsoft.Data.SqlClient;
using RoleBasedTaskManagement.Application.Interfaces;
using RoleBasedTaskManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
namespace RoleBasedTaskManagement.Infrastructure.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly IDbConnectionRepository _dbConnection;
        public TeamRepository(IDbConnectionRepository dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public bool SaveTeam(TeamModel team)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TeamsMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeamId", (object?)team.TeamId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@TeamName", (object?)team.TeamName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Flag", (object?)team.Flag ?? DBNull.Value);
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
        public bool DeleteTeam(int id)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TeamsMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeamId", id);
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
        public TeamModel? GetTeamById(int id)
        {
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TeamsMaster_CRUD", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TeamId", id);
                    cmd.Parameters.AddWithValue("@Flag", "GETBYID");
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            var row = ds.Tables[0].Rows[0];
                            return new TeamModel
                            {
                                TeamId = Convert.ToInt32(row["TeamId"]),
                                TeamName = row["TeamName"]?.ToString()
                            };
                        }
                        return null;
                    }
                }
            }
        }
        public List<TeamModel> GetAllTeams()
        {
            var teamList = new List<TeamModel>();
            using (SqlConnection con = _dbConnection.CreateConnection())
            {
                using (SqlCommand cmd = new SqlCommand("SP_TeamsMaster_CRUD", con))
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
                                teamList.Add(new TeamModel
                                {
                                    TeamId = Convert.ToInt32(row["TeamId"]),
                                    TeamName = row["TeamName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            return teamList;
        }
    }
}
