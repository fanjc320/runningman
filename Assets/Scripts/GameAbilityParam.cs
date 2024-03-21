using System.Collections.Generic;

public class GameAbilityParam : Dictionary<string, object>
{
	public GameAbilityParam()
	{
	}

	public GameAbilityParam(IDictionary<string, object> dictionary)
		: base(dictionary)
	{
	}
}
