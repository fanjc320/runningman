using Lean;
using SerializableClass;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WReceiver : MonoBehaviour
{
	private static Queue<string> payloadList = new Queue<string>();

	private IEnumerator Start()
	{
		yield return 0;
		GetPayload();
		ProcReward();
	}

	public static int GetPayload()
	{
		int num = 0;
		while (true)
		{
			string customPayload = FirebasePlugin.GetCustomPayload();
			if (!string.IsNullOrEmpty(customPayload))
			{
				payloadList.Enqueue(customPayload);
				num++;
				continue;
			}
			break;
		}
		return num;
	}

	public static void ProcReward()
	{
		MenuUIManager menuUI = MenuUIManager.Instance;
		if (menuUI == null || payloadList.Count <= 0)
		{
			return;
		}
		RewardPush rewardPush = null;
		string jsonString = payloadList.Dequeue();
		try
		{
			rewardPush = RewardPush.CreateFromJSON(jsonString);
		}
		catch (Exception)
		{
		}
		if (rewardPush == null)
		{
			return;
		}
		int value = rewardPush.value;
		string translationText;
		if (rewardPush.type == "gem")
		{
			translationText = LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[1]);
			int num = PlayerInfo.Instance.Currency[CurrencyType.Gem];
			CurrencyTypeMapInt currency;
			(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] + value;
			menuUI.gemText.text = num.ToString();
		}
		else if (rewardPush.type == "gold")
		{
			translationText = LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[0]);
			int num2 = PlayerInfo.Instance.Currency[CurrencyType.Gold];
			CurrencyTypeMapInt currency;
			(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] + value;
			menuUI.goldText.text = num2.ToString();
		}
		else
		{
			if (!(rewardPush.type == "ticket"))
			{
				if (payloadList.Count > 0)
				{
					ProcReward();
				}
				return;
			}
			translationText = LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[2]);
			int nameTagCount = PlayerInfo.Instance.NameTagCount;
			PlayerInfo.Instance.NameTagCount += value;
			menuUI.nameTagText.text = $"{nameTagCount.ToString():D}";
		}
		string value2 = string.Format(rewardPush.msg + "\n" + LeanLocalization.GetTranslationText("156"), translationText, (value != 0) ? value.ToString() : string.Empty);
		Action value3 = delegate
		{
			menuUI.ClosePopupCommon();
			PlayerInfo.Instance.Currency[CurrencyType.Gem] = PlayerInfo.Instance.Currency[CurrencyType.Gem];
			PlayerInfo.Instance.Currency[CurrencyType.Gold] = PlayerInfo.Instance.Currency[CurrencyType.Gold];
			PlayerInfo.Instance.NameTagCount = PlayerInfo.Instance.NameTagCount;
			if (payloadList.Count > 0)
			{
				ProcReward();
			}
		};
		menuUI.ShowPopupCommon(new Dictionary<string, object>
		{
			{
				"type",
				"Notify"
			},
			{
				"CloseHandler",
				value3
			},
			{
				"msg",
				value2
			}
		});
	}
}
