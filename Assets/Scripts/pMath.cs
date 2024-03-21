using UnityEngine;

public class pMath
{
	public static float Repeat(float t, float length)
	{
		return (!(0f <= Mathf.Sign(t))) ? ((Mathf.Floor((0f - t) / length + 1f) * length + t) % length) : (t - Mathf.Floor(t / length) * length);
	}

	public static float Bell(float x)
	{
		return Mathf.SmoothStep(0f, 1f, 1f - Mathf.Abs(x - 0.5f) / 0.5f);
	}

	public static float Lerp(float xFrom, float xTo, float x, float yFrom, float yTo)
	{
		return Mathf.Lerp(yFrom, yTo, Mathf.Clamp01((x - xFrom) / (xTo - xFrom)));
	}

	public static float Square(float x)
	{
		return (!(x - (float)Mathf.FloorToInt(x) < 0.5f)) ? 1f : 0f;
	}
}
