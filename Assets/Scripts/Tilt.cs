using UnityEngine;

public class Tilt : MonoBehaviour
{
	public Vector2 Range = new Vector2(3.33f, 3.33f);

	private Transform _t;

	private Quaternion _q;

	private Vector2 _r = Vector2.zero;

	private void Start()
	{
		_t = base.transform;
		_q = _t.localRotation;
	}

	private void Update()
	{
		Vector3 mousePosition = UnityEngine.Input.mousePosition;
		float num = (float)Screen.width * 0.5f;
		float num2 = (float)Screen.height * 0.5f;
		float x = Mathf.Clamp((mousePosition.x - num) / num, -1f, 1f);
		float y = Mathf.Clamp((mousePosition.y - num2) / num2, -1f, 1f);
		_r = Vector2.Lerp(_r, new Vector2(x, y), Time.deltaTime * 5f);
		_t.localRotation = _q * Quaternion.Euler((0f - _r.y) * Range.y, _r.x * Range.x, 0f);
	}
}
