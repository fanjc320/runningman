using System;
using System.Diagnostics;
using UnityEngine;

public static class Debug
{
	public static bool isDebugBuild => UnityEngine.Debug.isDebugBuild;

	[Conditional("ENABLE_LOG")]
	public static void Log(object message)
	{
		UnityEngine.Debug.Log(message);
	}

	[Conditional("ENABLE_LOG")]
	public static void Log(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.Log(message, context);
	}

	[Conditional("ENABLE_LOG")]
	public static void LogError(object message)
	{
		UnityEngine.Debug.LogError(message);
	}

	[Conditional("ENABLE_LOG")]
	public static void LogError(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogError(message, context);
	}

	[Conditional("ENABLE_LOG")]
	public static void LogWarning(object message)
	{
		UnityEngine.Debug.LogWarning(message.ToString());
	}

	[Conditional("ENABLE_LOG")]
	public static void LogWarning(object message, UnityEngine.Object context)
	{
		UnityEngine.Debug.LogWarning(message.ToString(), context);
	}

	[Conditional("ENABLE_LOG")]
	public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
	}

	[Conditional("ENABLE_LOG")]
	public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0f, bool depthTest = true)
	{
		UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
	}

	[Conditional("ENABLE_LOG")]
	public static void Assert(bool condition)
	{
		if (!condition)
		{
			throw new Exception();
		}
	}
}
