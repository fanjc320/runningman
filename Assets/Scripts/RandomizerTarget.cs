using System.Collections.Generic;
using UnityEngine;

public class RandomizerTarget : Selector
{
	public List<GameObject> Targets;

	public override void PerformSelection(List<GameObject> objectsToVisit)
	{
		int num = Random.Range(0, Targets.Count);
		int count = Targets.Count;
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = Targets[i];
			if (i == num)
			{
				objectsToVisit.Add(gameObject);
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}
	}
}
