namespace Soomla.Store
{
	public class ItemPurchaseStartedEvent : SoomlaEvent
	{
		private PurchasableVirtualItem mItem;

		public ItemPurchaseStartedEvent(PurchasableVirtualItem item)
			: this(item, null)
		{
		}

		public ItemPurchaseStartedEvent(PurchasableVirtualItem item, object sender)
			: base(sender)
		{
			mItem = item;
		}

		public PurchasableVirtualItem getItem()
		{
			return mItem;
		}
	}
}
