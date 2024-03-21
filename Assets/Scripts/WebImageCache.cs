using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebImageCache : MonoBehaviour
{
	public class RequestObject
	{
		public int key;

		public string url;

		public Image renderer;

		public SpriteRenderer sprRenderer;
	}

	private static WebImageCache instance;

	public int requestCount;

	public Queue<RequestObject> requestURLs;

	public Dictionary<int, Sprite> spriteDict;

	public Sprite LoadingSprite;

	public static WebImageCache Instance
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
		instance = (UnityEngine.Object.Instantiate(Resources.Load("WebImageCache", typeof(GameObject))) as GameObject).GetComponent<WebImageCache>();
		instance.gameObject.name = instance.GetType().Name;
		instance.gameObject.hideFlags = HideFlags.HideAndDontSave;
	}

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		requestCount = 0;
		requestURLs = new Queue<RequestObject>();
		spriteDict = new Dictionary<int, Sprite>();
	}

	public static Sprite GetSprite(string url, ref Image renderer)
	{
		if (string.IsNullOrEmpty(url))
		{
			return Instance.LoadingSprite;
		}
		int hashCode = url.GetHashCode();
		if (Instance.spriteDict.ContainsKey(hashCode))
		{
			renderer.sprite = Instance.spriteDict[hashCode];
			return Instance.spriteDict[hashCode];
		}
		renderer.sprite = Instance.LoadingSprite;
		Instance.requestURLs.Enqueue(new RequestObject
		{
			key = hashCode,
			renderer = renderer,
			url = url
		});
		Instance.requestCount++;
		return Instance.LoadingSprite;
	}

	public static Sprite GetSprite(string url, ref SpriteRenderer renderer)
	{
		if (string.IsNullOrEmpty(url))
		{
			renderer.sprite = Instance.LoadingSprite;
			return Instance.LoadingSprite;
		}
		int hashCode = url.GetHashCode();
		if (Instance.spriteDict.ContainsKey(hashCode))
		{
			renderer.sprite = Instance.spriteDict[hashCode];
			return Instance.spriteDict[hashCode];
		}
		renderer.sprite = Instance.LoadingSprite;
		Instance.requestURLs.Enqueue(new RequestObject
		{
			key = hashCode,
			sprRenderer = renderer,
			url = url
		});
		Instance.requestCount++;
		return Instance.LoadingSprite;
	}

	private void Start()
	{
		StartCoroutine(RequestImage());
	}

	private IEnumerator RequestImage()
	{
		while (Application.isPlaying)
		{
			while (requestCount == 0)
			{
				yield return 0;
			}
			requestCount--;
			RequestObject request = requestURLs.Dequeue();
			if (!spriteDict.ContainsKey(request.key))
			{
				string path = request.url;
				WWW www = new WWW(path);
				yield return www;
				spriteDict[request.key] = Sprite.Create(www.texture, new Rect(0f, 0f, 50f, 50f), new Vector2(0.5f, 0.5f), 100f);
				if ((bool)request.renderer)
				{
					request.renderer.sprite = spriteDict[request.key];
				}
				if ((bool)request.sprRenderer)
				{
					request.sprRenderer.sprite = spriteDict[request.key];
				}
			}
			else
			{
				if ((bool)request.renderer)
				{
					request.renderer.sprite = spriteDict[request.key];
				}
				if ((bool)request.sprRenderer)
				{
					request.sprRenderer.sprite = spriteDict[request.key];
				}
			}
		}
	}
}
