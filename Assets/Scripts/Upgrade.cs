public struct Upgrade
{
	public StringID name;

	public StringID nameTwoLines;

	public StringID description;

	public StringID mysteryBoxDescription;

	public int numberOfTiers;

	public float[] durations;

	public int spawnProbability;

	public int minimumMeters;

	public int coinmagnetRange;

	public int[] pricesRaw;

	public int levelPriceMultiplyer;

	public string iconName;

	public StringID GetName()
	{
		if (!string.IsNullOrEmpty(Strings.Get(nameTwoLines)))
		{
			return nameTwoLines;
		}
		return name;
	}

	public int getPrice(int tier)
	{
		if (pricesRaw == null)
		{
			return -1;
		}
		return pricesRaw[tier] + levelPriceMultiplyer * 0;
	}
}
