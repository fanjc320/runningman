using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackObject))]
public class MovingCoin : MonoBehaviour
{
	public float speed = 1f;

	private Transform coin;

	private Game game;

	private static CharacterController characterController;

	private static List<MovingCoin> activecoins = new List<MovingCoin>();

	private bool autoPilot;

	public static float autoPilotActivationDistance = 200f;

	public void Awake()
	{
		game = Game.Instance;
		if (!(game == null))
		{
			if (characterController == null)
			{
				characterController = Character.Instance.characterController;
			}
			if (base.transform.childCount == 0)
			{
			}
			coin = base.transform.GetChild(0);
			coin.localPosition = -Vector3.up * 200f;
			base.enabled = false;
			TrackObject component = GetComponent<TrackObject>();
			TrackObject trackObject = component;
			trackObject.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
			TrackObject trackObject2 = component;
			trackObject2.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(trackObject2.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
		}
	}

	public void OnActivate()
	{
		activecoins.Add(this);
		base.enabled = true;
		autoPilot = false;
		Transform transform = coin;
		Vector3 position = base.transform.position;
		float z = position.z;
		Vector3 position2 = characterController.transform.position;
		transform.localPosition = new Vector3(0f, 0f, (z - position2.z) * speed);
	}

	public void Update()
	{
		if (!(game == null))
		{
			if (autoPilot)
			{
				coin.position -= Vector3.forward * Time.deltaTime * game.currentSpeed * speed;
				return;
			}
			Vector3 position = base.transform.position;
			float z = position.z;
			Vector3 position2 = characterController.transform.position;
			Vector3 position3 = new Vector3(0f, 0f, (z - position2.z) * speed);
			Vector3 position4 = base.transform.TransformPoint(position3);
			coin.position = position4;
		}
	}

	public void OnDeactivate()
	{
		activecoins.Remove(this);
		base.enabled = false;
	}

	public void OnDrawGizmos()
	{
		if (coin != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(coin.position, base.transform.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position, 5f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(coin.position, 5f);
		}
	}

	public static void ActivateAutoPilot()
	{
		foreach (MovingCoin activecoin in activecoins)
		{
			Vector3 position = activecoin.GetComponent<Collider>().transform.position;
			float z = position.z;
			Vector3 position2 = characterController.transform.position;
			if (z - position2.z < autoPilotActivationDistance)
			{
				activecoin.autoPilot = true;
			}
		}
	}
}
