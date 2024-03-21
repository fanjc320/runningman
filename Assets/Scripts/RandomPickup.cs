using System;
using System.Linq;
using UnityEngine;

public class RandomPickup : MonoBehaviour
{
	private PickupDefault thisPickupDefault;

	private bool canPickup;

	private const int PICKUP_COUNT = 5;

	private float[] pickupProb;

	public void Awake()
	{
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		TrackObject trackObject2 = trackObject;
		trackObject2.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject2.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		Pickup component = GetComponent<Pickup>();
		Pickup pickup = component;
		pickup.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(pickup.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
		string[] array = null;
		if (PlayerInfo.Instance.ThisGameType == GameType.MissionSingle)
		{
			array = new string[1]
			{
				"4"
			};
		}
		float num = 0f;
		MysteryItemInfoData[] dataArray = DataContainer.Instance.MysteryItemTableRaw.dataArray;
		int num2 = dataArray.Length;
		pickupProb = new float[num2];
		for (int i = 0; i < num2; i++)
		{
			MysteryItemInfoData mysteryItemInfoData = dataArray[i];
			if (array == null || !mysteryItemInfoData.ID.Equals("4"))
			{
				num += mysteryItemInfoData.Probability;
			}
		}
		for (int j = 0; j < num2; j++)
		{
			MysteryItemInfoData mysteryItemInfoData = dataArray[j];
			if (array != null && mysteryItemInfoData.ID.Equals("4"))
			{
				pickupProb[j] = 0f;
			}
			else
			{
				pickupProb[j] = mysteryItemInfoData.Probability / num;
			}
		}
	}

	private void OnActivate()
	{
		canPickup = true;
		if (!Game.Instance.IsInGame.Value)
		{
			PickupDefault obj = thisPickupDefault ?? GetComponentInParent<PickupDefault>();
			PickupDefault pickupDefault = obj;
			thisPickupDefault = obj;
			pickupDefault.SetVisible(visible: false);
		}
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		if (!Game.Instance.IsInGame.Value || !canPickup)
		{
			return;
		}
		float value = UnityEngine.Random.value;
		float num = 0f;
		int num2 = 0;
		int num3 = pickupProb.Length;
		for (num2 = 0; num3 > num2; num2++)
		{
			num += pickupProb[num2];
			if (num >= value)
			{
				break;
			}
		}
		switch (num2)
		{
		case 0:
			Game.Instance.Modifiers.Add(Game.Instance.Modifiers.Hoverboard);
			break;
		case 1:
		{
			DoubleCoinPickup component2 = GetComponent<DoubleCoinPickup>();
			component2.OnPickup(particles);
			break;
		}
		case 2:
		{
			CoinMagnetPickup component4 = GetComponent<CoinMagnetPickup>();
			component4.OnPickup(particles);
			break;
		}
		case 3:
		{
			SaveMeTokenPickup component3 = GetComponent<SaveMeTokenPickup>();
			component3.OnPickup(particles);
			break;
		}
		case 4:
		{
			PlayerInfo.Instance.StartItems[3] = true;
			PlayerInfo.Instance.StartItemCounts[3] += Mathf.RoundToInt((from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
				where s.ID == "6"
				select s).First().Pvalue);
			MainUIManager.Instance.StartBuffIcon(3, 0f);
			PPItemDoubleJump nParent = GameObjectPoolMT<PPItemDoubleJump>.Instance.GetNParent(Character.Instance.transform, null);
			break;
		}
		case 5:
		{
			ConfusePickup component = GetComponent<ConfusePickup>();
			component.OnPickup(particles);
			break;
		}
		}
		PlayerInfo.Instance.AccMissionByCondTypeID("getmystery", "-1", 1.ToString());
		canPickup = false;
	}
}
