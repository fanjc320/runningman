using System;

namespace Soomla.Store
{
	public class MarketItem
	{
		public string ProductId;

		public double Price;

		public string MarketPriceAndCurrency;

		public string MarketTitle;

		public string MarketDescription;

		public string MarketCurrencyCode;

		public long MarketPriceMicros;

		public MarketItem(string productId, double price)
		{
			ProductId = productId;
			Price = price;
		}

		public MarketItem(JSONObject jsonObject)
		{
			string empty = string.Empty;
			empty = "androidId";
			if (!string.IsNullOrEmpty(empty) && jsonObject.HasField(empty))
			{
				ProductId = jsonObject[empty].str;
			}
			else
			{
				ProductId = jsonObject["productId"].str;
			}
			Price = jsonObject["price"].n;
			if ((bool)jsonObject["marketPrice"])
			{
				MarketPriceAndCurrency = jsonObject["marketPrice"].str;
			}
			else
			{
				MarketPriceAndCurrency = string.Empty;
			}
			if ((bool)jsonObject["marketTitle"])
			{
				MarketTitle = jsonObject["marketTitle"].str;
			}
			else
			{
				MarketTitle = string.Empty;
			}
			if ((bool)jsonObject["marketDesc"])
			{
				MarketDescription = jsonObject["marketDesc"].str;
			}
			else
			{
				MarketDescription = string.Empty;
			}
			if ((bool)jsonObject["marketCurrencyCode"])
			{
				MarketCurrencyCode = jsonObject["marketCurrencyCode"].str;
			}
			else
			{
				MarketCurrencyCode = string.Empty;
			}
			if ((bool)jsonObject["marketPriceMicros"])
			{
				MarketPriceMicros = Convert.ToInt64(jsonObject["marketPriceMicros"].n);
			}
			else
			{
				MarketPriceMicros = 0L;
			}
		}

		public JSONObject toJSONObject()
		{
			JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject.AddField("className", SoomlaUtils.GetClassName(this));
			jSONObject.AddField("productId", ProductId);
			jSONObject.AddField("price", (float)Price);
			jSONObject.AddField("marketPrice", MarketPriceAndCurrency);
			jSONObject.AddField("marketTitle", MarketTitle);
			jSONObject.AddField("marketDesc", MarketDescription);
			jSONObject.AddField("marketCurrencyCode", MarketCurrencyCode);
			jSONObject.AddField("marketPriceMicros", MarketPriceMicros);
			return jSONObject;
		}
	}
}
