using Microsoft.AspNetCore.Mvc;
using RoleBasedTaskManagement.Application.Interfaces;
using RoleBasedTaskManagement.Domain.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
namespace RoleBasedTaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        public CommentsController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        [HttpGet("task/{taskId}")]
        public ActionResult<IEnumerable<CommentModel>> GetByTaskId(int taskId)
        {
            return Ok(_commentRepository.GetCommentsByTaskId(taskId));
        }
        [HttpPost]
        public IActionResult Create([FromBody] CommentModel comment)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                comment.UserId = userId;
            }
            comment.Flag = "INSERT";
            var success = _commentRepository.SaveComment(comment);
            if (success) return Ok(new { message = "Comment added successfully" });
            return BadRequest(new { message = "Failed to add comment." });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Delete(int id)
        {
            var success = _commentRepository.DeleteComment(id);
            if (success) return Ok(new { message = "Comment deleted successfully" });
            return NotFound(new { message = "Comment not found or delete failed" });
        }
    }
}
