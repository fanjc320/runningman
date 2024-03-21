using System;
using System.Collections.Generic;

public class Characters
{
	public enum UnlockType
	{
		free,
		tokens,
		coins,
		keys,
		closed
	}

	public enum CharacterType
	{
		suckjin,
		gangsu,
		mcyou,
		garry,
		haha,
		jihyo,
		jongkuk
	}

	public struct Model
	{
		public int modelIndex;

		public StringID name;

		public string modelName;

		public int Price;

		public UnlockType unlockType;

		public string tokenName;

		public string tokenSprite2dName;

		public StringID characterLimitedDescription;

		public bool isNewInThisUpdate;

		public bool hasOnlineSettings;

		public DateTime defaultExpirationDate;

		public DateTime defaultStartDate;

		public StringID skillDescription;

		public bool isReserved;
	}

	public const string ONLINE_SETTINGS_CHARACTER_PREFIX_EXPIRE = "character_expiretime_";

	public const string ONLINE_SETTINGS_CHARACTER_PREFIX_START = "character_starttime_";

	public static readonly Dictionary<CharacterType, Model> characterData = new Dictionary<CharacterType, Model>
	{
		{
			CharacterType.gangsu,
			new Model
			{
				modelIndex = 0,
				name = StringID.CHARACTERS_GANGSU,
				modelName = "gangsu",
				unlockType = UnlockType.keys,
				Price = 49,
				isNewInThisUpdate = false,
				hasOnlineSettings = false,
				skillDescription = StringID.CHARACTERS_GANGSU_SKILL,
				isReserved = false
			}
		},
		{
			CharacterType.garry,
			new Model
			{
				modelIndex = 1,
				name = StringID.CHARACTERS_GARRY,
				modelName = "garry",
				unlockType = UnlockType.keys,
				Price = 10,
				isNewInThisUpdate = false,
				hasOnlineSettings = false,
				skillDescription = StringID.CHARACTERS_GARRY_SKILL,
				isReserved = true
			}
		},
		{
			CharacterType.haha,
			new Model
			{
				modelIndex = 2,
				name = StringID.CHARACTERS_HAHA,
				modelName = "haha",
				unlockType = UnlockType.keys,
				Price = 20,
				isNewInThisUpdate = false,
				hasOnlineSettings = false,
				skillDescription = StringID.CHARACTERS_HAHA_SKILL,
				isReserved = true
			}
		},
		{
			CharacterType.jihyo,
			new Model
			{
				modelIndex = 3,
				name = StringID.CHARACTERS_JIHYO,
				modelName = "jihyo",
				unlockType = UnlockType.keys,
				Price = 30,
				isNewInThisUpdate = false,
				hasOnlineSettings = false,
				skillDescription = StringID.CHARACTERS_JIHYO_SKILL,
				isReserved = true
			}
		},
		{
			CharacterType.jongkuk,
			new Model
			{
				modelIndex = 4,
				name = StringID.CHARACTERS_JONGKUK,
				modelName = "jongkuk",
				unlockType = UnlockType.keys,
				Price = 40,
				isNewInThisUpdate = false,
				hasOnlineSettings = false,
				skillDescription = StringID.CHARACTERS_JONGKUK_SKILL,
				isReserved = true
			}
		},
		{
			CharacterType.mcyou,
			new Model
			{
				modelIndex = 5,
				name = StringID.CHARACTERS_MCYOU,
				modelName = "McYou",
				unlockType = UnlockType.keys,
				Price = 49,
				isNewInThisUpdate = false,
				hasOnlineSettings = false,
				skillDescription = StringID.CHARACTERS_MCYOU_SKILL,
				isReserved = false
			}
		},
		{
			CharacterType.suckjin,
			new Model
			{
				modelIndex = 6,
				name = StringID.CHARACTERS_SUCKJIN,
				modelName = "suckjin",
				unlockType = UnlockType.free,
				Price = 60,
				isNewInThisUpdate = false,
				hasOnlineSettings = false,
				skillDescription = StringID.CHARACTERS_SUCKJIN_SKILL,
				isReserved = false
			}
		}
	};

	public static List<CharacterType> characterOrder = new List<CharacterType>
	{
		CharacterType.suckjin,
		CharacterType.gangsu,
		CharacterType.mcyou,
		CharacterType.garry,
		CharacterType.haha,
		CharacterType.jihyo,
		CharacterType.jongkuk
	};
}
