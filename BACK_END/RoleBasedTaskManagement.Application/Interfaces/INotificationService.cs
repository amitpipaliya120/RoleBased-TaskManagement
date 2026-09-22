namespace RoleBasedTaskManagement.Application.Interfaces
{
    public interface INotificationService
    {
        void SendTaskAssignmentNotification(string userEmail, string taskTitle);
        void SendTaskStatusUpdateNotification(string userEmail, string taskTitle, string newStatus);
    }
}
