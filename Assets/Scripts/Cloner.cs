using System;
using System.Reflection;

public class Cloner
{
	public static T DeepCopy<T>(T obj)
	{
		if (obj == null)
		{
			throw new ArgumentNullException("Object cannot be null");
		}
		return (T)Process(obj);
	}

	private static object Process(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		Type type = obj.GetType();
		if (type.IsValueType || type == typeof(string))
		{
			return obj;
		}
		if (type.IsArray)
		{
			Type type2 = Type.GetType(type.FullName.Replace("[]", string.Empty));
			Array array = obj as Array;
			Array array2 = Array.CreateInstance(type2, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				array2.SetValue(Process(array.GetValue(i)), i);
			}
			return Convert.ChangeType(array2, obj.GetType());
		}
		if (type.IsClass)
		{
			object obj2 = Activator.CreateInstance(obj.GetType());
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] array3 = fields;
			foreach (FieldInfo fieldInfo in array3)
			{
				object value = fieldInfo.GetValue(obj);
				if (value != null)
				{
					fieldInfo.SetValue(obj2, Process(value));
				}
			}
			return obj2;
		}
		throw new ArgumentException("Unknown type");
	}
}
