using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPoolMT<T> where T : PoolMT, new()
{
	private static int DEFAULT_SIZE = 16;

	private static GameObject prefabGO;

	private static GameObjectPoolMT<T> instance;

	private int poolerSize;

	private Queue<T> queue = new Queue<T>();

	public static GameObjectPoolMT<T> Instance => instance;

	private GameObjectPoolMT()
	{
	}

	public static void Reset(int defaultPoolSize, GameObject prefab)
	{
		prefabGO = prefab;
		DEFAULT_SIZE = defaultPoolSize;
		instance = new GameObjectPoolMT<T>();
		instance.incFillPooler();
	}

	private void incFillPooler()
	{
		lock (this)
		{
			for (int i = 0; DEFAULT_SIZE > i; i++)
			{
				T component = UnityEngine.Object.Instantiate(prefabGO).GetComponent<T>();
				component.Dispose();
				component.OnDisposing += PoolDisposing;
				queue.Enqueue(component);
			}
		}
		poolerSize = ((poolerSize != 0) ? (poolerSize + DEFAULT_SIZE) : DEFAULT_SIZE);
	}

	private void PoolDisposing(PoolMT obj)
	{
		lock (this)
		{
			queue.Enqueue((T)obj);
		}
	}

	public T Get(params object[] args)
	{
		T result = default(T);
		lock (this)
		{
			try
			{
				result = queue.Dequeue();
			}
			catch (Exception)
			{
				incFillPooler();
				result = queue.Dequeue();
			}
		}
		result.ResetPoolObject(args);
		return result;
	}

	public T GetNParent(Transform parent, params object[] args)
	{
		T val = default(T);
		val = Get(args);
		GameObject gameObject = (val as MonoBehaviour).gameObject;
		gameObject.transform.SetParent(parent);
		gameObject.transform.localPosition = prefabGO.transform.localPosition;
		gameObject.transform.localRotation = prefabGO.transform.localRotation;
		gameObject.transform.localScale = prefabGO.transform.localScale;
		return val;
	}

	public T GetNNoParent(Transform parent, params object[] args)
	{
		T val = default(T);
		val = Get(args);
		GameObject gameObject = (val as MonoBehaviour).gameObject;
		gameObject.transform.SetParent(parent);
		gameObject.transform.localPosition = prefabGO.transform.localPosition;
		gameObject.transform.localRotation = prefabGO.transform.localRotation;
		gameObject.transform.localScale = prefabGO.transform.localScale;
		gameObject.transform.SetParent(parent);
		return val;
	}

	public void GetUseDispose(T newInst, Action<T> useBlock)
	{
		useBlock?.Invoke(newInst);
		newInst.Dispose();
	}
}
