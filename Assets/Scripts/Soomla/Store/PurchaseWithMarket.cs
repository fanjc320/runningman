namespace Soomla.Store
{
	public class PurchaseWithMarket : PurchaseType
	{
		private const string TAG = "SOOMLA PurchaseWithMarket";

		public MarketItem MarketItem;

		public PurchaseWithMarket(string productId, double price)
		{
			MarketItem = new MarketItem(productId, price);
		}

		public PurchaseWithMarket(MarketItem marketItem)
		{
			MarketItem = marketItem;
		}

		public override void Buy(string payload)
		{
			SoomlaUtils.LogDebug("SOOMLA PurchaseWithMarket", "Starting in-app purchase for productId: " + MarketItem.ProductId);
			JSONObject jSONObject = new JSONObject();
			jSONObject.AddField("itemId", AssociatedItem.ItemId);
			StoreEvents.Instance.onItemPurchaseStarted(jSONObject.print(), alsoPush: true);
			SoomlaStore.BuyMarketItem(MarketItem.ProductId, payload);
		}

		public override bool CanAfford()
		{
			return true;
		}

		public override string GetPrice()
		{
			return MarketItem.Price.ToString();
		}
	}
}
