namespace Soomla.Store
{
	public class ItemPurchasedEvent : SoomlaEvent
	{
		private PurchasableVirtualItem mItem;

		private string mPayload;

		public ItemPurchasedEvent(PurchasableVirtualItem item, string payload)
			: this(item, payload, null)
		{
		}

		public ItemPurchasedEvent(PurchasableVirtualItem item, string payload, object sender)
			: base(sender)
		{
			mItem = item;
			mPayload = payload;
		}

		public PurchasableVirtualItem getItem()
		{
			return mItem;
		}

		public string getPayload()
		{
			return mPayload;
		}
	}
}
