namespace Soomla.Store
{
	public abstract class PurchaseType
	{
		public PurchasableVirtualItem AssociatedItem;

		public PurchaseType()
		{
		}

		public abstract void Buy(string payload);

		public abstract bool CanAfford();

		public abstract string GetPrice();
	}
}
