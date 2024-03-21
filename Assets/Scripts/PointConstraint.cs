using UnityEngine;

public class PointConstraint : MonoBehaviour
{
	public Transform master;

	private Transform transformCached;

	private void Awake()
	{
		transformCached = base.transform;
	}

	private void LateUpdate()
	{
		Transform transform = transformCached;
		Vector3 position = master.position;
		float x = position.x;
		Vector3 position2 = master.position;
		transform.position = new Vector3(x, 0f, position2.z);
		Transform transform2 = transformCached;
		Vector3 localPosition = transformCached.localPosition;
		float x2 = localPosition.x;
		Vector3 localPosition2 = transformCached.localPosition;
		transform2.localPosition = new Vector3(x2, 0f, localPosition2.z);
	}
}
