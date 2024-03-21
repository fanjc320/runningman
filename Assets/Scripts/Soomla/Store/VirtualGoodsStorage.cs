namespace Soomla.Store
{
	public class VirtualGoodsStorage : VirtualItemStorage
	{
		private static VirtualGoodsStorage _instance;

		private static VirtualGoodsStorage instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new VirtualGoodsStorageAndroid();
				}
				return _instance;
			}
		}

		protected VirtualGoodsStorage()
		{
			VirtualItemStorage.TAG = "SOOMLA VirtualGoodsStorage";
		}

		public static void RemoveUpgrades(VirtualGood good)
		{
			RemoveUpgrades(good, notify: true);
		}

		public static void RemoveUpgrades(VirtualGood good, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "Removing upgrade information from virtual good: " + good.ItemId);
			instance._removeUpgrades(good, notify);
		}

		public static void AssignCurrentUpgrade(VirtualGood good, UpgradeVG upgradeVG)
		{
			AssignCurrentUpgrade(good, upgradeVG, notify: true);
		}

		public static void AssignCurrentUpgrade(VirtualGood good, UpgradeVG upgradeVG, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "Assigning upgrade " + upgradeVG.ItemId + " to virtual good: " + good.ItemId);
			instance._assignCurrentUpgrade(good, upgradeVG, notify);
		}

		public static UpgradeVG GetCurrentUpgrade(VirtualGood good)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "Fetching upgrade to virtual good: " + good.ItemId);
			return instance._getCurrentUpgrade(good);
		}

		public static bool IsEquipped(EquippableVG good)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "checking if virtual good with itemId: " + good.ItemId + " is equipped.");
			return instance._isEquipped(good);
		}

		public static void Equip(EquippableVG good)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "equipping: " + good.ItemId);
			Equip(good);
		}

		public static void Equip(EquippableVG good, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "equipping: " + good.ItemId);
			instance._equip(good, notify);
		}

		public static void UnEquip(EquippableVG good)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "unequipping: " + good.ItemId);
			UnEquip(good, notify: true);
		}

		public static void UnEquip(EquippableVG good, bool notify)
		{
			SoomlaUtils.LogDebug(VirtualItemStorage.TAG, "unequipping: " + good.ItemId);
			instance._unequip(good, notify);
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

		protected virtual void _removeUpgrades(VirtualGood good, bool notify)
		{
		}

		protected virtual void _assignCurrentUpgrade(VirtualGood good, UpgradeVG upgradeVG, bool notify)
		{
		}

		protected virtual UpgradeVG _getCurrentUpgrade(VirtualGood good)
		{
			return null;
		}

		protected virtual bool _isEquipped(EquippableVG good)
		{
			return false;
		}

		protected virtual void _equip(EquippableVG good, bool notify)
		{
		}

		protected virtual void _unequip(EquippableVG good, bool notify)
		{
		}
	}
}
