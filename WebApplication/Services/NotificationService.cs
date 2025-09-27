using WebApplication.Models;
namespace WebApplication.Services
{
    public class NotificationService : INotificationService
    {
        public event Action<NotificationMessage> OnNotificationReceived;

        public void ShowSuccess(string message, int durationMs = 4000)
        {
            OnNotificationReceived?.Invoke(new NotificationMessage
            {
                Message = message,
                Type = NotificationType.Success,
                DurationMs = durationMs
            });
        }

        public void ShowError(string message, int durationMs = 5000)
        {
            OnNotificationReceived?.Invoke(new NotificationMessage
            {
                Message = message,
                Type = NotificationType.Error,
                DurationMs = durationMs
            });
        }

        public void ShowWarning(string message, int durationMs = 4000)
        {
            OnNotificationReceived?.Invoke(new NotificationMessage
            {
                Message = message,
                Type = NotificationType.Warning,
                DurationMs = durationMs
            });
        }

        public void ShowInfo(string message, int durationMs = 3000)
        {
            OnNotificationReceived?.Invoke(new NotificationMessage
            {
                Message = message,
                Type = NotificationType.Info,
                DurationMs = durationMs
            });
        }
    }
}