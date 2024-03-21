using System.Collections.Generic;

namespace Soomla.Store
{
	public class StoreInventory
	{
		private class LocalUpgrade
		{
			public int level;

			public string itemId;
		}

		protected const string TAG = "SOOMLA StoreInventory";

		private static Dictionary<string, int> localItemBalances;

		private static Dictionary<string, LocalUpgrade> localUpgrades;

		private static HashSet<string> localEquippedGoods;

		public static bool CanAfford(string itemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking can afford: " + itemId);
			PurchasableVirtualItem purchasableVirtualItem = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(itemId);
			return purchasableVirtualItem.CanAfford();
		}

		public static void BuyItem(string itemId)
		{
			BuyItem(itemId, string.Empty);
		}

		public static void BuyItem(string itemId, string payload)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Buying: " + itemId);
			PurchasableVirtualItem purchasableVirtualItem = (PurchasableVirtualItem)StoreInfo.GetItemByItemId(itemId);
			purchasableVirtualItem.Buy(payload);
		}

		public static int GetItemBalance(string itemId)
		{
			if (localItemBalances.TryGetValue(itemId, out int value))
			{
				return value;
			}
			VirtualItem itemByItemId = StoreInfo.GetItemByItemId(itemId);
			return itemByItemId.GetBalance();
		}

		public static void GiveItem(string itemId, int amount)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Giving: " + amount + " pieces of: " + itemId);
			VirtualItem itemByItemId = StoreInfo.GetItemByItemId(itemId);
			itemByItemId.Give(amount);
		}

		public static void TakeItem(string itemId, int amount)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Taking: " + amount + " pieces of: " + itemId);
			VirtualItem itemByItemId = StoreInfo.GetItemByItemId(itemId);
			itemByItemId.Take(amount);
		}

		public static void EquipVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Equipping: " + goodItemId);
			EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(goodItemId);
			try
			{
				equippableVG.Equip();
			}
			catch (NotEnoughGoodsException ex)
			{
				SoomlaUtils.LogError("SOOMLA StoreInventory", "UNEXPECTED! Couldn't equip something");
				throw ex;
			}
		}

		public static void UnEquipVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "UnEquipping: " + goodItemId);
			EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(goodItemId);
			equippableVG.Unequip();
		}

		public static bool IsVirtualGoodEquipped(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking if " + goodItemId + " is equipped");
			EquippableVG good = (EquippableVG)StoreInfo.GetItemByItemId(goodItemId);
			return VirtualGoodsStorage.IsEquipped(good);
		}

		public static EquippableVG GetEquippedVirtualGood(VirtualCategory category)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking equipped goood in " + category.Name + " category");
			foreach (string goodItemId in category.GoodItemIds)
			{
				EquippableVG equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(goodItemId);
				if (equippableVG != null && equippableVG.Equipping == EquippableVG.EquippingModel.CATEGORY && VirtualGoodsStorage.IsEquipped(equippableVG) && StoreInfo.GetCategoryForVirtualGood(goodItemId) == category)
				{
					return equippableVG;
				}
			}
			SoomlaUtils.LogError("SOOMLA StoreInventory", "There is no virtual good equipped in " + category.Name + " category");
			return null;
		}

		public static int GetGoodUpgradeLevel(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking " + goodItemId + " upgrade level");
			VirtualGood virtualGood = (VirtualGood)StoreInfo.GetItemByItemId(goodItemId);
			if (virtualGood == null)
			{
				SoomlaUtils.LogError("SOOMLA StoreInventory", "You tried to get the level of a non-existant virtual good.");
				return 0;
			}
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(virtualGood);
			if (currentUpgrade == null)
			{
				return 0;
			}
			UpgradeVG upgradeVG = StoreInfo.GetFirstUpgradeForVirtualGood(goodItemId);
			int num = 1;
			while (upgradeVG.ItemId != currentUpgrade.ItemId)
			{
				upgradeVG = (UpgradeVG)StoreInfo.GetItemByItemId(upgradeVG.NextItemId);
				num++;
			}
			return num;
		}

		public static string GetGoodCurrentUpgrade(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Checking " + goodItemId + " current upgrade");
			VirtualGood good = (VirtualGood)StoreInfo.GetItemByItemId(goodItemId);
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(good);
			if (currentUpgrade == null)
			{
				return string.Empty;
			}
			return currentUpgrade.ItemId;
		}

		public static void UpgradeGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "SOOMLA/UNITY Calling UpgradeGood with: " + goodItemId);
			VirtualGood good = (VirtualGood)StoreInfo.GetItemByItemId(goodItemId);
			UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(good);
			if (currentUpgrade != null)
			{
				string nextItemId = currentUpgrade.NextItemId;
				if (!string.IsNullOrEmpty(nextItemId))
				{
					UpgradeVG upgradeVG = (UpgradeVG)StoreInfo.GetItemByItemId(nextItemId);
					upgradeVG.Buy(string.Empty);
				}
			}
			else
			{
				UpgradeVG firstUpgradeForVirtualGood = StoreInfo.GetFirstUpgradeForVirtualGood(goodItemId);
				if (firstUpgradeForVirtualGood != null)
				{
					firstUpgradeForVirtualGood.Buy(string.Empty);
				}
			}
		}

		public static void RemoveGoodUpgrades(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "SOOMLA/UNITY Calling RemoveGoodUpgrades with: " + goodItemId);
			List<UpgradeVG> upgradesForVirtualGood = StoreInfo.GetUpgradesForVirtualGood(goodItemId);
			foreach (UpgradeVG item in upgradesForVirtualGood)
			{
				VirtualGoodsStorage.Remove(item, 1, notify: true);
			}
			VirtualGood good = (VirtualGood)StoreInfo.GetItemByItemId(goodItemId);
			VirtualGoodsStorage.RemoveUpgrades(good);
		}

		public static void RefreshLocalInventory()
		{
			SoomlaUtils.LogDebug("SOOMLA StoreInventory", "Refreshing local inventory");
			localItemBalances = new Dictionary<string, int>();
			localUpgrades = new Dictionary<string, LocalUpgrade>();
			localEquippedGoods = new HashSet<string>();
			foreach (VirtualCurrency currency in StoreInfo.Currencies)
			{
				localItemBalances[currency.ItemId] = VirtualCurrencyStorage.GetBalance(currency);
			}
			foreach (VirtualGood good in StoreInfo.Goods)
			{
				localItemBalances[good.ItemId] = VirtualGoodsStorage.GetBalance(good);
				UpgradeVG currentUpgrade = VirtualGoodsStorage.GetCurrentUpgrade(good);
				if (currentUpgrade != null)
				{
					int goodUpgradeLevel = GetGoodUpgradeLevel(good.ItemId);
					localUpgrades.AddOrUpdate(good.ItemId, new LocalUpgrade
					{
						itemId = currentUpgrade.ItemId,
						level = goodUpgradeLevel
					});
				}
				if (good is EquippableVG && VirtualGoodsStorage.IsEquipped((EquippableVG)good))
				{
					localEquippedGoods.Add(good.ItemId);
				}
			}
		}

		public static void RefreshOnGoodUpgrade(VirtualGood vg, UpgradeVG uvg)
		{
			if (uvg == null)
			{
				localUpgrades.Remove(vg.ItemId);
				return;
			}
			int goodUpgradeLevel = GetGoodUpgradeLevel(vg.ItemId);
			if (localUpgrades.TryGetValue(vg.ItemId, out LocalUpgrade value))
			{
				value.itemId = uvg.ItemId;
				value.level = goodUpgradeLevel;
			}
			else
			{
				localUpgrades.Add(vg.ItemId, new LocalUpgrade
				{
					itemId = uvg.ItemId,
					level = goodUpgradeLevel
				});
			}
		}

		public static void RefreshOnGoodEquipped(EquippableVG equippable)
		{
			localEquippedGoods.Add(equippable.ItemId);
		}

		public static void RefreshOnGoodUnEquipped(EquippableVG equippable)
		{
			localEquippedGoods.Remove(equippable.ItemId);
		}

		public static void RefreshOnCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded)
		{
			UpdateLocalBalance(virtualCurrency.ItemId, balance);
		}

		public static void RefreshOnGoodBalanceChanged(VirtualGood good, int balance, int amountAdded)
		{
			UpdateLocalBalance(good.ItemId, balance);
		}

		private static void UpdateLocalBalance(string itemId, int balance)
		{
			localItemBalances[itemId] = balance;
		}
	}
}
