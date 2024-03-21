namespace Soomla.Store
{
	public abstract class VirtualItem : SoomlaEntity<VirtualItem>
	{
		private const string TAG = "SOOMLA VirtualItem";

		public string ItemId
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		protected VirtualItem(string name, string description, string itemId)
			: base(itemId, name, description)
		{
		}

		protected VirtualItem(JSONObject jsonItem)
			: base(jsonItem)
		{
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj.GetType() == GetType() && ((VirtualItem)obj).ItemId == ItemId;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public int Give(int amount)
		{
			return Give(amount, notify: true);
		}

		public abstract int Give(int amount, bool notify);

		public int Take(int amount)
		{
			return Take(amount, notify: true);
		}

		public abstract int Take(int amount, bool notify);

		public int ResetBalance(int balance)
		{
			return ResetBalance(balance, notify: true);
		}

		public abstract int ResetBalance(int balance, bool notify);

		public abstract int GetBalance();

		public void Save(bool saveToDB = true)
		{
			StoreInfo.Save(this, saveToDB);
		}
	}
}
