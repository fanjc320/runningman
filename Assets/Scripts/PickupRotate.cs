using System;
using UnityEngine;

public class PickupRotate : MonoBehaviour
{
	public Transform target;

	public float speed = 180f;

	public float rotatePhase = 0.9f;

	private float z;

	public TrackObject trackObject;

	public VisibleObject visibleObject;

	private bool isAwake;

	public void Awake()
	{
		TrackObject obj = trackObject;
		obj.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(obj.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		if (visibleObject != null)
		{
			VisibleObject obj2 = visibleObject;
			obj2.OnVisibleChange = (VisibleObject.OnVisibleChangeDelegate)Delegate.Combine(obj2.OnVisibleChange, (VisibleObject.OnVisibleChangeDelegate)delegate(bool visible)
			{
				base.enabled = visible;
			});
		}
		base.enabled = false;
		isAwake = true;
	}

	private void OnActivate()
	{
		Vector3 position = base.transform.position;
		z = position.z;
		base.enabled = true;
	}

	private void OnEnable()
	{
		CoinPool.Instance.ActiveRotatingPickups.Add(this);
	}

	private void OnDisable()
	{
		if (isAwake && null != CoinPool.instance)
		{
			CoinPool.Instance.ActiveRotatingPickups.Remove(this);
		}
	}

	public void PhasedRotate()
	{
		target.localRotation = Quaternion.AngleAxis(Time.time * speed + z * rotatePhase, Vector3.up);
	}
}
