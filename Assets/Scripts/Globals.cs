using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Globals
{
	public enum LogInState
	{
		Offline,
		Facebook,
		GameCenter,
		Both
	}

	public struct AddedAnimationEventInfo
	{
		public Animation animation;

		public string clipName;

		public float time;

		public string functionName;

		public AddedAnimationEventInfo(Animation animation, string clipName, float time, string functionName)
		{
			this.animation = animation;
			this.clipName = clipName;
			this.time = time;
			this.functionName = functionName;
		}
	}

	public const bool DEBUG_SOCIAL_MANAGER_SERVER = false;

	public const bool DEBUG = false;

	public const bool DEBUG_FREE_PURCHASES = false;

	public const bool DEBUG_ALL_CHARS = false;

	public const bool DEBUG_ALL_BOARDS = false;

	public const bool DEBUG_FREE_INAPP_PURCHASE = false;

	public const bool DEBUG_USE_DEBUG_DAILYWORD = false;

	public const bool DEBUG_SKIP_TUTORIAL_IN_EDITOR = false;

	public const bool DEBUG_USE_FPS_SCRIPT = false;

	public const bool DEBUG_USE_DEBUG_LOGIN_STATE = false;

	public const bool DEBUG_USE_FAKE_ONLINE_SETTINGS = false;

	public const bool DEBUG_OVERRIDE_ONLINE_SETTINGS_URL = false;

	public const bool DEBUG_SKIP_DAILY_LETTERS = false;

	public const bool DEBUG_SAVE_ME_ENABLED = false;

	public const bool DEBUG_PRINT_SPAWNED_POWER_UPS = false;

	public const string DEBUG_DAILYWORD = "xxxxxxx";

	public const string OVERRIDEN_ONLINE_SETTINGS_URL = "";

	public const LogInState DEBUG_LOGIN_STATE = LogInState.Facebook;

	public const string fakeOnlineSettings = "";

	public const int MAX_MULTIPLIER = 30;

	public const int MAX_RANK = 1;

	public const int BONUS_FACEBOOK = 5000;

	public const int BONUS_GAMECENTER = 250;

	public const int INITIAL_AMOUNT_OF_KEYS = 5;

	public const int MIN_FRIEND_SCORE_REQUEST_INTERVAL = 15;

	public const int GIFT_BREADCRUMBS_TO_SHOW = 30;

	public const string SAVE_ME_DEFAULT_LOCALES = "all";

	public const string BASE_URL = "";

	public const string FLURRY_API_KEY = "HTSJ6ZHZGMYSSSJZPRRS";

	public const string FLURRY_ADSPACE = "Test";

	public const string ADCOLONY_APPVERSION = "1.5";

	public const string ADCOLONY_APPID = "";

	public const string ADCOLONY_ZONEID = "";

	public const string CHARTBOOST_APPID = "";

	public const string CHARTBOOST_APPSIGNATURE = "";

	public const string VUNGLE_APPID = "";

	public const string W3i_OFFERS_APPID = "";

	public const int PLAYERINFO_REDUNDANT_FILES = 5;

	public const int PLAYERINFO_BACKUP_SLOTS = 5;

	public const int PLAYERINFO_ALTERNATE_REDUNDANT_FILES = 0;

	public const int PLAYERINFO_ALTERNATE_BACKUP_SLOTS = 0;

	public const int DAILY_QUEST_MIN_REQUEST_INTERVAL = 120;

	public const string DAILY_QUEST_SECRET = "xxx";

	public const string DAILY_QUEST_URL = "";

	public const string PRIVACY_POLICY_URL = "";

	public const int LAYER_2DGUI = 30;

	public const int LAYER_2DOVERLAY = 28;

	public const int LAYER_3DCLIP = 29;

	public const int LAYER_2DOVERLAY_CLIP = 26;

	public const float DRAG_THRESHOLD = 0.08f;

	public const float BUTTON_TWEEN_DURATION = 0.05f;

	public static List<AddedAnimationEventInfo> addedAnimEvents = new List<AddedAnimationEventInfo>();

	public static string RATING_URL
	{
		get
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				return string.Empty;
			}
			if (Application.platform == RuntimePlatform.Android)
			{
				return string.Empty;
			}
			return string.Empty;
		}
	}

	public static LogInState GetLoginState()
	{
		return LogInState.Offline;
	}

	public static Dictionary<T, int> convertStringToEnumIntDictionary<T>(string sourceString)
	{
		string[] array = sourceString.Split(',');
		Dictionary<T, int> dictionary = new Dictionary<T, int>(array.Length / 2);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string value = array[i];
			string s = array[i + 1];
			if (Enum.IsDefined(typeFromHandle, value))
			{
				T key = (T)Enum.Parse(typeFromHandle, value, ignoreCase: true);
				if (int.TryParse(s, out int result))
				{
					dictionary[key] = result;
				}
			}
		}
		return dictionary;
	}

	public static string convertEnumIntDictionaryToString<T>(Dictionary<T, int> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, int> item in sourceDict)
		{
			string name = Enum.GetName(typeof(T), item.Key);
			string arg = item.Value.ToString();
			stringBuilder.AppendFormat("{0},{1},", name, arg);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static Dictionary<T, bool> convertStringToEnumBoolDictionary<T>(string sourceString)
	{
		string[] array = sourceString.Split(',');
		Dictionary<T, bool> dictionary = new Dictionary<T, bool>(array.Length / 2);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string value = array[i];
			string value2 = array[i + 1];
			if (Enum.IsDefined(typeFromHandle, value))
			{
				T key = (T)Enum.Parse(typeFromHandle, value, ignoreCase: true);
				if (bool.TryParse(value2, out bool result))
				{
					dictionary[key] = result;
				}
			}
		}
		return dictionary;
	}

	public static string convertEnumBoolDictionaryToString<T>(Dictionary<T, bool> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, bool> item in sourceDict)
		{
			string name = Enum.GetName(typeof(T), item.Key);
			string arg = item.Value.ToString();
			stringBuilder.AppendFormat("{0},{1},", name, arg);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static Dictionary<string, string> convertStringToStringStringDictionary(string sourceString)
	{
		string[] array = sourceString.Split(',');
		Dictionary<string, string> dictionary = new Dictionary<string, string>(array.Length / 2);
		for (int i = 0; i < array.Length - 1; i += 2)
		{
			string key = array[i];
			string text2 = dictionary[key] = array[i + 1];
		}
		return dictionary;
	}

	public static string convertStringStringDictionaryToString(Dictionary<string, string> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<string, string> item in sourceDict)
		{
			string key = item.Key;
			string value = item.Value;
			stringBuilder.AppendFormat("{0},{1},", key, value);
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static Dictionary<T, int[]> convertStringToEnumIntArrayDictionary<T>(string sourceString)
	{
		string[] array = sourceString.Split(';');
		Dictionary<T, int[]> dictionary = new Dictionary<T, int[]>(array.Length);
		Type typeFromHandle = typeof(T);
		for (int i = 0; i < array.Length; i++)
		{
			if (string.IsNullOrEmpty(array[i]))
			{
				continue;
			}
			string[] array2 = array[i].Split('-');
			string value = array2[0];
			string text = array2[1];
			if (!Enum.IsDefined(typeFromHandle, value))
			{
				continue;
			}
			T key = (T)Enum.Parse(typeFromHandle, value, ignoreCase: true);
			string[] array3 = text.Split(',');
			int[] array4 = new int[array3.Length];
			for (int j = 0; j < array3.Length; j++)
			{
				if (int.TryParse(array3[j], out int result))
				{
					array4[j] = result;
				}
			}
			dictionary[key] = array4;
		}
		return dictionary;
	}

	public static string convertEnumIntArrayDictionaryToString<T>(Dictionary<T, int[]> sourceDict)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<T, int[]> item in sourceDict)
		{
			string name = Enum.GetName(typeof(T), item.Key);
			StringBuilder stringBuilder2 = new StringBuilder();
			for (int i = 0; i < item.Value.Length; i++)
			{
				stringBuilder2.AppendFormat("{0},", item.Value[i]);
			}
			stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
			stringBuilder.AppendFormat("{0}-{1};", name, stringBuilder2.ToString());
		}
		if (stringBuilder.Length > 0)
		{
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
		}
		return stringBuilder.ToString();
	}

	public static bool TryAddAnimationEvent(Animation animation, string clipName, AnimationEvent aniEvent)
	{
		foreach (AddedAnimationEventInfo addedAnimEvent in addedAnimEvents)
		{
			AddedAnimationEventInfo current = addedAnimEvent;
			if (current.animation == animation && current.clipName == clipName && current.time == aniEvent.time && current.functionName == aniEvent.functionName)
			{
				return false;
			}
		}
		addedAnimEvents.Add(new AddedAnimationEventInfo(animation, clipName, aniEvent.time, aniEvent.functionName));
		animation[clipName].clip.AddEvent(aniEvent);
		return true;
	}
}
