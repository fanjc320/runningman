using System.Collections.Generic;
using UnityEngine;

public class Pool : Singleton<Pool>
{
	public int Count = 7;

	public bool Grow;

	public GameObject Prefab;

	private List<GameObject> _pool;

	private void Awake()
	{
		_pool = new List<GameObject>(Count);
		for (int i = 0; i < Count; i++)
		{
			_pool.Add(New());
		}
	}

	private GameObject New()
	{
		GameObject gameObject = Object.Instantiate(Prefab);
		gameObject.transform.parent = base.gameObject.transform;
		gameObject.transform.position = Vector3.zero;
		gameObject.SetActive(value: false);
		return gameObject;
	}

	public GameObject Enter()
	{
		for (int i = 0; i < _pool.Count; i++)
		{
			if (!_pool[i].activeInHierarchy)
			{
				return _pool[i];
			}
			if (Grow)
			{
				_pool.Add(New());
			}
		}
		return null;
	}

	public void Exit(GameObject o)
	{
		o.SetActive(value: false);
	}
}
