using UnityEngine;

public class ParticleFollow : MonoBehaviour
{
	public Transform Target;

	public float TweenTime;

	public float SineOffset;

	public float SineSpeed;

	public float RotationTweenTime;

	private float tweenVelocity;

	private Vector3 baseRotation;

	private float tweenRotVelocity;

	private void Awake()
	{
		baseRotation = base.transform.localEulerAngles;
		base.gameObject.SetActive(value: false);
	}

	private void OnEnable()
	{
		tweenVelocity = 0f;
		tweenRotVelocity = 0f;
	}

	private void LateUpdate()
	{
		Vector3 position = Target.position;
		Vector3 position2 = base.transform.position;
		position.x = position2.x;
		float x = position.x;
		Vector3 position3 = Target.position;
		float num = Mathf.SmoothDamp(x, position3.x, ref tweenVelocity, TweenTime);
		if (!float.IsNaN(num))
		{
			position.x = num;
		}
		else
		{
			tweenVelocity = 0f;
			tweenRotVelocity = 0f;
		}
		base.transform.position = position;
		Vector3 localEulerAngles = baseRotation;
		float y = localEulerAngles.y;
		Vector3 localEulerAngles2 = Target.localEulerAngles;
		localEulerAngles.y = Mathf.SmoothDampAngle(y, localEulerAngles2.y, ref tweenRotVelocity, RotationTweenTime);
		float num2 = Mathf.Sin(SineOffset + Time.time * SineSpeed) * 5.5f;
		localEulerAngles.y += num2;
		base.transform.localEulerAngles = localEulerAngles;
	}
}
