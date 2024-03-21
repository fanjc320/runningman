namespace Soomla
{
	public class SoomlaEvent
	{
		public readonly object Sender;

		public readonly string Payload;

		public SoomlaEvent()
		{
		}

		public SoomlaEvent(object sender)
			: this(sender, string.Empty)
		{
		}

		public SoomlaEvent(object sender, string payload)
		{
			Sender = sender;
			Payload = payload;
		}
	}
}
