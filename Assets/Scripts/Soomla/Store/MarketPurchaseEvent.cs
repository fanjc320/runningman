using System.Collections.Generic;

namespace Soomla.Store
{
	public class MarketPurchaseEvent : SoomlaEvent
	{
		public readonly PurchasableVirtualItem PurchasableVirtualItem;

		public new readonly string Payload;

		public readonly Dictionary<string, string> ExtraInfo;

		public MarketPurchaseEvent(PurchasableVirtualItem purchasableVirtualItem, string payload, Dictionary<string, string> extraInfo)
			: this(purchasableVirtualItem, payload, extraInfo, null)
		{
		}

		public MarketPurchaseEvent(PurchasableVirtualItem purchasableVirtualItem, string payload, Dictionary<string, string> extraInfo, object sender)
			: base(sender)
		{
			PurchasableVirtualItem = purchasableVirtualItem;
			Payload = payload;
			ExtraInfo = extraInfo;
		}
	}
}
