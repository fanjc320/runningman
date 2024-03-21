using System.Collections.Generic;

public class Trophies
{
	public enum TrophyObtainSource
	{
		Mysterybox,
		Mission
	}

	public enum Trophy
	{
		diamond,
		goldbar,
		goldChainClock,
		goldChainDollar,
		goldSkull,
		headphones,
		lpBlack,
		tapeBlack
	}

	public const int NUMBER_OF_TROPHIES_FOR_FIRST_ACHIEVEMENT = 8;

	public static readonly Dictionary<Trophy, TrophyData> trophyData = new Dictionary<Trophy, TrophyData>
	{
		{
			Trophy.diamond,
			new TrophyData
			{
				name = StringID.TROPHY_GEMSTONE,
				description = StringID.TROPHY_GEMSTONE_DESCRIPTION,
				spriteUnlocked = "trophy_diamond",
				obtainSource = TrophyObtainSource.Mysterybox
			}
		},
		{
			Trophy.goldbar,
			new TrophyData
			{
				name = StringID.TROPHY_GOLD_BAR,
				description = StringID.TROPHY_GOLD_BAR_DESCRIPTION,
				spriteUnlocked = "trophy_gold",
				obtainSource = TrophyObtainSource.Mysterybox
			}
		},
		{
			Trophy.goldChainClock,
			new TrophyData
			{
				name = StringID.TROPHY_CLASSY_CLOCK,
				description = StringID.TROPHY_CLASSY_CLOCK_DESCRIPTION,
				spriteUnlocked = "trophy_goldChainClock",
				obtainSource = TrophyObtainSource.Mysterybox
			}
		},
		{
			Trophy.goldChainDollar,
			new TrophyData
			{
				name = StringID.TROPHY_DOLLAR_CHAIN,
				description = StringID.TROPHY_DOLLAR_CHAIN_DESCRIPTION,
				spriteUnlocked = "trophy_goldChainDollar",
				obtainSource = TrophyObtainSource.Mysterybox
			}
		},
		{
			Trophy.goldSkull,
			new TrophyData
			{
				name = StringID.TROPHY_PAPERWEIGHT,
				description = StringID.TROPHY_PAPERWEIGHT_DESCRIPTION,
				spriteUnlocked = "trophy_goldSkull",
				obtainSource = TrophyObtainSource.Mysterybox
			}
		},
		{
			Trophy.headphones,
			new TrophyData
			{
				name = StringID.TROPHY_HEADPHONES,
				description = StringID.TROPHY_HEADPHONES_DESCRIPTION,
				spriteUnlocked = "trophy_headphones",
				obtainSource = TrophyObtainSource.Mysterybox
			}
		},
		{
			Trophy.lpBlack,
			new TrophyData
			{
				name = StringID.TROPHY_PLATINUM_RECORD,
				description = StringID.TROPHY_PLATINUM_RECORD_DESCRIPTION,
				spriteUnlocked = "trophy_lpBlack",
				obtainSource = TrophyObtainSource.Mysterybox
			}
		},
		{
			Trophy.tapeBlack,
			new TrophyData
			{
				name = StringID.TROPHY_CASSETTE,
				description = StringID.TROPHY_CASSETTE_DESCRIPTION,
				spriteUnlocked = "trophy_tapeBlack",
				obtainSource = TrophyObtainSource.Mysterybox
			}
		}
	};
}
