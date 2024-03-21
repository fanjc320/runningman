using System;

namespace Soomla
{
	public class CoreSettings : ISoomlaSettings
	{
		private static string CoreModulePrefix = "Core";

		public static string ONLY_ONCE_DEFAULT = "SET ONLY ONCE";

		private static string soomlaSecret;

		private static string debugMessages;

		private static string debugUnityMessages;

		public static string SoomlaSecret
		{
			get
			{
				if (soomlaSecret == null)
				{
					soomlaSecret = SoomlaEditorScript.GetConfigValue(CoreModulePrefix, "SoomlaSecret");
					if (soomlaSecret == null)
					{
						soomlaSecret = ONLY_ONCE_DEFAULT;
					}
				}
				return soomlaSecret;
			}
			set
			{
				if (soomlaSecret != value)
				{
					soomlaSecret = value;
					SoomlaEditorScript.SetConfigValue(CoreModulePrefix, "SoomlaSecret", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool DebugMessages
		{
			get
			{
				if (debugMessages == null)
				{
					debugMessages = SoomlaEditorScript.GetConfigValue(CoreModulePrefix, "DebugMessages");
					if (debugMessages == null)
					{
						debugMessages = false.ToString();
					}
				}
				return Convert.ToBoolean(debugMessages);
			}
			set
			{
				if (Convert.ToBoolean(debugMessages) != value)
				{
					debugMessages = value.ToString();
					SoomlaEditorScript.SetConfigValue(CoreModulePrefix, "DebugMessages", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool DebugUnityMessages
		{
			get
			{
				if (debugUnityMessages == null)
				{
					debugUnityMessages = SoomlaEditorScript.GetConfigValue(CoreModulePrefix, "DebugUnityMessages");
					if (debugUnityMessages == null)
					{
						debugUnityMessages = true.ToString();
					}
				}
				return Convert.ToBoolean(debugUnityMessages);
			}
			set
			{
				if (Convert.ToBoolean(debugUnityMessages) != value)
				{
					debugUnityMessages = value.ToString();
					SoomlaEditorScript.SetConfigValue(CoreModulePrefix, "DebugUnityMessages", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}
	}
}
