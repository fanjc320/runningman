using System;

namespace Soomla.Store
{
	public class SingleUsePackVG : VirtualGood
	{
		private static string TAG = "SOOMLA SingleUsePackVG";

		public string GoodItemId;

		public int GoodAmount;

		public SingleUsePackVG(string goodItemId, int amount, string name, string description, string itemId, PurchaseType purchaseType)
			: base(name, description, itemId, purchaseType)
		{
			GoodItemId = goodItemId;
			GoodAmount = amount;
		}

		public SingleUsePackVG(JSONObject jsonItem)
			: base(jsonItem)
		{
			GoodItemId = jsonItem["good_itemId"].str;
			GoodAmount = Convert.ToInt32(jsonItem["good_amount"].n);
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jSONObject = base.toJSONObject();
			jSONObject.AddField("good_itemId", GoodItemId);
			jSONObject.AddField("good_amount", GoodAmount);
			return jSONObject;
		}

		public override int Give(int amount, bool notify)
		{
			SingleUseVG singleUseVG = null;
			try
			{
				singleUseVG = (SingleUseVG)StoreInfo.GetItemByItemId(GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(TAG, "SingleUseVG with itemId: " + GoodItemId + " doesn't exist! Can't give this pack.");
				return 0;
			}
			return VirtualGoodsStorage.Add(singleUseVG, GoodAmount * amount, notify);
		}

		public override int Take(int amount, bool notify)
		{
			SingleUseVG singleUseVG = null;
			try
			{
				singleUseVG = (SingleUseVG)StoreInfo.GetItemByItemId(GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(TAG, "SingleUseVG with itemId: " + GoodItemId + " doesn't exist! Can't give this pack.");
				return 0;
			}
			return VirtualGoodsStorage.Remove(singleUseVG, GoodAmount * amount, notify);
		}

		public override int ResetBalance(int balance, bool notify)
		{
			SoomlaUtils.LogError(TAG, "Someone tried to reset balance of GoodPack. That's not right.");
			return 0;
		}

		public override int GetBalance()
		{
			SoomlaUtils.LogError(TAG, "Someone tried to check balance of GoodPack. That's not right.");
			return 0;
		}

		protected override bool canBuy()
		{
			return true;
		}
	}
}
