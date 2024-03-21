using Google.Developers;
using SerializableClass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayerInfo
{
	public class HighMetersTarget
	{
		public string Name;

		public Texture TargetPic;

		public int Meter;
	}

	public class MultiplayOppnent
	{
		public string NickName;

		public int CharID;

		public float StartTime;

		public float Meter;

		public int PosX;

		public int PosY;

		public bool Dead;

		public bool Out;

		public bool Ready;

		public string AnimID;

		public string ModelName;
	}

	private static PlayerInfo instance = null;

	private static StringBuilder sbLogText = new StringBuilder(5120);

	private const string SECRET = "wlrmadms1dnflrk2gpdjwudi3gkf4tlrks5ekdmadp6Eh7aksskdy";

	public const int VERSION = 10;

	private bool isSortMissionDirty;

	private static ModelSerializer Serializer = new ModelSerializer();

	private UserSaveData userSaveData;

	private string selectedCharAnimPrefix = string.Empty;

	public string AppVersion = string.Empty;

	public bool IsFirstLaunch;

	public bool IsMenuSceneByTitleScene;

	public Dictionary<string, string> SelCharSkillAttribute;

	public List<HighMetersTarget> HighMetersTargetList = new List<HighMetersTarget>();

	public GameType ThisGameType;

	public DateTime LoadedDateTime = DateTime.Now;

	public bool IsRetryGame;

	public byte[] LastSavedData;

	public bool IsSenseBackBtn = true;

	public MultiplayOppnent ThisGameOpponent;

	public List<List<string>> SortedMissionUI = new List<List<string>>();

	public string SelectedCharIDVolatile;

	public bool[] StartItems = new bool[Enum.GetValues(typeof(StartItemType)).Length];

	public int[] StartItemCounts = new int[Enum.GetValues(typeof(StartItemType)).Length];

	public bool[] CharacterSkills = new bool[Enum.GetValues(typeof(CharacterSkillType)).Length];

	public TimeSpan dailyWordTimeLeft;

	private bool dirtyFrame;

	public static PlayerInfo Instance => instance ?? (instance = new PlayerInfo());

	public StoredStartItemTypeMapBool StoredStartItemsWithGameTypes
	{
		get
		{
			if (userSaveData.Player.StoredStartItemsWithGameTypes == null)
			{
				userSaveData.Player.StoredStartItemsWithGameTypes = new StoredStartItemTypeMapBool(Enumerable.Range(0, Enum.GetValues(typeof(StartItemType)).Length).ToDictionary((int i) => i, (int i) => (from s in Enumerable.Range(0, Enum.GetValues(typeof(GameType)).Length)
					select false).ToArray()));
			}
			return userSaveData.Player.StoredStartItemsWithGameTypes;
		}
	}

	public int TempTotalPlayTimes
	{
		get
		{
			return userSaveData.Player.TempTotalPlayTimes;
		}
		set
		{
			userSaveData.Player.TempTotalPlayTimes = value;
		}
	}

	public double PlayedTimeSpanTotalMilliseconds
	{
		get
		{
			return userSaveData.Player.PlayedTimeSpanTotalMilliseconds;
		}
		set
		{
			userSaveData.Player.PlayedTimeSpanTotalMilliseconds = value;
		}
	}

	public TimeSpan PlayedTimeSpanSinceLoad => DateTime.Now.Subtract(LoadedDateTime);

	public int MultiraceWinCount
	{
		get
		{
			return userSaveData.Player.MultiraceWin;
		}
		set
		{
			userSaveData.Player.MultiraceWin = value;
		}
	}

	public int MultiraceLoseCount
	{
		get
		{
			return userSaveData.Player.MultiraceLose;
		}
		set
		{
			userSaveData.Player.MultiraceLose = value;
		}
	}

	public int LocaleIndex
	{
		get
		{
			try
			{
				return userSaveData.Preference.LocaleIndex;
			}
			catch (NullReferenceException)
			{
				InitNew();
				return userSaveData.Preference.LocaleIndex;
			}
		}
		set
		{
			try
			{
				userSaveData.Preference.LocaleIndex = value;
				DirtyAll();
			}
			catch (NullReferenceException)
			{
				InitNew();
				userSaveData.Preference.LocaleIndex = value;
				DirtyAll();
			}
		}
	}

	public bool MusicOn
	{
		get
		{
			try
			{
				return userSaveData.Preference.MusicOn;
			}
			catch (NullReferenceException)
			{
				InitNew();
				return userSaveData.Preference.MusicOn;
			}
		}
		set
		{
			try
			{
				userSaveData.Preference.MusicOn = value;
				DirtyAll();
			}
			catch (NullReferenceException)
			{
				InitNew();
				userSaveData.Preference.MusicOn = value;
				DirtyAll();
			}
		}
	}

	public bool SoundOn
	{
		get
		{
			try
			{
				return userSaveData.Preference.SoundOn;
			}
			catch (NullReferenceException)
			{
				InitNew();
				return userSaveData.Preference.SoundOn;
			}
		}
		set
		{
			try
			{
				userSaveData.Preference.SoundOn = value;
				DirtyAll();
			}
			catch (NullReferenceException)
			{
				InitNew();
				userSaveData.Preference.SoundOn = value;
				DirtyAll();
			}
		}
	}

	public bool CheckAgreement
	{
		get
		{
			return userSaveData.Player.CheckAgreement;
		}
		set
		{
			userSaveData.Player.CheckAgreement = value;
		}
	}

	public string LastGPGSID
	{
		get
		{
			return userSaveData.Player.GPGSID;
		}
		set
		{
			userSaveData.Player.GPGSID = value;
		}
	}

	public CurrencyTypeMapInt Currency => userSaveData.Player.Currency;

	public int NameTagCount
	{
		get
		{
			return userSaveData.Player.NameTagCount;
		}
		set
		{
			int nameTagCount = userSaveData.Player.NameTagCount;
			userSaveData.Player.NameTagCount = value;
			if (this.OnEvtNameTagCount != null)
			{
				this.OnEvtNameTagCount(value, nameTagCount);
			}
		}
	}

	public int HighScore
	{
		get
		{
			return userSaveData.Player.HighScore;
		}
		set
		{
			userSaveData.Player.HighScore = value;
		}
	}

	public int HighMeters
	{
		get
		{
			return userSaveData.Player.HighMeters;
		}
		set
		{
			userSaveData.Player.HighMeters = value;
			if (this.OnEvtHighMeters != null)
			{
				this.OnEvtHighMeters(value);
			}
		}
	}

	public string SelectedCharID
	{
		get
		{
			return userSaveData.Player.SelectedCharID;
		}
		set
		{
			userSaveData.Player.SelectedCharID = value;
			if (this.OnEvtSelectedCharID != null)
			{
				this.OnEvtSelectedCharID(value);
			}
		}
	}

	public bool WelcomeUpdateReward
	{
		get
		{
			return true;
		}
		set
		{
			userSaveData.Player.WelcomeUpdateReward = value;
		}
	}

	public bool FBFirstLoginReward
	{
		get
		{
			return userSaveData.Player.FBFirstLoginReward;
		}
		set
		{
			userSaveData.Player.FBFirstLoginReward = value;
		}
	}

	public string SelectedCharAnimPrefix => selectedCharAnimPrefix;

	public long TimeRelative
	{
		get
		{
			return userSaveData.Player.TimeRelative;
		}
		set
		{
			userSaveData.Player.TimeRelative = value;
		}
	}

	public long NametagViewADTimeTick
	{
		get
		{
			return userSaveData.Player.NametagViewADTimeTick;
		}
		set
		{
			userSaveData.Player.NametagViewADTimeTick = value;
		}
	}

	public AttributeMapString TimeRelativeAttribute
	{
		get
		{
			if (userSaveData.Player.TimeRelativeAttribute == null)
			{
				userSaveData.Player.TimeRelativeAttribute = new AttributeMapString();
			}
			return userSaveData.Player.TimeRelativeAttribute;
		}
	}

	public bool TutorialCompleted
	{
		get
		{
			return userSaveData.Player.TutorialCompleted;
		}
		set
		{
			userSaveData.Player.TutorialCompleted = value;
		}
	}

	public PlayerParameterTypeMapIntArr CharParamLevels => userSaveData.Character.ParamLevels;

	public AttributeMapBool CharUnlocks => userSaveData.Character.Unlocks;

	public AttributeMapString CharUnlockInfos
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public AttributeMapString CharBusiness => userSaveData.Character.Business;

	public AttributeMapInt CharOwnedTokens => userSaveData.Character.OwnedTokens;

	public AttributeMapString MsnGoalValues => userSaveData.Mission.GoalValues;

	public AttributeMapBool MsnCompleted => userSaveData.Mission.Completed;

	public AttributeMapBool MsnRewarded => userSaveData.Mission.Rewarded;

	public List<string> DailyRandomLimits
	{
		get
		{
			return userSaveData.Mission.DailyRandomLimits;
		}
		set
		{
			userSaveData.Mission.DailyRandomLimits = value;
		}
	}

	public AttributeMapInt MsnCollectableGolals
	{
		get
		{
			if (userSaveData.Mission.CollectableGolals == null)
			{
				userSaveData.Mission.CollectableGolals = new AttributeMapInt();
			}
			return userSaveData.Mission.CollectableGolals;
		}
	}

	public event Action<string> OnEvtSelectedCharID;

	public event Action<int> OnEvtHighMeters;

	public event Action<int, int> OnEvtNameTagCount;

	public PlayerInfo()
	{
		InitAppVersion();
		Load();
		RegistEvents();
		initMission();
	}

	private void InitAppVersion()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		JavaObjWrapper javaObjWrapper = new JavaObjWrapper(@static.GetRawObject());
		JavaObjWrapper javaObjWrapper2 = new JavaObjWrapper(javaObjWrapper.InvokeCall<IntPtr>("getApplicationContext", "()Landroid/content/Context;", new object[0]));
		JavaObjWrapper javaObjWrapper3 = new JavaObjWrapper(javaObjWrapper2.InvokeCall<IntPtr>("getPackageManager", "()Landroid/content/pm/PackageManager;", new object[0]));
		string text = javaObjWrapper2.InvokeCall<string>("getPackageName", "()Ljava/lang/String;", new object[0]);
		JavaObjWrapper javaObjWrapper4 = new JavaObjWrapper(javaObjWrapper3.InvokeCall<IntPtr>("getPackageInfo", "(Ljava/lang/String;I)Landroid/content/pm/PackageInfo;", new object[2]
		{
			new AndroidJavaObject("java.lang.String", text),
			0
		}));
		IntPtr fieldID = AndroidJNI.GetFieldID(AndroidJNI.GetObjectClass(javaObjWrapper4.RawObject), "versionName", "Ljava/lang/String;");
		string stringField = AndroidJNI.GetStringField(javaObjWrapper4.RawObject, fieldID);
		IntPtr fieldID2 = AndroidJNI.GetFieldID(AndroidJNI.GetObjectClass(javaObjWrapper4.RawObject), "versionCode", "I");
		int intField = AndroidJNI.GetIntField(javaObjWrapper4.RawObject, fieldID2);
		AppVersion = $"{stringField}.{intField}";
	}

	public static void LogCallback(string condition, string stackTrace, LogType type)
	{
	}

	public void SortMissionIDs()
	{
		if (!isSortMissionDirty)
		{
			isSortMissionDirty = true;
			LateUpdater.Instance.AddAction(delegate
			{
				isSortMissionDirty = false;
				internalSortMissionIDs();
			});
		}
	}

	private void internalSortMissionIDs()
	{
		SortedMissionUI.Clear();
		int i;
		for (i = 0; Enum.GetValues(typeof(MenuMissionType)).Length > i; i++)
		{
			if (i == 1)
			{
				SortedMissionUI.Add(DailyRandomLimits);
			}
			else
			{
				SortedMissionUI.Add((from s in DataContainer.Instance.MissionTableRaw.dataArray.Where(delegate(MissionInfoData s)
					{
						MenuMissionType menuMissionType = (MenuMissionType)i;
						return menuMissionType.ToString().ToLower().Equals(s.Type);
					})
					select s.ID).ToList());
			}
			SortedMissionUI.Last().Sort((string lhs, string rhs) => int.Parse(lhs).CompareTo(int.Parse(rhs)));
		}
	}

	public void AccMissionByCondTypeID(string goalCondType, string goalCondTypeID, string value)
	{
		List<string> list = DataContainer.Instance.MissionGoalConditionTypeIDs[goalCondType];
		MissionInfoData missionInfoData = DataContainer.Instance.MissionTableRaw[list[0]];
		string value2 = missionInfoData.GoalConditions["typeid"];
		bool isHasTypeID = !"-1".Equals(value2);
		bool isNumberic = "number".Equals(missionInfoData.Goaltype);
		list.All(delegate(string s)
		{
			if (isHasTypeID)
			{
				if (DataContainer.Instance.MissionTableRaw[s].GoalConditions["typeid"].Equals(goalCondTypeID))
				{
					if (isNumberic)
					{
						MsnGoalValues[s] = (int.Parse(MsnGoalValues[s]) + int.Parse(value)).ToString();
					}
					else
					{
						MsnGoalValues[s] = (float.Parse(MsnGoalValues[s]) + float.Parse(value)).ToString();
					}
				}
			}
			else if (isNumberic)
			{
				MsnGoalValues[s] = (int.Parse(MsnGoalValues[s]) + int.Parse(value)).ToString();
			}
			else
			{
				MsnGoalValues[s] = (float.Parse(MsnGoalValues[s]) + float.Parse(value)).ToString();
			}
			return true;
		});
	}

	public void CheckMissionDailyTick()
	{
		if (0 >= userSaveData.Mission.DailyCheckTick)
		{
			userSaveData.Mission.DailyCheckTick = DateTime.Today.Ticks;
			missionRandomDailyLimit();
			return;
		}
		if (DateTime.Today.Ticks > userSaveData.Mission.DailyCheckTick)
		{
			userSaveData.Mission.DailyCheckTick = DateTime.Today.Ticks;
			missionRandomDailyLimit();
			(from s in DataContainer.Instance.MissionTableRaw.dataArray
				where s.Type == "daily"
				select s).All(delegate(MissionInfoData s)
			{
				MsnGoalValues[s.ID] = "0";
				MsnCompleted[s.ID] = false;
				MsnRewarded[s.ID] = false;
				return true;
			});
		}
		SortMissionIDs();
	}

	private void missionRandomDailyLimit()
	{
		System.Random rand = new System.Random((int)DateTime.Now.Ticks);
		List<string> list = (from s in DataContainer.Instance.MissionTableRaw.dataArray
			where s.Type == "daily"
			select s.ID).ToList();
		list.Sort((string lhs, string rhs) => rand.Next(-1, 1));
		DailyRandomLimits = list.GetRange(0, 10);
		DirtyAll();
	}

	private void initMission()
	{
		CheckMissionDailyTick();
		MsnGoalValues.OnValue += delegate(string key, string oldValue, string value)
		{
			if (!MsnCompleted[key] && !MsnRewarded[key])
			{
				MissionInfoData missionInfoData = DataContainer.Instance.MissionTableRaw[key];
				bool flag = "number".Equals(missionInfoData.Goaltype);
				bool flag2 = false;
				if ((!flag) ? (float.Parse(missionInfoData.Goalvalue) <= float.Parse(value)) : (int.Parse(missionInfoData.Goalvalue) <= int.Parse(value)))
				{
					MsnCompleted[key] = true;
				}
			}
		};
		MsnCompleted.OnValue += delegate(string key, bool oldValue, bool value)
		{
			if (value)
			{
				MsnGoalValues[key] = "0";
			}
			SortMissionIDs();
		};
		MsnRewarded.OnValue += delegate(string key, bool oldValue, bool value)
		{
			if (value)
			{
				MsnCompleted[key] = false;
			}
			SortMissionIDs();
			DirtyAll();
		};
		MsnCollectableGolals.OnValue += delegate(string key, int oldValue, int value)
		{
			PlayerInfo playerInfo = this;
			string[] array = key.Split('_');
			string text = array[0];
			string text2 = array[1];
			if (text != null && text == "chcoins")
			{
				bool isCollectAll = true;
				Enumerable.Range(1, DataContainer.Instance.CharacterIDTierByCID.Count).All(delegate(int s)
				{
					string text3 = $"chcoins_{s}";
					if (key.Equals(text3))
					{
						isCollectAll = ((byte)((isCollectAll ? 1 : 0) & 1) != 0);
					}
					else
					{
						isCollectAll &= (1 == playerInfo.MsnCollectableGolals[text3]);
					}
					return true;
				});
				if (isCollectAll)
				{
					string iD = (from s in DataContainer.Instance.MissionTableRaw.dataArray
						where s.Goaltype.Equals("chcoins")
						select s).First().ID;
					MsnCompleted[iD] = true;
				}
			}
		};
		Enumerable.Range(1, DataContainer.Instance.CharacterIDTierByCID.Count).All(delegate(int s)
		{
			MsnCollectableGolals[$"chcoins_{s}"] = MsnCollectableGolals[$"chcoins_{s}"];
			return true;
		});
		SortMissionIDs();
	}

	public void RegistEvents()
	{
		OnEvtSelectedCharID += delegate(string charID)
		{
			SelCharSkillAttribute = DataContainer.Instance.CharacterTableRaw[charID].SkillAttribute;
			selectedCharAnimPrefix = (from s in DataContainer.Instance.CharacterTableRaw.dataArray
				where s.ID == charID
				select s).First().Modelname;
			selectedCharAnimPrefix = Regex.Replace(selectedCharAnimPrefix, "[0-9]+$", string.Empty);
			selectedCharAnimPrefix = $"{selectedCharAnimPrefix}01_";
		};
		CharOwnedTokens.OnValue += delegate(string tokenID, int oldValue, int value)
		{
			string key2 = DataContainer.Instance.TokenRelativeCharacterID[tokenID];
			if (!CharUnlocks[key2])
			{
			}
		};
		CharUnlocks.OnNotKey += delegate(string key)
		{
			CharUnlocks.Add(key, value: false);
		};
		CharOwnedTokens.OnNotKey += delegate(string key)
		{
			CharOwnedTokens.Add(key, 0);
		};
		CharParamLevels.OnNotKey += delegate(string key)
		{
			userSaveData.Character.ParamLevels[key] = (from i in Enumerable.Range(0, Enum.GetValues(typeof(PlayerParameterType)).Length)
				select 0).ToArray();
		};
		MsnGoalValues.OnNotKey += delegate(string key)
		{
			userSaveData.Mission.GoalValues.Add(key, 0.ToString());
		};
		MsnCompleted.OnNotKey += delegate(string key)
		{
			userSaveData.Mission.Completed.Add(key, value: false);
		};
		MsnRewarded.OnNotKey += delegate(string key)
		{
			userSaveData.Mission.Rewarded.Add(key, value: false);
		};
		MsnCollectableGolals.OnNotKey += delegate(string key)
		{
			userSaveData.Mission.CollectableGolals.Add(key, 0);
		};
		TimeRelativeAttribute.OnNotKey += delegate(string key)
		{
			userSaveData.Player.TimeRelativeAttribute[key] = DateTime.Now.Ticks.ToString();
		};
		Currency.OnValue += delegate(CurrencyType cType, int oldValue, int newValue)
		{
			if (cType == CurrencyType.Gold && oldValue < newValue)
			{
				AccMissionByCondTypeID("getgold", "-1", (newValue - oldValue).ToString());
			}
			DirtyAll();
		};
	}

	public void UseUpgrade(PowerupType type)
	{
	}

	public void TriggerOnScoreMultiplierChanged()
	{
	}

	public void PickedupLetter(char letter)
	{
	}

	public void AddSaveMeTokenToUnlock()
	{
	}

	public void InitDailyWord(string word, double sec)
	{
	}

	public void IncreaseUpgradeAmount(PowerupType type, int amount)
	{
	}

	public char GetNewDailyLetter()
	{
		return '\0';
	}

	public int GetUpgradeAmount(PowerupType type)
	{
		return 0;
	}

	public float GetPowerupDuration(PowerupType type)
	{
		return 0f;
	}

	public float GetHoverBoardCoolDown()
	{
		return 0f;
	}

	public string GetDailyWordDaysInARow(out bool dummy)
	{
		dummy = true;
		return string.Empty;
	}

	public int GetTotalProbCHParamLvl(string chKey)
	{
		int length = Enum.GetValues(typeof(PlayerParameterType)).Length;
		int num = 0;
		for (int i = 0; i < length; i++)
		{
			PlayerParameterType playerParameterType = (PlayerParameterType)i;
			num += 105 - 2 * userSaveData.Character.ParamLevels[chKey][(int)playerParameterType];
		}
		return num;
	}

	public int DiceCHParamLvl(string chKey, float randValue)
	{
		float num = 0f;
		int length = Enum.GetValues(typeof(PlayerParameterType)).Length;
		int totalProbCHParamLvl = GetTotalProbCHParamLvl(chKey);
		for (int i = 0; i < length; i++)
		{
			PlayerParameterType playerParameterType = (PlayerParameterType)i;
			if (randValue < (num += (float)(105 - 2 * userSaveData.Character.ParamLevels[chKey][(int)playerParameterType]) / (float)totalProbCHParamLvl))
			{
				return i;
			}
		}
		return -1;
	}

	public void DirtyAll()
	{
		if (!dirtyFrame)
		{
			dirtyFrame = true;
			LateUpdater.Instance.AddAction(delegate
			{
				Save();
				GPGSManager.Instance.SaveToCloud();
				dirtyFrame = false;
			});
		}
	}

	private void InitNew()
	{
		userSaveData = new UserSaveData();
		userSaveData.Player = new UserPlayerData();
		userSaveData.Character = new UserCharacterData();
		userSaveData.Mission = new UserMissionData();
		userSaveData.Preference = new UserPreferenceData();
		userSaveData.VersionIdentifier = 10;
		try
		{
			userSaveData.Preference.LocaleIndex = DataContainer.SysLangToLocale[Application.systemLanguage];
		}
		catch (Exception)
		{
			userSaveData.Preference.LocaleIndex = 0;
		}
		userSaveData.Preference.MusicOn = true;
		userSaveData.Preference.SoundOn = true;
		userSaveData.Player.TempTotalPlayTimes = 0;
		userSaveData.Player.Currency = new CurrencyTypeMapInt(Enumerable.Range(0, Enum.GetValues(typeof(CurrencyType)).Length).ToDictionary((int i) => (CurrencyType)i, (int i) => (i == 0) ? 50 : 0));
		userSaveData.Player.NameTagCount = 30;
		userSaveData.Player.SelectedCharID = DataContainer.Instance.CharacterTableRaw.dataArray[0].ID;
		userSaveData.Player.TimeRelativeAttribute = new AttributeMapString();
		userSaveData.Player.StoredStartItemsWithGameTypes = new StoredStartItemTypeMapBool(Enumerable.Range(0, Enum.GetValues(typeof(StartItemType)).Length).ToDictionary((int i) => i, (int i) => (from s in Enumerable.Range(0, Enum.GetValues(typeof(GameType)).Length)
			select false).ToArray()));
		userSaveData.Player.PlayedTimeSpanTotalMilliseconds = 0.0;
		userSaveData.Player.GPGSID = null;
		userSaveData.Player.MultiraceWin = 0;
		userSaveData.Player.MultiraceLose = 0;
		userSaveData.Character.ParamLevels = new PlayerParameterTypeMapIntArr((from s in DataContainer.Instance.CharacterTableRaw.dataArray
			select s.ID).AsEnumerable().ToDictionary((string s) => s, (string s) => (from i in Enumerable.Range(0, Enum.GetValues(typeof(PlayerParameterType)).Length)
			select 0).ToArray()));
		userSaveData.Character.Unlocks = new AttributeMapBool(Enumerable.Range(0, DataContainer.Instance.CharacterTableRaw.dataArray.Length).ToDictionary((int i) => DataContainer.Instance.CharacterTableRaw.dataArray[i].ID, (int i) => false));
		userSaveData.Character.Unlocks[DataContainer.Instance.CharacterTableRaw.dataArray[0].ID] = true;
		userSaveData.Character.UnlockInfos = new AttributeMapString(Enumerable.Range(0, DataContainer.Instance.CharacterTableRaw.dataArray.Length).ToDictionary((int i) => DataContainer.Instance.CharacterTableRaw.dataArray[i].ID, (int i) => string.Empty));
		userSaveData.Character.Business = new AttributeMapString(Enumerable.Range(0, DataContainer.Instance.CharacterTableRaw.dataArray.Length).ToDictionary((int i) => DataContainer.Instance.CharacterTableRaw.dataArray[i].ID, (int i) => string.Empty));
		userSaveData.Character.OwnedTokens = new AttributeMapInt(Enumerable.Range(0, DataContainer.Instance.TokenTableRaw.dataArray.Length).ToDictionary((int i) => DataContainer.Instance.TokenTableRaw.dataArray[i].ID, (int i) => 0));
		userSaveData.Mission.GoalValues = new AttributeMapString(Enumerable.Range(0, DataContainer.Instance.MissionTableRaw.dataArray.Length).ToDictionary((int i) => DataContainer.Instance.MissionTableRaw.dataArray[i].ID, (int i) => 0.ToString()));
		userSaveData.Mission.Completed = new AttributeMapBool(Enumerable.Range(0, DataContainer.Instance.MissionTableRaw.dataArray.Length).ToDictionary((int i) => DataContainer.Instance.MissionTableRaw.dataArray[i].ID, (int i) => false));
		userSaveData.Mission.Rewarded = new AttributeMapBool(Enumerable.Range(0, DataContainer.Instance.MissionTableRaw.dataArray.Length).ToDictionary((int i) => DataContainer.Instance.MissionTableRaw.dataArray[i].ID, (int i) => false));
		userSaveData.Mission.DailyCheckTick = 0L;
		userSaveData.Mission.DailyRandomLimits = new List<string>();
		userSaveData.Mission.CollectableGolals = new AttributeMapInt();
		userSaveData.Player.TutorialCompleted = false;
		userSaveData.Player.WelcomeUpdateReward = false;
		userSaveData.Player.CheckAgreement = false;
		Save();
	}

	public UserSaveData DecryptData(byte[] data)
	{
		UserSaveData result = null;
		if (data != null)
		{
			try
			{
				byte[] array = FileUtilL.DecryptData(data, "wlrmadms1dnflrk2gpdjwudi3gkf4tlrks5ekdmadp6Eh7aksskdy");
				if (array != null)
				{
					byte[] array2 = new byte[array.Length - 5];
					Array.Copy(array, 5, array2, 0, array2.Length);
					using (MemoryStream ms = new MemoryStream(array2))
					{
						return DeserializeObject<UserSaveData>(ms);
					}
				}
				return result;
			}
			catch (Exception)
			{
				return null;
			}
		}
		return result;
	}

	public void LoadExceptionHandler(Exception e)
	{
		if (e is FileNotFoundException)
		{
			IsFirstLaunch = true;
			InitNew();
		}
		if (e is NullReferenceException)
		{
			InitNew();
		}
		else if (e is EntryPointNotFoundException)
		{
			InitNew();
		}
		else if (e is NotSupportedException)
		{
			InitNew();
		}
		else
		{
			InitNew();
		}
	}

	public void Load()
	{
		try
		{
			GetLoadPaths(out string path, out string externalPath);
			byte[] array = FileUtilL.Load(path, "wlrmadms1dnflrk2gpdjwudi3gkf4tlrks5ekdmadp6Eh7aksskdy", externalPath, useLastModifiedPathIfNoSlots: true);
			if (array == null)
			{
				IsFirstLaunch = true;
				throw new NullReferenceException();
			}
			if (array[0] != 82)
			{
				throw new EntryPointNotFoundException();
			}
			uint num = 0u;
			try
			{
				num = BitConverter.ToUInt32(array, 1);
			}
			catch
			{
				num = 0u;
			}
			if (num < 10)
			{
				NotSupportedException ex = new NotSupportedException();
				ex.Data.Add("verison", num);
				ex.Data.Add("savedata", array);
				ex.Data.Add("isCloud", false);
				throw new NotSupportedException();
			}
			byte[] array2 = new byte[array.Length - 5];
			Array.Copy(array, 5, array2, 0, array2.Length);
			MemoryStream memoryStream = new MemoryStream(array2);
			userSaveData = DeserializeObject<UserSaveData>(memoryStream);
			memoryStream.Close();
		}
		catch (Exception e)
		{
			LoadExceptionHandler(e);
		}
	}

	private byte[] Save(bool needReturn = false)
	{
		byte[] array = null;
		try
		{
			if (GPGSManager.Instance.Authenticated)
			{
				LastGPGSID = Social.localUser.id;
			}
			PlayedTimeSpanTotalMilliseconds += PlayedTimeSpanSinceLoad.TotalMilliseconds;
			LoadedDateTime = DateTime.Now;
			MemoryStream memoryStream = SerializeObject(userSaveData);
			List<byte> list = new List<byte>();
			list.Add(82);
			list.AddRange(BitConverter.GetBytes(10u));
			byte[] array2 = new byte[memoryStream.Length];
			Array.Copy(memoryStream.GetBuffer(), 0L, array2, 0L, memoryStream.Length);
			list.AddRange(array2);
			array = FileUtilL.Save(GetSavePath(), "wlrmadms1dnflrk2gpdjwudi3gkf4tlrks5ekdmadp6Eh7aksskdy", list.ToArray(), 0, list.Count, 5, 5, null, 0, 0, needReturn);
			memoryStream.Close();
			LastSavedData = array;
			return array;
		}
		catch (Exception)
		{
			return array;
		}
	}

	public void SaveFromData(byte[] data)
	{
		try
		{
			FileUtilL.SaveReadyHash(GetSavePath(), "wlrmadms1dnflrk2gpdjwudi3gkf4tlrks5ekdmadp6Eh7aksskdy", data, 0, data.Length, 5, 5);
		}
		catch (Exception)
		{
		}
	}

	public int LevelConv(string level)
	{
		return int.Parse(level);
	}

	public string LevelConv(int level)
	{
		return level.ToString();
	}

	public MemoryStream SerializeObject(object theObj)
	{
		MemoryStream memoryStream = new MemoryStream();
		Serializer.Serialize(memoryStream, theObj);
		return memoryStream;
	}

	public byte[] SerializeObject()
	{
		byte[] array = null;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			Serializer.Serialize(memoryStream, userSaveData);
			array = (memoryStream.GetBuffer().Clone() as byte[]);
			memoryStream.Close();
			return array;
		}
	}

	public T DeserializeObject<T>(MemoryStream ms)
	{
		ms.Seek(0L, SeekOrigin.Begin);
		return (T)Serializer.Deserialize(ms, null, typeof(T));
	}

	private static void GetLoadPaths(out string path, out string externalPath)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		JavaObjWrapper javaObjWrapper = new JavaObjWrapper(@static.GetRawObject());
		JavaObjWrapper javaObjWrapper2 = new JavaObjWrapper(javaObjWrapper.InvokeCall<IntPtr>("getApplicationContext", "()Landroid/content/Context;", new object[0]));
		JavaObjWrapper javaObjWrapper3 = new JavaObjWrapper(javaObjWrapper2.InvokeCall<IntPtr>("getFilesDir", "()Ljava/io/File;", new object[0]));
		string str = javaObjWrapper3.InvokeCall<string>("getAbsolutePath", "()Ljava/lang/String;", new object[0]);
		path = "/playerdata";
		externalPath = str + "/playerdata";
	}

	private static string GetSavePath()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		JavaObjWrapper javaObjWrapper = new JavaObjWrapper(@static.GetRawObject());
		JavaObjWrapper javaObjWrapper2 = new JavaObjWrapper(javaObjWrapper.InvokeCall<IntPtr>("getApplicationContext", "()Landroid/content/Context;", new object[0]));
		JavaObjWrapper javaObjWrapper3 = new JavaObjWrapper(javaObjWrapper2.InvokeCall<IntPtr>("getFilesDir", "()Ljava/io/File;", new object[0]));
		string str = javaObjWrapper3.InvokeCall<string>("getAbsolutePath", "()Ljava/lang/String;", new object[0]);
		return str + "/playerdata";
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
