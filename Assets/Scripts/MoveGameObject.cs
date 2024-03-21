using UnityEngine;

public class MoveGameObject : MonoBehaviour
{
	public Vector3 vector = Vector3.zero;

	public float scaleFactor = 1f;

	private CharacterCamera followCamera;

	public Transform model;

	public float startScale = 1f;

	private void Update()
	{
		if (followCamera == null)
		{
			followCamera = CharacterCamera.Instance;
		}
		if (followCamera.speed > 0f)
		{
			base.transform.Translate(vector * followCamera.speed * Time.deltaTime);
			float num = startScale;
			Vector3 localPosition = base.transform.localPosition;
			float num2 = num + localPosition.x * scaleFactor;
			model.localScale = new Vector3(num2, num2, num2);
		}
	}
}
