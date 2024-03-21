using UnityEngine;

public static class FirebasePlugin
{
	private static string FBAClass = "net.wenee.plugin_firebase.WFirebasePlugin";

	private static bool isInit;

	public static void Init()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(FBAClass))
		{
			androidJavaClass.CallStatic("FireBaseInit");
		}
		isInit = true;
	}

	public static void LogEvent(FBALogEvent eventLog)
	{
		if (isInit)
		{
			string text = JsonUtility.ToJson(eventLog);
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(FBAClass))
			{
				androidJavaClass.CallStatic("FireBaseLogEvent", text);
			}
		}
	}

	public static string GetCustomPayload()
	{
		string empty = string.Empty;
		if (isInit)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(FBAClass))
			{
				return androidJavaClass.CallStatic<string>("GetCustomPayload", new object[0]);
			}
		}
		return empty;
	}
}
