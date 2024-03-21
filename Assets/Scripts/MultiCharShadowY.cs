using UnityEngine;

public class MultiCharShadowY : MonoBehaviour
{
	private void LateUpdate()
	{
		Vector3 position = base.transform.position;
		position.y = 0f;
		base.transform.position = position;
	}
}
