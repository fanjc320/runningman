using UnityEngine;

public class Clouds : MonoBehaviour
{
	public float skyLength = 1700f;

	public float cloudDistance = 200f;

	public int numberOfClouds = 10;

	public float cloudSize = 50f;

	public GameObject cloudPrefab;

	private void Start()
	{
		for (int i = 0; i < numberOfClouds; i++)
		{
			float d = skyLength * (float)i / (float)numberOfClouds;
			GameObject gameObject = UnityEngine.Object.Instantiate(cloudPrefab);
			Vector3 position = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-45f, 45f)) * (Vector3.up * cloudDistance + Vector3.forward * d);
			gameObject.transform.position = position;
			gameObject.transform.localScale = Vector3.one * cloudSize;
		}
	}
}
