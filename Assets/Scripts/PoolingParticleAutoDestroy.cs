using System;
using System.Collections;
using UnityEngine;

public class PoolingParticleAutoDestroy : MonoBehaviour, PoolMT
{
	private ParticleSystem partSys;

	public event Action<PoolMT> OnDisposing;

	public void ResetPoolObject(params object[] setupParameters)
	{
		base.gameObject.SetActive(value: true);
		partSys.Play();
	}

	public void Dispose()
	{
		base.gameObject.SetActive(value: false);
		if (this.OnDisposing != null)
		{
			this.OnDisposing(this);
		}
	}

	private void OnEnable()
	{
		if (null == partSys)
		{
			partSys = GetComponent<ParticleSystem>();
		}
		StartCoroutine("CheckIfAlive");
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator CheckIfAlive()
	{
		do
		{
			yield return new WaitForSeconds(0.5f);
		}
		while (partSys.IsAlive(withChildren: false));
		Dispose();
	}
}
