using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinLine : MonoBehaviour
{
	public float length = 100f;

	public float coinSpacing = 15f;

	public Vector3 fowardDir = Vector3.forward;

	private List<Transform> activeCoins = new List<Transform>();

	private void Awake()
	{
		TrackObject component = GetComponent<TrackObject>();
		TrackObject trackObject = component;
		trackObject.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		TrackObject trackObject2 = component;
		trackObject2.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(trackObject2.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
	}

	private void OnActivate()
	{
		if (Game.Instance.CharacterState == Game.Instance.Jetpack)
		{
			return;
		}
		for (float num = 0f; num < length; num += coinSpacing)
		{
			Transform coin = CoinPool.Instance.GetCoin();
			coin.parent = base.transform;
			coin.position = base.transform.position + base.transform.rotation * fowardDir * num;
			TrackObject component = coin.GetComponent<TrackObject>();
			if (component != null)
			{
				component.OnActivate();
			}
			activeCoins.Add(coin);
		}
	}

	private void OnDeactivate()
	{
		int count = activeCoins.Count;
		for (int i = 0; count > i; i++)
		{
			activeCoins[i].GetComponent<TrackObject>().OnDeactivate();
		}
		CoinPool.Instance.Put(activeCoins);
		activeCoins.Clear();
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.rotation * fowardDir * length);
		for (float num = 0f; num < length; num += coinSpacing)
		{
			Vector3 center = base.transform.position + base.transform.rotation * fowardDir * num;
			Gizmos.DrawSphere(center, 1f);
		}
	}
}
