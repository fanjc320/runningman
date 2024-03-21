using System.Collections.Generic;
using UnityEngine;

public abstract class Selector : MonoBehaviour
{
	public Transform thisTransform;

	public GameObject thisGameObject;

	public abstract void PerformSelection(List<GameObject> objectsToVisit);

	private void Awake()
	{
		thisTransform = base.transform;
		thisGameObject = base.gameObject;
	}

	public void InitializeSelector()
	{
		int childCount = base.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(value: false);
		}
	}
}
