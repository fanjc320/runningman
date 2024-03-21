using System;

public class Variable<T> where T : IComparable
{
	public delegate void OnChangeDelegate(T value);

	private T value;

	public OnChangeDelegate OnChange;

	public T Value
	{
		get
		{
			return value;
		}
		set
		{
			bool flag = value.CompareTo(this.value) != 0;
			this.value = value;
			if (flag)
			{
				NotifyOnChange();
			}
		}
	}

	public Variable(T initialValue)
	{
		value = initialValue;
	}

	public void FireOnChange()
	{
		NotifyOnChange();
	}

	private void NotifyOnChange()
	{
		if (OnChange != null)
		{
			OnChange(value);
		}
	}
}
