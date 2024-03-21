using System.Diagnostics;
using UnityEngine;

public class DeviceUtility
{
	private static AndroidJavaClass _deviceUtilityClass;

	private static string _callbackGameObjectName;

	private static string _callbackDidCloseFunctionName;

	private static string _button1String;

	private static AndroidJavaClass deviceUtilityClass
	{
		get
		{
			if (_deviceUtilityClass == null)
			{
				_deviceUtilityClass = new AndroidJavaClass("com.mobirix.unityplugins.DeviceUtility");
			}
			return _deviceUtilityClass;
		}
	}

	public static string GetSavePath()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.mobirix.unityplugins.DeviceUtility"))
			{
				return androidJavaClass.CallStatic<string>("getFilesDir", new object[0]);
			}
		}
		return Application.persistentDataPath;
	}

	public static int GetApiLevelAndroid()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.mobirix.unityplugins.DeviceUtility"))
			{
				return androidJavaClass.CallStatic<int>("getAPILevel", new object[0]);
			}
		}
		return 0;
	}

	public static string GetExternalSavePath()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.mobirix.unityplugins.DeviceUtility"))
			{
				return androidJavaClass.CallStatic<string>("getExternalFilesDir", new object[0]);
			}
		}
		return null;
	}

	public static string GetDeviceUniqueIdentifier()
	{
		return GetUniqueIdentifierAndroid();
	}

	public static string GetBundleVersion()
	{
		return GetBundleVersionAndroid();
	}

	public static string GetIosSystemVersion()
	{
		return string.Empty;
	}

	public static string GetIosNotificationTypes()
	{
		return string.Empty;
	}

	public static bool IsOtherAudioPlaying()
	{
		return false;
	}

	public static string GetLocale()
	{
		return GetLocaleAndroid();
	}

	public static void showNativePopup(string title, string message, string cancelButtonTitle)
	{
		showNativePopupAndroid(title, message, cancelButtonTitle);
	}

	public static void showNativePopupWithCallback(string callbackGameObjectName, string callbackDidCloseFunctionName, string title, string message, string cancelButtonTitle, string optionalButton2, string optionalButton3)
	{
		showNativePopupWithCallbackAndroid(callbackGameObjectName, callbackDidCloseFunctionName, title, message, cancelButtonTitle, optionalButton2, optionalButton3);
	}

	public static int GetVersionCode()
	{
		return GetVersionCodeAndroid();
	}

	private static string GetUniqueIdentifierAndroid()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return deviceUtilityClass.CallStatic<string>("GetUniqueIdentifier", new object[0]);
		}
		return string.Empty;
	}

	private static string GetBundleVersionAndroid()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return deviceUtilityClass.CallStatic<string>("getVersionName", new object[0]);
		}
		if (Application.isEditor)
		{
		}
		return string.Empty;
	}

	private static int GetVersionCodeAndroid()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return deviceUtilityClass.CallStatic<int>("getVersionCode", new object[0]);
		}
		return -1;
	}

	private static string GetLocaleAndroid()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return deviceUtilityClass.CallStatic<string>("GetLocale", new object[0]);
		}
		return string.Empty;
	}

	private static void showNativePopupAndroid(string title, string message, string cancelButtonTitle)
	{
	}

	public static void showNativePopupWithCallbackAndroid(string callbackGameObjectName, string callbackDidCloseFunctionName, string title, string message, string cancelButtonTitle, string optionalButton2, string optionalButton3)
	{
		_callbackGameObjectName = callbackGameObjectName;
		_callbackDidCloseFunctionName = callbackDidCloseFunctionName;
		_button1String = cancelButtonTitle;
		if (optionalButton2 == null)
		{
		}
	}

	private static void alertButtonClickedEvent(string button)
	{
		GameObject gameObject = GameObject.Find(_callbackGameObjectName);
		string value = (!button.Equals(_button1String)) ? "1" : "0";
		gameObject.SendMessage(_callbackDidCloseFunctionName, value);
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, Object context = null)
	{
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, Object context = null)
	{
	}

	[Conditional("ENABLE_DEBUG_LOGS")]
	public static void Log(string msg, Object context = null)
	{
	}
}
