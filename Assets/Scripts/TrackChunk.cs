using Lean;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackChunk : MonoBehaviour
{
	[Serializable]
	public class TrackCheckPoint
	{
		public int TrackNumber;

		public float Z;
	}

	public float zSize = 40f;

	public int probability = 1;

	public float zMinimum;

	public bool zMaximumActive;

	public float zMaximum;

	public List<TrackCheckPoint> CheckPoints;

	public TrackObject[] objects;

	public Selector[] selectors;

	public List<GameObject> spawnObject = new List<GameObject>();

	public bool wasDisabledDueToHoverBoard;

	public bool isTutorial;

	private Dictionary<Transform, Vector3> hiddenObstacles = new Dictionary<Transform, Vector3>();

	public TrackChunkData chunkData;

	public bool isActive;

	public bool isPrebuildChunk;

	public Transform thisTransform;

	public void Awake()
	{
		thisTransform = base.transform;
		if (!isPrebuildChunk)
		{
			return;
		}
		isActive = true;
		objects = GetComponentsInChildren<TrackObject>(includeInactive: true);
		if (!zMaximumActive)
		{
			zMaximum = float.MaxValue;
		}
		if (!(base.tag == "unloadmap"))
		{
			TrackChunkCollection.AddToChunks(this);
			selectors = GetComponentsInChildren<Selector>(includeInactive: true);
			int num = selectors.Length;
			for (int i = 0; i < num; i++)
			{
				selectors[i].InitializeSelector();
			}
		}
	}

	public void Init()
	{
		if (!zMaximumActive)
		{
			zMaximum = float.MaxValue;
		}
		if (!(base.tag == "unloadmap"))
		{
			TrackChunkCollection.AddToChunks(this);
		}
	}

	public void InitPool()
	{
		if (isActive && (!isActive || isPrebuildChunk || spawnObject != null))
		{
			return;
		}
		if (spawnObject == null)
		{
			spawnObject = new List<GameObject>();
		}
		isActive = true;
		Transform parent = base.transform.Find("Ground");
		Transform parent2 = base.transform.Find("Objects");
		Transform parent3 = base.transform.Find("Items");
		int num = chunkData.objects.Length;
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = LeanPool.Spawn(chunkData.objects[i].prefab, chunkData.objects[i].position, chunkData.objects[i].rotation, parent2);
			if ((bool)gameObject)
			{
				spawnObject.Add(gameObject);
			}
		}
		num = chunkData.grounds.Length;
		for (int j = 0; j < num; j++)
		{
			GameObject gameObject = LeanPool.Spawn(chunkData.grounds[j].prefab, chunkData.grounds[j].position, chunkData.grounds[j].rotation, parent);
			if ((bool)gameObject)
			{
				spawnObject.Add(gameObject);
			}
		}
		num = chunkData.items.Length;
		for (int k = 0; k < num; k++)
		{
			GameObject gameObject = LeanPool.Spawn(chunkData.items[k].prefab, chunkData.items[k].position, chunkData.items[k].rotation, parent3);
			if ((bool)gameObject)
			{
				spawnObject.Add(gameObject);
			}
		}
		objects = GetComponentsInChildren<TrackObject>(includeInactive: true);
		selectors = GetComponentsInChildren<Selector>(includeInactive: true);
		int num2 = selectors.Length;
		for (int l = 0; l < num2; l++)
		{
			selectors[l].InitializeSelector();
		}
	}

	public void Deactivate()
	{
		int num = objects.Length;
		for (int i = 0; num > i; i++)
		{
			if (null != objects[i])
			{
				objects[i].Deactivate();
			}
		}
		if (!isPrebuildChunk && spawnObject != null)
		{
			int count = spawnObject.Count;
			for (int j = 0; j < count; j++)
			{
				LeanPool.Despawn(spawnObject[j]);
			}
			spawnObject = null;
		}
	}

	public void DeactivateObstacles(float maxZ)
	{
		wasDisabledDueToHoverBoard = true;
		int childCount = thisTransform.childCount;
		for (int i = 0; childCount > i; i++)
		{
			DeactiveObstaclesRecursive(thisTransform.GetChild(i), maxZ);
		}
	}

	private void DeactiveObstaclesRecursive(Transform target, float maxZ)
	{
		float z;
		if (target.GetComponent<Collider>() != null)
		{
			Vector3 min = target.GetComponent<Collider>().bounds.min;
			z = min.z;
		}
		else
		{
			Vector3 position = target.transform.position;
			z = position.z;
		}
		float num = z;
		if (target.tag != "typePrefab")
		{
			for (int i = 0; target.childCount > i; i++)
			{
				DeactiveObstaclesRecursive(target.GetChild(i), maxZ);
			}
		}
		else if (num < maxZ && target.gameObject.layer != 16)
		{
			Vector3 localPosition = target.localPosition;
			if (!hiddenObstacles.ContainsKey(target))
			{
				hiddenObstacles.Add(target, localPosition);
			}
			target.localPosition = new Vector3(localPosition.x, -1000f, localPosition.z);
		}
	}

	public void DeactivateObstacles(float maxZ, float sideX)
	{
		wasDisabledDueToHoverBoard = true;
		int childCount = thisTransform.childCount;
		for (int i = 0; childCount > i; i++)
		{
			DeactiveObstaclesRecursive(thisTransform.GetChild(i), maxZ, sideX);
		}
	}

	private void DeactiveObstaclesRecursive(Transform target, float maxZ, float sideX)
	{
		float z;
		if (target.GetComponent<Collider>() != null)
		{
			Vector3 min = target.GetComponent<Collider>().bounds.min;
			z = min.z;
		}
		else
		{
			Vector3 position = target.transform.position;
			z = position.z;
		}
		float num = z;
		if (target.tag == "typePrefab")
		{
			int childCount = target.childCount;
			for (int i = 0; childCount > i; i++)
			{
				DeactiveObstaclesRecursive(target.GetChild(i), maxZ);
			}
		}
		else
		{
			if (!(num < maxZ) || target.gameObject.layer == 16)
			{
				return;
			}
			Vector3 localPosition = target.localPosition;
			if (localPosition.x != sideX)
			{
				if (!hiddenObstacles.ContainsKey(target))
				{
					hiddenObstacles.Add(target, localPosition);
				}
				Vector3 position2 = target.position;
				if (position2.x == 0f)
				{
					target.localPosition = new Vector3(localPosition.x, -1000f, localPosition.z);
				}
			}
		}
	}

	public void RestoreHiddenObstacles()
	{
		foreach (KeyValuePair<Transform, Vector3> hiddenObstacle in hiddenObstacles)
		{
			if (hiddenObstacle.Key != null)
			{
				hiddenObstacle.Key.localPosition = hiddenObstacle.Value;
			}
		}
		hiddenObstacles.Clear();
	}

	public float GetLastCheckPoint(float characterZ)
	{
		return (from c in CheckPoints
			orderby c.Z
			where c.Z <= characterZ
			select c).LastOrDefault()?.Z ?? 0f;
	}

	private void DrawCheckPointGizmos()
	{
		if (CheckPoints != null)
		{
			int count = CheckPoints.Count;
			for (int i = 0; count > i; i++)
			{
				Vector3 position = base.transform.position;
				position.z = CheckPoints[i].Z;
				Gizmos.DrawSphere(position + Vector3.up * 5f, 5f);
			}
		}
	}

	public void OnDrawGizmos()
	{
		DrawCheckPointGizmos();
	}
}
