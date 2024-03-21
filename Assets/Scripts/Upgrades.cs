using System.Collections.Generic;

public class Upgrades
{
	public const int SPAWNRATE_FOR_LETTERS = 1500000000;

	public const float UPGRADE_FIRST_SPAWN_METERS = 250f;

	public const float UPGRADE_SPAWN_SPACING_METERS = 300f;

	public const int SCORE_BOOSTER_MULTIPLIER = 5;

	public static readonly Dictionary<PowerupType, Upgrade> upgrades = new Dictionary<PowerupType, Upgrade>
	{
		{
			PowerupType.hoverboard,
			new Upgrade
			{
				name = StringID.UPGRADES_HOVERBOARD,
				description = StringID.UPGRADES_HOVERBOARD_DESCRIPTION,
				mysteryBoxDescription = StringID.UPGRADES_HOVERBOARD_DESCRIPTION_MYSTERY,
				durations = new float[1]
				{
					30f
				},
				pricesRaw = new int[1]
				{
					300
				},
				iconName = "magnet"
			}
		},
		{
			PowerupType.headstart500,
			new Upgrade
			{
				name = StringID.UPGRADES_HEADSTART,
				durations = new float[1]
				{
					250f
				},
				description = StringID.UPGRADES_HEADSTART_DESCRIPTION,
				mysteryBoxDescription = StringID.UPGRADES_HEADSTART_DESCRIPTION_MYSTERY,
				pricesRaw = new int[1]
				{
					400
				},
				iconName = "1"
			}
		},
		{
			PowerupType.scorebooster,
			new Upgrade
			{
				name = StringID.UPGRADES_SCOREBOOSTER,
				description = StringID.UPGRADES_SCOREBOOSTER_DESCRIPTION,
				mysteryBoxDescription = StringID.UPGRADES_SCOREBOOSTER_DESCRIPTION_MYSTERY,
				pricesRaw = new int[1]
				{
					3000
				},
				iconName = "double up"
			}
		},
		{
			PowerupType.headstart2000,
			new Upgrade
			{
				name = StringID.UPGRADES_MEGAHEADSTART,
				nameTwoLines = StringID.UPGRADES_MEGAHEADSTART_TWO_LINES,
				mysteryBoxDescription = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION_MYSTERY,
				durations = new float[1]
				{
					1000f
				},
				description = StringID.UPGRADES_MEGAHEADSTART_DESCRIPTION,
				pricesRaw = new int[1]
				{
					2000
				},
				iconName = "1"
			}
		},
		{
			PowerupType.mysterybox,
			new Upgrade
			{
				name = StringID.UPGRADES_MYSTERY_BOX,
				description = StringID.EMPTY_STRING,
				spawnProbability = 12,
				minimumMeters = 0,
				pricesRaw = new int[1]
				{
					500
				},
				iconName = "double up"
			}
		},
		{
			PowerupType.jetpack,
			new Upgrade
			{
				name = StringID.UPGRADES_JETPACK,
				description = StringID.UPGRADES_JETPACK_DESCRIPTION,
				numberOfTiers = 7,
				durations = new float[7]
				{
					8f,
					9f,
					10.5f,
					12.5f,
					15f,
					19f,
					25f
				},
				spawnProbability = 14,
				minimumMeters = 1000,
				pricesRaw = new int[7]
				{
					0,
					500,
					1500,
					3000,
					10000,
					30000,
					60000
				},
				iconName = "1"
			}
		},
		{
			PowerupType.supersneakers,
			new Upgrade
			{
				name = StringID.UPGRADES_SUPERSNEAKERS,
				description = StringID.UPGRADES_SUPERSNEAKERS_DESCRIPTION,
				numberOfTiers = 7,
				spawnProbability = 26,
				durations = new float[7]
				{
					10f,
					11.5f,
					13.4f,
					15.8f,
					19f,
					24f,
					30f
				},
				pricesRaw = new int[7]
				{
					0,
					500,
					1500,
					3000,
					10000,
					30000,
					60000
				},
				iconName = "double up"
			}
		},
		{
			PowerupType.coinmagnet,
			new Upgrade
			{
				name = StringID.UPGRADES_COIN_MAGNET,
				description = StringID.UPGRADES_COIN_MAGNET_DESCRIPTION,
				numberOfTiers = 7,
				durations = new float[7]
				{
					10f,
					11.5f,
					13.4f,
					15.8f,
					19f,
					24f,
					30f
				},
				spawnProbability = 29,
				coinmagnetRange = 2,
				pricesRaw = new int[7]
				{
					0,
					500,
					1500,
					3000,
					10000,
					30000,
					60000
				},
				iconName = "magnet"
			}
		},
		{
			PowerupType.doubleMultiplier,
			new Upgrade
			{
				name = StringID.UPGRADES_DOUBLE_MULTIPLIER,
				description = StringID.UPGRADES_DOUBLE_MULTIPLIER_DESCRIPTION,
				numberOfTiers = 7,
				durations = new float[7]
				{
					10f,
					11.5f,
					13.4f,
					15.8f,
					19f,
					24f,
					30f
				},
				spawnProbability = 29,
				pricesRaw = new int[7]
				{
					0,
					500,
					1500,
					3000,
					10000,
					30000,
					60000
				},
				iconName = "double up"
			}
		},
		{
			PowerupType.saveMeToken,
			new Upgrade
			{
				name = StringID.UPGRADES_KEY,
				description = StringID.EMPTY_STRING,
				spawnProbability = 2,
				minimumMeters = 1000,
				pricesRaw = new int[1],
				iconName = "double up"
			}
		},
		{
			PowerupType.skipmission1,
			new Upgrade
			{
				name = StringID.UPGRADES_SKIP_MISSION_1,
				description = StringID.UPGRADES_SKIP_MISSION_1_DESCRIPTION,
				pricesRaw = new int[1]
				{
					1500
				},
				iconName = "double up",
				levelPriceMultiplyer = 100
			}
		},
		{
			PowerupType.skipmission2,
			new Upgrade
			{
				name = StringID.UPGRADES_SKIP_MISSION_2,
				description = StringID.UPGRADES_SKIP_MISSION_2_DESCRIPTION,
				pricesRaw = new int[1]
				{
					1500
				},
				iconName = "double up",
				levelPriceMultiplyer = 100
			}
		},
		{
			PowerupType.skipmission3,
			new Upgrade
			{
				name = StringID.UPGRADES_SKIP_MISSION_3,
				description = StringID.UPGRADES_SKIP_MISSION_3_DESCRIPTION,
				pricesRaw = new int[1]
				{
					1500
				},
				iconName = "double up",
				levelPriceMultiplyer = 100
			}
		},
		{
			PowerupType.letters,
			new Upgrade
			{
				minimumMeters = 2000,
				spawnProbability = 1500000000
			}
		},
		{
			PowerupType.confuse,
			new Upgrade
			{
				name = StringID.UPGRADES_SUPERSNEAKERS,
				description = StringID.UPGRADES_SUPERSNEAKERS_DESCRIPTION,
				numberOfTiers = 7,
				spawnProbability = 26,
				durations = new float[7]
				{
					5f,
					11.5f,
					13.4f,
					15.8f,
					19f,
					24f,
					30f
				},
				pricesRaw = new int[7]
				{
					0,
					500,
					1500,
					3000,
					10000,
					30000,
					60000
				},
				iconName = "double up"
			}
		},
		{
			PowerupType.doubleCoin,
			new Upgrade
			{
				name = StringID.UPGRADES_SUPERSNEAKERS,
				description = StringID.UPGRADES_SUPERSNEAKERS_DESCRIPTION,
				numberOfTiers = 7,
				spawnProbability = 26,
				durations = new float[7]
				{
					5f,
					11.5f,
					13.4f,
					15.8f,
					19f,
					24f,
					30f
				},
				pricesRaw = new int[7]
				{
					0,
					500,
					1500,
					3000,
					10000,
					30000,
					60000
				},
				iconName = "double up"
			}
		}
	};
}
