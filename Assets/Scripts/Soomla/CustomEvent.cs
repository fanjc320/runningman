using System.Collections.Generic;

namespace Soomla
{
	public class CustomEvent : SoomlaEvent
	{
		private string Name;

		private Dictionary<string, string> Extra;

		public CustomEvent(string name, Dictionary<string, string> extra)
			: this(name, extra, null)
		{
		}

		public CustomEvent(string name, Dictionary<string, string> extra, object sender)
			: base(sender)
		{
			Name = name;
			Extra = extra;
		}

		public string GetName()
		{
			return Name;
		}

		public Dictionary<string, string> GetExtra()
		{
			return Extra;
		}
	}
}
