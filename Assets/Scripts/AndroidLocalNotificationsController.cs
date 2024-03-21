using System;
using System.Diagnostics;
using UnityEngine;

public class AndroidLocalNotificationsController : ILocalNotificationController
{
	private bool _logEnabled;

	private static AndroidJavaObject _notifManager;

	private static void CheckIfInitialized()
	{
		if (_notifManager == null && Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.mobirix.unityplugins.localnotifications.LocalNotificationsManager"))
			{
				_notifManager = androidJavaClass.CallStatic<AndroidJavaObject>("GetInstance", new object[0]);
			}
		}
	}

	private static void SetLocaleNotificationAndroid(DateTime date, string title, string message, string soundName)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			CheckIfInitialized();
			DateTime dateTime = new DateTime(1970, 1, 1);
			TimeZone currentTimeZone = TimeZone.CurrentTimeZone;
			dateTime = dateTime.AddHours(currentTimeZone.GetUtcOffset(DateTime.Now).TotalHours);
			long num = (date.Ticks - dateTime.Ticks) / 10000;
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.Date", num);
			_notifManager.Call("ScheduleLocalNotification", androidJavaObject, title, message, soundName);
		}
	}

	private static void RemoveAllLocalNotifications()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			CheckIfInitialized();
			_notifManager.Call("RemoveAllActiveNotifications");
		}
	}

	private static void ShowLogInNotificationPlugin(bool enable)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			CheckIfInitialized();
			_notifManager.Call("ShowLog", enable);
		}
	}

	public void ScheduleLocalNotification(DateTime date, string title, string message, string soundPath)
	{
		if (title != null && message != null)
		{
			SetLocaleNotificationAndroid(date, title, message, soundPath);
		}
	}

	public void CancelAllNotifications()
	{
		RemoveAllLocalNotifications();
	}

	public void EnableLogs(bool enable)
	{
		ShowLogInNotificationPlugin(enable);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_DEBUG_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
