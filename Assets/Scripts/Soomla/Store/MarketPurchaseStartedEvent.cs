namespace Soomla.Store
{
	public class MarketPurchaseStartedEvent : SoomlaEvent
	{
		private PurchasableVirtualItem mPurchasableVirtualItem;

		private bool mFraudProtection;

		public MarketPurchaseStartedEvent(PurchasableVirtualItem purchasableVirtualItem)
			: this(purchasableVirtualItem, fraudProtection: false, null)
		{
		}

		public MarketPurchaseStartedEvent(PurchasableVirtualItem purchasableVirtualItem, bool fraudProtection)
			: this(purchasableVirtualItem, fraudProtection, null)
		{
		}

		public MarketPurchaseStartedEvent(PurchasableVirtualItem purchasableVirtualItem, object sender)
			: this(purchasableVirtualItem, fraudProtection: false, sender)
		{
		}

		public MarketPurchaseStartedEvent(PurchasableVirtualItem purchasableVirtualItem, bool fraudProtection, object sender)
			: base(sender)
		{
			mPurchasableVirtualItem = purchasableVirtualItem;
			mFraudProtection = fraudProtection;
		}

		public PurchasableVirtualItem getPurchasableVirtualItem()
		{
			return mPurchasableVirtualItem;
		}

		public bool isFraudProtection()
		{
			return mFraudProtection;
		}
	}
}
