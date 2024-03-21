namespace Soomla
{
	public class MarketItemsRefreshFailedEvent : SoomlaEvent
	{
		public string ErrorMessage;

		public MarketItemsRefreshFailedEvent(string errorMessage)
			: this(errorMessage, null)
		{
		}

		public MarketItemsRefreshFailedEvent(string errorMessage, object sender)
			: base(sender)
		{
			ErrorMessage = errorMessage;
		}
	}
}
