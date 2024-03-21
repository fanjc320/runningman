using UnityEngine;

[ExecuteInEditMode]
public class CameraRect : MonoBehaviour
{
	public RectTransform baseanchor;

	public Vector2 refResolution = new Vector2(1280f, 720f);

	public Camera myCam;

	private void Update1()
	{
		Camera camera = myCam;
		Vector2 anchoredPosition = baseanchor.anchoredPosition;
		float x = anchoredPosition.x / refResolution.x;
		Vector2 anchoredPosition2 = baseanchor.anchoredPosition;
		float y = anchoredPosition2.y / refResolution.y;
		Vector2 sizeDelta = baseanchor.sizeDelta;
		float width = sizeDelta.x / refResolution.x;
		Vector2 sizeDelta2 = baseanchor.sizeDelta;
		camera.rect = new Rect(x, y, width, sizeDelta2.y / refResolution.y);
	}
}
