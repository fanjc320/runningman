using System.Collections.Generic;

namespace Soomla
{
	public class SequenceReward : Reward
	{
		private static string TAG = "SOOMLA SequenceReward";

		public List<Reward> Rewards;

		public SequenceReward(string id, string name, List<Reward> rewards)
			: base(id, name)
		{
			if (rewards == null || rewards.Count == 0)
			{
				SoomlaUtils.LogError(TAG, "This reward doesn't make sense without items");
			}
			Rewards = rewards;
		}

		public SequenceReward(JSONObject jsonReward)
			: base(jsonReward)
		{
			List<JSONObject> list = jsonReward["rewards"].list;
			if (list == null || list.Count == 0)
			{
				SoomlaUtils.LogWarning(TAG, "Reward has no meaning without children");
				list = new List<JSONObject>();
			}
			Rewards = new List<Reward>();
			foreach (JSONObject item in list)
			{
				Rewards.Add(Reward.fromJSONObject(item));
			}
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jSONObject = base.toJSONObject();
			JSONObject jSONObject2 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (Reward reward in Rewards)
			{
				jSONObject2.Add(reward.toJSONObject());
			}
			jSONObject.AddField("rewards", jSONObject2);
			return jSONObject;
		}

		public Reward GetLastGivenReward()
		{
			int lastSeqIdxGiven = RewardStorage.GetLastSeqIdxGiven(this);
			if (lastSeqIdxGiven < 0)
			{
				return null;
			}
			return Rewards[lastSeqIdxGiven];
		}

		public bool HasMoreToGive()
		{
			return RewardStorage.GetLastSeqIdxGiven(this) < Rewards.Count;
		}

		public bool ForceNextRewardToGive(Reward reward)
		{
			for (int i = 0; i < Rewards.Count; i++)
			{
				if (Rewards[i].ID == reward.ID)
				{
					RewardStorage.SetLastSeqIdxGiven(this, i - 1);
					return true;
				}
			}
			return false;
		}

		protected override bool giveInner()
		{
			int lastSeqIdxGiven = RewardStorage.GetLastSeqIdxGiven(this);
			if (lastSeqIdxGiven >= Rewards.Count)
			{
				return false;
			}
			RewardStorage.SetLastSeqIdxGiven(this, ++lastSeqIdxGiven);
			return true;
		}

		protected override bool takeInner()
		{
			int lastSeqIdxGiven = RewardStorage.GetLastSeqIdxGiven(this);
			if (lastSeqIdxGiven <= 0)
			{
				return false;
			}
			RewardStorage.SetLastSeqIdxGiven(this, --lastSeqIdxGiven);
			return true;
		}
	}
}
