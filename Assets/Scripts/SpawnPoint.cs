using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : Selector
{
	public GameObject mysteryBox;

	public override void PerformSelection(List<GameObject> objectsToVisit)
	{
		if (!(Game.Instance.CharacterState == Game.Instance.Jetpack))
		{
			SpawnPointManager.Instance.PerformSelection(this, objectsToVisit);
		}
	}
}
