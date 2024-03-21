using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyLetterPickup : MonoBehaviour
{
	public Collider pickupCollider;

	public MeshRenderer meshRenderer;

	public Glow glow;

	public MeshFilter LetterMesh;

	public List<Mesh> Letters;

	public bool shouldSpawnParticles;

	private Pickup pickup;

	private char letter;

	public char Letter
	{
		set
		{
			letter = value;
			if (HasDailyLetter)
			{
				int num = letter - 65;
				if (num < Letters.Count && num >= 0)
				{
					LetterMesh.mesh = Letters[num];
				}
			}
			SetVisible(HasDailyLetter);
		}
	}

	private bool HasDailyLetter => letter != '\0';

	private void Awake()
	{
		pickup = GetComponent<Pickup>();
		Pickup obj = pickup;
		obj.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(obj.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
		DailyLetterPickupManager.Instance.InitializePickup(this);
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		TrackObject trackObject2 = trackObject;
		trackObject2.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject2.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		TrackObject trackObject3 = trackObject;
		trackObject3.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(trackObject3.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
	}

	private void OnActivate()
	{
		SetVisible(HasDailyLetter);
	}

	private void OnDeactivate()
	{
		SetVisible(visible: false);
	}

	private void SetVisible(bool visible)
	{
		pickupCollider.enabled = visible;
		meshRenderer.enabled = visible;
		if (glow != null)
		{
			glow.SetVisible(visible);
		}
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		StartCoroutine(PickupCoroutine(particles));
	}

	private IEnumerator PickupCoroutine(CharacterPickupParticles particles)
	{
		SetVisible(visible: false);
		GameStats.Instance.AddScoreForPickup(PowerupType.letters);
		PlayerInfo.Instance.PickedupLetter(letter);
		particles.PickedUpPowerUp();
		GameStats.Instance.letterPickups++;
		yield return new WaitForSeconds(2f);
		DailyLetterPickupManager.Instance.UpdateLetter();
	}
}
