using System;

namespace Soomla.Store
{
	public class StoreSettings : ISoomlaSettings
	{
		private static string StoreModulePrefix = "Store";

		public static string AND_PUB_KEY_DEFAULT = "YOUR GOOGLE PLAY PUBLIC KEY";

		public static string PLAY_CLIENT_ID_DEFAULT = "YOUR CLIENT ID";

		public static string PLAY_CLIENT_SECRET_DEFAULT = "YOUR CLIENT SECRET";

		public static string PLAY_REFRESH_TOKEN_DEFAULT = "YOUR REFRESH TOKEN";

		private static string androidPublicKey;

		private static string playClientId;

		private static string playClientSecret;

		private static string playRefreshToken;

		public static string playVerifyOnServerFailure;

		private static string androidTestPurchases;

		private static string playSsvValidation;

		private static string iosSSV;

		private static string iosVerifyOnServerFailure;

		private static string noneBP;

		private static string gPlayBP;

		private static string amazonBP;

		private static string wP8SimulatorBuild;

		private static string wP8TestMode;

		public static string AndroidPublicKey
		{
			get
			{
				if (androidPublicKey == null)
				{
					androidPublicKey = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "AndroidPublicKey");
					if (androidPublicKey == null)
					{
						androidPublicKey = AND_PUB_KEY_DEFAULT;
					}
				}
				return androidPublicKey;
			}
			set
			{
				if (androidPublicKey != value)
				{
					androidPublicKey = value;
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "AndroidPublicKey", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static string PlayClientId
		{
			get
			{
				if (playClientId == null)
				{
					playClientId = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "PlayClientId");
					if (playClientId == null)
					{
						playClientId = PLAY_CLIENT_ID_DEFAULT;
					}
				}
				return playClientId;
			}
			set
			{
				if (playClientId != value)
				{
					playClientId = value;
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "PlayClientId", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static string PlayClientSecret
		{
			get
			{
				if (playClientSecret == null)
				{
					playClientSecret = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "PlayClientSecret");
					if (playClientSecret == null)
					{
						playClientSecret = PLAY_CLIENT_SECRET_DEFAULT;
					}
				}
				return playClientSecret;
			}
			set
			{
				if (playClientSecret != value)
				{
					playClientSecret = value;
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "PlayClientSecret", value);
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static string PlayRefreshToken
		{
			get
			{
				if (playRefreshToken == null)
				{
					playRefreshToken = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "PlayRefreshToken");
					if (playRefreshToken == null)
					{
						playRefreshToken = PLAY_REFRESH_TOKEN_DEFAULT;
					}
				}
				return playRefreshToken;
			}
			set
			{
				if (playRefreshToken != value)
				{
					playRefreshToken = value;
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "PlayRefreshToken", value);
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool PlayVerifyOnServerFailure
		{
			get
			{
				if (playVerifyOnServerFailure == null)
				{
					playVerifyOnServerFailure = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "PlayVerifyOnServerFailure");
					if (playVerifyOnServerFailure == null)
					{
						playVerifyOnServerFailure = false.ToString();
					}
				}
				return Convert.ToBoolean(playVerifyOnServerFailure);
			}
			set
			{
				if (playVerifyOnServerFailure != value.ToString())
				{
					playVerifyOnServerFailure = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "PlayVerifyOnServerFailure", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool AndroidTestPurchases
		{
			get
			{
				if (androidTestPurchases == null)
				{
					androidTestPurchases = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "AndroidTestPurchases");
					if (androidTestPurchases == null)
					{
						androidTestPurchases = false.ToString();
					}
				}
				return Convert.ToBoolean(androidTestPurchases);
			}
			set
			{
				if (androidTestPurchases != value.ToString())
				{
					androidTestPurchases = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "AndroidTestPurchases", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool PlaySsvValidation
		{
			get
			{
				if (playSsvValidation == null)
				{
					playSsvValidation = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "PlaySsvValidation");
					if (playSsvValidation == null)
					{
						playSsvValidation = false.ToString();
					}
				}
				return Convert.ToBoolean(playSsvValidation);
			}
			set
			{
				if (playSsvValidation != value.ToString())
				{
					playSsvValidation = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "PlaySsvValidation", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool IosSSV
		{
			get
			{
				if (iosSSV == null)
				{
					iosSSV = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "IosSSV");
					if (iosSSV == null)
					{
						iosSSV = false.ToString();
					}
				}
				return Convert.ToBoolean(iosSSV);
			}
			set
			{
				if (iosSSV != value.ToString())
				{
					iosSSV = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "IosSSV", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool IosVerifyOnServerFailure
		{
			get
			{
				if (iosVerifyOnServerFailure == null)
				{
					iosVerifyOnServerFailure = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "IosVerifyOnServerFailure");
					if (iosVerifyOnServerFailure == null)
					{
						iosVerifyOnServerFailure = false.ToString();
					}
				}
				return Convert.ToBoolean(iosVerifyOnServerFailure);
			}
			set
			{
				if (iosVerifyOnServerFailure != value.ToString())
				{
					iosVerifyOnServerFailure = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "IosVerifyOnServerFailure", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool NoneBP
		{
			get
			{
				if (noneBP == null)
				{
					noneBP = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "NoneBP");
					if (noneBP == null)
					{
						noneBP = false.ToString();
					}
				}
				return Convert.ToBoolean(noneBP);
			}
			set
			{
				if (noneBP != value.ToString())
				{
					noneBP = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "NoneBP", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool GPlayBP
		{
			get
			{
				if (gPlayBP == null)
				{
					gPlayBP = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "GPlayBP");
					if (gPlayBP == null)
					{
						gPlayBP = false.ToString();
					}
				}
				return Convert.ToBoolean(gPlayBP);
			}
			set
			{
				if (gPlayBP != value.ToString())
				{
					gPlayBP = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "GPlayBP", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool AmazonBP
		{
			get
			{
				if (amazonBP == null)
				{
					amazonBP = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "AmazonBP");
					if (amazonBP == null)
					{
						amazonBP = false.ToString();
					}
				}
				return Convert.ToBoolean(amazonBP);
			}
			set
			{
				if (amazonBP != value.ToString())
				{
					amazonBP = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "AmazonBP", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool WP8SimulatorBuild
		{
			get
			{
				if (wP8SimulatorBuild == null)
				{
					wP8SimulatorBuild = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "WP8SimulatorBuild");
					if (wP8SimulatorBuild == null)
					{
						wP8SimulatorBuild = false.ToString();
					}
				}
				return Convert.ToBoolean(wP8SimulatorBuild);
			}
			set
			{
				if (wP8SimulatorBuild != value.ToString())
				{
					wP8SimulatorBuild = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "WP8SimulatorBuild", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}

		public static bool WP8TestMode
		{
			get
			{
				if (wP8TestMode == null)
				{
					wP8TestMode = SoomlaEditorScript.GetConfigValue(StoreModulePrefix, "WP8TestMode");
					if (wP8TestMode == null)
					{
						wP8TestMode = false.ToString();
					}
				}
				return Convert.ToBoolean(wP8TestMode);
			}
			set
			{
				if (wP8TestMode != value.ToString())
				{
					wP8TestMode = value.ToString();
					SoomlaEditorScript.SetConfigValue(StoreModulePrefix, "WP8TestMode", value.ToString());
					SoomlaEditorScript.DirtyEditor();
				}
			}
		}
	}
}
