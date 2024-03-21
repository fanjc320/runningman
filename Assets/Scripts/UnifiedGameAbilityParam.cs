using System;
using System.Collections.Generic;
using System.Linq;

public class UnifiedGameAbilityParam
{
	public const string kGuard = "guard";

	public const string kRGaugeMultiplier = "rgaugemultiplier";

	public const string kMagnet = "magnet";

	public const string kDoubleCoin = "doublecoin";

	public const string kFillRGauge = "fillrgauge";

	public const string kGoldBonus = "goldbonus";

	public const string kDoubleJump = "doublejump";

	public const string kIgnoreConfusion = "ignoreconfusion";

	public const string kProtectNameSticker = "protectnamesticker";

	public const string kRevive = "revive";

	public const string kConfusion = "confusion";

	private Dictionary<string, GameAbilityParam> itemParams = new Dictionary<string, GameAbilityParam>
	{
		{
			"guard",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kGuard"
				},
				{
					"desc_loc",
					"kGuard"
				},
				{
					"numbers",
					-1
				}
			}
		},
		{
			"rgaugemultiplier",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kRGaugeMultiplier"
				},
				{
					"desc_loc",
					"kRGaugeMultiplier"
				},
				{
					"multiplyratio",
					-1f
				}
			}
		},
		{
			"magnet",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kMagnet"
				},
				{
					"desc_loc",
					"kMagnet"
				}
			}
		},
		{
			"doublecoin",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kDoubleCoin"
				},
				{
					"desc_loc",
					"kDoubleCoin"
				}
			}
		},
		{
			"fillrgauge",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kStartFever"
				},
				{
					"desc_loc",
					"kStartFever"
				},
				{
					"amount",
					-1f
				}
			}
		},
		{
			"goldbonus",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kEndGoldBonus"
				},
				{
					"desc_loc",
					"kEndGoldBonus"
				},
				{
					"bonusratio",
					-1f
				}
			}
		},
		{
			"doublejump",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kDoubleJump"
				},
				{
					"desc_loc",
					"kDoubleJump"
				},
				{
					"numbers",
					-1
				}
			}
		},
		{
			"ignoreconfusion",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kIgnoreConfusion"
				},
				{
					"desc_loc",
					"kIgnoreConfusion"
				},
				{
					"numbers",
					-1
				}
			}
		},
		{
			"protectnamesticker",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kProtectNameSticker"
				},
				{
					"desc_loc",
					"kProtectNameSticker"
				},
				{
					"numbers",
					-1
				}
			}
		},
		{
			"revive",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kRevive"
				},
				{
					"desc_loc",
					"kRevive"
				},
				{
					"ispreview",
					-1
				}
			}
		},
		{
			"confusion",
			new GameAbilityParam
			{
				{
					"name_loc",
					"kConfusion"
				},
				{
					"desc_loc",
					"kConfusion"
				},
				{
					"duration",
					-1f
				}
			}
		}
	};

	private ActiveGameAbilityParam[] activeParams = new ActiveGameAbilityParam[Enum.GetNames(typeof(ActiveGameAbilityType)).Length];

	public Dictionary<string, GameAbilityParam> ItemParams => itemParams;

	public void Activate(ActiveGameAbilityType type, string itemName, List<KeyValuePair<string, object>> variables, WAttributeVariable<object>.EventValueChanged valueChanged)
	{
		activeParams[(int)type] = (activeParams[(int)type] ?? new ActiveGameAbilityParam());
		if (!activeParams[(int)type].abilities.ContainsKey(itemName))
		{
			activeParams[(int)type].abilities[itemName] = new GameAbilityParam(itemParams[itemName].ToDictionary((KeyValuePair<string, object> k) => k.Key, (KeyValuePair<string, object> v) => v.Value));
		}
		GameAbilityParam gameAbilityParam = activeParams[(int)type].abilities[itemName];
		for (int i = 0; variables.Count > i; i++)
		{
			WAttributeVariable<object> wAttributeVariable = new WAttributeVariable<object>(gameAbilityParam, variables[i].Key);
			if (i == 0)
			{
				wAttributeVariable.ValueChanged += valueChanged;
			}
			wAttributeVariable.Value = gameAbilityParam[variables[i].Key];
			activeParams[(int)type].variables.Add(wAttributeVariable);
		}
	}

	public void TriggerActivated(ActiveGameAbilityType type, string itemName)
	{
	}
}
