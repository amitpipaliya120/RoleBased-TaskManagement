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
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<IEnumerable<UserModel>> GetAll()
        {
            var users = _userRepository.GetAllUsers();
            return Ok(users);
        }
        [HttpGet("{id}")]
        public ActionResult<UserModel> GetById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create([FromBody] UserModel user)
        {
            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            }
            user.Flag = "INSERT";
            var success = _userRepository.SaveUser(user);
            if (success)
            {
                return Ok(new { message = "User created successfully" });
            }
            return BadRequest(new { message = "Failed to create user. Email may already exist." });
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] UserModel user)
        {
            user.UserId = id;
            user.Flag = "UPDATE";
            var success = _userRepository.SaveUser(user);
            if (success)
            {
                return Ok(new { message = "User updated successfully" });
            }
            return NotFound(new { message = "User not found or update failed" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var success = _userRepository.DeleteUser(id);
            if (success)
            {
                return Ok(new { message = "User deleted successfully" });
            }
            return NotFound(new { message = "User not found or delete failed" });
        }
        [HttpGet("managers")]
        [Authorize(Roles = "Admin")]
        public ActionResult<IEnumerable<UserModel>> GetManagers()
        {
            var model = new UserModel { Flag = "GET_MANAGERS" };
            var managers = _userRepository.GetUsersByFlag(model);
            return Ok(managers);
        }
        [HttpGet("my-users")]
        [Authorize(Roles = "Manager")]
        public ActionResult<IEnumerable<UserModel>> GetMyUsers()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                              ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized(new { message = "User ID claim missing in token." });
            var model = new UserModel { Flag = "GET_MY_USERS", ManagerId = int.Parse(userIdClaim) };
            var users = _userRepository.GetUsersByFlag(model);
            return Ok(users);
        }
    }
}
