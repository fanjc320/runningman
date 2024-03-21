using System.Collections.Generic;

public class WAttributeVariable<T>
{
	public delegate void EventValueChanged(IDictionary<string, T> dictionary, string key, T value);

	private IDictionary<string, T> attributeMap;

	private string key;

	public T Value
	{
		get
		{
			return attributeMap[key];
		}
		set
		{
			bool flag = value.Equals(Value);
			attributeMap[key] = value;
			if (flag)
			{
				FireEventChanged();
			}
		}
	}

	public event EventValueChanged ValueChanged;

	private WAttributeVariable()
	{
	}

	public WAttributeVariable(IDictionary<string, T> dictionary, string key)
	{
		attributeMap = dictionary;
		this.key = key;
	}

	public void FireEventChanged()
	{
		if (this.ValueChanged != null)
		{
			this.ValueChanged(attributeMap, key, Value);
		}
	}
}
