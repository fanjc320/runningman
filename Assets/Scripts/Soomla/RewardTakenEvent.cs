namespace Soomla
{
	public class RewardTakenEvent : SoomlaEvent
	{
		public readonly Reward Reward;

		public RewardTakenEvent(string rewardId)
			: this(rewardId, null)
		{
		}

		public RewardTakenEvent(Reward reward)
			: this(reward, null)
		{
		}

		public RewardTakenEvent(string rewardId, object sender)
			: base(sender)
		{
			Reward = Reward.GetReward(rewardId);
		}

		public RewardTakenEvent(Reward reward, object sender)
			: base(sender)
		{
			Reward = reward;
		}
	}
}
