using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackObject))]
public class MovingTrain : MonoBehaviour
{
	public float speed = 1f;

	private Transform train;

	private BoxCollider trainCollider;

	public float trainCount = 3f;

	public static List<MovingTrain> activeTrains = new List<MovingTrain>();

	private bool autoPilot;

	public static float autoPilotActivationDistance = 200f;

	public AudioClip trianPassClip;

	private AudioSource trainPassSource;

	private bool isInitialized;

	private bool startSound;

	public void Awake()
	{
		if (base.transform.childCount == 0)
		{
		}
		train = base.transform.GetChild(0);
		train.localPosition = -Vector3.up * 200f;
		trainCollider = GetComponent<BoxCollider>();
		Vector3 size = trainCollider.size;
		trainCollider.size = new Vector3(size.x, size.y, size.z / (1f + speed));
		Vector3 center = trainCollider.center;
		trainCollider.center = new Vector3(0f, center.y, (30f * trainCount + 1f) / (1f + speed));
		base.enabled = false;
		TrackObject component = GetComponent<TrackObject>();
		TrackObject trackObject = component;
		trackObject.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		TrackObject trackObject2 = component;
		trackObject2.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(trackObject2.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
		trainPassSource = base.gameObject.AddComponent<AudioSource>();
		trainPassSource.minDistance = 20f;
		trainPassSource.maxDistance = 50f;
		trainPassSource.playOnAwake = false;
		trainPassSource.loop = true;
		trainPassSource.clip = trianPassClip;
	}

	public void OnActivate()
	{
		activeTrains.Add(this);
		base.enabled = true;
		autoPilot = false;
		Transform transform = train;
		Vector3 position = base.transform.position;
		float z = position.z;
		Vector3 position2 = Character.Instance.characterController.transform.position;
		transform.localPosition = new Vector3(0f, 0f, (z - position2.z) * speed);
		startSound = true;
	}

	public void Update()
	{
		if (startSound)
		{
			trainPassSource.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
			trainPassSource.volume = UnityEngine.Random.Range(0.1f, 0.6f);
			trainPassSource.timeSamples = UnityEngine.Random.Range(0, trainPassSource.timeSamples);
			trainPassSource.Play();
			startSound = false;
		}
		if (autoPilot)
		{
			train.position -= Vector3.forward * Time.deltaTime * Game.Instance.currentSpeed * speed;
			return;
		}
		Vector3 position = base.transform.position;
		float z = position.z;
		Vector3 position2 = Character.Instance.characterController.transform.position;
		Vector3 position3 = new Vector3(0f, 0f, (z - position2.z) * speed);
		Vector3 position4 = base.transform.TransformPoint(position3);
		train.position = position4;
	}

	public void OnDeactivate()
	{
		trainPassSource.Stop();
		activeTrains.Remove(this);
		base.enabled = false;
		train.transform.localPosition = -100f * Vector3.up;
	}

	public void OnDrawGizmos()
	{
		if (train != null)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawLine(train.position, base.transform.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(base.transform.position, 5f);
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(train.position, 5f);
		}
	}

	public static void ActivateAutoPilot()
	{
		foreach (MovingTrain activeTrain in activeTrains)
		{
			Vector3 min = activeTrain.GetComponent<Collider>().bounds.min;
			float z = min.z;
			Vector3 position = Character.Instance.characterController.transform.position;
			if (z - position.z < autoPilotActivationDistance)
			{
				activeTrain.autoPilot = true;
			}
		}
	}
}
