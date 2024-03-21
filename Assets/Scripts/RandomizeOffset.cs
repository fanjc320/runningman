using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeOffset : MonoBehaviour
{
	[Serializable]
	public class RandomOffsets
	{
		public bool left = true;

		public bool mid = true;

		public bool right = true;
	}

	public RandomOffsets randomOffsets;

	public void ChooseRandomOffset()
	{
		List<float> list = new List<float>(3);
		if (randomOffsets.left)
		{
			list.Add(-20f);
		}
		if (randomOffsets.mid)
		{
			list.Add(0f);
		}
		if (randomOffsets.right)
		{
			list.Add(20f);
		}
		int count = list.Count;
		if (count > 0)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.x = list[UnityEngine.Random.Range(0, count)];
			base.transform.localPosition = localPosition;
		}
	}
}
