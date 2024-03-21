namespace Soomla.Store
{
	public class PurchaseWithVirtualItem : PurchaseType
	{
		private const string TAG = "SOOMLA PurchaseWithVirtualItem";

		public string TargetItemId;

		public int Amount;

		public PurchaseWithVirtualItem(string targetItemId, int amount)
		{
			TargetItemId = targetItemId;
			Amount = amount;
		}

		public override void Buy(string payload)
		{
			SoomlaUtils.LogDebug("SOOMLA PurchaseWithVirtualItem", "Trying to buy a " + AssociatedItem.Name + " with " + Amount + " pieces of " + TargetItemId);
			VirtualItem targetVirtualItem = getTargetVirtualItem();
			if (!(targetVirtualItem == null))
			{
				JSONObject eventJSON = new JSONObject();
				eventJSON.AddField("itemId", AssociatedItem.ItemId);
				StoreEvents.Instance.onItemPurchaseStarted(eventJSON.print(), alsoPush: true);
				if (!checkTargetBalance(targetVirtualItem))
				{
					throw new InsufficientFundsException(TargetItemId);
				}
				targetVirtualItem.Take(Amount);
				AssociatedItem.Give(1);
				StoreEvents.Instance.RunLater(delegate
				{
					eventJSON = new JSONObject();
					eventJSON.AddField("itemId", AssociatedItem.ItemId);
					eventJSON.AddField("payload", payload);
					StoreEvents.Instance.onItemPurchased(eventJSON.print(), alsoPush: true);
				});
			}
		}

		public override bool CanAfford()
		{
			SoomlaUtils.LogDebug("SOOMLA PurchaseWithVirtualItem", "Checking affordability of " + AssociatedItem.Name + " with " + Amount + " pieces of " + TargetItemId);
			VirtualItem targetVirtualItem = getTargetVirtualItem();
			return checkTargetBalance(targetVirtualItem);
		}

		public override string GetPrice()
		{
			return Amount.ToString();
		}

		private VirtualItem getTargetVirtualItem()
		{
			VirtualItem result = null;
			try
			{
				result = StoreInfo.GetItemByItemId(TargetItemId);
				return result;
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError("SOOMLA PurchaseWithVirtualItem", "Target virtual item doesn't exist !");
				return result;
			}
		}

		private bool checkTargetBalance(VirtualItem item)
		{
			int itemBalance = StoreInventory.GetItemBalance(item.ItemId);
			return itemBalance >= Amount;
		}
	}
}
