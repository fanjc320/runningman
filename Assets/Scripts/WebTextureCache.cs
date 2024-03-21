using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebTextureCache : MonoBehaviour
{
	private Dictionary<string, Texture2D> imageCache = new Dictionary<string, Texture2D>();

	private Dictionary<string, WWW> requestCache = new Dictionary<string, WWW>();

	private static WebTextureCache instance;

	public static WebTextureCache InstantiateGlobal(string name = "WebTextureCache")
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject(name);
			gameObject.AddComponent<WebTextureCache>();
			instance = gameObject.GetComponent<WebTextureCache>();
		}
		return instance;
	}

	public IEnumerator GetTexture(string url, Action<Texture2D, string> callback)
	{
		if (!imageCache.ContainsKey(url))
		{
			int retryTimes = 3;
			WWW request;
			do
			{
				retryTimes--;
				if (!requestCache.ContainsKey(url))
				{
					requestCache[url] = new WWW(url);
				}
				request = requestCache[url];
				yield return request;
				if (requestCache.ContainsKey(url) && requestCache[url] == request)
				{
					requestCache.Remove(url);
				}
			}
			while (request.error != null && retryTimes >= 0);
			if (request.error == null && !imageCache.ContainsKey(url))
			{
				imageCache[url] = request.texture;
			}
		}
		if (callback != null)
		{
			Texture2D value = null;
			imageCache.TryGetValue(url, out value);
			callback(value, url);
		}
	}
}
