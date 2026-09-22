using System;
namespace RoleBasedTaskManagement.Domain.Models
{
    public class TaskModel
    {
        public int? TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public int? AssigneeId { get; set; }
        public int? ManagerId { get; set; }
        public DateTime? Deadline { get; set; }
        public string? AssigneeName { get; set; }
        public string? ManagerName { get; set; }
        public string? Flag { get; set; }
    }
}
