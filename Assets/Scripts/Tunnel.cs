using UnityEngine;

public class Tunnel : MonoBehaviour
{
	private float tunnelLength;

	public AudioStateLoop audioStateLoop;

	private void Awake()
	{
		Vector3 size = GetComponent<Collider>().bounds.size;
		tunnelLength = size.z;
	}

	private void OnTriggerEnter(Collider collider)
	{
		if ("Player".Equals(collider.tag))
		{
			Game.Instance.Running.StartTunnel(tunnelLength);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if ("Player".Equals(collider.tag))
		{
			Game.Instance.Running.EndTunnel();
		}
	}
}
