namespace MultiServices.Maui.Services.Notification;

public class NotificationService
{
    public async Task<bool> RequestPermissionAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            }
            return status == PermissionStatus.Granted;
        }
        catch { return false; }
    }

    public async Task ShowLocalNotificationAsync(string title, string body)
    {
        // Using Plugin.LocalNotification
        var notification = new Plugin.LocalNotification.NotificationRequest
        {
            NotificationId = new Random().Next(1000),
            Title = title,
            Description = body,
            ReturningData = "notification_data",
        };
        await Plugin.LocalNotification.LocalNotificationCenter.Current.Show(notification);
    }
}
