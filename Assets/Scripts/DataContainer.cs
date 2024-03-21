using Lean;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DataContainer : MonoBehaviour
{
	public class PlayerParameterLevelDataArray
	{
		public PlayerParameterLevelData[] PPLevelRaws;
	}

	public static readonly string[] LocaleIdentifier = new string[3]
	{
		"en",
		"ko",
		"zh"
	};

	public static readonly Dictionary<SystemLanguage, int> SysLangToLocale = new Dictionary<SystemLanguage, int>
	{
		{
			SystemLanguage.English,
			0
		},
		{
			SystemLanguage.Korean,
			1
		},
		{
			SystemLanguage.ChineseSimplified,
			2
		}
	};

	private static DataContainer instance = null;

	public CharacterInfo CharacterTableRaw;

	public Dictionary<string, string[]> CharacterIDTierByCID;

	public Dictionary<string, int> CharacterTierByID;

	public Dictionary<string, int[]> CharacterUnlockCounts;

	public PlayerParameterInfo PlayerParamTableRaw;

	public PlayerParameterLevel PlayerParamLevelTableRaw;

	public StartItemInfo StartItemTableRaw;

	public TokenInfo TokenTableRaw;

	public Dictionary<string, string> TokenRelativeCharacterID;

	public MissionInfo MissionTableRaw;

	public Dictionary<string, List<string>> MissionGoalConditionTypeIDs;

	public ShopInfo ShopTableRaw;

	public List<List<string>> ShopIDByMenuCurrencyType;

	public MysteryItemInfo MysteryItemTableRaw;

	public BasicStatus BasicStatusTableRaw;

	public Bonusbox BonusboxTableRaw;

	public LocalizationInfo LocalizationTableRaw;

	public MarketInfo MarketTableRaw;

	public string[] NameByCurrency;

	public PlayerParameterLevelDataArray[] PlayerParamLevelTableRawByLevel;

	private Dictionary<string, Dictionary<Type, UnityEngine.Object>> assetResources = new Dictionary<string, Dictionary<Type, UnityEngine.Object>>();

	public static DataContainer Instance
	{
		get
		{
			if (null == instance)
			{
				_init();
			}
			return instance;
		}
	}

	private static void _init()
	{
		instance = (UnityEngine.Object.Instantiate(Resources.Load("DataContainer", typeof(GameObject))) as GameObject).GetComponent<DataContainer>();
		instance.gameObject.name = instance.GetType().Name;
		instance.CharacterTableRaw.InitMappers();
		instance.PlayerParamTableRaw.InitMappers();
		instance.PlayerParamLevelTableRaw.InitMappers();
		instance.TokenTableRaw.InitMappers();
		instance.MissionTableRaw.InitMappers();
		instance.StartItemTableRaw.InitMappers();
		instance.ShopTableRaw.InitMappers();
		instance.MysteryItemTableRaw.InitMappers();
		instance.BasicStatusTableRaw.InitMappers();
		instance.BonusboxTableRaw.InitMappers();
		instance.LocalizationTableRaw.InitMappers();
		instance.MarketTableRaw.InitMappers();
		instance.initDataProcessing();
		LeanLocalization.OnLocalizationChanged = (Action)Delegate.Combine(LeanLocalization.OnLocalizationChanged, (Action)delegate
		{
			Resources.UnloadUnusedAssets();
			LeanTween.delayedCall(0f, (Action)delegate
			{
				Resources.UnloadUnusedAssets();
			});
		});
	}

	private static void ChangeLocaleImage(Image image, string key)
	{
		image.GetComponent<LLocImage>().SetPhraseName(key);
	}

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public UnityEngine.Object GetAssetResources(string fullpath, Type type)
	{
		UnityEngine.Object result = null;
		if (assetResources.ContainsKey(fullpath))
		{
			Dictionary<Type, UnityEngine.Object> dictionary = assetResources[fullpath];
			if (dictionary.ContainsKey(type))
			{
				result = dictionary[type];
			}
			else
			{
				UnityEngine.Object value = Resources.Load(fullpath, type);
				dictionary.Add(type, value);
			}
		}
		else
		{
			UnityEngine.Object @object = Resources.Load(fullpath, type);
			Dictionary<Type, UnityEngine.Object> dictionary2 = new Dictionary<Type, UnityEngine.Object>();
			dictionary2.Add(type, @object);
			Dictionary<Type, UnityEngine.Object> value2 = dictionary2;
			assetResources.Add(fullpath, value2);
			result = @object;
		}
		return result;
	}

	public T GetAssetResources<T>(string fullpath) where T : UnityEngine.Object
	{
		return (T)GetAssetResources(fullpath, typeof(T));
	}

	private void initDataProcessing()
	{
		int num = 1;
		int num2 = (from s in CharacterTableRaw.dataArray
			select int.Parse(s.CID)).Max();
		CharacterIDTierByCID = new Dictionary<string, string[]>();
		for (int k = num; num2 + 1 > k; k++)
		{
			string cInfoID = k.ToString();
			CharacterIDTierByCID.Add(cInfoID, (from s in CharacterTableRaw.dataArray
				where s.CID == cInfoID
				select s.ID).ToArray());
		}
		CharacterTierByID = new Dictionary<string, int>();
		CharacterTableRaw.dataArray.All(delegate(CharacterInfoData s)
		{
			string[] array = CharacterIDTierByCID[s.CID];
			for (int l = 0; array.Length > l; l++)
			{
				if (array[l].Equals(s.ID))
				{
					CharacterTierByID.Add(s.ID, l);
				}
			}
			return true;
		});
		int unlockKeyCount = Enum.GetNames(typeof(UnlockKeyType)).Length;
		CharacterUnlockCounts = new Dictionary<string, int[]>();
		CharacterUnlockCounts = (from s in CharacterTableRaw.dataArray
			select (s)).ToDictionary((CharacterInfoData key) => key.ID, (CharacterInfoData value) => Enumerable.Range(0, unlockKeyCount).ToArray());
		CharacterUnlockCounts.All(delegate(KeyValuePair<string, int[]> keyPair)
		{
			CharacterTableRaw[keyPair.Key].UnlockAttribute.All(delegate(KeyValuePair<string, string> attrKeyPair)
			{
				UnlockKeyType unlockKeyType = (UnlockKeyType)Enum.Parse(typeof(UnlockKeyType), attrKeyPair.Key.ToUpper());
				keyPair.Value[(int)unlockKeyType] = int.Parse(attrKeyPair.Value);
				return true;
			});
			return true;
		});
		TokenRelativeCharacterID = new Dictionary<string, string>();
		CharacterUnlockCounts.All(delegate(KeyValuePair<string, int[]> keyPair)
		{
			int num3 = keyPair.Value[3];
			if (0 >= num3)
			{
				return true;
			}
			int num4 = keyPair.Value[2];
			TokenRelativeCharacterID[num4.ToString()] = keyPair.Key;
			return true;
		});
		PlayerParamLevelTableRawByLevel = Enumerable.Range(0, PlayerParamTableRaw.dataArray.Length).Select(delegate(int index)
		{
			string infoID = PlayerParamTableRaw.dataArray[index].ID;
			return new PlayerParameterLevelDataArray
			{
				PPLevelRaws = (from levelData in PlayerParamLevelTableRaw.dataArray
					where levelData.Infoid == infoID
					select levelData into data
					orderby int.Parse(data.ID)
					select data).ToArray()
			};
		}).ToArray();
		string[] missionGoalConditionTypeKeys = new string[14]
		{
			"launchgame",
			"buystartitem",
			"playgamemode",
			"getgold",
			"runamount",
			"upgradeparam",
			"dofever",
			"getmystery",
			"doplucksticker",
			"dogiftbox",
			"dorandomlvlup",
			"dojumpobstacle",
			"dorollobstacle",
			"getchcoin"
		};
		List<string>[] missionGoalConditionTypeValues = new List<string>[14]
		{
			(from s in Enumerable.Range(0, 1)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(1, 16)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(17, 12)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(29, 3)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(32, 5)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(37, 12)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(49, 3)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(52, 3)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(55, 3)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(58, 2)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(60, 2)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(62, 5)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(67, 5)
				select s.ToString()).ToList(),
			(from s in Enumerable.Range(72, 1)
				select s.ToString()).ToList()
		};
		MissionGoalConditionTypeIDs = new Dictionary<string, List<string>>(Enumerable.Range(0, missionGoalConditionTypeKeys.Length).ToDictionary((int i) => missionGoalConditionTypeKeys[i], (int i) => missionGoalConditionTypeValues[i]));
		ShopIDByMenuCurrencyType = new List<List<string>>();
		string[] shopTypes = new string[3]
		{
			"gold",
			"jewel",
			"ticket"
		};
		int j;
		for (j = 0; Enum.GetNames(typeof(MenuCurrencyType)).Length > j; j++)
		{
			ShopIDByMenuCurrencyType.Add((from s in Instance.ShopTableRaw.dataArray
				where s.Type == shopTypes[j]
				select s.ID).ToList());
		}
		NameByCurrency = new string[3]
		{
			(from s in Instance.ShopTableRaw.dataArray
				where s.Type == "gold"
				select s).First().Name1loc,
			(from s in Instance.ShopTableRaw.dataArray
				where s.Type == "jewel"
				select s).First().Name1loc,
			(from s in Instance.ShopTableRaw.dataArray
				where s.Type == "ticket"
				select s).First().Name1loc
		};
	}
}
