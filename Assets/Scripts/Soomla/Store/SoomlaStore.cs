using UnityEngine;

namespace Soomla.Store
{
	public class SoomlaStore
	{
		private static SoomlaStore _instance;

		protected const string TAG = "SOOMLA SoomlaStore";

		private static SoomlaStore instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SoomlaStoreAndroid();
				}
				return _instance;
			}
		}

		public static bool Initialized
		{
			get;
			private set;
		}

		public static bool Initialize(IStoreAssets storeAssets)
		{
			StoreEvents.Initialize();
			if (string.IsNullOrEmpty(CoreSettings.SoomlaSecret))
			{
				SoomlaUtils.LogError("SOOMLA SoomlaStore", "MISSING SoomlaSecret !!! Stopping here !!");
				throw new ExitGUIException();
			}
			if (CoreSettings.SoomlaSecret == CoreSettings.ONLY_ONCE_DEFAULT)
			{
				SoomlaUtils.LogError("SOOMLA SoomlaStore", "You have to change SoomlaSecret !!! Stopping here !!");
				throw new ExitGUIException();
			}
			StoreEvents x = Object.FindObjectOfType<StoreEvents>();
			if (x == null)
			{
				SoomlaUtils.LogDebug("SOOMLA SoomlaStore", "StoreEvents Component not found in scene. We're continuing from here but you won't get many events.");
			}
			if (Initialized)
			{
				StoreEvents.Instance.onUnexpectedStoreError("{\"errorCode\": 0}", alsoPush: true);
				SoomlaUtils.LogError("SOOMLA SoomlaStore", "SoomlaStore is already initialized. You can't initialize it twice!");
				return false;
			}
			SoomlaUtils.LogDebug("SOOMLA SoomlaStore", "SoomlaStore Initializing ...");
			StoreInfo.SetStoreAssets(storeAssets);
			instance._loadBillingService();
			instance._refreshInventory();
			Initialized = true;
			StoreEvents.Instance.onSoomlaStoreInitialized(string.Empty, alsoPush: true);
			return true;
		}

		public static void BuyMarketItem(string productId, string payload)
		{
			instance._buyMarketItem(productId, payload);
		}

		public static void RefreshInventory()
		{
			instance._refreshInventory();
		}

		public static void RefreshMarketItemsDetails()
		{
			instance._refreshMarketItemsDetails();
		}

		public static void RestoreTransactions()
		{
			instance._restoreTransactions();
		}

		public static bool TransactionsAlreadyRestored()
		{
			return instance._transactionsAlreadyRestored();
		}

		public static void StartIabServiceInBg()
		{
			instance._startIabServiceInBg();
		}

		public static void StopIabServiceInBg()
		{
			instance._stopIabServiceInBg();
		}

		protected virtual void _loadBillingService()
		{
		}

		protected virtual void _buyMarketItem(string productId, string payload)
		{
		}

		protected virtual void _refreshInventory()
		{
		}

		protected virtual void _restoreTransactions()
		{
		}

		protected virtual void _refreshMarketItemsDetails()
		{
		}

		protected virtual bool _transactionsAlreadyRestored()
		{
			return true;
		}

		protected virtual void _startIabServiceInBg()
		{
		}

		protected virtual void _stopIabServiceInBg()
		{
		}
	}
}
