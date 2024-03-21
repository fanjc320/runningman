namespace Soomla.Store
{
	public class BillingSupportedEvent : SoomlaEvent
	{
		public BillingSupportedEvent()
			: this(null)
		{
		}

		public BillingSupportedEvent(object sender)
			: base(sender)
		{
		}
	}
}
