using System.Collections.Generic;

namespace Soomla.Store
{
	public class VirtualCategory
	{
		private const string TAG = "SOOMLA VirtualCategory";

		public string Name;

		public List<string> GoodItemIds = new List<string>();

		public VirtualCategory(string name, List<string> goodItemIds)
		{
			Name = name;
			GoodItemIds = goodItemIds;
		}

		public VirtualCategory(JSONObject jsonItem)
		{
			Name = jsonItem["name"].str;
			JSONObject jSONObject = jsonItem["goods_itemIds"];
			foreach (JSONObject item in jSONObject.list)
			{
				GoodItemIds.Add(item.str);
			}
		}

		public JSONObject toJSONObject()
		{
			JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
			jSONObject.AddField("className", SoomlaUtils.GetClassName(this));
			jSONObject.AddField("name", Name);
			JSONObject jSONObject2 = new JSONObject(JSONObject.Type.ARRAY);
			foreach (string goodItemId in GoodItemIds)
			{
				jSONObject2.Add(goodItemId);
			}
			jSONObject.AddField("goods_itemIds", jSONObject2);
			return jSONObject;
		}
	}
}
