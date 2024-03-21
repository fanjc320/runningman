using System;
using UnityEngine;

public class DoubleCoinPickup : MonoBehaviour
{
	private bool canPickup;

	public void Awake()
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
			Game.Instance.Modifiers.Add(Game.Instance.Modifiers.DoubleCoin);
			GameStats.Instance.doubleCoinPickups++;
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}
}
