using System.Collections.Generic;
using UnityEngine;

public static class UtilRMan
{
	public static T FindObject<T>(this MonoBehaviour obj) where T : class
	{
		T val = Object.FindObjectOfType(typeof(T)) as T;
		if (val == null)
		{
		}
		return val;
	}

	public static T FindObject<T>() where T : class
	{
		T[] array = Object.FindObjectsOfType(typeof(T)) as T[];
		if (array.Length == 0)
		{
			return (T)null;
		}
		if (array.Length > 1)
		{
		}
		return array[0];
	}

	public static T FindComponentInParents<T>(this MonoBehaviour obj) where T : Component
	{
		return FindComponentInThisOrParents<T>(obj.transform.parent);
	}

	public static T FindComponentInThisOrParents<T>(Transform t) where T : Component
	{
		Transform transform = t;
		while (transform != null)
		{
			T component = t.GetComponent<T>();
			if ((Object)component != (Object)null)
			{
				return component;
			}
			transform = transform.parent;
		}
		return (T)null;
	}

	public static string GetLongName(Transform transform)
	{
		return (!(transform == null)) ? (GetLongName(transform.parent) + "/" + transform.name) : string.Empty;
	}

	public static string GetLongNameList(Component[] components)
	{
		return string.Join(", ", new List<Component>(components).ConvertAll((Component c) => GetLongName(c.transform)).ToArray());
	}

	public static void Bar(string text, float ratio, int offset, Color color)
	{
		float num = 10f;
		float num2 = 20f;
		GUI.color = color;
		GUI.Button(new Rect(num, (float)Screen.height - num2 - num - (float)offset * num2, ((float)Screen.width - 2f * num) * ratio, num2), text);
	}
}
