using System;
using System.Collections;
using System.Text;
using UnityEngine;

public class ResultLogger : UnityEngine.Object
{
	public static void logObject(object result)
	{
		if (result.GetType() == typeof(ArrayList))
		{
			logArraylist((ArrayList)result);
		}
		else if (result.GetType() == typeof(Hashtable))
		{
			logHashtable((Hashtable)result);
		}
		else
		{
			UnityEngine.Debug.Log("result is not a hashtable or arraylist");
		}
	}

	public static void logArraylist(ArrayList result)
	{
		StringBuilder stringBuilder = new StringBuilder();
		IEnumerator enumerator = result.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Hashtable item = (Hashtable)enumerator.Current;
				addHashtableToString(stringBuilder, item);
				stringBuilder.Append("\n--------------------\n");
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
		UnityEngine.Debug.Log(stringBuilder.ToString());
	}

	public static void logHashtable(Hashtable result)
	{
		StringBuilder stringBuilder = new StringBuilder();
		addHashtableToString(stringBuilder, result);
		UnityEngine.Debug.Log(stringBuilder.ToString());
	}

	public static void addHashtableToString(StringBuilder builder, Hashtable item)
	{
		IDictionaryEnumerator enumerator = item.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
				if (dictionaryEntry.Value is Hashtable)
				{
					builder.AppendFormat("{0}: ", dictionaryEntry.Key);
					addHashtableToString(builder, (Hashtable)dictionaryEntry.Value);
				}
				else if (dictionaryEntry.Value is ArrayList)
				{
					builder.AppendFormat("{0}: ", dictionaryEntry.Key);
					addArraylistToString(builder, (ArrayList)dictionaryEntry.Value);
				}
				else
				{
					builder.AppendFormat("{0}: {1}\n", dictionaryEntry.Key, dictionaryEntry.Value);
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
	}

	public static void addArraylistToString(StringBuilder builder, ArrayList result)
	{
		IEnumerator enumerator = result.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				if (current is Hashtable)
				{
					addHashtableToString(builder, (Hashtable)current);
				}
				else if (current is ArrayList)
				{
					addArraylistToString(builder, (ArrayList)current);
				}
				builder.Append("\n--------------------\n");
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
