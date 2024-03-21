using SerializableClass;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CVMissionContentCHCoin : CVMission
{
	public Text CoinText;

	public Button RewardBtn;

	public GridLayoutGroup CHIcons;

	public Sprite[] characterImages;

	private Image[] ActiveCHIncons;

	private void Awake()
	{
		ActiveCHIncons = new Image[DataContainer.Instance.CharacterIDTierByCID.Count];
		Enumerable.Range(1, DataContainer.Instance.CharacterIDTierByCID.Count).All(delegate(int s)
		{
			int num = s - 1;
			Transform child = CHIcons.transform.GetChild(num);
			string key = DataContainer.Instance.CharacterIDTierByCID[s.ToString()][0];
			CharacterInfoData characterInfoData = DataContainer.Instance.CharacterTableRaw[key];
			Image component = child.Find("BG").GetComponent<Image>();
			Sprite sprite = FindCharacterImageFromPath(characterInfoData.Chcoinimagepath);
			child.Find("Active").GetComponent<Image>().sprite = sprite;
			component.sprite = sprite;
			ActiveCHIncons[num] = child.Find("Active").GetComponent<Image>();
			return true;
		});
	}

	public override void SetData(CVMissionData newData)
	{
		base.SetData(newData);
		RefreshCellView();
	}

	public override void RefreshCellView()
	{
		MissionInfoData missionInfoData = DataContainer.Instance.MissionTableRaw[data.DataKey];
		int gemCount = missionInfoData.PresentAttribute["gem"];
		Action doRewardAction = delegate
		{
			CurrencyTypeMapInt currency;
			(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] + gemCount;
			Enumerable.Range(1, DataContainer.Instance.CharacterIDTierByCID.Count).All(delegate(int s)
			{
				PlayerInfo.Instance.MsnCollectableGolals[$"chcoins_{s}"] = 0;
				return true;
			});
		};
		CoinText.text = $"X{gemCount:D2}";
		Enumerable.Range(1, DataContainer.Instance.CharacterIDTierByCID.Count).All(delegate(int s)
		{
			bool active = 1 == PlayerInfo.Instance.MsnCollectableGolals[$"chcoins_{s}"];
			ActiveCHIncons[s - 1].gameObject.SetActive(active);
			return true;
		});
		bool flag = PlayerInfo.Instance.MsnCompleted[data.DataKey];
		bool flag2 = PlayerInfo.Instance.MsnRewarded[data.DataKey];
		RewardBtn.interactable = (!flag2 && flag);
		RewardBtn.onClick.RemoveAllListeners();
		RewardBtn.onClick.AddListener(delegate
		{
			Camera.main.GetComponent<AudioSource>().PlayOneShot(MenuUIManager.Instance.ClickAud);
			doRewardAction();
			PlayerInfo.Instance.MsnRewarded[data.DataKey] = true;
			PlayerInfo.Instance.MsnRewarded[data.DataKey] = false;
			RefreshCellView();
		});
	}

	private Sprite FindCharacterImageFromPath(string path)
	{
		int num = characterImages.Length;
		for (int i = 0; i < num; i++)
		{
			if (characterImages[i].name == path.Remove(0, 7))
			{
				return characterImages[i];
			}
		}
		return null;
	}
}
