using Soomla.Store;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class MarketManager
{
	public static MarketManager instance = new MarketManager();

	private string buyID;

	private bool onRequest;

	private Action<bool> callback;

	[CompilerGenerated]
	private static Action<PurchasableVirtualItem, string, Dictionary<string, string>> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<PurchasableVirtualItem> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Action<PurchasableVirtualItem, string, Dictionary<string, string>> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Action<PurchasableVirtualItem> _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Action<PurchasableVirtualItem, string, Dictionary<string, string>> _003C_003Ef__mg_0024cache4;

	[CompilerGenerated]
	private static Action<PurchasableVirtualItem> _003C_003Ef__mg_0024cache5;

	public static void BuyProduct(string productID, Action<bool> onResult)
	{
		instance.callback = onResult;
		SoomlaStore.StartIabServiceInBg();
		StoreEvents.OnMarketPurchase = (Action<PurchasableVirtualItem, string, Dictionary<string, string>>)Delegate.Combine(StoreEvents.OnMarketPurchase, new Action<PurchasableVirtualItem, string, Dictionary<string, string>>(OnMarketPurchase));
		StoreEvents.OnMarketPurchaseCancelled = (Action<PurchasableVirtualItem>)Delegate.Combine(StoreEvents.OnMarketPurchaseCancelled, new Action<PurchasableVirtualItem>(OnMarketPurchaseCancelled));
		StoreInventory.BuyItem(productID);
	}

	private void RequestBuy(string productID)
	{
		if (!onRequest)
		{
			try
			{
				onRequest = true;
				buyID = productID;
				StoreInventory.BuyItem(buyID);
			}
			catch (Exception)
			{
				onRequest = false;
			}
		}
	}

	public static void OnMarketPurchase(PurchasableVirtualItem pvi, string purchaseToken, Dictionary<string, string> payload)
	{
		if (instance.callback != null)
		{
			instance.callback(obj: true);
		}
		SoomlaStore.StopIabServiceInBg();
		StoreEvents.OnMarketPurchase = (Action<PurchasableVirtualItem, string, Dictionary<string, string>>)Delegate.Remove(StoreEvents.OnMarketPurchase, new Action<PurchasableVirtualItem, string, Dictionary<string, string>>(OnMarketPurchase));
		StoreEvents.OnMarketPurchaseCancelled = (Action<PurchasableVirtualItem>)Delegate.Remove(StoreEvents.OnMarketPurchaseCancelled, new Action<PurchasableVirtualItem>(OnMarketPurchaseCancelled));
	}

	public static void OnMarketPurchaseCancelled(PurchasableVirtualItem pvi)
	{
		if (instance.callback != null)
		{
			instance.callback(obj: false);
		}
		SoomlaStore.StopIabServiceInBg();
		StoreEvents.OnMarketPurchase = (Action<PurchasableVirtualItem, string, Dictionary<string, string>>)Delegate.Remove(StoreEvents.OnMarketPurchase, new Action<PurchasableVirtualItem, string, Dictionary<string, string>>(OnMarketPurchase));
		StoreEvents.OnMarketPurchaseCancelled = (Action<PurchasableVirtualItem>)Delegate.Remove(StoreEvents.OnMarketPurchaseCancelled, new Action<PurchasableVirtualItem>(OnMarketPurchaseCancelled));
	}
}
