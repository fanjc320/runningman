using System.Collections.Generic;

public class ActiveGameAbilityParam
{
	public Dictionary<string, GameAbilityParam> abilities = new Dictionary<string, GameAbilityParam>();

	public List<WAttributeVariable<object>> variables = new List<WAttributeVariable<object>>();
}
