using UnityEngine;

public class HackFix : MonoBehaviour
{
	private int originalMask;

	private void Awake()
	{
		originalMask = GetComponent<Camera>().cullingMask;
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.C))
		{
			GetComponent<Camera>().cullingMask = 2048;
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.V))
		{
			GetComponent<Camera>().cullingMask = originalMask;
		}
	}
}
