using System;
using System.Collections.Generic;

public static class EnumerableExt
{
	public static IEnumerable<IEnumerable<T>> Chunks<T>(this IEnumerable<T> enumerable, int chunkSize)
	{
		if (chunkSize < 1)
		{
			throw new ArgumentException("chunkSize must be positive");
		}
		IEnumerator<T> e = (IEnumerator<T>)enumerable.GetEnumerator();
		try
		{
			while (e.MoveNext())
			{
				Func<bool> innerMoveNext = () => --chunkSize > 0 && e.MoveNext();
				yield return ((IEnumerator<T>)e).GetChunk(innerMoveNext);
				while (innerMoveNext())
				{
				}
			}
		}
		finally
		{
			base._003C_003E__Finally0();
		}
	}
}
