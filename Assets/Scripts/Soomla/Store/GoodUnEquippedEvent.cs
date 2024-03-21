namespace Soomla.Store
{
	public class GoodUnEquippedEvent : SoomlaEvent
	{
		private EquippableVG mItem;

		public GoodUnEquippedEvent(EquippableVG item)
			: this(item, null)
		{
		}

		public GoodUnEquippedEvent(EquippableVG item, object sender)
			: base(sender)
		{
			mItem = item;
		}

		public EquippableVG getGoodItem()
		{
			return mItem;
		}
	}
}
