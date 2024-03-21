using System.Collections.Generic;
using UnityEngine;

public class SpawnPointEgg : Selector
{
	public GameObject easterEgg;

	private const float SPAWN_MINIMUM_TIME = 5f;

	private const float SPAWN_MAXIMUM_TIME = 240f;

	private const float SPAWN_PROBABILTY = 0.06f;

	private static float lastSpawn;

	public override void PerformSelection(List<GameObject> objectsToVisit)
	{
		bool flag = false;
		if (Game.Instance.ElapsedGameTime < lastSpawn + 5f)
		{
			flag = false;
		}
		else if (Game.Instance.ElapsedGameTime > lastSpawn + 240f)
		{
			flag = true;
		}
		else if (Random.value <= 0.06f)
		{
			flag = true;
		}
		easterEgg.SetActive(value: false);
	}
}
