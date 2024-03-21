using System;
using System.Collections.Generic;

namespace Soomla
{
	public class RandomReward : Reward
	{
		private static string TAG = "SOOMLA RandomReward";

		public List<Reward> Rewards;

		public Reward LastGivenReward;

		public RandomReward(string id, string name, List<Reward> rewards)
			: base(id, name)
		{
			if (rewards == null || rewards.Count == 0)
			{
				SoomlaUtils.LogError(TAG, "This reward doesn't make sense without items");
			}
			Rewards = rewards;
		}

		public RandomReward(JSONObject jsonReward)
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

		protected override bool giveInner()
		{
			List<Reward> list = new List<Reward>();
			foreach (Reward reward2 in Rewards)
			{
				if (reward2.CanGive())
				{
					list.Add(reward2);
				}
			}
			if (list.Count == 0)
			{
				SoomlaUtils.LogDebug(TAG, "No more rewards to give in this Random Reward: " + base.ID);
				return false;
			}
			Random random = new Random();
			int index = random.Next(list.Count);
			Reward reward = list[index];
			reward.Give();
			LastGivenReward = reward;
			return true;
		}

		protected override bool takeInner()
		{
			if (LastGivenReward == null)
			{
				return false;
			}
			bool result = LastGivenReward.Take();
			LastGivenReward = null;
			return result;
		}
	}
}
