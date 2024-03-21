using UnityEngine;

public class SSDebug : MonoBehaviour
{
	public Camera mainCamera;

	private void Start()
	{
		if (DeviceInfo.Instance.formFactor != DeviceInfo.FormFactor.iPad)
		{
		}
	}

	private void DisableCamera()
	{
		mainCamera.enabled = false;
	}

	private void EnableCamera()
	{
		mainCamera.enabled = true;
	}
}
