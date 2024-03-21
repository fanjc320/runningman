using UnityEngine;

public class LookAt : MonoBehaviour
{
	public Transform Target;

	public float Damping = 3.33f;

	protected void LateUpdate()
	{
		Quaternion b = Quaternion.LookRotation(base.transform.position - Target.position);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * Damping);
	}
}
