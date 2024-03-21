using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ParticleLayer : MonoBehaviour
{
	[SerializeField]
	private Camera _particleCamera;

	private RawImage _particleImage;

	private void Awake()
	{
		if (_particleCamera == null)
		{
		}
		_particleImage = GetComponent<RawImage>();
		_particleImage.enabled = true;
	}

	private void LateUpdate()
	{
		_particleImage.texture = _particleCamera.targetTexture;
	}
}
