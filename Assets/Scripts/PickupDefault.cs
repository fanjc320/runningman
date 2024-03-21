using System;
using System.Collections.Generic;
using UnityEngine;

public class PickupDefault : MonoBehaviour
{
	public static HashSet<PickupDefault> ActivatedPickups = new HashSet<PickupDefault>();

	public MeshRenderer meshRenderer;

	public Glow glow;

	public bool ShouldSpawnParticles;

	private Collider parentCollider;

	private void Awake()
	{
		Pickup component = GetComponent<Pickup>();
		Pickup pickup = component;
		pickup.OnPickup = (Pickup.OnPickupDelegate)Delegate.Combine(pickup.OnPickup, new Pickup.OnPickupDelegate(OnPickup));
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		TrackObject trackObject2 = trackObject;
		trackObject2.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject2.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		TrackObject trackObject3 = trackObject;
		trackObject3.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(trackObject3.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
		parentCollider = FindParentCollider(base.transform);
		if (parentCollider == null)
		{
		}
		SetVisible(visible: false);
	}

	private Collider FindParentCollider(Transform current)
	{
		if (current.GetComponent<Collider>() != null)
		{
			return current.GetComponent<Collider>();
		}
		if (current.parent != null)
		{
			return FindParentCollider(current.parent);
		}
		return null;
	}

	public void SetVisible(bool visible)
	{
		meshRenderer.enabled = visible;
		if (glow != null)
		{
			glow.SetVisible(visible);
		}
	}

	private void OnActivate()
	{
		try
		{
			ActivatedPickups.Add(this);
		}
		catch (Exception)
		{
		}
		parentCollider.enabled = true;
		SetVisible(visible: true);
	}

	public void OnDeactivate()
	{
		try
		{
			ActivatedPickups.Remove(this);
		}
		catch (Exception)
		{
		}
		parentCollider.enabled = false;
		SetVisible(visible: false);
	}

	private void OnPickup(CharacterPickupParticles particles)
	{
		if (base.gameObject != null)
		{
			parentCollider.enabled = false;
		}
		SetVisible(visible: false);
		if (ShouldSpawnParticles)
		{
			particles.PickedUpDefaultPowerUp();
		}
	}
}
