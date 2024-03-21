using Lean;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIViewAdButton : MonoBehaviour
{
	public GameObject TimerGO;

	public TextMeshProUGUI Timer;

	public Button ViewAdBtn;

	private void Start()
	{
		ViewAdBtn.onClick.AddListener(delegate
		{
			if (ADRewardManager.isLoadedAD())
			{
				ADRewardManager.ShowRewardAD(delegate
				{
					GoogleAnalyticsV4.getInstance().LogScreen("UNITYAD_VIEW_COMPLETE_NAMETAG");
					PlayerInfo.Instance.NameTagCount++;
					PlayerInfo.Instance.NametagViewADTimeTick = DateTime.Now.Ticks;
					StartCoroutine(CheckTime());
				});
			}
			else
			{
				Action value = delegate
				{
				};
				MenuUIManager.Instance.ShowPopupCommon(new Dictionary<string, object>
				{
					{
						"type",
						"Notify"
					},
					{
						"CloseHandler",
						value
					},
					{
						"msg",
						LeanLocalization.GetTranslationText("154")
					}
				});
			}
		});
	}

	private void OnEnable()
	{
		StartCoroutine(CheckTime());
	}

	private IEnumerator CheckTime()
	{
		ViewAdBtn.interactable = false;
		TimerGO.SetActive(value: true);
		long targetTick = 6000000000L + PlayerInfo.Instance.NametagViewADTimeTick;
		while (targetTick > DateTime.Now.Ticks)
		{
			TimeSpan span = TimeSpan.FromTicks(targetTick - DateTime.Now.Ticks);
			Timer.text = $"{span.Minutes:D2}:{span.Seconds:D2}";
			yield return new WaitForSeconds(0.5f);
		}
		TimerGO.SetActive(value: false);
		ViewAdBtn.interactable = true;
	}
}
