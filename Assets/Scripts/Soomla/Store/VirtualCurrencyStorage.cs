namespace Soomla.Store
{
	public class VirtualCurrencyStorage : VirtualItemStorage
	{
		private static VirtualCurrencyStorage _instance;

		private static VirtualCurrencyStorage instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new VirtualCurrencyStorageAndroid();
				}
				return _instance;
			}
		}

		protected VirtualCurrencyStorage()
		{
			VirtualItemStorage.TAG = "SOOMLA VirtualCurrencyStorage";
		}

		public static int GetBalance(VirtualItem item)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "fetching balance for virtual item with itemId: " + item.ItemId);
			return instance._getBalance(item);
		}

		public static int SetBalance(VirtualItem item, int balance)
		{
			return SetBalance(item, balance, notify: true);
		}

		public static int SetBalance(VirtualItem item, int balance, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "setting balance " + balance + " to " + item.ItemId + ".");
			return instance._setBalance(item, balance, notify);
		}

		public static int Add(VirtualItem item, int amount)
		{
			return Add(item, amount, notify: true);
		}

		public static int Add(VirtualItem item, int amount, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "adding " + amount + " " + item.ItemId);
			return instance._add(item, amount, notify);
		}

		public static int Remove(VirtualItem item, int amount)
		{
			return Remove(item, amount, notify: true);
		}

		public static int Remove(VirtualItem item, int amount, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "Removing " + amount + " " + item.ItemId + ".");
			return instance._remove(item, amount, notify: true);
		}
	}
}
