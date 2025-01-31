using UnityEngine;

namespace Soomla
{
	public class RewardGivenEvent : SoomlaEvent
	{
		public readonly Reward Reward;

		public RewardGivenEvent(string rewardId)
			: this(rewardId, null)
		{
		}

		public RewardGivenEvent(Reward reward)
			: this(reward, null)
		{
		}

		public RewardGivenEvent(string rewardId, Object sender)
			: base(sender)
		{
			Reward = Reward.GetReward(rewardId);
		}

		public RewardGivenEvent(Reward reward, Object sender)
			: base(sender)
		{
			Reward = reward;
		}
	}
}
