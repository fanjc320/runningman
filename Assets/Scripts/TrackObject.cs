using System;
using UnityEngine;

[Serializable]
public class TrackObject : MonoBehaviour
{
	public delegate void OnActivateDelegate();

	public delegate void OnDeactivateDelegate();

	public OnActivateDelegate OnActivate;

	public OnDeactivateDelegate OnDeactivate;

	public void Awake()
	{
	}

	public void Activate()
	{
		if (OnActivate != null)
		{
			OnActivate();
		}
	}

	public void Deactivate()
	{
		if (OnDeactivate != null)
		{
			OnDeactivate();
		}
	}
}
