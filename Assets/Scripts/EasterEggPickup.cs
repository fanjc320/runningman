using System;
using UnityEngine;

public class EasterEggPickup : MonoBehaviour
{
	private bool canPickup;

	private void Awake()
	{
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		TrackObject trackObject2 = trackObject;
		trackObject2.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject2.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		Pickup component = GetComponent<Pickup>();
		Pickup pickup = component;
		pickup.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(pickup.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
	}

	private void OnActivate()
	{
		canPickup = true;
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		if (canPickup)
		{
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}
}
