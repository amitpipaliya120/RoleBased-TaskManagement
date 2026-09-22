namespace RoleBasedTaskManagement.Domain.Models
{
    public class CommentModel
    {
        public int? CommentId { get; set; }
        public int? TaskId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? CommentText { get; set; }
        public string? Flag { get; set; }
    }
}
