using UnityEngine;

public class JetPackCloud : MonoBehaviour
{
	public float scrollSpeed = 0.5f;

	private Material material;

	public float startOffset;

	private void Awake()
	{
		material = base.gameObject.GetComponent<Renderer>().material;
		material.mainTextureOffset = new Vector2(startOffset, 0f);
	}

	private void Update()
	{
		Vector2 mainTextureOffset = material.mainTextureOffset;
		float x = mainTextureOffset.x;
		x = (x + Time.deltaTime * scrollSpeed) % 1f;
		material.mainTextureOffset = new Vector2(x, 0f);
	}
}
