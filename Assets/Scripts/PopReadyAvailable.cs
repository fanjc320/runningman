using System.Collections.Generic;

public static class PopReadyAvailable
{
	public static readonly Dictionary<GameType, HashSet<StartItemType>> AttributeMap = new Dictionary<GameType, HashSet<StartItemType>>
	{
		{
			GameType.NormalSingle,
			new HashSet<StartItemType>
			{
				StartItemType.StartFever,
				StartItemType.LastFever,
				StartItemType.Helmet,
				StartItemType.DoubleJump,
				StartItemType.IgnoreConfuse,
				StartItemType.RandomRevive,
				StartItemType.GoldBonus50P
			}
		},
		{
			GameType.MissionSingle,
			new HashSet<StartItemType>
			{
				StartItemType.StartFever,
				StartItemType.LastFever,
				StartItemType.Helmet,
				StartItemType.DoubleJump,
				StartItemType.IgnoreConfuse,
				StartItemType.ProtectTagSticker,
				StartItemType.RandomRevive,
				StartItemType.GoldBonus50P
			}
		},
		{
			GameType.Multi,
			new HashSet<StartItemType>
			{
				StartItemType.StartFever,
				StartItemType.Helmet,
				StartItemType.DoubleJump,
				StartItemType.IgnoreConfuse,
				StartItemType.RandomRevive
			}
		}
	};
}
