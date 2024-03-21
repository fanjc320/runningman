using SerializableClass;

public static class SaveMeManager
{
	private static int _reviveAmount = 1;

	private const string SAVE_ME_LOCALES_KEY = "save_me_locales";

	public static bool IS_PURCHASE_MADE_FROM_INGAME;

	public static bool IS_PURCHASE_RUNNING_INGAME;

	public static void IncreasingGems()
	{
		if (_reviveAmount <= 0)
		{
			_reviveAmount = 1;
		}
		else
		{
			_reviveAmount *= 2;
		}
	}

	public static int GetReviveAmount()
	{
		return _reviveAmount;
	}

	public static void ResetSaveMeForNewRun()
	{
		_reviveAmount = 1;
	}

	public static void SkipReviveIfPurchaseFailed()
	{
	}

	public static void SendReviveIfPurchaseSucceeded()
	{
		int num = PlayerInfo.Instance.Currency[CurrencyType.Gem];
		if (num - GetReviveAmount() >= 0)
		{
			PlayerInfo.Instance.Currency[CurrencyType.Gem] = num - GetReviveAmount();
		}
		IS_PURCHASE_MADE_FROM_INGAME = false;
		IS_PURCHASE_RUNNING_INGAME = false;
		Revive.Instance.SendRevive();
		IncreasingGems();
	}
}
