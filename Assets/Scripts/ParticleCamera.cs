using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ParticleCamera : MonoBehaviour
{
	private RenderTexture _renderTexture;

	private void Awake()
	{
		if (!GetComponent<Camera>().orthographic)
		{
		}
		GetComponent<Camera>().orthographicSize = Screen.height / 2;
		_renderTexture = new RenderTexture(Screen.width / 2, Screen.height / 2, 16);
		GetComponent<Camera>().targetTexture = _renderTexture;
		StartCoroutine(RenderParticles());
		base.gameObject.SetActive(value: false);
	}

	private IEnumerator RenderParticles()
	{
		while (true)
		{
			RenderTexture currentRT = RenderTexture.active;
			RenderTexture.active = GetComponent<Camera>().targetTexture;
			GetComponent<Camera>().Render();
			RenderTexture.active = currentRT;
			yield return new WaitForEndOfFrame();
		}
	}
}
