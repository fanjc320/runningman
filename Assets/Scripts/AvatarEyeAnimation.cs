using UnityEngine;

public class AvatarEyeAnimation : MonoBehaviour
{
	public bool animate;

	public Renderer closedEyes;

	public float blinkTime = 0.1f;

	public float blinkWaitTimeMax = 5.7f;

	public float blinkWaitTimeMin = 0.6f;

	private float blinkWaitEndTime;

	private float blinkEndTime;

	public Renderer animatedEyes;

	private Material animatedEyesMaterial;

	public float shiftEyeHorizontalLimit = 0.5f;

	private float currentHorizontalLimit;

	public float shiftEyeVerticalLimit = 0.5f;

	private float currentVerticalLimit;

	public float shiftEyeHorizontalMultiplier = 0.1f;

	public float shiftEyeVerticalMultiplier = 0.1f;

	public Transform headboneTransform;

	public GameObject testTargetPoint;

	public Vector2 maxLookOffset = new Vector2(10f, 10f);

	public Vector3 popupMenuFocusPointOffset = new Vector3(-40f, -80f, -200f);

	public Vector3 characterMenuFocusPointOffset = new Vector3(10f, -80f, -200f);

	public Vector3 boardMenuFocusPointOffset = new Vector3(10f, -80f, -200f);

	public GameObject targetPosition;

	private Vector3 originalPosition;

	private Vector3 originalRotation;

	private Vector3 newTargetPosition;

	private Vector3 lookToCamera;

	private Vector2 offsetVector = default(Vector2);

	public float timeToMoveFocus = 0.04f;

	public float timeBetweenFocusShiftMax = 1f;

	public float timeBetweenFocusShiftMin = 0.02f;

	private float endTimeToShift;

	private bool useFocuPoint = true;

	private bool shiftToFocusBlink;

	private void Start()
	{
		currentHorizontalLimit = shiftEyeHorizontalLimit;
		currentVerticalLimit = shiftEyeVerticalLimit;
		if (animatedEyes != null)
		{
			animatedEyesMaterial = animatedEyes.material;
		}
		blinkWaitEndTime = UnityEngine.Random.Range(blinkWaitTimeMin, blinkWaitTimeMax);
		blinkEndTime = blinkWaitEndTime + blinkTime;
		if (testTargetPoint != null)
		{
			targetPosition = testTargetPoint;
			targetPosition.transform.LookAt(base.transform.position);
		}
		else if (!(targetPosition != null))
		{
		}
	}

	private void OnDestroy()
	{
		if (targetPosition != null)
		{
			UnityEngine.Object.Destroy(targetPosition);
		}
	}

	public void StopAnimatingEyes()
	{
		animate = false;
	}

	private void ForceBlink()
	{
		blinkWaitEndTime = Time.time;
		blinkEndTime = blinkWaitEndTime + blinkTime;
	}

	private void UpdateEyeBlinking()
	{
		if (!(closedEyes != null) || !(blinkWaitTimeMax >= blinkWaitTimeMin) || !(Time.time >= blinkWaitEndTime))
		{
			return;
		}
		if (Time.time < blinkEndTime)
		{
			if (!closedEyes.GetComponent<Renderer>().enabled)
			{
				closedEyes.GetComponent<Renderer>().enabled = true;
			}
		}
		else
		{
			closedEyes.GetComponent<Renderer>().enabled = false;
			blinkWaitEndTime = Time.time + UnityEngine.Random.Range(blinkWaitTimeMin, blinkWaitTimeMax);
			blinkEndTime = blinkWaitEndTime + blinkTime;
		}
	}

	private Vector3 GetFocusPoint()
	{
		targetPosition.transform.position = originalPosition;
		targetPosition.transform.rotation = Quaternion.Euler(originalRotation);
		targetPosition.transform.Translate(UnityEngine.Random.Range(0f - maxLookOffset.x, maxLookOffset.x), UnityEngine.Random.Range(0f - maxLookOffset.y, maxLookOffset.y), 0f);
		return targetPosition.transform.position;
	}

	private void ShiftFocusPoint(Vector3 focusPoint)
	{
		Vector3 fromPosition = targetPosition.transform.position;
		StartCoroutine(pTween.To(timeToMoveFocus, delegate(float t)
		{
			targetPosition.transform.position = Vector3.Lerp(fromPosition, focusPoint, t);
		}));
	}

	private void ResetEyes()
	{
		offsetVector = new Vector2(0f, 0f);
		animatedEyesMaterial.SetTextureOffset("_MainTex", offsetVector);
	}

	private void UpdateGetNewFocusPoint()
	{
		if (Time.time > endTimeToShift)
		{
			ShiftFocusPoint(GetFocusPoint());
			endTimeToShift = Time.time + UnityEngine.Random.Range(timeBetweenFocusShiftMin, timeBetweenFocusShiftMax);
		}
	}

	private void UpdateAnimatedEyes()
	{
		if (!(animatedEyes != null) || !(animatedEyesMaterial != null) || !(headboneTransform != null))
		{
			return;
		}
		if (useFocuPoint)
		{
			UpdateGetNewFocusPoint();
		}
		lookToCamera = headboneTransform.position - targetPosition.transform.position;
		Vector3 vector = headboneTransform.InverseTransformDirection(lookToCamera);
		offsetVector.x = Mathf.Atan2(vector.x, vector.z);
		offsetVector.y = Mathf.Atan2(vector.y, vector.z);
		if (offsetVector.x < 0f - currentHorizontalLimit)
		{
			useFocuPoint = false;
		}
		else if (offsetVector.x > currentHorizontalLimit)
		{
			useFocuPoint = false;
		}
		else if (offsetVector.y < 0f - currentVerticalLimit)
		{
			useFocuPoint = false;
		}
		else if (offsetVector.y > currentVerticalLimit)
		{
			useFocuPoint = false;
		}
		else
		{
			useFocuPoint = true;
		}
		if (useFocuPoint)
		{
			if (!shiftToFocusBlink)
			{
				currentHorizontalLimit = shiftEyeHorizontalLimit;
				currentVerticalLimit = shiftEyeVerticalLimit;
				ForceBlink();
				shiftToFocusBlink = true;
			}
			offsetVector.x = Mathf.Clamp(offsetVector.x, 0f - currentHorizontalLimit, currentHorizontalLimit) * shiftEyeHorizontalMultiplier;
			offsetVector.y = Mathf.Clamp(offsetVector.y, 0f - currentVerticalLimit, currentVerticalLimit) * shiftEyeVerticalMultiplier;
			animatedEyesMaterial.SetTextureOffset("_MainTex", offsetVector);
		}
		else
		{
			ResetEyes();
			if (shiftToFocusBlink)
			{
				currentHorizontalLimit = shiftEyeHorizontalLimit - 0.1f;
				currentVerticalLimit = shiftEyeVerticalLimit - 0.1f;
				ForceBlink();
				shiftToFocusBlink = false;
			}
		}
	}

	private void Update()
	{
		if (animate)
		{
			UpdateEyeBlinking();
			if (animatedEyes != null)
			{
				if (!animatedEyes.enabled)
				{
					animatedEyes.enabled = true;
				}
				UpdateAnimatedEyes();
			}
		}
		else
		{
			closedEyes.enabled = false;
			if (animatedEyes != null)
			{
				animatedEyes.enabled = false;
			}
		}
	}
}
