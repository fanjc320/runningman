using System;

namespace Soomla.Store
{
	public class VirtualCurrencyPack : PurchasableVirtualItem
	{
		private static string TAG = "SOOMLA VirtualCurrencyPack";

		public int CurrencyAmount;

		public string CurrencyItemId;

		public VirtualCurrencyPack(string name, string description, string itemId, int currencyAmount, string currencyItemId, PurchaseType purchaseType)
			: base(name, description, itemId, purchaseType)
		{
			CurrencyAmount = currencyAmount;
			CurrencyItemId = currencyItemId;
		}

		public VirtualCurrencyPack(JSONObject jsonItem)
			: base(jsonItem)
		{
			CurrencyAmount = Convert.ToInt32(jsonItem["currency_amount"].n);
			CurrencyItemId = jsonItem["currency_itemId"].str;
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jSONObject = base.toJSONObject();
			jSONObject.AddField("currency_amount", CurrencyAmount);
			jSONObject.AddField("currency_itemId", CurrencyItemId);
			return jSONObject;
		}

		public override int Give(int amount, bool notify)
		{
			VirtualCurrency virtualCurrency = null;
			try
			{
				virtualCurrency = (VirtualCurrency)StoreInfo.GetItemByItemId(CurrencyItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(TAG, "VirtualCurrency with itemId: " + CurrencyItemId + " doesn't exist! Can't give this pack.");
				return 0;
			}
			return VirtualCurrencyStorage.Add(virtualCurrency, CurrencyAmount * amount, notify);
		}

		public override int Take(int amount, bool notify)
		{
			VirtualCurrency virtualCurrency = null;
			try
			{
				virtualCurrency = (VirtualCurrency)StoreInfo.GetItemByItemId(CurrencyItemId);
			}
			catch (VirtualItemNotFoundException)
			{
				SoomlaUtils.LogError(TAG, "VirtualCurrency with itemId: " + CurrencyItemId + " doesn't exist! Can't take this pack.");
				return 0;
			}
			return VirtualCurrencyStorage.Remove(virtualCurrency, CurrencyAmount * amount, notify);
		}

		public override int ResetBalance(int balance, bool notify)
		{
			SoomlaUtils.LogError(TAG, "Someone tried to reset balance of CurrencyPack. That's not right.");
			return 0;
		}

		public override int GetBalance()
		{
			SoomlaUtils.LogError(TAG, "Someone tried to check balance of CurrencyPack. That's not right.");
			return 0;
		}

		protected override bool canBuy()
		{
			return true;
		}
	}
}
