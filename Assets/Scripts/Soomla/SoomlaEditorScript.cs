using UnityEngine;

namespace Soomla
{
	public class SoomlaEditorScript : ScriptableObject
	{
		public static string AND_PUB_KEY_DEFAULT = "YOUR GOOGLE PLAY PUBLIC KEY";

		public static string ONLY_ONCE_DEFAULT = "SET ONLY ONCE";

		private const string soomSettingsAssetName = "SoomlaEditorScript";

		private const string soomSettingsPath = "Soomla/Resources";

		private const string soomSettingsAssetExtension = ".asset";

		private static SoomlaEditorScript instance;

		[SerializeField]
		public ObjectDictionary SoomlaSettings = new ObjectDictionary();

		public static SoomlaEditorScript Instance
		{
			get
			{
				if (instance == null)
				{
					instance = (Resources.Load("SoomlaEditorScript") as SoomlaEditorScript);
					if (instance == null)
					{
						instance = ScriptableObject.CreateInstance<SoomlaEditorScript>();
					}
				}
				return instance;
			}
		}

		public static void DirtyEditor()
		{
		}

		public static void SetConfigValue(string prefix, string key, string value)
		{
			PlayerPrefs.SetString("Soomla." + prefix + "." + key, value);
			Instance.SoomlaSettings["Soomla." + prefix + "." + key] = value;
			PlayerPrefs.Save();
		}

		public static string GetConfigValue(string prefix, string key)
		{
			if (Instance.SoomlaSettings.TryGetValue("Soomla." + prefix + "." + key, out string value) && value.Length > 0)
			{
				return value;
			}
			value = PlayerPrefs.GetString("Soomla." + prefix + "." + key);
			SetConfigValue(prefix, key, value);
			return (value.Length <= 0) ? null : value;
		}
	}
}
