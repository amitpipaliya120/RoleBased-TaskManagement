using RoleBasedTaskManagement.Domain.Models;
using System.Collections.Generic;
namespace RoleBasedTaskManagement.Application.Interfaces
{
    public interface ITeamRepository
    {
        bool SaveTeam(TeamModel team);
        bool DeleteTeam(int id);
        TeamModel? GetTeamById(int id);
        List<TeamModel> GetAllTeams();
    }
}
