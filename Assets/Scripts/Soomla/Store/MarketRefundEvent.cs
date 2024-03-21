namespace Soomla.Store
{
	public class MarketRefundEvent : SoomlaEvent
	{
		private PurchasableVirtualItem mPurchasableVirtualItem;

		public MarketRefundEvent(PurchasableVirtualItem purchasableVirtualItem)
			: this(purchasableVirtualItem, null)
		{
		}

		public MarketRefundEvent(PurchasableVirtualItem purchasableVirtualItem, object sender)
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
