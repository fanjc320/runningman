using System;
using UnityEngine;

public class PoolingGameobjectAttacher : MonoBehaviour
{
	[SerializeField]
	private string typeName;

	private PoolingGameobject poolingGO;

	private void Awake()
	{
		Type type = Type.GetType(typeName);
		Component component = base.gameObject.GetComponent(type);
		if (null == component)
		{
			base.gameObject.AddComponent(type);
		}
	}
}
