using System;

namespace Soomla.Store
{
	public class EquippableVG : LifetimeVG
	{
		public sealed class EquippingModel
		{
			private readonly string name;

			private readonly int value;

			public static readonly EquippingModel LOCAL = new EquippingModel(0, "local");

			public static readonly EquippingModel CATEGORY = new EquippingModel(1, "category");

			public static readonly EquippingModel GLOBAL = new EquippingModel(2, "global");

			private EquippingModel(int value, string name)
			{
				this.name = name;
				this.value = value;
			}

			public override string ToString()
			{
				return name;
			}

			public int toInt()
			{
				return value;
			}
		}

		private static string TAG = "SOOMLA EquippableVG";

		public EquippingModel Equipping;

		public EquippableVG(EquippingModel equippingModel, string name, string description, string itemId, PurchaseType purchaseType)
			: base(name, description, itemId, purchaseType)
		{
			Equipping = equippingModel;
		}

		public EquippableVG(JSONObject jsonItem)
			: base(jsonItem)
		{
			switch (jsonItem["equipping"].str)
			{
			case "local":
				Equipping = EquippingModel.LOCAL;
				break;
			case "global":
				Equipping = EquippingModel.GLOBAL;
				break;
			default:
				Equipping = EquippingModel.CATEGORY;
				break;
			}
		}

		public override JSONObject toJSONObject()
		{
			JSONObject jSONObject = base.toJSONObject();
			jSONObject.AddField("equipping", Equipping.ToString());
			return jSONObject;
		}

		public void Equip()
		{
			Equip(notify: true);
		}

		public void Equip(bool notify)
		{
			if (VirtualGoodsStorage.GetBalance(this) > 0)
			{
				if (Equipping == EquippingModel.CATEGORY)
				{
					VirtualCategory virtualCategory = null;
					try
					{
						virtualCategory = StoreInfo.GetCategoryForVirtualGood(base.ItemId);
					}
					catch (VirtualItemNotFoundException)
					{
						SoomlaUtils.LogError(TAG, "Tried to unequip all other category VirtualGoods but there was no associated category. virtual good itemId: " + base.ItemId);
						return;
					}
					foreach (string goodItemId in virtualCategory.GoodItemIds)
					{
						EquippableVG equippableVG = null;
						try
						{
							equippableVG = (EquippableVG)StoreInfo.GetItemByItemId(goodItemId);
							if (equippableVG != null && equippableVG != this)
							{
								equippableVG.Unequip(notify);
							}
						}
						catch (VirtualItemNotFoundException)
						{
							SoomlaUtils.LogError(TAG, "On equip, couldn't find one of the itemIds in the category. Continuing to the next one. itemId: " + goodItemId);
						}
						catch (InvalidCastException)
						{
							SoomlaUtils.LogDebug(TAG, "On equip, an error occurred. It's a debug message b/c the VirtualGood may just not be an EquippableVG. itemId: " + goodItemId);
						}
					}
				}
				else if (Equipping == EquippingModel.GLOBAL)
				{
					foreach (VirtualGood good in StoreInfo.Goods)
					{
						if (good != this && good is EquippableVG)
						{
							((EquippableVG)good).Unequip(notify);
						}
					}
				}
				VirtualGoodsStorage.Equip(this, notify);
				return;
			}
			throw new NotEnoughGoodsException(base.ItemId);
		}

		public void Unequip()
		{
			Unequip(notify: true);
		}

		public void Unequip(bool notify)
		{
			VirtualGoodsStorage.UnEquip(this, notify);
		}
	}
}
