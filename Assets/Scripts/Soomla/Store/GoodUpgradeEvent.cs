namespace Soomla.Store
{
	public class GoodUpgradeEvent : SoomlaEvent
	{
		private VirtualGood mItem;

		private UpgradeVG mCurrentUpgradeItem;

		public GoodUpgradeEvent(VirtualGood item, UpgradeVG upgradeVGItem)
			: this(item, upgradeVGItem, null)
		{
		}

		public GoodUpgradeEvent(VirtualGood item, UpgradeVG upgradeVGItem, object sender)
			: base(sender)
		{
			mItem = item;
			mCurrentUpgradeItem = upgradeVGItem;
		}

		public VirtualGood getGoodItem()
		{
			return mItem;
		}

		public UpgradeVG getCurrentUpgrade()
		{
			return mCurrentUpgradeItem;
		}
	}
}
