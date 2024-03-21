namespace Soomla.Store
{
	public class MarketPurchaseCancelledEvent : SoomlaEvent
	{
		private PurchasableVirtualItem mPurchasableVirtualItem;

		public MarketPurchaseCancelledEvent(PurchasableVirtualItem purchasableVirtualItem)
			: this(purchasableVirtualItem, null)
		{
		}

		public MarketPurchaseCancelledEvent(PurchasableVirtualItem purchasableVirtualItem, object sender)
			: base(sender)
		{
			mPurchasableVirtualItem = purchasableVirtualItem;
		}

		public PurchasableVirtualItem getPurchasableVirtualItem()
		{
			return mPurchasableVirtualItem;
		}
	}
}
