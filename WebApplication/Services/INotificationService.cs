using WebApplication.Models;

namespace WebApplication.Services
{
    public interface INotificationService
    {
        event Action<NotificationMessage> OnNotificationReceived;
        void ShowSuccess(string message, int durationMs = 4000);
        void ShowError(string message, int durationMs = 5000);
        void ShowWarning(string message, int durationMs = 4000);
        void ShowInfo(string message, int durationMs = 3000);
    }
}