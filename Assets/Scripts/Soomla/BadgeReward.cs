namespace Soomla
{
	public class BadgeReward : Reward
	{
		public string IconUrl;

		public BadgeReward(string id, string name)
			: base(id, name)
		{
		}

		public BadgeReward(string id, string name, string iconUrl)
			: base(id, name)
		{
			IconUrl = iconUrl;
		}

		public BadgeReward(JSONObject jsonReward)
			: base(jsonReward)
		{
			IconUrl = jsonReward["iconUrl"].str;
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jSONObject = base.toJSONObject();
			jSONObject.AddField("iconUrl", IconUrl);
			return jSONObject;
		}

		protected override bool giveInner()
		{
			return true;
		}

		protected override bool takeInner()
		{
			return true;
		}
	}
}
