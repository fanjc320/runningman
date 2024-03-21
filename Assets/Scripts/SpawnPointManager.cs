using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointManager
{
	private class PickupType
	{
		public Func<SpawnPoint, GameObject> ExtractGameObject;

		public int spawnProbability;

		public float spawnDistanceMin;

		public float spawnZ;
	}

	public static SpawnPointManager instance;

	private PickupType mysteryBox;

	private PickupType[] pickups;

	private float spawnZ;

	private float spawnSpacing;

	private System.Random randomGen = new System.Random();

	private float totalProbability;

	private float[] accumulatedProbability;

	public static SpawnPointManager Instance => instance ?? (instance = new SpawnPointManager());

	public SpawnPointManager()
	{
		float distancePerMeter = Game.Instance.distancePerMeter;
		Upgrade upgrade = Upgrades.upgrades[PowerupType.mysterybox];
		mysteryBox = new PickupType();
		mysteryBox.spawnDistanceMin = upgrade.minimumMeters;
		mysteryBox.spawnProbability = upgrade.spawnProbability;
		mysteryBox.ExtractGameObject = ((SpawnPoint spawnPoint) => spawnPoint.mysteryBox);
		pickups = new PickupType[1]
		{
			mysteryBox
		};
	}

	public void PerformSelection(SpawnPoint spawnPoint, List<GameObject> objectsToVisit)
	{
		Vector3 position = spawnPoint.transform.position;
		float z = position.z;
		PickupType pickupType = null;
		if (z > spawnZ)
		{
			List<PickupType> list = new List<PickupType>(pickups).FindAll((PickupType p) => p.spawnZ < z);
			if (list.Count > 0)
			{
				int[] array = new int[list.Count];
				int num = 0;
				pickupType = list[0];
				pickupType.spawnZ = z + pickupType.spawnDistanceMin;
				spawnZ = z + spawnSpacing;
			}
		}
		for (int i = 0; i < pickups.Length; i++)
		{
			PickupType pickupType2 = pickups[i];
			GameObject gameObject = pickupType2.ExtractGameObject(spawnPoint);
			if (pickupType2 == pickupType)
			{
				objectsToVisit.Add(gameObject);
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	public void Restart()
	{
		float distancePerMeter = Game.Instance.distancePerMeter;
		spawnZ = 0f;
		spawnSpacing = 0f;
		PickupType[] array = pickups;
		foreach (PickupType pickupType in array)
		{
			pickupType.spawnZ = float.MinValue;
		}
	}
}
