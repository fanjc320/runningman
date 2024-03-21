using System;
using System.Collections.Generic;

public static class EnumeratorExt
{
	public static IEnumerable<T> GetChunk<T>(this IEnumerator<T> e, Func<bool> innerMoveNext)
	{
		do
		{
			yield return e.Current;
		}
		while (innerMoveNext());
	}
}
