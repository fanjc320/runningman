using Soomla.Store;

namespace Soomla
{
	public class VirtualItemReward : Reward
	{
		private static string TAG = "SOOMLA VirtualItemReward";

		public string AssociatedItemId;

		public int Amount;

		public VirtualItemReward(string rewardId, string name, string associatedItemId, int amount)
			: base(rewardId, name)
		{
			AssociatedItemId = associatedItemId;
			Amount = amount;
		}

		public VirtualItemReward(JSONObject jsonReward)
			: base(jsonReward)
		{
			AssociatedItemId = jsonReward["associatedItemId"].str;
			Amount = (int)jsonReward["amount"].n;
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jSONObject = base.toJSONObject();
			jSONObject.AddField("associatedItemId", AssociatedItemId);
			jSONObject.AddField("amount", Amount);
			jSONObject.AddField("className", GetType().Name);
			return jSONObject;
		}

		protected override bool giveInner()
		{
			try
			{
				StoreInventory.GiveItem(AssociatedItemId, Amount);
			}
			catch (VirtualItemNotFoundException ex)
			{
				SoomlaUtils.LogError(TAG, "(give) Couldn't find associated itemId: " + AssociatedItemId);
				SoomlaUtils.LogError(TAG, ex.Message);
				return false;
			}
			return true;
		}

		protected override bool takeInner()
		{
			try
			{
				StoreInventory.TakeItem(AssociatedItemId, Amount);
			}
			catch (VirtualItemNotFoundException ex)
			{
				SoomlaUtils.LogError(TAG, "(take) Couldn't find associated itemId: " + AssociatedItemId);
				SoomlaUtils.LogError(TAG, ex.Message);
				return false;
			}
			return true;
		}
	}
}
