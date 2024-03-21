using System;
using UnityEngine;

public class SuperSneakersPickup : MonoBehaviour
{
	private Game game;

	private bool canPickup;

	private void Awake()
	{
		game = Game.Instance;
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
			game.Modifiers.Add(game.Modifiers.SuperSneakes);
			GameStats.Instance.superSneakerPickups++;
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}
}
