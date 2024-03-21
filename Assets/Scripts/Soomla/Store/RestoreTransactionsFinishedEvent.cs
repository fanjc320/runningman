namespace Soomla.Store
{
	public class RestoreTransactionsFinishedEvent : SoomlaEvent
	{
		private bool mSuccess;

		public RestoreTransactionsFinishedEvent(bool success)
			: this(success, null)
		{
		}

		public RestoreTransactionsFinishedEvent(bool success, object sender)
			: base(sender)
		{
			mSuccess = success;
		}

		public bool isSuccess()
		{
			return mSuccess;
		}
	}
}
