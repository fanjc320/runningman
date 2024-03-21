using System;
using System.Collections;
using UnityEngine;

public class TransformUtility
{
	public static Transform FindChild(Transform transform, string pattern)
	{
		return FindChild(transform, pattern, StringUtility.MatchType.Is, ignoreCase: true);
	}

	public static Transform FindChild(Transform transform, string pattern, StringUtility.MatchType matchType)
	{
		return FindChild(transform, pattern, matchType, ignoreCase: true);
	}

	public static Transform FindChild(Transform transform, string pattern, StringUtility.MatchType matchType, bool ignoreCase)
	{
		if (StringUtility.Match(transform.name, pattern, matchType, ignoreCase))
		{
			return transform;
		}
		IEnumerator enumerator = transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform2 = (Transform)enumerator.Current;
				Transform transform3 = FindChild(transform2, pattern, matchType, ignoreCase);
				if (transform3 != null)
				{
					return transform3;
				}
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
		return null;
	}

	public static void AddAndResetChild(Transform transform, Transform child)
	{
		child.parent = transform;
		child.transform.localRotation = Quaternion.identity;
		child.transform.localPosition = Vector3.zero;
		child.transform.localScale = Vector3.one;
	}

	public static int CountAllChildren(Transform parent)
	{
		int num = 0;
		IEnumerator enumerator = parent.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform parent2 = (Transform)enumerator.Current;
				num += CountAllChildren(parent2);
				num++;
			}
			return num;
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public static void SetChildrenActiveRecursively(Transform parent, bool active)
	{
		IEnumerator enumerator = parent.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				transform.gameObject.SetActive(active);
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
	}

	public static T GetComponentInParents<T>(Transform child) where T : Component
	{
		Transform parent = child.parent;
		if (parent != null)
		{
			T component = parent.GetComponent<T>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				return component;
			}
			return GetComponentInParents<T>(parent);
		}
		return (T)null;
	}

	public static void SetLayerRecursively(Transform t, int layer)
	{
		t.gameObject.layer = layer;
		IEnumerator enumerator = t.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform t2 = (Transform)enumerator.Current;
				SetLayerRecursively(t2, layer);
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
	}
}
