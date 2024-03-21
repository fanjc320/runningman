using System.Collections.Generic;

namespace Soomla.Store
{
	public class MarketItemsRefreshFinishedEvent : SoomlaEvent
	{
		private List<MarketItem> mMarketItems;

		public MarketItemsRefreshFinishedEvent(List<MarketItem> marketItems)
			: this(marketItems, null)
		{
		}

		public MarketItemsRefreshFinishedEvent(List<MarketItem> marketItems, object sender)
			: base(sender)
		{
			mMarketItems = marketItems;
		}

		public List<MarketItem> getMarketItems()
		{
			return mMarketItems;
		}
	}
}
