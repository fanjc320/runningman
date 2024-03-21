namespace Soomla.Store
{
	public class UpgradeVG : LifetimeVG
	{
		private static string TAG = "SOOMLA UpgradeVG";

		public string GoodItemId;

		public string NextItemId;

		public string PrevItemId;

		public UpgradeVG(string goodItemId, string nextItemId, string prevItemId, string name, string description, string itemId, PurchaseType purchaseType)
			: base(name, description, itemId, purchaseType)
		{
			GoodItemId = goodItemId;
			PrevItemId = prevItemId;
			NextItemId = nextItemId;
		}

		public UpgradeVG(JSONObject jsonItem)
			: base(jsonItem)
		{
			GoodItemId = jsonItem["good_itemId"].str;
			PrevItemId = jsonItem["prev_itemId"].str;
			NextItemId = jsonItem["next_itemId"].str;
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jSONObject = base.toJSONObject();
			jSONObject.AddField("good_itemId", GoodItemId);
			jSONObject.AddField("prev_itemId", (!string.IsNullOrEmpty(PrevItemId)) ? PrevItemId : string.Empty);
			jSONObject.AddField("next_itemId", (!string.IsNullOrEmpty(NextItemId)) ? NextItemId : string.Empty);
			return jSONObject;
		}

		protected override bool canBuy()
		{
			VirtualGood virtualGood = null;
			try
			{
				virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(TAG, "VirtualGood with itemId: " + GoodItemId + " doesn't exist! Returning NO (can't buy).");
				return false;
			}
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(virtualGood);
			return ((currentUpgrade == null && string.IsNullOrEmpty(PrevItemId)) || (currentUpgrade != null && (currentUpgrade.NextItemId == base.ItemId || currentUpgrade.PrevItemId == base.ItemId))) && base.canBuy();
		}

		public override int Give(int amount, bool notify)
		{
			SoomlaUtils.LogDebug(TAG, "Assigning " + Name + " to: " + GoodItemId);
			VirtualGood virtualGood = null;
			try
			{
				virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(TAG, "VirtualGood with itemId: " + GoodItemId + " doesn't exist! Can't upgrade.");
				return 0;
			}
			VirtualGoodsStorage.AssignCurrentUpgrade(virtualGood, this, notify);
			return base.Give(amount, notify);
		}

		public override int Take(int amount, bool notify)
		{
			VirtualGood virtualGood = null;
			try
			{
				virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(GoodItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(TAG, "VirtualGood with itemId: " + GoodItemId + " doesn't exist! Can't downgrade.");
				return 0;
			}
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(virtualGood);
			if (currentUpgrade != this)
			{
				SoomlaUtils.LogError(TAG, "You can't take an upgrade that's not currently assigned.The UpgradeVG " + Name + " is not assigned to the VirtualGood: " + virtualGood.Name);
				return 0;
			}
			if (!string.IsNullOrEmpty(PrevItemId))
			{
				UpgradeVG upgradeVG = null;
				try
				{
					upgradeVG = (UpgradeVG)StoreInfo.GetItemByItemId(PrevItemId);
				}
				catch (VirtualItemNotFoundException)
				{
					SoomlaUtils.LogError(TAG, "Previous UpgradeVG with itemId: " + PrevItemId + " doesn't exist! Can't downgrade.");
					return 0;
				}
				SoomlaUtils.LogDebug(TAG, "Downgrading " + virtualGood.Name + " to: " + upgradeVG.Name);
				VirtualGoodsStorage.AssignCurrentUpgrade(virtualGood, upgradeVG, notify);
			}
			else
			{
				SoomlaUtils.LogDebug(TAG, "Downgrading " + virtualGood.Name + " to NO-UPGRADE");
				VirtualGoodsStorage.RemoveUpgrades(virtualGood, notify);
			}
			return base.Take(amount, notify);
		}
	}
}
