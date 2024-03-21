using System.Collections.Generic;
using UnityEngine;

public class RandomizerHold : Selector
{
	private static int startIndex = 0;

	private static int[] randomIndices = new int[21]
	{
		0,
		1,
		2,
		3,
		0,
		4,
		5,
		1,
		0,
		2,
		4,
		1,
		3,
		2,
		0,
		5,
		1,
		0,
		3,
		1,
		3
	};

	private static float holdDistance = 3000f;

	public static int mapSelect = 0;

	[SerializeField]
	private GameObject[] children;

	public static void Initialize()
	{
		startIndex = Random.Range(0, randomIndices.Length);
	}

	public override void PerformSelection(List<GameObject> objectsToVisit)
	{
		Vector3 position = thisTransform.position;
		int num = Mathf.FloorToInt(position.z / holdDistance) + startIndex;
		int num2 = randomIndices[(startIndex + num) % randomIndices.Length];
		for (int i = 0; i < children.Length; i++)
		{
			GameObject gameObject = children[i];
			if (i == mapSelect)
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
