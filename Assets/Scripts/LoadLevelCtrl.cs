using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class LoadLevelCtrl : MonoBehaviour
{
	[SerializeField]
	private GameObject backrender;

	private AsyncOperation asyncMerge;

	private GameObject notebook;

	[SerializeField]
	private Transform backgroundAnchor;

	private float sliderSteps;

	private bool sliderStepsIsSet;

	[SerializeField]
	private float inspectorProgress;

	private string[] tipArray = new string[1]
	{
		"Have a good day!"
	};

	private float progress;

	private bool sendDebug;

	public static float continueTime = 99999f;

	private float fakeProgress
	{
		get
		{
			float num = 0f;
			num += progress * 0.5f;
			if (progress > 0.74f)
			{
				num += (progress - 0.74f) * 3.7f;
			}
			return num;
		}
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	private void Update()
	{
		LoadBar();
	}

	private string GetTip()
	{
		return tipArray[Random.Range(0, tipArray.Length)];
	}

	private IEnumerator Start()
	{
		asyncMerge = Application.LoadLevelAdditiveAsync("Merge");
		StartCoroutine(Progress());
		yield return asyncMerge;
	}

	private IEnumerator Progress()
	{
		while (!asyncMerge.isDone)
		{
			progress = 0f;
			if (asyncMerge != null)
			{
				progress = asyncMerge.progress;
			}
			if (progress > 0.89f && !sendDebug)
			{
				sendDebug = true;
			}
			yield return null;
		}
		continueTime = 0f;
		yield return null;
		UnityEngine.Object.Destroy(base.gameObject);
		UnloadAssets();
	}

	public float LoadBar()
	{
		if (!sliderStepsIsSet)
		{
			sliderStepsIsSet = true;
		}
		return 1f;
	}

	private void UnloadAssets()
	{
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogError(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_ERROR_LOGS")]
	public static void LogWarning(string msg, UnityEngine.Object context = null)
	{
	}

	[Conditional("ENABLE_DEBUG_LOGS")]
	public static void Log(string msg, UnityEngine.Object context = null)
	{
	}
}
