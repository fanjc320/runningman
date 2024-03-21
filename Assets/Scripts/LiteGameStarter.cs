using System.Collections;
using System.Linq;
using UnityEngine;

public class LiteGameStarter : MonoBehaviour
{
	public static string startScene = "GameScene";

	private void Awake()
	{
	}

	private IEnumerator Start()
	{
		yield break;
	}

	private void Update()
	{
	}

	private void addTestCBuffer()
	{
		Object.FindObjectsOfType<GameObject>().All(delegate(GameObject go)
		{
			Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(includeInactive: true);
			if (componentsInChildren == null || 0 >= componentsInChildren.Length)
			{
				return true;
			}
			componentsInChildren.All(delegate(Renderer rnd)
			{
				CBufferTest component = rnd.GetComponent<CBufferTest>();
				if (null == component)
				{
					go.AddComponent<CBufferTest>();
				}
				return true;
			});
			return true;
		});
	}
}
