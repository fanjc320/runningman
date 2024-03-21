using System;

public interface ILocalNotificationController
{
	void ScheduleLocalNotification(DateTime date, string title, string message, string soundName);

	void CancelAllNotifications();

	void EnableLogs(bool enable);
}
