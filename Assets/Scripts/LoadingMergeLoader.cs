using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingMergeLoader : MonoBehaviour
{
	private static LoadingMergeLoader instance;

	public Canvas CanvasRoot;

	public string ActiveLoadSceneName = string.Empty;

	public string ActiveLoadSceneNameSecondary = string.Empty;

	public RawImage BGImage;

	public Transform AllElements;

	public Action OnLoadCompleted;

	public Action OnLoadSceneCompleted;

	public Action OnLoadSceneSecondaryCompleted;

	public Action<AsyncOperation> OnLoadProgress;

	public Coroutine crtLoading;

	public AsyncOperation asyncOpLevelLoad;

	public GameObject MultiWaitCancelBtnRoot;

	public static LoadingMergeLoader Instance
	{
		get
		{
			if (null == instance)
			{
				_init();
			}
			return instance;
		}
	}

	private static void _init()
	{
		instance = (UnityEngine.Object.Instantiate(Resources.Load("LoadingMergeLoader", typeof(GameObject))) as GameObject).GetComponent<LoadingMergeLoader>();
		instance.gameObject.name = instance.GetType().Name;
	}

	private void Awake()
	{
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		BGImage.gameObject.SetActive(value: false);
	}

	private void Start()
	{
	}

	public void StartLoadLevel2(bool autoDisableLoadingIndicater = true)
	{
		crtLoading = StartCoroutine(_StartLoadLevel2(autoDisableLoadingIndicater));
	}

	public void StartLoadLevel(bool autoDisableLoadingIndicater = true)
	{
		crtLoading = StartCoroutine(_StartLoadLevel(autoDisableLoadingIndicater));
	}

	private IEnumerator _StartLoadLevel(bool autoDisableLoadingIndicater)
	{
		yield return new WaitForEndOfFrame();
		ShowIndicater(isShow: true);
		yield return 0;
		AsyncOperation asyncMerge3 = Application.LoadLevelAsync(ActiveLoadSceneName);
		while (!asyncMerge3.isDone)
		{
			yield return 0;
		}
		if (OnLoadSceneCompleted != null)
		{
			OnLoadSceneCompleted();
		}
		if (!string.IsNullOrEmpty(ActiveLoadSceneNameSecondary))
		{
			AsyncOperation asyncOperation;
			AsyncOperation asyncMerge2 = asyncOperation = Application.LoadLevelAdditiveAsync(ActiveLoadSceneNameSecondary);
			asyncOpLevelLoad = asyncOperation;
			while (!asyncMerge2.isDone)
			{
				yield return 0;
			}
			if (OnLoadSceneSecondaryCompleted != null)
			{
				OnLoadSceneSecondaryCompleted();
			}
		}
		if (OnLoadCompleted != null)
		{
			OnLoadCompleted();
		}
		if (autoDisableLoadingIndicater)
		{
			ShowIndicater(isShow: false);
		}
		BGImage.texture = null;
		Instance.ActiveLoadSceneName = string.Empty;
		Instance.ActiveLoadSceneNameSecondary = string.Empty;
		Instance.OnLoadCompleted = null;
		Instance.OnLoadSceneCompleted = null;
		Instance.OnLoadSceneSecondaryCompleted = null;
		Instance.OnLoadProgress = null;
		crtLoading = null;
	}

	private IEnumerator _StartLoadLevel2(bool autoDisableLoadingIndicater)
	{
		yield return new WaitForEndOfFrame();
		BGImage.gameObject.SetActive(value: true);
		BGImage.texture = Capture(CanvasRoot.pixelRect, 0, 0);
		Camera[] allCam = new Camera[Camera.allCamerasCount];
		Camera.GetAllCameras(allCam);
		Camera selfCam = AllElements.Find("Camera").GetComponent<Camera>();
		for (int i = 0; allCam.Length > i; i++)
		{
			if (null != allCam[i] && selfCam != allCam[i])
			{
				allCam[i].enabled = false;
			}
		}
		yield return 0;
		yield return 0;
		ShowIndicater(isShow: true);
		yield return 0;
		UnityEngine.SceneManagement.SceneManager.LoadScene(ActiveLoadSceneName);
		if (OnLoadCompleted != null)
		{
			OnLoadCompleted();
		}
		BGImage.gameObject.SetActive(value: false);
		if (autoDisableLoadingIndicater)
		{
			ShowIndicater(isShow: false);
		}
		UnityEngine.Object.Destroy(BGImage.texture);
		BGImage.texture = null;
		Instance.ActiveLoadSceneName = string.Empty;
		Instance.ActiveLoadSceneNameSecondary = string.Empty;
		Instance.OnLoadCompleted = null;
		Instance.OnLoadSceneCompleted = null;
		Instance.OnLoadSceneSecondaryCompleted = null;
		Instance.OnLoadProgress = null;
		crtLoading = null;
	}

	public void ShowIndicater(bool isShow)
	{
		AllElements.gameObject.SetActive(isShow);
		MultiWaitCancelBtnRoot.gameObject.SetActive(value: false);
	}

	public static Texture Capture(Rect captureZone, int destX, int destY)
	{
		Texture2D texture2D = new Texture2D(Mathf.RoundToInt(captureZone.width) + destX, Mathf.RoundToInt(captureZone.height) + destY, TextureFormat.RGB24, mipChain: false);
		texture2D.wrapMode = TextureWrapMode.Clamp;
		texture2D.ReadPixels(captureZone, destX, destY, recalculateMipMaps: false);
		texture2D.Apply();
		return texture2D;
	}
}
