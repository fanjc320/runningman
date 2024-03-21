using UnityEngine;

public class Settings : MonoBehaviour
{
	private static bool _optionsLoaded;

	private const string OPTION_SOUND_KEY = "OPTION_SOUND";

	private const int OPTION_SOUND_DEFAULT = 1;

	private static bool _optionSound;

	private const string OPTION_REMINDER_KEY = "OPTION_REMINDER_WITH_DEFAULT_OFF";

	private const int OPTION_REMINDER_DEFAULT = 0;

	private static bool _optionReminder;

	public static bool optionSound
	{
		get
		{
			LoadOptionsIfNeeded();
			return _optionSound;
		}
		set
		{
			_optionSound = value;
			PlayerPrefs.SetInt("OPTION_SOUND", _optionSound ? 1 : 0);
			AudioListener.volume = ((!_optionSound) ? 0f : 1f);
		}
	}

	public static bool optionReminder
	{
		get
		{
			LoadOptionsIfNeeded();
			return _optionReminder;
		}
		set
		{
			_optionReminder = value;
			PlayerPrefs.SetInt("OPTION_REMINDER_WITH_DEFAULT_OFF", _optionReminder ? 1 : 0);
		}
	}

	private void Awake()
	{
		LoadOptionsIfNeeded();
	}

	private static void LoadOptionsIfNeeded()
	{
		if (!_optionsLoaded)
		{
			_optionSound = (PlayerPrefs.GetInt("OPTION_SOUND", 1) != 0);
			AudioListener.volume = ((!_optionSound) ? 0f : 1f);
			_optionReminder = (PlayerPrefs.GetInt("OPTION_REMINDER_WITH_DEFAULT_OFF", 0) != 0);
			_optionsLoaded = true;
		}
	}
}
