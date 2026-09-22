using RoleBasedTaskManagement.Domain.Models;
using System.Collections.Generic;
namespace RoleBasedTaskManagement.Application.Interfaces
{
    public interface ICommentRepository
    {
        bool SaveComment(CommentModel comment);
        bool DeleteComment(int id);
        List<CommentModel> GetCommentsByTaskId(int taskId);
    }
}
