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
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        public TasksController(ITaskRepository taskRepository, IUserRepository userRepository, INotificationService notificationService)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public ActionResult<IEnumerable<TaskModel>> GetAll()
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (role == "Manager")
            {
                var managerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                                     ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(managerIdClaim)) return Unauthorized();
                var tasks = _taskRepository.GetTasksByManagerId(int.Parse(managerIdClaim));
                return Ok(tasks);
            }
            var allTasks = _taskRepository.GetAllTasks();
            return Ok(allTasks);
        }
        [HttpGet("{id}")]
        public ActionResult<TaskModel> GetById(int id)
        {
            var task = _taskRepository.GetTaskById(id);
            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }
            return Ok(task);
        }
        [HttpGet("assignee/{assigneeId}")]
        public ActionResult<IEnumerable<TaskModel>> GetByAssignee(int assigneeId)
        {
            var tasks = _taskRepository.GetTasksByAssigneeId(assigneeId);
            return Ok(tasks);
        }
        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create([FromBody] TaskModel task)
        {
            task.Flag = "INSERT";
            var success = _taskRepository.SaveTask(task);
            if (success)
            {
                if (task.AssigneeId.HasValue)
                {
                    var assignee = _userRepository.GetUserById(task.AssigneeId.Value);
                    if (assignee != null && !string.IsNullOrEmpty(assignee.Email))
                    {
                        _notificationService.SendTaskAssignmentNotification(assignee.Email, task.Title ?? "New Task");
                    }
                }
                return Ok(new { message = "Task created successfully" });
            }
            return BadRequest(new { message = "Failed to create task." });
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Update(int id, [FromBody] TaskModel task)
        {
            task.TaskId = id;
            task.Flag = "UPDATE";
            var success = _taskRepository.SaveTask(task);
            if (success)
            {
                return Ok(new { message = "Task updated successfully" });
            }
            return NotFound(new { message = "Task not found or update failed" });
        }
        public class StatusUpdateDto {
            public string Status { get; set; } = string.Empty;
        }
        [HttpPatch("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            if (string.IsNullOrEmpty(dto?.Status)) return BadRequest(new { message = "Status is required" });
            var task = _taskRepository.GetTaskById(id);
            if (task == null) return NotFound(new { message = "Task not found" });
            task.Status = dto.Status;
            task.Flag = "UPDATE_STATUS";
            var success = _taskRepository.SaveTask(task);
            if (success)
            {
                if (task.AssigneeId.HasValue)
                {
                    var assignee = _userRepository.GetUserById(task.AssigneeId.Value);
                    if (assignee != null && !string.IsNullOrEmpty(assignee.Email))
                    {
                        _notificationService.SendTaskStatusUpdateNotification(assignee.Email, task.Title ?? "Task", dto.Status);
                    }
                }
                return Ok(new { message = "Task status updated successfully" });
            }
            return BadRequest(new { message = "Task status update failed" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Delete(int id)
        {
            var success = _taskRepository.DeleteTask(id);
            if (success)
            {
                return Ok(new { message = "Task deleted successfully" });
            }
            return NotFound(new { message = "Task not found or delete failed" });
        }
    }
}
