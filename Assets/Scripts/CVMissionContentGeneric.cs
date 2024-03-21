using Lean;
using SerializableClass;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CVMissionContentGeneric : CVMission
{
	private static string[] kRewardTypes = new string[3]
	{
		"gem",
		"gold",
		"nametag"
	};

	private static Dictionary<Tuple<string, string>, Func<string>> kConditionTypes = null;

	public Text NumberText;

	public Text ContentMsgText;

	public Image RewardCoinIcon;

	public Text RewardCoinText;

	public Button RewardBtn;

	public Text ConditionText;

	public Image CompletedIcon;

	private void Awake()
	{
		if (kConditionTypes == null)
		{
			Dictionary<Tuple<string, string>, Func<string>> dictionary = new Dictionary<Tuple<string, string>, Func<string>>();
			dictionary.Add(new Tuple<string, string>("buystartitem", "0"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.StartItemTableRaw[0].Name1loc));
			dictionary.Add(new Tuple<string, string>("buystartitem", "1"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.StartItemTableRaw[1].Name1loc));
			dictionary.Add(new Tuple<string, string>("buystartitem", "2"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.StartItemTableRaw[2].Name1loc));
			dictionary.Add(new Tuple<string, string>("buystartitem", "3"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.StartItemTableRaw[3].Name1loc));
			dictionary.Add(new Tuple<string, string>("buystartitem", "4"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.StartItemTableRaw[4].Name1loc));
			dictionary.Add(new Tuple<string, string>("buystartitem", "5"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.StartItemTableRaw[5].Name1loc));
			dictionary.Add(new Tuple<string, string>("buystartitem", "6"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.StartItemTableRaw[6].Name1loc));
			dictionary.Add(new Tuple<string, string>("buystartitem", "7"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.StartItemTableRaw[7].Name1loc));
			dictionary.Add(new Tuple<string, string>("playgamemode", "0"), () => LeanLocalization.GetTranslationText("39"));
			dictionary.Add(new Tuple<string, string>("playgamemode", "1"), () => LeanLocalization.GetTranslationText("40"));
			dictionary.Add(new Tuple<string, string>("playgamemode", "2"), () => LeanLocalization.GetTranslationText("41"));
			dictionary.Add(new Tuple<string, string>("getgold", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("runamount", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("upgradeparam", "0"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.PlayerParamTableRaw["0"].Name1loc));
			dictionary.Add(new Tuple<string, string>("upgradeparam", "1"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.PlayerParamTableRaw["1"].Name1loc));
			dictionary.Add(new Tuple<string, string>("upgradeparam", "2"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.PlayerParamTableRaw["2"].Name1loc));
			dictionary.Add(new Tuple<string, string>("upgradeparam", "3"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.PlayerParamTableRaw["3"].Name1loc));
			dictionary.Add(new Tuple<string, string>("upgradeparam", "4"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.PlayerParamTableRaw["4"].Name1loc));
			dictionary.Add(new Tuple<string, string>("upgradeparam", "5"), () => LeanLocalization.GetTranslationText(DataContainer.Instance.PlayerParamTableRaw["5"].Name1loc));
			dictionary.Add(new Tuple<string, string>("dofever", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("getmystery", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("doplucksticker", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("dogiftbox", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("dorandomlvlup", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("dojumpobstacle", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("dorollobstacle", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("launchgame", "-1"), () => string.Empty);
			dictionary.Add(new Tuple<string, string>("getchcoin", "-1"), () => string.Empty);
			kConditionTypes = dictionary;
		}
	}

	public override void RefreshCellView()
	{
		MissionInfoData missionInfoData = DataContainer.Instance.MissionTableRaw[data.DataKey];
		NumberText.text = $"{data.Index + 1}.";
		int rewardType = -1;
		int rewardValue = -1;
		for (int i = 0; kRewardTypes.Length > i; i++)
		{
			rewardType = i;
			rewardValue = missionInfoData.PresentAttribute[kRewardTypes[i]];
			if (0 < rewardValue)
			{
				RewardCoinIcon.sprite = MenuUIManager.Instance.CurrencySmallIcons[i];
				RewardCoinText.text = $"X{rewardValue:D4}";
				break;
			}
		}
		Action doRewardAction = delegate
		{
			if (rewardType != 0)
			{
				if (rewardType != 1)
				{
					if (rewardType == 2)
					{
						PlayerInfo.Instance.NameTagCount += rewardValue;
					}
				}
				else
				{
					CurrencyTypeMapInt currency;
					(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] + rewardValue;
				}
			}
			else
			{
				CurrencyTypeMapInt currency;
				(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] + rewardValue;
			}
		};
		ContentMsgText.text = string.Format(LeanLocalization.GetTranslationText(missionInfoData.Desc1loc), kConditionTypes[new Tuple<string, string>(missionInfoData.GoalConditions["type"], missionInfoData.GoalConditions["typeid"])](), missionInfoData.Goalvalue);
		bool flag = PlayerInfo.Instance.MsnCompleted[data.DataKey];
		bool flag2 = PlayerInfo.Instance.MsnRewarded[data.DataKey];
		RewardBtn.gameObject.SetActive(value: false);
		ConditionText.gameObject.SetActive(value: false);
		CompletedIcon.gameObject.SetActive(value: false);
		if (flag2)
		{
			CompletedIcon.gameObject.SetActive(value: true);
		}
		else if (flag)
		{
			RewardBtn.gameObject.SetActive(value: true);
			RewardBtn.interactable = true;
			RewardBtn.onClick.RemoveAllListeners();
			RewardBtn.onClick.AddListener(delegate
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(MenuUIManager.Instance.ClickAud);
				doRewardAction();
				PlayerInfo.Instance.MsnRewarded[data.DataKey] = true;
				PlayerInfo.Instance.MsnCompleted[data.DataKey] = false;
				RefreshCellView();
			});
		}
		else
		{
			ConditionText.gameObject.SetActive(value: true);
			bool flag3 = "number".Equals(missionInfoData.Goaltype);
			ConditionText.text = string.Format("{0:D3}{1}{2:D3}", (!flag3) ? Mathf.RoundToInt(float.Parse(PlayerInfo.Instance.MsnGoalValues[data.DataKey])) : int.Parse(PlayerInfo.Instance.MsnGoalValues[data.DataKey]), "/\n", (!flag3) ? Mathf.RoundToInt(float.Parse(missionInfoData.Goalvalue)) : int.Parse(missionInfoData.Goalvalue));
		}
	}

	public override void SetData(CVMissionData newData)
	{
		base.SetData(newData);
		RefreshCellView();
	}
}
