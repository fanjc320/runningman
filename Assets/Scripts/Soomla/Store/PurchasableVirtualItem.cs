using System;

namespace Soomla.Store
{
	public abstract class PurchasableVirtualItem : VirtualItem
	{
		private const string TAG = "SOOMLA PurchasableVirtualItem";

		public PurchaseType PurchaseType;

		protected PurchasableVirtualItem(string name, string description, string itemId, PurchaseType purchaseType)
			: base(name, description, itemId)
		{
			PurchaseType = purchaseType;
			if (PurchaseType != null)
			{
				PurchaseType.AssociatedItem = this;
			}
		}

		protected PurchasableVirtualItem(JSONObject jsonItem)
			: base(jsonItem)
		{
			JSONObject jSONObject = jsonItem["purchasableItem"];
			string str = jSONObject["purchaseType"].str;
			if (str == "market")
			{
				JSONObject jsonObject = jSONObject["marketItem"];
				PurchaseType = new PurchaseWithMarket(new MarketItem(jsonObject));
			}
			else if (str == "virtualItem")
			{
				string str2 = jSONObject["pvi_itemId"].str;
				int amount = Convert.ToInt32(jSONObject["pvi_amount"].n);
				PurchaseType = new PurchaseWithVirtualItem(str2, amount);
			}
			else
			{
				SoomlaUtils.LogError("SOOMLA PurchasableVirtualItem", "Couldn't determine what type of class is the given purchaseType.");
			}
			if (PurchaseType != null)
			{
				PurchaseType.AssociatedItem = this;
			}
		}

		public bool CanAfford()
		{
			return PurchaseType.CanAfford();
		}

		public void Buy(string payload)
		{
			if (canBuy())
			{
				PurchaseType.Buy(payload);
			}
		}

		protected abstract bool canBuy();

		public override JSONObject toJSONObject()
		{
			JSONObject jSONObject = base.toJSONObject();
			try
			{
				JSONObject jSONObject2 = new JSONObject(JSONObject.Type.OBJECT);
				if (PurchaseType is PurchaseWithMarket)
				{
					jSONObject2.AddField("purchaseType", "market");
					MarketItem marketItem = ((PurchaseWithMarket)PurchaseType).MarketItem;
					jSONObject2.AddField("marketItem", marketItem.toJSONObject());
				}
				else if (PurchaseType is PurchaseWithVirtualItem)
				{
					jSONObject2.AddField("purchaseType", "virtualItem");
					jSONObject2.AddField("pvi_itemId", ((PurchaseWithVirtualItem)PurchaseType).TargetItemId);
					jSONObject2.AddField("pvi_amount", ((PurchaseWithVirtualItem)PurchaseType).Amount);
				}
				jSONObject.AddField("purchasableItem", jSONObject2);
				return jSONObject;
			}
			catch (Exception ex)
			{
				SoomlaUtils.LogError("SOOMLA PurchasableVirtualItem", "An error occurred while generating JSON object. " + ex.Message);
				return jSONObject;
			}
		}
	}
}
