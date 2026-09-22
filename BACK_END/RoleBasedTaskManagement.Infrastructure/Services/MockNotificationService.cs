using Microsoft.Extensions.Logging;
using RoleBasedTaskManagement.Application.Interfaces;
namespace RoleBasedTaskManagement.Infrastructure.Services
{
    public class MockNotificationService : INotificationService
    {
        private readonly ILogger<MockNotificationService> _logger;
        public MockNotificationService(ILogger<MockNotificationService> logger)
        {
            _logger = logger;
        }
        public void SendTaskAssignmentNotification(string userEmail, string taskTitle)
        {
            _logger.LogInformation($"[EMAIL SENT to {userEmail}]: You have been assigned a new task -> '{taskTitle}'");
        }
        public void SendTaskStatusUpdateNotification(string userEmail, string taskTitle, string newStatus)
        {
            _logger.LogInformation($"[EMAIL SENT to {userEmail}]: The status of your task '{taskTitle}' is now '{newStatus}'");
        }
    }
}
