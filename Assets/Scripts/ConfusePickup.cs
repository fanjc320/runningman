using System;
using UnityEngine;

public class ConfusePickup : MonoBehaviour
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
			if (PlayerInfo.Instance.StartItems[4] || PlayerInfo.Instance.CharacterSkills[6])
			{
				PPItemConfusion nParent = GameObjectPoolMT<PPItemConfusion>.Instance.GetNParent(Character.Instance.transform, null);
				MainUIManager.Instance.StartStartItemIconDirector(StartItemType.IgnoreConfuse);
			}
			else
			{
				Game.Instance.Modifiers.Add(Game.Instance.Modifiers.Confuse);
			}
			particles.PickedUpPowerUp();
			canPickup = false;
		}
	}
}
