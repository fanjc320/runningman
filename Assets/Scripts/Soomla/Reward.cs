using System;
using System.Collections.Generic;

namespace Soomla
{
	public abstract class Reward : SoomlaEntity<Reward>
	{
		private static string TAG = "SOOMLA Reward";

		public Schedule Schedule;

		private static Dictionary<string, Reward> RewardsMap = new Dictionary<string, Reward>();

		public bool Owned => RewardStorage.IsRewardGiven(this);

		public Reward(string id, string name)
			: base(id, name, string.Empty)
		{
			Schedule = Schedule.AnyTimeOnce();
			RewardsMap.AddOrUpdate(base.ID, this);
		}

		public Reward(JSONObject jsonReward)
			: base(jsonReward)
		{
			JSONObject jSONObject = jsonReward["schedule"];
			if ((bool)jSONObject)
			{
				Schedule = new Schedule(jSONObject);
			}
			else
			{
				Schedule = null;
			}
			RewardsMap.AddOrUpdate(base.ID, this);
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jSONObject = base.toJSONObject();
			if (Schedule != null)
			{
				jSONObject.AddField("schedule", Schedule.toJSONObject());
			}
			else
			{
				jSONObject.AddField("schedule", Schedule.AnyTimeOnce().toJSONObject());
			}
			return jSONObject;
		}

		public static Reward fromJSONObject(JSONObject rewardObj)
		{
			string str = rewardObj["className"].str;
			Reward reward = (Reward)Activator.CreateInstance(Type.GetType("Soomla." + str), new object[1]
			{
				rewardObj
			});
			RewardsMap.AddOrUpdate(reward.ID, reward);
			return reward;
		}

		public bool Take()
		{
			if (!RewardStorage.IsRewardGiven(this))
			{
				SoomlaUtils.LogDebug(TAG, "Reward not given. id: " + _id);
				return false;
			}
			if (takeInner())
			{
				RewardStorage.SetRewardStatus(this, give: false);
				return true;
			}
			return false;
		}

		public bool CanGive()
		{
			return Schedule.Approve(RewardStorage.GetTimesGiven(this));
		}

		public bool Give()
		{
			if (!CanGive())
			{
				SoomlaUtils.LogDebug(TAG, "(Give) Reward is not approved by Schedule. id: " + _id);
				return false;
			}
			if (giveInner())
			{
				RewardStorage.SetRewardStatus(this, give: true);
				return true;
			}
			return false;
		}

		protected abstract bool giveInner();

		protected abstract bool takeInner();

		public static Reward GetReward(string rewardID)
		{
			Reward value = null;
			RewardsMap.TryGetValue(rewardID, out value);
			return value;
		}

		public static List<Reward> GetRewards()
		{
			List<Reward> list = new List<Reward>();
			foreach (Reward value in RewardsMap.Values)
			{
				list.Add(value);
			}
			return list;
		}
	}
}
