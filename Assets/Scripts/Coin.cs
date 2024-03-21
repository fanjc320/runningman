using System;
using UnityEngine;

[RequireComponent(typeof(Pickup))]
public class Coin : MonoBehaviour
{
	private Character character;

	private GameStats gameStats;

	private bool canPickup;

	public Transform pivot;

	private Vector3 initialPivotPosition;

	public GameObject glow;

	private Vector3 initialGlowPosition;

	public Renderer coinRenderer;

	public Renderer glowRenderer;

	public MeshFilter coinMeshFilter;

	private Coroutine colorTweening;

	public TrackObject trackObject;

	public Pickup pickup;

	public Transform PivotTransform => pivot;

	private void Awake()
	{
		character = Character.Instance;
		gameStats = GameStats.Instance;
		initialPivotPosition = pivot.localPosition;
		Pickup obj = pickup;
		obj.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(obj.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
		TrackObject obj2 = trackObject;
		obj2.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(obj2.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		if (glow != null)
		{
			initialGlowPosition = glow.transform.localPosition;
		}
	}

	private void OnPickup(CharacterPickupParticles pickupParticles)
	{
		if (canPickup)
		{
			if (GameStats.Instance.IsDoubleCoin)
			{
				gameStats.coins += 2;
			}
			else
			{
				gameStats.coins++;
			}
			if (character.IsAboveGround)
			{
				gameStats.coinsInAir++;
			}
			if (Jetpack.Instance.isActive)
			{
				gameStats.coinsWithJetpack++;
			}
			if (character.trackIndex == 0)
			{
				gameStats.coinsCollectedOnLeftTrack++;
			}
			else if (character.trackIndex == 1)
			{
				gameStats.coinsCollectedOnCenterTrack++;
			}
			else if (character.trackIndex == 2)
			{
				gameStats.coinsCollectedOnRightTrack++;
			}
			pickupParticles.PickedUpCoin(pickup);
			canPickup = false;
		}
	}

	private void OnActivate()
	{
		canPickup = true;
		pivot.localPosition = initialPivotPosition;
		if (glow != null)
		{
			glow.transform.localPosition = initialGlowPosition;
		}
	}
}
