using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soomla.Store
{
	public class StoreInfo
	{
		protected const string TAG = "SOOMLA/UNITY StoreInfo";

		private static StoreInfo _instance = null;

		public static Dictionary<string, VirtualItem> VirtualItems = new Dictionary<string, VirtualItem>();

		public static Dictionary<string, PurchasableVirtualItem> PurchasableItems = new Dictionary<string, PurchasableVirtualItem>();

		public static Dictionary<string, VirtualCategory> GoodsCategories = new Dictionary<string, VirtualCategory>();

		public static Dictionary<string, List<UpgradeVG>> GoodsUpgrades = new Dictionary<string, List<UpgradeVG>>();

		public static List<VirtualCurrency> Currencies = new List<VirtualCurrency>();

		public static List<VirtualCurrencyPack> CurrencyPacks = new List<VirtualCurrencyPack>();

		public static List<VirtualGood> Goods = new List<VirtualGood>();

		public static List<VirtualCategory> Categories = new List<VirtualCategory>();

		private static StoreInfo instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StoreInfoAndroid();
				}
				return _instance;
			}
		}

		private static bool assetsArrayHasMarketIdDuplicates(PurchasableVirtualItem[] assetsArray)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (PurchasableVirtualItem purchasableVirtualItem in assetsArray)
			{
				if (purchasableVirtualItem.PurchaseType.GetType() == typeof(PurchaseWithMarket))
				{
					string productId = ((PurchaseWithMarket)purchasableVirtualItem.PurchaseType).MarketItem.ProductId;
					if (hashSet.Contains(productId))
					{
						return false;
					}
					hashSet.Add(productId);
				}
			}
			return true;
		}

		private static void validateStoreAssets(IStoreAssets storeAssets)
		{
			if (storeAssets == null)
			{
				throw new ArgumentException("The given store assets can't be null!");
			}
			if (storeAssets.GetCurrencies() == null || storeAssets.GetCurrencyPacks() == null || storeAssets.GetGoods() == null || storeAssets.GetCategories() == null)
			{
				throw new ArgumentException("All IStoreAssets methods shouldn't return NULL-pointer references!");
			}
			if (!assetsArrayHasMarketIdDuplicates(storeAssets.GetGoods()) || !assetsArrayHasMarketIdDuplicates(storeAssets.GetCurrencyPacks()))
			{
				throw new ArgumentException("The given store assets has duplicates at marketItem productId!");
			}
		}

		public static void SetStoreAssets(IStoreAssets storeAssets)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Setting store assets in SoomlaInfo");
			try
			{
				validateStoreAssets(storeAssets);
				instance._setStoreAssets(storeAssets);
				initializeFromDB();
			}
			catch (ArgumentException ex)
			{
				SoomlaUtils.LogError("SOOMLA/UNITY StoreInfo", ex.Message);
			}
		}

		public static VirtualItem GetItemByItemId(string itemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch an item with itemId: " + itemId);
			if (VirtualItems != null && VirtualItems.TryGetValue(itemId, out VirtualItem value))
			{
				return value;
			}
			throw new VirtualItemNotFoundException("itemId", itemId);
		}

		public static PurchasableVirtualItem GetPurchasableItemWithProductId(string productId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch a purchasable item with productId: " + productId);
			if (PurchasableItems != null && PurchasableItems.TryGetValue(productId, out PurchasableVirtualItem value))
			{
				return value;
			}
			throw new VirtualItemNotFoundException("productId", productId);
		}

		public static VirtualCategory GetCategoryForVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch a category for a good with itemId: " + goodItemId);
			if (GoodsCategories != null && GoodsCategories.TryGetValue(goodItemId, out VirtualCategory value))
			{
				return value;
			}
			throw new VirtualItemNotFoundException("goodItemId of category", goodItemId);
		}

		public static UpgradeVG GetFirstUpgradeForVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch first upgrade of a good with itemId: " + goodItemId);
			if (GoodsUpgrades != null && GoodsUpgrades.TryGetValue(goodItemId, out List<UpgradeVG> value))
			{
				return value.FirstOrDefault((UpgradeVG up) => string.IsNullOrEmpty(up.PrevItemId));
			}
			return null;
		}

		public static UpgradeVG GetLastUpgradeForVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch last upgrade of a good with itemId: " + goodItemId);
			if (GoodsUpgrades != null && GoodsUpgrades.TryGetValue(goodItemId, out List<UpgradeVG> value))
			{
				return value.FirstOrDefault((UpgradeVG up) => string.IsNullOrEmpty(up.NextItemId));
			}
			return null;
		}

		public static List<UpgradeVG> GetUpgradesForVirtualGood(string goodItemId)
		{
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "Trying to fetch upgrades of a good with itemId: " + goodItemId);
			if (GoodsUpgrades != null && GoodsUpgrades.TryGetValue(goodItemId, out List<UpgradeVG> value))
			{
				return value;
			}
			return null;
		}

		public static void Save()
		{
			string text = toJSONObject().print();
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "saving StoreInfo to DB. json is: " + text);
			string key = keyMetaStoreInfo();
			KeyValueStorage.SetValue(key, text);
			instance.loadNativeFromDB();
		}

		public static void Save(VirtualItem virtualItem, bool saveToDB = true)
		{
			replaceVirtualItem(virtualItem);
			if (saveToDB)
			{
				Save();
			}
		}

		public static void Save(List<VirtualItem> virtualItems, bool saveToDB = true)
		{
			if (virtualItems != null || virtualItems.Count != 0)
			{
				foreach (VirtualItem virtualItem in virtualItems)
				{
					replaceVirtualItem(virtualItem);
				}
				if (saveToDB)
				{
					Save();
				}
			}
		}

		protected virtual void _setStoreAssets(IStoreAssets storeAssets)
		{
		}

		protected virtual void loadNativeFromDB()
		{
		}

		protected static string IStoreAssetsToJSON(IStoreAssets storeAssets)
		{
			JSONObject jSONObject = new JSONObject(JSONObject.Type.ARRAY);
			VirtualCurrency[] currencies = storeAssets.GetCurrencies();
			foreach (VirtualCurrency virtualCurrency in currencies)
			{
				jSONObject.Add(virtualCurrency.toJSONObject());
			}
			JSONObject jSONObject2 = new JSONObject(JSONObject.Type.ARRAY);
			VirtualCurrencyPack[] currencyPacks = storeAssets.GetCurrencyPacks();
			foreach (VirtualCurrencyPack virtualCurrencyPack in currencyPacks)
			{
				jSONObject2.Add(virtualCurrencyPack.toJSONObject());
			}
			JSONObject jSONObject3 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jSONObject4 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jSONObject5 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jSONObject6 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jSONObject7 = new JSONObject(JSONObject.Type.ARRAY);
			VirtualGood[] goods = storeAssets.GetGoods();
			foreach (VirtualGood virtualGood in goods)
			{
				if (virtualGood is SingleUseVG)
				{
					jSONObject3.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is EquippableVG)
				{
					jSONObject5.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is UpgradeVG)
				{
					jSONObject6.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is LifetimeVG)
				{
					jSONObject4.Add(virtualGood.toJSONObject());
				}
				else if (virtualGood is SingleUsePackVG)
				{
					jSONObject7.Add(virtualGood.toJSONObject());
				}
			}
			JSONObject jSONObject8 = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject8.AddField("singleUse", jSONObject3);
			jSONObject8.AddField("lifetime", jSONObject4);
			jSONObject8.AddField("equippable", jSONObject5);
			jSONObject8.AddField("goodUpgrades", jSONObject6);
			jSONObject8.AddField("goodPacks", jSONObject7);
			JSONObject jSONObject9 = new JSONObject(JSONObject.Type.ARRAY);
			VirtualCategory[] categories = storeAssets.GetCategories();
			foreach (VirtualCategory virtualCategory in categories)
			{
				jSONObject9.Add(virtualCategory.toJSONObject());
			}
			JSONObject jSONObject10 = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject10.AddField("categories", jSONObject9);
			jSONObject10.AddField("currencies", jSONObject);
			jSONObject10.AddField("currencyPacks", jSONObject2);
			jSONObject10.AddField("goods", jSONObject8);
			return jSONObject10.print();
		}

		private static void initializeFromDB()
		{
			string key = keyMetaStoreInfo();
			string value = KeyValueStorage.GetValue(key);
			if (string.IsNullOrEmpty(value))
			{
				SoomlaUtils.LogError("SOOMLA/UNITY StoreInfo", "store json is not in DB. Make sure you initialized SoomlaStore with your Store assets. The App will shut down now.");
				Application.Quit();
			}
			SoomlaUtils.LogDebug("SOOMLA/UNITY StoreInfo", "the metadata-economy json (from DB) is " + value);
			JSONObject storeJSON = new JSONObject(value);
			fromJSONObject(storeJSON);
		}

		private static void fromJSONObject(JSONObject storeJSON)
		{
			VirtualItems = new Dictionary<string, VirtualItem>();
			PurchasableItems = new Dictionary<string, PurchasableVirtualItem>();
			GoodsCategories = new Dictionary<string, VirtualCategory>();
			GoodsUpgrades = new Dictionary<string, List<UpgradeVG>>();
			CurrencyPacks = new List<VirtualCurrencyPack>();
			Goods = new List<VirtualGood>();
			Categories = new List<VirtualCategory>();
			Currencies = new List<VirtualCurrency>();
			if (storeJSON.HasField("currencies"))
			{
				List<JSONObject> list = storeJSON["currencies"].list;
				foreach (JSONObject item9 in list)
				{
					VirtualCurrency item = new VirtualCurrency(item9);
					Currencies.Add(item);
				}
			}
			if (storeJSON.HasField("currencyPacks"))
			{
				List<JSONObject> list2 = storeJSON["currencyPacks"].list;
				foreach (JSONObject item10 in list2)
				{
					VirtualCurrencyPack item2 = new VirtualCurrencyPack(item10);
					CurrencyPacks.Add(item2);
				}
			}
			if (storeJSON.HasField("goods"))
			{
				JSONObject jSONObject = storeJSON["goods"];
				if (jSONObject.HasField("singleUse"))
				{
					List<JSONObject> list3 = jSONObject["singleUse"].list;
					foreach (JSONObject item11 in list3)
					{
						SingleUseVG item3 = new SingleUseVG(item11);
						Goods.Add(item3);
					}
				}
				if (jSONObject.HasField("lifetime"))
				{
					List<JSONObject> list4 = jSONObject["lifetime"].list;
					foreach (JSONObject item12 in list4)
					{
						LifetimeVG item4 = new LifetimeVG(item12);
						Goods.Add(item4);
					}
				}
				if (jSONObject.HasField("equippable"))
				{
					List<JSONObject> list5 = jSONObject["equippable"].list;
					foreach (JSONObject item13 in list5)
					{
						EquippableVG item5 = new EquippableVG(item13);
						Goods.Add(item5);
					}
				}
				if (jSONObject.HasField("goodPacks"))
				{
					List<JSONObject> list6 = jSONObject["goodPacks"].list;
					foreach (JSONObject item14 in list6)
					{
						SingleUsePackVG item6 = new SingleUsePackVG(item14);
						Goods.Add(item6);
					}
				}
				if (jSONObject.HasField("goodUpgrades"))
				{
					List<JSONObject> list7 = jSONObject["goodUpgrades"].list;
					foreach (JSONObject item15 in list7)
					{
						UpgradeVG item7 = new UpgradeVG(item15);
						Goods.Add(item7);
					}
				}
			}
			if (storeJSON.HasField("categories"))
			{
				List<JSONObject> list8 = storeJSON["categories"].list;
				foreach (JSONObject item16 in list8)
				{
					VirtualCategory item8 = new VirtualCategory(item16);
					Categories.Add(item8);
				}
			}
			updateAggregatedLists();
		}

		private static void updateAggregatedLists()
		{
			foreach (VirtualCurrency currency in Currencies)
			{
				VirtualItems.AddOrUpdate(currency.ItemId, currency);
			}
			foreach (VirtualCurrencyPack currencyPack in CurrencyPacks)
			{
				VirtualItems.AddOrUpdate(currencyPack.ItemId, currencyPack);
				PurchaseType purchaseType = currencyPack.PurchaseType;
				if (purchaseType is PurchaseWithMarket)
				{
					PurchasableItems.AddOrUpdate(((PurchaseWithMarket)purchaseType).MarketItem.ProductId, currencyPack);
				}
			}
			foreach (VirtualGood good in Goods)
			{
				VirtualItems.AddOrUpdate(good.ItemId, good);
				if (good is UpgradeVG)
				{
					if (!GoodsUpgrades.TryGetValue(((UpgradeVG)good).GoodItemId, out List<UpgradeVG> value))
					{
						value = new List<UpgradeVG>();
						GoodsUpgrades.Add(((UpgradeVG)good).GoodItemId, value);
					}
					value.Add((UpgradeVG)good);
				}
				PurchaseType purchaseType2 = good.PurchaseType;
				if (purchaseType2 is PurchaseWithMarket)
				{
					PurchasableItems.AddOrUpdate(((PurchaseWithMarket)purchaseType2).MarketItem.ProductId, good);
				}
			}
			foreach (VirtualCategory category in Categories)
			{
				foreach (string goodItemId in category.GoodItemIds)
				{
					GoodsCategories.AddOrUpdate(goodItemId, category);
				}
			}
		}

		private static void replaceVirtualItem(VirtualItem virtualItem)
		{
			VirtualItems.AddOrUpdate(virtualItem.ItemId, virtualItem);
			if (virtualItem is VirtualCurrency)
			{
				for (int i = 0; i < Currencies.Count(); i++)
				{
					if (Currencies[i].ItemId == virtualItem.ItemId)
					{
						Currencies.RemoveAt(i);
						break;
					}
				}
				Currencies.Add((VirtualCurrency)virtualItem);
			}
			if (virtualItem is VirtualCurrencyPack)
			{
				VirtualCurrencyPack virtualCurrencyPack = (VirtualCurrencyPack)virtualItem;
				if (virtualCurrencyPack.PurchaseType is PurchaseWithMarket)
				{
					PurchasableItems.AddOrUpdate(((PurchaseWithMarket)virtualCurrencyPack.PurchaseType).MarketItem.ProductId, virtualCurrencyPack);
				}
				for (int j = 0; j < CurrencyPacks.Count(); j++)
				{
					if (CurrencyPacks[j].ItemId == virtualCurrencyPack.ItemId)
					{
						CurrencyPacks.RemoveAt(j);
						break;
					}
				}
				CurrencyPacks.Add(virtualCurrencyPack);
			}
			if (!(virtualItem is VirtualGood))
			{
				return;
			}
			VirtualGood virtualGood = (VirtualGood)virtualItem;
			if (virtualGood is UpgradeVG)
			{
				if (!GoodsUpgrades.TryGetValue(((UpgradeVG)virtualGood).GoodItemId, out List<UpgradeVG> value))
				{
					value = new List<UpgradeVG>();
					GoodsUpgrades.Add(((UpgradeVG)virtualGood).ItemId, value);
				}
				value.Add((UpgradeVG)virtualGood);
			}
			if (virtualGood.PurchaseType is PurchaseWithMarket)
			{
				PurchasableItems.AddOrUpdate(((PurchaseWithMarket)virtualGood.PurchaseType).MarketItem.ProductId, virtualGood);
			}
			for (int k = 0; k < Goods.Count(); k++)
			{
				if (Goods[k].ItemId == virtualGood.ItemId)
				{
					Goods.RemoveAt(k);
					break;
				}
			}
			Goods.Add(virtualGood);
		}

		private static JSONObject toJSONObject()
		{
			JSONObject jSONObject = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualCurrency currency in Currencies)
			{
				jSONObject.Add(currency.toJSONObject());
			}
			JSONObject jSONObject2 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualCurrencyPack currencyPack in CurrencyPacks)
			{
				jSONObject2.Add(currencyPack.toJSONObject());
			}
			JSONObject jSONObject3 = new JSONObject();
			JSONObject jSONObject4 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jSONObject5 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jSONObject6 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jSONObject7 = new JSONObject(JSONObject.Type.ARRAY);
			JSONObject jSONObject8 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualGood good in Goods)
			{
				if (good is SingleUseVG)
				{
					jSONObject4.Add(good.toJSONObject());
				}
				else if (good is UpgradeVG)
				{
					jSONObject8.Add(good.toJSONObject());
				}
				else if (good is EquippableVG)
				{
					jSONObject6.Add(good.toJSONObject());
				}
				else if (good is SingleUsePackVG)
				{
					jSONObject7.Add(good.toJSONObject());
				}
				else if (good is LifetimeVG)
				{
					jSONObject5.Add(good.toJSONObject());
				}
			}
			JSONObject jSONObject9 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (VirtualCategory category in Categories)
			{
				jSONObject9.Add(category.toJSONObject());
			}
			JSONObject jSONObject10 = new JSONObject();
			jSONObject3.AddField("singleUse", jSONObject4);
			jSONObject3.AddField("lifetime", jSONObject5);
			jSONObject3.AddField("equippable", jSONObject6);
			jSONObject3.AddField("goodPacks", jSONObject7);
			jSONObject3.AddField("goodUpgrades", jSONObject8);
			jSONObject10.AddField("categories", jSONObject9);
			jSONObject10.AddField("currencies", jSONObject);
			jSONObject10.AddField("goods", jSONObject3);
			jSONObject10.AddField("currencyPacks", jSONObject2);
			return jSONObject10;
		}

		private static string keyMetaStoreInfo()
		{
			return "meta.storeinfo";
		}
	}
}
