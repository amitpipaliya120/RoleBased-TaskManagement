using RoleBasedTaskManagement.Domain.Models;
using System.Collections.Generic;
namespace RoleBasedTaskManagement.Application.Interfaces
{
    public interface ITaskRepository
    {
        bool SaveTask(TaskModel task);
        bool DeleteTask(int id);
        TaskModel? GetTaskById(int id);
        List<TaskModel> GetAllTasks();
        List<TaskModel> GetTasksByAssigneeId(int assigneeId);
        List<TaskModel> GetTasksByManagerId(int managerId);
    }
}
