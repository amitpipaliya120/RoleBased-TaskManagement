using Microsoft.AspNetCore.Mvc;
using RoleBasedTaskManagement.Application.Interfaces;
using RoleBasedTaskManagement.Domain.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
namespace RoleBasedTaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamRepository _teamRepository;
        public TeamsController(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<IEnumerable<TeamModel>> GetAll()
        {
            return Ok(_teamRepository.GetAllTeams());
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<TeamModel> GetById(int id)
        {
            var team = _teamRepository.GetTeamById(id);
            if (team == null) return NotFound(new { message = "Team not found" });
            return Ok(team);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody] TeamModel team)
        {
            team.Flag = "INSERT";
            var success = _teamRepository.SaveTeam(team);
            if (success) return Ok(new { message = "Team created successfully" });
            return BadRequest(new { message = "Failed to create team. Name may exist." });
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] TeamModel team)
        {
            team.TeamId = id;
            team.Flag = "UPDATE";
            var success = _teamRepository.SaveTeam(team);
            if (success) return Ok(new { message = "Team updated successfully" });
            return NotFound(new { message = "Team not found or update failed" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var success = _teamRepository.DeleteTeam(id);
            if (success) return Ok(new { message = "Team deleted successfully" });
            return NotFound(new { message = "Team not found or delete failed" });
        }
    }
}
