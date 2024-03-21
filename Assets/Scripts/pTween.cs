using System;
using System.Collections;
using UnityEngine;

public class pTween
{
	public class StatefulVar
	{
		public bool IsFinish;
	}

	public static IEnumerator ToStateful(float duration, float startValue, float endValue, Action<float> callback, Action<float> complete = null, StatefulVar sVar = null)
	{
		float elapsed = 0f - Time.deltaTime;
		while (duration > elapsed && (sVar == null || !sVar.IsFinish))
		{
			elapsed += Time.deltaTime;
			callback(Mathf.Lerp(startValue, endValue, elapsed / duration));
			yield return 0;
		}
		if (complete != null)
		{
			complete(endValue);
		}
		else
		{
			callback?.Invoke(endValue);
		}
	}

	public static IEnumerator While(Func<bool> fnCheckCondition, Action<float> callback = null, Action<float> complete = null)
	{
		float elapsed2 = 0f - Time.deltaTime;
		while (fnCheckCondition())
		{
			elapsed2 += Time.deltaTime;
			callback?.Invoke(elapsed2);
			yield return 0;
		}
		elapsed2 += Time.deltaTime;
		if (complete != null)
		{
			complete(elapsed2);
		}
		else
		{
			callback?.Invoke(elapsed2);
		}
	}

	public static IEnumerator DelayWhile(float delay, Func<bool> fnCheckCondition, Action<float> callback = null, Action<float> complete = null)
	{
		float elapsed3 = 0f - Time.deltaTime;
		while (delay > elapsed3)
		{
			elapsed3 += Time.deltaTime;
			yield return 0;
		}
		elapsed3 -= delay + Time.deltaTime;
		while (fnCheckCondition())
		{
			elapsed3 += Time.deltaTime;
			callback?.Invoke(elapsed3);
			yield return 0;
		}
		elapsed3 += Time.deltaTime;
		if (complete != null)
		{
			complete(elapsed3);
		}
		else
		{
			callback?.Invoke(elapsed3);
		}
	}

	public static IEnumerator DelayTo(float delay, float duration, float startValue, float endValue, Action<float> callback, Action<float> complete = null)
	{
		float elapsed2 = 0f - Time.deltaTime;
		while (delay > elapsed2)
		{
			elapsed2 += Time.deltaTime;
			yield return 0;
		}
		elapsed2 -= delay + Time.deltaTime;
		while (duration > elapsed2)
		{
			elapsed2 += Time.deltaTime;
			callback(Mathf.Lerp(startValue, endValue, elapsed2 / duration));
			yield return 0;
		}
		if (complete != null)
		{
			complete(endValue);
		}
		else
		{
			callback?.Invoke(endValue);
		}
	}

	public static IEnumerator To(float duration, float startValue, float endValue, Action<float> callback, Action<float> complete = null)
	{
		float elapsed = 0f - Time.deltaTime;
		while (duration > elapsed)
		{
			elapsed += Time.deltaTime;
			callback(Mathf.Lerp(startValue, endValue, elapsed / duration));
			yield return 0;
		}
		if (complete != null)
		{
			complete(endValue);
		}
		else
		{
			callback?.Invoke(endValue);
		}
	}

	public static IEnumerator RealtimeTo(float duration, float startValue, float endValue, Action<float> callback, Action<float> complete = null)
	{
		float elapsed = 0f - Time.unscaledDeltaTime;
		while (duration > elapsed)
		{
			elapsed += Time.unscaledDeltaTime;
			callback(Mathf.Lerp(startValue, endValue, elapsed / duration));
			yield return 0;
		}
		if (complete != null)
		{
			complete(endValue);
		}
		else
		{
			callback(endValue);
		}
	}

	public static IEnumerator To(float duration, Action<float> callback, Action<float> complete = null)
	{
		return To(duration, 0f, 1f, callback, complete);
	}

	public static IEnumerator To(float duration, float endValue, Action<float> callback, Action<float> complete = null)
	{
		return To(duration, 0f, endValue, callback, complete);
	}
}
