using System;
using UnityEngine;

[RequireComponent(typeof(Pickup))]
public class CoinMagnetPickup : MonoBehaviour
{
	private Pickup pickup;

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
			Game.Instance.Modifiers.Add(Game.Instance.Modifiers.CoinMagnet);
			GameStats.Instance.coinMagnetsPickups++;
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}
}
