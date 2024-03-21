using System;
using System.Collections.Generic;

public class Hoverboards
{
	public enum BoardType
	{
		normal,
		bouncer,
		lowrider,
		snowboard,
		surfboard,
		theoriginal,
		starboard,
		miami,
		monster,
		skullfire,
		liberty,
		speedboard,
		toucan,
		treehugger,
		chick,
		rome,
		soccer,
		theOutback,
		greatWhiteWakeboard,
		cherry,
		kitty,
		hotrod,
		flamingo,
		rose
	}

	public enum UnlockType
	{
		alwaysUnlocked,
		free,
		coins,
		eggHunt,
		itemHunt
	}

	public struct Board
	{
		public StringID name;

		public string boardModelName;

		public int price;

		public UnlockType unlockType;

		public string tokenSprite2dName;

		public StringID description;

		public StringID limitedDescription;

		public bool isNewInThisUpdate;

		public bool hasOnlineTimeLimit;

		public DateTime defaultExpirationTime;

		public DateTime defaultStartDate;
	}

	public const string ONLINE_SETTINGS_BOARD_PREFIX_EXPIRE = "hoverboard_expiretime_";

	public const string ONLINE_SETTINGS_BOARD_PREFIX_START = "hoverboard_starttime_";

	public static readonly Dictionary<BoardType, Board> boardData = new Dictionary<BoardType, Board>
	{
		{
			BoardType.normal,
			new Board
			{
				name = StringID.HOVERBOARDS_HOVERBOARD,
				boardModelName = "Hoverboard",
				price = 0,
				unlockType = UnlockType.alwaysUnlocked,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.bouncer,
			new Board
			{
				name = StringID.HOVERBOARDS_BOUNCER,
				boardModelName = "Jumpboard",
				price = 280000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_BOUNCER_DESCRIPTION,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.lowrider,
			new Board
			{
				name = StringID.HOVERBOARDS_LOWRIDER,
				boardModelName = "Lowrider",
				price = 320000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_LOWRIDER_DESCRIPTION,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.snowboard,
			new Board
			{
				name = StringID.HOVERBOARDS_FREESTYLER,
				boardModelName = "Snowboard",
				price = 45000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.surfboard,
			new Board
			{
				name = StringID.HOVERBOARDS_BIG_KAHUNA,
				boardModelName = "Surfboard",
				price = 65000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.theoriginal,
			new Board
			{
				name = StringID.HOVERBOARDS_SUPERHERO,
				boardModelName = "Bamboard",
				price = 8000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.starboard,
			new Board
			{
				name = StringID.HOVERBOARDS_STARBOARD,
				boardModelName = "Starboard",
				price = 0,
				unlockType = UnlockType.free,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.miami,
			new Board
			{
				name = StringID.HOVERBOARDS_MIAMI,
				boardModelName = "Miami",
				price = 12000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.monster,
			new Board
			{
				name = StringID.HOVERBOARDS_MONSTER,
				boardModelName = "Monster",
				price = 30000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.skullfire,
			new Board
			{
				name = StringID.HOVERBOARDS_SKULL_FIRE,
				boardModelName = "SkullFire",
				price = 75000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.liberty,
			new Board
			{
				name = StringID.HOVERBOARDS_LIBERTY,
				boardModelName = "Liberty",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.HOVERBOARDS_LIBERTY_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 2, 1),
				defaultStartDate = new DateTime(2012, 12, 20)
			}
		},
		{
			BoardType.speedboard,
			new Board
			{
				name = StringID.HOVERBOARDS_DAREDEVIL,
				boardModelName = "Daredevil",
				price = 85000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_DAREDEVIL_DESCRIPTION,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.toucan,
			new Board
			{
				name = StringID.HOVERBOARDS_TOUCAN,
				boardModelName = "RioBoard",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.HOVERBOARDS_TOUCAN_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 2, 28),
				defaultStartDate = new DateTime(2013, 1, 28)
			}
		},
		{
			BoardType.treehugger,
			new Board
			{
				name = StringID.HOVERBOARDS_LUMBERJACK,
				boardModelName = "Treehugger",
				price = 4000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.chick,
			new Board
			{
				name = StringID.HOVERBOARDS_CHICKY,
				boardModelName = "Chick",
				tokenSprite2dName = "icon_egg",
				price = 100,
				unlockType = UnlockType.eggHunt,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.HOVERBOARDS_CHICKY_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 4, 4),
				defaultStartDate = new DateTime(2013, 2, 7)
			}
		},
		{
			BoardType.rome,
			new Board
			{
				name = StringID.HOVERBOARDS_SCOOT,
				boardModelName = "Rome",
				price = 35000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.soccer,
			new Board
			{
				name = StringID.HOVERBOARDS_KICK_OFF,
				boardModelName = "Soccer",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.HOVERBOARDS_KICK_OFF_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 4, 4),
				defaultStartDate = new DateTime(2013, 2, 7)
			}
		},
		{
			BoardType.theOutback,
			new Board
			{
				name = StringID.HOVERBOARDS_OUTBACK,
				boardModelName = "TheOutback",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.HOVERBOARDS_OUTBACK_LIMITED_DESCRIPTION,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 5, 2),
				defaultStartDate = new DateTime(2013, 3, 20)
			}
		},
		{
			BoardType.greatWhiteWakeboard,
			new Board
			{
				name = StringID.HOVERBOARDS_GREAT_WHITE,
				boardModelName = "GreatWhiteWakeboard",
				price = 20000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = false,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.cherry,
			new Board
			{
				name = StringID.HOVERBOARDS_CHERRY,
				boardModelName = "Cherry",
				tokenSprite2dName = "icon_doll_medium",
				price = 60,
				unlockType = UnlockType.itemHunt,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.HOVERBOARDS_CHERRY_LIMITED_DESCRIPTION,
				isNewInThisUpdate = true,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 5, 30),
				defaultStartDate = new DateTime(2013, 5, 23)
			}
		},
		{
			BoardType.kitty,
			new Board
			{
				name = StringID.HOVERBOARDS_KITTY,
				boardModelName = "Fortune",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.HOVERBOARDS_KITTY_LIMITED_DESCRIPTION,
				isNewInThisUpdate = true,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 5, 30),
				defaultStartDate = new DateTime(2013, 4, 15)
			}
		},
		{
			BoardType.hotrod,
			new Board
			{
				name = StringID.HOVERBOARDS_HOTROD,
				boardModelName = "HotRod",
				price = 280000,
				unlockType = UnlockType.coins,
				description = StringID.HOVERBOARDS_HOTROD_DESCRIPTION,
				limitedDescription = StringID.EMPTY_STRING,
				isNewInThisUpdate = true,
				hasOnlineTimeLimit = false
			}
		},
		{
			BoardType.flamingo,
			new Board
			{
				name = StringID.HOVERBOARDS_FLAMINGO,
				boardModelName = "Flamingo",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.HOVERBOARDS_FLAMINGO_LIMITED_DESCRIPTION,
				isNewInThisUpdate = true,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 6, 27),
				defaultStartDate = new DateTime(2013, 5, 13)
			}
		},
		{
			BoardType.rose,
			new Board
			{
				name = StringID.HOVERBOARDS_ROSE,
				boardModelName = "Rose",
				price = 50000,
				unlockType = UnlockType.coins,
				description = StringID.EMPTY_STRING,
				limitedDescription = StringID.HOVERBOARDS_ROSE_LIMITED_DESCRIPTION,
				isNewInThisUpdate = true,
				hasOnlineTimeLimit = true,
				defaultExpirationTime = new DateTime(2013, 8, 1),
				defaultStartDate = new DateTime(2013, 6, 2)
			}
		}
	};

	public static List<BoardType> boardOrder = new List<BoardType>
	{
		BoardType.snowboard,
		BoardType.bouncer,
		BoardType.chick,
		BoardType.hotrod,
		BoardType.skullfire,
		BoardType.normal,
		BoardType.rose,
		BoardType.flamingo,
		BoardType.liberty,
		BoardType.toucan,
		BoardType.soccer,
		BoardType.cherry,
		BoardType.kitty,
		BoardType.theOutback,
		BoardType.lowrider,
		BoardType.starboard,
		BoardType.speedboard,
		BoardType.greatWhiteWakeboard,
		BoardType.rome,
		BoardType.treehugger,
		BoardType.theoriginal,
		BoardType.surfboard,
		BoardType.miami,
		BoardType.monster
	};
}
