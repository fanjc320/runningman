using UnityEngine;

public class coinGizmo : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(base.transform.position, 3f);
	}
}
