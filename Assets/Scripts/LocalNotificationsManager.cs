using UnityEngine;

public class LocalNotificationsManager
{
	public const int TIME_OF_NOTIFICATION = 17;

	public const string ONLINE_SETTING = "enable_local_notifications";

	public const string NOTIFICATION_SOUND = "cpush.wav";

	public static LocalNotificationsManager _instance;

	public static ILocalNotificationController _notificationCtrl;

	public ILocalNotificationController NotificationController => _notificationCtrl;

	public static LocalNotificationsManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new LocalNotificationsManager();
				if (Application.platform == RuntimePlatform.Android)
				{
					_notificationCtrl = new AndroidLocalNotificationsController();
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					_notificationCtrl = new IOSLocalNotificationsController();
				}
				else
				{
					_notificationCtrl = new EditorLocalNotificationCtrl();
				}
			}
			return _instance;
		}
	}
}
