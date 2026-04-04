using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AcademicPlanner.Services;

namespace AcademicPlanner.Platforms.Android;

public class NotificationManagerService : INotificationManagerService
{
    private const string ChannelId = "default";
    private const string ChannelName = "Default";
    private const string ChannelDescription = "Academic Planner notifications";

    public const string TitleKey = "title";
    public const string MessageKey = "message";

    private bool _channelInitialized;
    private int _messageId = 1000;
    private int _pendingIntentId = 2000;

    private NotificationManagerCompat? _compatManager;

    public event EventHandler? NotificationReceived;

    public static NotificationManagerService? Instance { get; private set; }

    public NotificationManagerService()
    {
        if (Instance is null)
        {
            CreateNotificationChannel();
            _compatManager = NotificationManagerCompat.From(Platform.AppContext);
            Instance = this;
        }
        else
        {
            _compatManager = NotificationManagerCompat.From(Platform.AppContext);
        }
    }

    public void SendNotification(string title, string message, DateTime? notifyTime = null)
    {
        if (!_channelInitialized)
            CreateNotificationChannel();

        if (notifyTime is null)
        {
            Show(title, message);
            return;
        }

        Intent intent = new Intent(Platform.AppContext, typeof(AlarmHandler));
        intent.PutExtra(TitleKey, title);
        intent.PutExtra(MessageKey, message);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        var pendingIntentFlags = Build.VERSION.SdkInt >= BuildVersionCodes.S
            ? PendingIntentFlags.CancelCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.CancelCurrent;

        PendingIntent? pendingIntent = PendingIntent.GetBroadcast(
            Platform.AppContext,
            _pendingIntentId++,
            intent,
            pendingIntentFlags);

        AlarmManager? alarmManager =
            Platform.AppContext.GetSystemService(Context.AlarmService) as AlarmManager;

        if (alarmManager is null || pendingIntent is null)
            return;

        long triggerTime = GetNotifyTime(notifyTime.Value);
        alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
    }

    public void ReceiveNotification(string title, string message)
    {
        NotificationReceived?.Invoke(this, EventArgs.Empty);
    }

    public void Show(string title, string message)
    {
        if (!_channelInitialized)
            CreateNotificationChannel();

        _compatManager ??= NotificationManagerCompat.From(Platform.AppContext);

        Intent intent = new Intent(Platform.AppContext, typeof(MainActivity));
        intent.PutExtra(TitleKey, title);
        intent.PutExtra(MessageKey, message);
        intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);

        var pendingIntentFlags = Build.VERSION.SdkInt >= BuildVersionCodes.S
            ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            : PendingIntentFlags.UpdateCurrent;

        PendingIntent? pendingIntent = PendingIntent.GetActivity(
            Platform.AppContext,
            _pendingIntentId++,
            intent,
            pendingIntentFlags);

        if (pendingIntent is null || _compatManager is null)
            return;

        var builder = new NotificationCompat.Builder(Platform.AppContext, ChannelId)
            .SetContentIntent(pendingIntent)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetSmallIcon(global::AcademicPlanner.Resource.Mipmap.appicon)
            .SetAutoCancel(true);

        _compatManager.Notify(_messageId++, builder.Build());
    }

    private void CreateNotificationChannel()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(
                ChannelId,
                ChannelName,
                NotificationImportance.Default)
            {
                Description = ChannelDescription
            };

            var manager =
                (NotificationManager?)Platform.AppContext.GetSystemService(Context.NotificationService);

            manager?.CreateNotificationChannel(channel);
        }

        _channelInitialized = true;
    }

    private static long GetNotifyTime(DateTime notifyTime)
    {
        DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
        double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
        long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
        return utcAlarmTime;
    }
}