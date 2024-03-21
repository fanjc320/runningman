using Soomla.Store;
using System.Collections.Generic;

public class StoreAsset : IStoreAssets
{
	public VirtualGood[] goods;

	public StoreAsset(MarketInfo marketTable)
	{
		string b = "and";
		List<VirtualGood> list = new List<VirtualGood>();
		MarketInfoData[] dataArray = marketTable.dataArray;
		foreach (MarketInfoData marketInfoData in dataArray)
		{
			if (marketInfoData.Type == b)
			{
				list.Add(new SingleUseVG(marketInfoData.Marketkey, string.Empty, marketInfoData.Shopinfoid, new PurchaseWithMarket(marketInfoData.Marketkey, 0.0)));
			}
		}
		goods = list.ToArray();
	}

	public VirtualCurrency[] GetCurrencies()
	{
		return new VirtualCurrency[0];
	}

	public VirtualGood[] GetGoods()
	{
		return goods;
	}

	public VirtualCurrencyPack[] GetCurrencyPacks()
	{
		return new VirtualCurrencyPack[0];
	}

	public VirtualCategory[] GetCategories()
	{
		return new VirtualCategory[0];
	}

	public int GetVersion()
	{
		return 0;
	}
}
