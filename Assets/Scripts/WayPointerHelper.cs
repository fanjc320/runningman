using System.Collections;
using UnityEngine;

public class WayPointerHelper : MonoBehaviour
{
	public enum Mode
	{
		jump,
		left,
		right,
		roll,
		mirror
	}

	[SerializeField]
	private bool displayText;

	[SerializeField]
	private string text;

	[SerializeField]
	private bool displayMesh;

	[SerializeField]
	private float direction;

	[SerializeField]
	private float time = 1f;

	[SerializeField]
	private bool endRace;

	[SerializeField]
	public Mode mode;

	private Character character;

	private Track track;

	private bool Initialiseret;

	private GameObject _mesh;

	private GameObject mesh
	{
		get
		{
			if (_mesh == null)
			{
				_mesh = Camera.main.gameObject.transform.Find("arrow").gameObject;
			}
			return _mesh;
		}
	}

	private void Awake()
	{
	}

	private void Update()
	{
		if (!(Game.DirectInstance == null) && !Initialiseret)
		{
			character = Game.Instance.character;
			track = Track.Instance;
			Initialiseret = true;
		}
	}

	private IEnumerator ShowArrow()
	{
		mesh.transform.rotation = Quaternion.AngleAxis(direction, new Vector3(0f, 0f, 1f)) * Quaternion.Euler(0f, 180f, 0f);
		mesh.SetActive(value: true);
		Vector3 pos = new Vector3(0f, 0f, 20f);
		yield return StartCoroutine(pTween.To(time, delegate(float t)
		{
			mesh.transform.localPosition = Vector3.Lerp(pos - mesh.transform.up * 5f, pos + mesh.transform.up * 5f, t);
			mesh.GetComponent<Renderer>().material.mainTextureOffset = Vector2.Lerp(Vector2.zero, new Vector2(0f, -0.035f), t);
			if (!Game.Instance.IsInGame.Value)
			{
				mesh.transform.localPosition = new Vector3(2000f, 2000f, 0f);
			}
		}));
		mesh.SetActive(value: false);
	}

	public void OnHitProc()
	{
		switch (mode)
		{
		case Mode.jump:
			NpcEnemiesNew.Instance.OnObstacleJump();
			break;
		case Mode.left:
			NpcEnemiesNew.Instance.OnObstacleSide(-20f);
			break;
		case Mode.right:
			NpcEnemiesNew.Instance.OnObstacleSide(20f);
			break;
		case Mode.roll:
			NpcEnemiesNew.Instance.OnObstacleRoll();
			break;
		case Mode.mirror:
			NpcEnemiesNew.Instance.OnObstacleSide(0f);
			break;
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.name.Equals("Collider Npcs"))
		{
			OnHitProc();
			if (displayMesh)
			{
				StartCoroutine(ShowArrow());
			}
			if (endRace)
			{
				PlayerInfo instance = PlayerInfo.Instance;
			}
		}
	}
}
