using System;
using UnityEngine;

public class CoinPlaceholder : MonoBehaviour
{
	private Transform coin;

	private void Awake()
	{
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		TrackObject trackObject2 = trackObject;
		trackObject2.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject2.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		TrackObject trackObject3 = trackObject;
		trackObject3.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(trackObject3.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
	}

	public void OnActivate()
	{
		coin = CoinPool.Instance.GetCoin();
		coin.parent = base.transform;
		coin.position = base.transform.position;
		TrackObject component = coin.GetComponent<TrackObject>();
		component.Activate();
	}

	public void OnDeactivate()
	{
		if (coin != null)
		{
			TrackObject component = coin.GetComponent<TrackObject>();
			component.Deactivate();
			CoinPool.Instance.Put(coin);
			coin = null;
		}
	}
}
