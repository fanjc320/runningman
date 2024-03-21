using System;
using UnityEngine;

public class PoolingGameobject : MonoBehaviour, PoolMT
{
	public event Action<PoolMT> OnDisposing;

	public void ResetPoolObject(params object[] setupParameters)
	{
		base.gameObject.SetActive(value: true);
	}

	public void Dispose()
	{
		base.gameObject.SetActive(value: false);
		if (this.OnDisposing != null)
		{
			this.OnDisposing(this);
		}
	}
}
