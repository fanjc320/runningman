using UnityEngine;

public class GameSystem : MonoBehaviour
{
	private static GameObject gameObject_;

	private static GameSystem instance_;

	public static GameSystem Instance
	{
		get
		{
			if (null == gameObject_)
			{
				gameObject_ = new GameObject("GameSystem");
				instance_ = gameObject_.AddComponent<GameSystem>();
			}
			return instance_;
		}
	}

	public bool Initialize()
	{
		Object.DontDestroyOnLoad(this);
		return true;
	}

	public void Terminate()
	{
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	private void Update()
	{
	}

	public void onResultBuyItem(string szParam)
	{
		string empty = string.Empty;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		string empty4 = string.Empty;
		if ((!(szParam == "1")) ? true : false)
		{
		}
	}

	public void onCancelBuyItem(string szMsg)
	{
	}
}
