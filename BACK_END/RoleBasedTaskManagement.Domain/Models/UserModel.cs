namespace RoleBasedTaskManagement.Domain.Models
{
    public class UserModel
    {
        public int? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public int? TeamId { get; set; }
        public int? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public bool? IsActive { get; set; }
        public string? Flag { get; set; }
    }
}
