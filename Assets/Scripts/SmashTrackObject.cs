using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SmashTrackObject : MonoBehaviour
{
	private const float kWeightOffsetY = -17f;

	private const float kSmashForce = 22f;

	private const float kSmashGravity = 0f;

	private const float kSmashMass = 0.1f;

	private const float kSmashDamp = 0.9f;

	private const float kSmashPeriod = 2f;

	private const float kSmashRotScalar = 2f;

	private const float kMinForce = 0.01f;

	public AudioClip AudCrash;

	private AudioSource AudSrc;

	private IEnumerator Start()
	{
		yield return 0;
		AudSrc = Camera.main.GetComponent<AudioSource>();
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.layer == 10 || collider.gameObject.layer == 13)
		{
			Transform transform = null;
			try
			{
				TrackObject componentInParent = collider.GetComponentInParent<TrackObject>();
				transform = ((!(null != componentInParent)) ? collider.transform : componentInParent.transform);
			}
			catch (Exception)
			{
			}
			Vector3 position = base.transform.position;
			position.y = -17f;
			StartCoroutine(SmashIt(transform, 22f * (transform.position - position).normalized, 0f * Vector3.down, 0.1f, 0.9f));
		}
	}

	private IEnumerator SmashIt(Transform target, Vector3 force, Vector3 gravity, float mass, float damping)
	{
		Quaternion originQuat = target.rotation;
		Vector3 originPos = target.position;
		Collider[] colls = target.GetComponentsInChildren<Collider>(includeInactive: true);
		if (colls != null && 0 < colls.Length)
		{
			colls.All(delegate(Collider coll)
			{
				coll.enabled = false;
				return true;
			});
		}
		AudSrc.PlayOneShot(AudCrash, UnityEngine.Random.Range(0.8f, 1.2f));
		Vector3 rotNormalized = UnityEngine.Random.insideUnitSphere;
		float rotScalar = 2f;
		Vector3 velocity2 = Vector3.zero;
		bool first = true;
		float elapsedTime = 0f - Time.deltaTime;
		while (first || 0.01f <= velocity2.magnitude)
		{
			force += gravity * mass;
			velocity2 += force / mass * Time.deltaTime;
			damping *= Mathf.Pow(damping, Time.deltaTime);
			velocity2 *= damping;
			target.position += velocity2;
			target.Rotate(rotNormalized, rotScalar);
			first = false;
			yield return 0;
			elapsedTime += Time.deltaTime;
			if (2f + (2f - 2f * (Game.Instance.currentSpeed / 270f)) < elapsedTime)
			{
				break;
			}
		}
		LateUpdater.Instance.AddAction(delegate
		{
			target.rotation = originQuat;
			target.position = originPos;
			if (colls != null && 0 < colls.Length)
			{
				colls.All(delegate(Collider coll)
				{
					coll.enabled = true;
					return true;
				});
			}
		});
	}
}
