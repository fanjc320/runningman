using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
	public Vector3 position;

	public Vector3 target;

	private float lastZ;

	public float speed;

	private Vector3 shake = Vector3.zero;

	[SerializeField]
	private EnvironmentBackground environmentBackground;

	public static CharacterCamera instance;

	public static CharacterCamera Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(CharacterCamera)) as CharacterCamera));

	public void Awake()
	{
		instance = this;
		Vector3 vector = base.transform.position;
		lastZ = vector.z;
	}

	private void Start()
	{
	}

	public void Shake()
	{
		Vector3 diff = Vector3.zero;
		float amplitude = 100f;
		StartCoroutine(pTween.To(0.3f, delegate(float t)
		{
			diff += UnityEngine.Random.insideUnitSphere;
			shake = (1f - t) * diff * amplitude * Time.deltaTime;
		}));
	}

	public void SetPosition()
	{
		base.transform.position = position + shake;
		base.transform.LookAt(target + shake);
	}

	public void LateUpdate()
	{
		base.transform.position = position + shake;
		base.transform.LookAt(target + shake);
		speed = (position.z - lastZ) / Time.deltaTime;
		if (float.IsNaN(speed))
		{
			speed = 0f;
		}
		lastZ = position.z;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine(position, target);
	}
}
