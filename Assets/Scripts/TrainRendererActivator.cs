using System;
using UnityEngine;

public class TrainRendererActivator : MonoBehaviour
{
	public bool isPrebuild;

	public Renderer[] renderers;

	public TrackObject trackObject;

	public void Awake()
	{
		if (!isPrebuild)
		{
			trackObject = (GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>());
			renderers = GetComponentsInChildren<Renderer>();
		}
		TrackObject obj = trackObject;
		obj.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(obj.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		TrackObject obj2 = trackObject;
		obj2.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(obj2.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
		OnDeactivate();
	}

	public void OnSpawn()
	{
		OnDeactivate();
	}

	public void OnActivate()
	{
		int num = renderers.Length;
		for (int i = 0; num > i; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = true;
			}
		}
	}

	public void OnDeactivate()
	{
		int num = renderers.Length;
		for (int i = 0; num > i; i++)
		{
			if (renderers[i] != null)
			{
				renderers[i].enabled = false;
			}
		}
	}
}
