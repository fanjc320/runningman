using System;
using System.Diagnostics;
using UnityEngine;

public class Strings
{
	public static string language;

	public static string[] values;

	public static string Language
	{
		get
		{
			return language;
		}
		set
		{
			Load(value);
		}
	}

	public static string Get(StringID key)
	{
		if (language == null)
		{
			Language = "korean";
		}
		return values[(int)key];
	}

	public static string Get(string keyString)
	{
		return (!string.IsNullOrEmpty(keyString)) ? Get((StringID)Enum.Parse(typeof(StringID), keyString, ignoreCase: true)) : null;
	}

	public static bool Exists(string keyString)
	{
		try
		{
			Get(keyString);
		}
		catch (ArgumentException)
		{
			return false;
		}
		return true;
	}

	private static void Load(string language)
	{
		if (!(Strings.language != language))
		{
			return;
		}
		Strings.language = language;
		if (values == null)
		{
			int[] array = (int[])Enum.GetValues(typeof(StringID));
			values = new string[array[array.Length - 1] + 1];
		}
		else
		{
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = null;
			}
		}
		TextAsset textAsset = (TextAsset)Resources.Load("text/" + language, typeof(TextAsset));
		string text = textAsset.text;
		int num = 0;
		string key;
		string value;
		while ((num = StringUtility.GetNextKeyValuePair(text, num, out key, out value)) >= 0)
		{
			int num2 = (int)Enum.Parse(typeof(StringID), key, ignoreCase: true);
			values[num2] = value;
			if (num == text.Length)
			{
				break;
			}
		}
		if (values[0] != null)
		{
			throw new Exception("Strings.Load: String set for " + Enum.GetName(typeof(StringID), 0));
		}
		int num3 = 1;
		while (true)
		{
			if (num3 < values.Length)
			{
				if (values[num3] == null && Enum.IsDefined(typeof(StringID), num3))
				{
					break;
				}
				num3++;
				continue;
			}
			return;
		}
		throw new Exception("Strings.Load: String not set for " + Enum.GetName(typeof(StringID), num3));
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
