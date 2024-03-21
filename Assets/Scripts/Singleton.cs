using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected static T instance;

	public static T Instance
	{
		get
		{
			if ((Object)instance == (Object)null)
			{
				instance = (T)UnityEngine.Object.FindObjectOfType(typeof(T));
				if ((Object)instance == (Object)null)
				{
					GameObject gameObject = new GameObject(typeof(T).ToString());
					instance = gameObject.AddComponent<T>();
				}
			}
			return instance;
		}
	}
}
