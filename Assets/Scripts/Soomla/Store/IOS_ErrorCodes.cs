namespace Soomla.Store
{
	public static class IOS_ErrorCodes
	{
		public static int NO_ERROR;

		public static int EXCEPTION_ITEM_NOT_FOUND = -101;

		public static int EXCEPTION_INSUFFICIENT_FUNDS = -102;

		public static int EXCEPTION_NOT_ENOUGH_GOODS = -103;

		public static void CheckAndThrowException(int error)
		{
			if (error == EXCEPTION_ITEM_NOT_FOUND)
			{
				throw new VirtualItemNotFoundException();
			}
			if (error == EXCEPTION_INSUFFICIENT_FUNDS)
			{
				throw new InsufficientFundsException();
			}
			if (error == EXCEPTION_NOT_ENOUGH_GOODS)
			{
				throw new NotEnoughGoodsException();
			}
		}
	}
}
