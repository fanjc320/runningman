using System;
using System.Collections;
using UnityEngine;

public class Glow : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	public void Awake()
	{
		Transform transform = base.transform;
		Transform parent = transform.parent;
		Transform transform2 = (!(parent == null)) ? parent.parent : null;
		if (DeviceInfo.Instance.performanceLevel == DeviceInfo.PerformanceLevel.Low && (parent.gameObject.name.Contains("coin") || (transform2 != null && transform2.gameObject.name.Contains("coin"))))
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform3 = (Transform)enumerator.Current;
					UnityEngine.Object.Destroy(transform3.gameObject);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			meshRenderer = GetComponentInChildren<MeshRenderer>();
			if (meshRenderer != null)
			{
				meshRenderer.enabled = false;
				base.enabled = false;
				UnityEngine.Object.Destroy(meshRenderer);
				meshRenderer = null;
			}
		}
		else
		{
			meshRenderer = GetComponentInChildren<MeshRenderer>();
		}
	}

	public void SetVisible(bool visible)
	{
		if (meshRenderer != null)
		{
			meshRenderer.enabled = visible;
			base.enabled = visible;
		}
	}
}
