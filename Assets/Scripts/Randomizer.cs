using System.Collections.Generic;
using UnityEngine;

public class Randomizer : Selector
{
	public bool isTrains;

	public override void PerformSelection(List<GameObject> objectsToVisit)
	{
		int num = 0;
		int childCount = thisTransform.childCount;
		num = ((!isTrains) ? Random.Range(0, childCount) : Random.Range(0, childCount - 1));
		for (int i = 0; i < childCount; i++)
		{
			GameObject gameObject = thisTransform.GetChild(i).gameObject;
			if (i == num)
			{
				objectsToVisit.Add(gameObject);
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}
		if (isTrains)
		{
			GameObject gameObject2 = thisTransform.GetChild(childCount - 1).gameObject;
			gameObject2.SetActive(value: true);
		}
	}
}
