using System.Collections;
using UnityEngine;

public class PrevScene : MonoBehaviour
{
	[SerializeField]
	private float DelayTime = 2f;

	private AsyncOperation asyncLoad;

	private float fTimePivot;

	private IEnumerator Start()
	{
		fTimePivot = Time.realtimeSinceStartup;
		asyncLoad = Application.LoadLevelAdditiveAsync("LoadScene");
		StartCoroutine(Progress());
		yield return asyncLoad;
	}

	private IEnumerator Progress()
	{
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
		yield return null;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void UnloadAssets()
	{
	}
}
