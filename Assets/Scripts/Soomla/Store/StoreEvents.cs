using Soomla.Singletons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soomla.Store
{
	public class StoreEvents : CodeGeneratedSingleton
	{
		public delegate void RunLaterDelegate();

		public delegate void Action();

		public class StoreEventPusher
		{
			public void PushEventSoomlaStoreInitialized()
			{
				_pushEventSoomlaStoreInitialized(string.Empty);
			}

			public void PushEventUnexpectedStoreError(int errorCode)
			{
				JSONObject jSONObject = new JSONObject();
				jSONObject.AddField("errorCode", errorCode);
				_pushEventUnexpectedStoreError(jSONObject.print());
			}

			public void PushEventOnCurrencyBalanceChanged(VirtualCurrency currency, int balance, int amountAdded)
			{
				JSONObject jSONObject = new JSONObject();
				jSONObject.AddField("itemId", currency.ItemId);
				jSONObject.AddField("balance", balance);
				jSONObject.AddField("amountAdded", amountAdded);
				_pushEventCurrencyBalanceChanged(jSONObject.print());
			}

			public void PushEventOnGoodBalanceChanged(VirtualGood good, int balance, int amountAdded)
			{
				JSONObject jSONObject = new JSONObject();
				jSONObject.AddField("itemId", good.ItemId);
				jSONObject.AddField("balance", balance);
				jSONObject.AddField("amountAdded", amountAdded);
				_pushEventGoodBalanceChanged(jSONObject.print());
			}

			public void PushEventOnGoodEquipped(EquippableVG good)
			{
				JSONObject jSONObject = new JSONObject();
				jSONObject.AddField("itemId", good.ItemId);
				_pushEventGoodEquipped(jSONObject.print());
			}

			public void PushEventOnGoodUnequipped(EquippableVG good)
			{
				JSONObject jSONObject = new JSONObject();
				jSONObject.AddField("itemId", good.ItemId);
				_pushEventGoodUnequipped(jSONObject.print());
			}

			public void PushEventOnGoodUpgrade(VirtualGood good, UpgradeVG upgrade)
			{
				JSONObject jSONObject = new JSONObject();
				jSONObject.AddField("itemId", good.ItemId);
				jSONObject.AddField("upgradeItemId", (!(upgrade == null)) ? upgrade.ItemId : null);
				_pushEventGoodUpgrade(jSONObject.print());
			}

			public void PushEventOnItemPurchased(PurchasableVirtualItem item, string payload)
			{
				JSONObject jSONObject = new JSONObject();
				jSONObject.AddField("itemId", item.ItemId);
				jSONObject.AddField("payload", payload);
				_pushEventItemPurchased(jSONObject.print());
			}

			public void PushEventOnItemPurchaseStarted(PurchasableVirtualItem item)
			{
				JSONObject jSONObject = new JSONObject();
				jSONObject.AddField("itemId", item.ItemId);
				_pushEventItemPurchaseStarted(jSONObject.print());
			}

			protected virtual void _pushEventSoomlaStoreInitialized(string message)
			{
			}

			protected virtual void _pushEventUnexpectedStoreError(string message)
			{
			}

			protected virtual void _pushEventCurrencyBalanceChanged(string message)
			{
			}

			protected virtual void _pushEventGoodBalanceChanged(string message)
			{
			}

			protected virtual void _pushEventGoodEquipped(string message)
			{
			}

			protected virtual void _pushEventGoodUnequipped(string message)
			{
			}

			protected virtual void _pushEventGoodUpgrade(string message)
			{
			}

			protected virtual void _pushEventItemPurchased(string message)
			{
			}

			protected virtual void _pushEventItemPurchaseStarted(string message)
			{
			}
		}

		private const string TAG = "SOOMLA StoreEvents";

		public static StoreEvents Instance = null;

		private static StoreEventPusher sep = null;

		public static Action OnBillingNotSupported = delegate
		{
		};

		public static Action OnBillingSupported = delegate
		{
		};

		public static Action<VirtualCurrency, int, int> OnCurrencyBalanceChanged = delegate
		{
		};

		public static Action<VirtualGood, int, int> OnGoodBalanceChanged = delegate
		{
		};

		public static Action<EquippableVG> OnGoodEquipped = delegate
		{
		};

		public static Action<EquippableVG> OnGoodUnEquipped = delegate
		{
		};

		public static Action<VirtualGood, UpgradeVG> OnGoodUpgrade = delegate
		{
		};

		public static Action<PurchasableVirtualItem, string> OnItemPurchased = delegate
		{
		};

		public static Action<PurchasableVirtualItem> OnItemPurchaseStarted = delegate
		{
		};

		public static Action<PurchasableVirtualItem> OnMarketPurchaseCancelled = delegate
		{
		};

		public static Action<PurchasableVirtualItem, string> OnMarketPurchaseDeferred = delegate
		{
		};

		public static Action<PurchasableVirtualItem, string, Dictionary<string, string>> OnMarketPurchase = delegate
		{
		};

		public static Action<PurchasableVirtualItem> OnMarketPurchaseStarted = delegate
		{
		};

		public static Action<PurchasableVirtualItem> OnMarketRefund = delegate
		{
		};

		public static Action<bool> OnRestoreTransactionsFinished = delegate
		{
		};

		public static Action OnRestoreTransactionsStarted = delegate
		{
		};

		public static Action OnMarketItemsRefreshStarted = delegate
		{
		};

		public static Action<string> OnMarketItemsRefreshFailed = delegate
		{
		};

		public static Action<List<MarketItem>> OnMarketItemsRefreshFinished = delegate
		{
		};

		public static Action<int> OnUnexpectedStoreError = delegate
		{
		};

		public static Action<PurchasableVirtualItem> OnVerificationStarted = delegate
		{
		};

		public static Action OnSoomlaStoreInitialized = delegate
		{
		};

		public static Action OnIabServiceStarted = delegate
		{
		};

		public static Action OnIabServiceStopped = delegate
		{
		};

		protected override bool DontDestroySingleton => true;

		public void RunLater(RunLaterDelegate runLaterDelegate)
		{
			StartCoroutine(RunLaterPriv(0.1f, runLaterDelegate));
		}

		private IEnumerator RunLaterPriv(float delay, RunLaterDelegate runLaterDelegate)
		{
			float pauseEndTime = Time.realtimeSinceStartup + delay;
			while (Time.realtimeSinceStartup < pauseEndTime)
			{
				yield return null;
			}
			runLaterDelegate();
		}

		public static void Initialize()
		{
			if (Instance == null)
			{
				CoreEvents.Initialize();
				Instance = UnitySingleton.GetSynchronousCodeGeneratedInstance<StoreEvents>();
				SoomlaUtils.LogDebug("SOOMLA StoreEvents", "Initializing StoreEvents ...");
				AndroidJNI.PushLocalFrame(100);
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.soomla.unity.StoreEventHandler"))
				{
					androidJavaClass.CallStatic("initialize");
				}
				AndroidJNI.PopLocalFrame(IntPtr.Zero);
				sep = new StoreEventPusherAndroid();
			}
		}

		public void onBillingSupported(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onBillingSupported");
			OnBillingSupported();
		}

		public void onBillingNotSupported(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onBillingNotSupported");
			OnBillingNotSupported();
		}

		public void onCurrencyBalanceChanged(string message)
		{
			onCurrencyBalanceChanged(message, alsoPush: false);
		}

		public void onCurrencyBalanceChanged(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onCurrencyBalanceChanged:" + message);
			JSONObject jSONObject = new JSONObject(message);
			VirtualCurrency virtualCurrency = (VirtualCurrency)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			int num = (int)jSONObject["balance"].n;
			int num2 = (int)jSONObject["amountAdded"].n;
			StoreInventory.RefreshOnCurrencyBalanceChanged(virtualCurrency, num, num2);
			OnCurrencyBalanceChanged(virtualCurrency, num, num2);
			if (alsoPush)
			{
				sep.PushEventOnCurrencyBalanceChanged(virtualCurrency, num, num2);
			}
		}

		public void onGoodBalanceChanged(string message)
		{
			onGoodBalanceChanged(message, alsoPush: false);
		}

		public void onGoodBalanceChanged(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onGoodBalanceChanged:" + message);
			JSONObject jSONObject = new JSONObject(message);
			VirtualGood virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			int num = (int)jSONObject["balance"].n;
			int num2 = (int)jSONObject["amountAdded"].n;
			StoreInventory.RefreshOnGoodBalanceChanged(virtualGood, num, num2);
			OnGoodBalanceChanged(virtualGood, num, num2);
			if (alsoPush)
			{
				sep.PushEventOnGoodBalanceChanged(virtualGood, num, num2);
			}
		}

		public void onGoodEquipped(string message)
		{
			onGoodEquipped(message, alsoPush: false);
		}

		public void onGoodEquipped(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onVirtualGoodEquipped:" + message);
			JSONObject jSONObject = new JSONObject(message);
			EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			StoreInventory.RefreshOnGoodEquipped(equippableVG);
			OnGoodEquipped(equippableVG);
			if (alsoPush)
			{
				sep.PushEventOnGoodEquipped(equippableVG);
			}
		}

		public void onGoodUnequipped(string message)
		{
			onGoodUnequipped(message, alsoPush: false);
		}

		public void onGoodUnequipped(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onVirtualGoodUnEquipped:" + message);
			JSONObject jSONObject = new JSONObject(message);
			EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			StoreInventory.RefreshOnGoodUnEquipped(equippableVG);
			OnGoodUnEquipped(equippableVG);
			if (alsoPush)
			{
				sep.PushEventOnGoodUnequipped(equippableVG);
			}
		}

		public void onGoodUpgrade(string message)
		{
			onGoodUpgrade(message, alsoPush: false);
		}

		public void onGoodUpgrade(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onGoodUpgrade:" + message);
			JSONObject jSONObject = new JSONObject(message);
			VirtualGood virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			UpgradeVG upgradeVG = null;
			if (jSONObject.HasField("upgradeItemId") && !string.IsNullOrEmpty(jSONObject["upgradeItemId"].str))
			{
				upgradeVG = (UpgradeVG)StoreInfo.GetItemByItemId(jSONObject["upgradeItemId"].str);
			}
			StoreInventory.RefreshOnGoodUpgrade(virtualGood, upgradeVG);
			OnGoodUpgrade(virtualGood, upgradeVG);
			if (alsoPush)
			{
				sep.PushEventOnGoodUpgrade(virtualGood, upgradeVG);
			}
		}

		public void onItemPurchased(string message)
		{
			onItemPurchased(message, alsoPush: false);
		}

		public void onItemPurchased(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onItemPurchased:" + message);
			JSONObject jSONObject = new JSONObject(message);
			PurchasableVirtualItem purchasableVirtualItem = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			string text = string.Empty;
			if (jSONObject.HasField("payload"))
			{
				text = jSONObject["payload"].str;
			}
			OnItemPurchased(purchasableVirtualItem, text);
			if (alsoPush)
			{
				sep.PushEventOnItemPurchased(purchasableVirtualItem, text);
			}
		}

		public void onItemPurchaseStarted(string message)
		{
			onItemPurchaseStarted(message, alsoPush: false);
		}

		public void onItemPurchaseStarted(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onItemPurchaseStarted:" + message);
			JSONObject jSONObject = new JSONObject(message);
			PurchasableVirtualItem purchasableVirtualItem = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			OnItemPurchaseStarted(purchasableVirtualItem);
			if (alsoPush)
			{
				sep.PushEventOnItemPurchaseStarted(purchasableVirtualItem);
			}
		}

		public void onMarketPurchaseCancelled(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketPurchaseCancelled: " + message);
			JSONObject jSONObject = new JSONObject(message);
			PurchasableVirtualItem obj = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			OnMarketPurchaseCancelled(obj);
		}

		public void onMarketPurchaseDeferred(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketPurchaseDeferred: " + message);
			JSONObject jSONObject = new JSONObject(message);
			PurchasableVirtualItem arg = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			string arg2 = string.Empty;
			if (jSONObject.HasField("payload"))
			{
				arg2 = jSONObject["payload"].str;
			}
			OnMarketPurchaseDeferred(arg, arg2);
		}

		public void onMarketPurchase(string message)
		{
			JSONObject jSONObject = new JSONObject(message);
			PurchasableVirtualItem arg = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			string arg2 = string.Empty;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (jSONObject.HasField("payload"))
			{
				arg2 = jSONObject["payload"].str;
			}
			if (jSONObject.HasField("extra"))
			{
				JSONObject jSONObject2 = jSONObject["extra"];
				if (jSONObject2.keys != null)
				{
					foreach (string key in jSONObject2.keys)
					{
						if (jSONObject2[key] != null)
						{
							dictionary.Add(key, jSONObject2[key].str);
						}
					}
				}
			}
			OnMarketPurchase(arg, arg2, dictionary);
		}

		public void onMarketPurchaseStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketPurchaseStarted: " + message);
			JSONObject jSONObject = new JSONObject(message);
			PurchasableVirtualItem obj = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			OnMarketPurchaseStarted(obj);
		}

		public void onMarketRefund(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketRefund:" + message);
			JSONObject jSONObject = new JSONObject(message);
			PurchasableVirtualItem obj = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			OnMarketRefund(obj);
		}

		public void onRestoreTransactionsFinished(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onRestoreTransactionsFinished:" + message);
			JSONObject jSONObject = new JSONObject(message);
			bool b = jSONObject["success"].b;
			OnRestoreTransactionsFinished(b);
		}

		public void onRestoreTransactionsStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onRestoreTransactionsStarted");
			OnRestoreTransactionsStarted();
		}

		public void onMarketItemsRefreshStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketItemsRefreshStarted");
			OnMarketItemsRefreshStarted();
		}

		public void onMarketItemsRefreshFailed(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketItemsRefreshFailed");
			JSONObject jSONObject = new JSONObject(message);
			string str = jSONObject["errorMessage"].str;
			OnMarketItemsRefreshFailed(str);
		}

		public void onMarketItemsRefreshFinished(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onMarketItemsRefreshFinished: " + message);
			JSONObject jSONObject = new JSONObject(message);
			List<VirtualItem> list = new List<VirtualItem>();
			List<MarketItem> list2 = new List<MarketItem>();
			foreach (JSONObject item in jSONObject.list)
			{
				string str = item["productId"].str;
				string str2 = item["marketPrice"].str;
				string str3 = item["marketTitle"].str;
				string str4 = item["marketDesc"].str;
				string str5 = item["marketCurrencyCode"].str;
				long marketPriceMicros = Convert.ToInt64(item["marketPriceMicros"].n);
				try
				{
					PurchasableVirtualItem purchasableItemWithProductId = StoreInfo.GetPurchasableItemWithProductId(str);
					MarketItem marketItem = ((PurchaseWithMarket)purchasableItemWithProductId.PurchaseType).MarketItem;
					marketItem.MarketPriceAndCurrency = str2;
					marketItem.MarketTitle = str3;
					marketItem.MarketDescription = str4;
					marketItem.MarketCurrencyCode = str5;
					marketItem.MarketPriceMicros = marketPriceMicros;
					list2.Add(marketItem);
					list.Add(purchasableItemWithProductId);
				}
				catch (VirtualItemNotFoundException ex)
				{
					SoomlaUtils.LogDebug("SOOMLA StoreEvents", ex.Message);
				}
			}
			if (list.Count > 0)
			{
				StoreInfo.Save(list, saveToDB: false);
			}
			OnMarketItemsRefreshFinished(list2);
		}

		public void onUnexpectedStoreError(string message)
		{
			onUnexpectedStoreError(message, alsoPush: false);
		}

		public void onUnexpectedStoreError(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY OnUnexpectedStoreError");
			JSONObject jSONObject = new JSONObject(message);
			int num = (int)jSONObject["errorCode"].n;
			OnUnexpectedStoreError(num);
			if (alsoPush)
			{
				sep.PushEventUnexpectedStoreError(num);
			}
		}

		public void onVerificationStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onVerificationStarted: " + message);
			JSONObject jSONObject = new JSONObject(message);
			PurchasableVirtualItem obj = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(jSONObject["itemId"].str);
			OnVerificationStarted(obj);
		}

		public void onSoomlaStoreInitialized(string message)
		{
			onSoomlaStoreInitialized(message, alsoPush: false);
		}

		public void onSoomlaStoreInitialized(string message, bool alsoPush)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onSoomlaStoreInitialized");
			StoreInventory.RefreshLocalInventory();
			OnSoomlaStoreInitialized();
			if (alsoPush)
			{
				sep.PushEventSoomlaStoreInitialized();
			}
		}

		public void onIabServiceStarted(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onIabServiceStarted");
			OnIabServiceStarted();
		}

		public void onIabServiceStopped(string message)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreEvents", "SOOMLA/UNITY onIabServiceStopped");
			OnIabServiceStopped();
		}
	}
}
