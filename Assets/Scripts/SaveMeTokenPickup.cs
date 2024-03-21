using System;
using System.Linq;
using UnityEngine;

public class SaveMeTokenPickup : MonoBehaviour
{
	private bool canPickup;

	private void Awake()
	{
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		TrackObject trackObject2 = trackObject;
		trackObject2.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject2.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
	}

	private void OnActivate()
	{
		canPickup = true;
	}

	public void OnPickup(CharacterPickupParticles particles)
	{
		if (canPickup)
		{
			GameStats.Instance.FeverGauge.Value += GameStats.Instance.FeverGauge.Maximum * (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
				where s.ID == "7"
				select s).First().Pvalue;
			PPItemRunningCoin nParent = GameObjectPoolMT<PPItemRunningCoin>.Instance.GetNParent(Character.Instance.transform, null);
			PPEffRunningGaugeUp nNoParent = GameObjectPoolMT<PPEffRunningGaugeUp>.Instance.GetNNoParent(null, null);
			particles.PickedUpPowerUp();
			GameStats.Instance.AddScoreForPickup(PowerupType.saveMeToken);
			canPickup = false;
		}
	}
}
