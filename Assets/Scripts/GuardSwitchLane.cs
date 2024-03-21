using UnityEngine;

public class GuardSwitchLane : MonoBehaviour
{
	private SmoothDampFloat smoothGuardX;

	public float guardXSmoothTime = 1f;

	public GameObject character;

	private Vector3 initPos;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 position = character.transform.position;
		Transform transform = base.gameObject.transform;
		float x = position.x;
		float y = position.y;
		Vector3 position2 = base.gameObject.transform.position;
		transform.position = new Vector3(x, y, position2.z);
	}
}
