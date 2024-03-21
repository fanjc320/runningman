using System;
using UnityEngine;

public class RendererActivator : MonoBehaviour
{
	private Renderer thisRenderer;

	public void Awake()
	{
		TrackObject trackObject = GetComponent<TrackObject>() ?? base.gameObject.AddComponent<TrackObject>();
		TrackObject trackObject2 = trackObject;
		trackObject2.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject2.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		TrackObject trackObject3 = trackObject;
		trackObject3.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(trackObject3.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
		(thisRenderer = (thisRenderer ?? GetComponent<Renderer>())).enabled = false;
	}

	public void OnActivate()
	{
		Renderer obj = thisRenderer ?? GetComponent<Renderer>();
		Renderer renderer = obj;
		thisRenderer = obj;
		renderer.enabled = true;
	}

	public void OnDeactivate()
	{
		Renderer obj = thisRenderer ?? GetComponent<Renderer>();
		Renderer renderer = obj;
		thisRenderer = obj;
		renderer.enabled = false;
	}
}
