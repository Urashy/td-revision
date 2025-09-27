namespace WebApplication.Models
{
    public class NotificationMessage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Message { get; set; } = "";
        public NotificationType Type { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public int DurationMs { get; set; } = 4000;
    }

    public enum NotificationType
    {
        Success,
        Error,
        Warning,
        Info
    }
}