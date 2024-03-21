using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Track : MonoBehaviour
{
	private struct GameObjectWrapper
	{
		public GameObject GameObject;

		public float z;

		public float Z => z;

		public GameObjectWrapper(GameObject gameObject)
		{
			GameObject = gameObject;
			Vector3 position = gameObject.transform.position;
			z = position.z;
		}
	}

	public Transform trackLeft;

	public Transform trackRight;

	public int numberOfTracks = 3;

	public float cleanUpDistance = 2000f;

	public float trackAheadDistance = 700f;

	public Transform levelChunksParent;

	public TrackChunk jetpackLandingChunk;

	public bool tutorial;

	public TrackChunk tutorialTrackChunk;

	private bool firstTrackChunk = true;

	private TrackChunkCollection trackChunks;

	private float trackSpacing;

	private float trackChunkZ;

	private List<TrackChunk> activeTrackChunks = new List<TrackChunk>(5);

	private List<TrackChunk> trackChunksForDeactivation = new List<TrackChunk>(5);

	public static Track instance;

	private const int ActiveTruePerFrame = 4;

	private const int ActivatePerFrame = 1;

	private Transform thisTransform;

	public float ThisCharZ;

	public bool IsRunningOnTutorialTrack
	{
		get;
		set;
	}

	public static Track Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(Track)) as Track));

	private void Awake()
	{
		instance = this;
		thisTransform = base.transform;
	}

	public void Init()
	{
		instance = this;
		trackSpacing = (trackRight.position - trackLeft.position).magnitude / (float)(numberOfTracks - 1);
		trackChunks = new TrackChunkCollection();
		tutorial = ((!PlayerInfo.Instance.TutorialCompleted) ? true : false);
	}

	public Vector3 GetPosition(float x, float z)
	{
		return Vector3.forward * z + trackLeft.position + x * Vector3.right;
	}

	public float GetTrackX(int trackIndex)
	{
		return trackSpacing * (float)trackIndex;
	}

	public float LayJetpackChunks(float characterZ, float flyLength)
	{
		LayTracksUpTo(characterZ, flyLength / 3f, isJetpack: true);
		float result = trackChunkZ - characterZ;
		LayTrackChunk(jetpackLandingChunk);
		return result;
	}

	public void LayEmptyChunks(float characterZ, float removeDistance)
	{
		RemoveChunkObstacles(characterZ + removeDistance);
	}

	public void RemoveChunkObstacles(float removeDistance)
	{
		foreach (TrackChunk activeTrackChunk in activeTrackChunks)
		{
			activeTrackChunk.DeactivateObstacles(removeDistance);
		}
	}

	public void LayEmptyChunks(float characterZ, float removeDistance, float sideX)
	{
		RemoveChunkObstacles(characterZ + removeDistance, sideX);
	}

	public void RemoveChunkObstacles(float removeDistance, float sideX)
	{
		foreach (TrackChunk activeTrackChunk in activeTrackChunks)
		{
			activeTrackChunk.DeactivateObstacles(removeDistance, sideX);
		}
	}

	public void Initialize(float characterZ)
	{
		trackChunks.Initialize(characterZ);
	}

	public void LayTrackChunks(float characterZ)
	{
		LayTracksUpTo(characterZ, trackAheadDistance, isJetpack: false);
	}

	public void LayTracksUpTo(float characterZ, float trackAheadDistance, bool isJetpack)
	{
		if (!trackChunks.CanDeliver())
		{
			return;
		}
		float num = characterZ + trackAheadDistance;
		ThisCharZ = characterZ - cleanUpDistance;
		float num2 = 200f;
		if (trackChunkZ < num)
		{
			CleanupTrackChunks(characterZ);
		}
		int num3 = 0;
		while (trackChunkZ < num)
		{
			trackChunks.MoveForward(trackChunkZ);
			TrackChunk trackChunk;
			if (firstTrackChunk && tutorial)
			{
				trackChunk = tutorialTrackChunk;
				firstTrackChunk = false;
				if (trackChunk.CheckPoints.Count > 0)
				{
					IsRunningOnTutorialTrack = true;
				}
				Hoverboard.Instance.isAllowed = false;
			}
			else
			{
				int randomSpaceCount = trackChunks.RandomSpaceCount;
				int num4 = 0;
				trackChunk = trackChunks.GetShuffleActive(num4);
				while (activeTrackChunks.Contains(trackChunk) && num4 < randomSpaceCount)
				{
					trackChunk = trackChunks.GetShuffleActive(num4);
					num4++;
				}
				if (num4 == randomSpaceCount)
				{
					trackChunk = trackChunks.AddRangedActiveTrackChunk();
					if (!(null == trackChunk))
					{
					}
				}
			}
			LayTrackChunk(trackChunk);
			if (trackChunk.gameObject.tag == "themechange")
			{
				RandomizerHold.mapSelect = UnityEngine.Random.Range(0, 3);
				if (RandomizerHold.mapSelect >= 3)
				{
					RandomizerHold.mapSelect = 0;
				}
			}
		}
	}

	private void LayTrackChunk(TrackChunk trackChunk)
	{
		StartCoroutine(LayTrackChunkAsync(trackChunk));
	}

	private IEnumerator LayTrackChunkAsync(TrackChunk trackChunk)
	{
		trackChunk.InitPool();
		trackChunk.thisTransform.position = Vector3.forward * trackChunkZ;
		trackChunkZ += trackChunk.zSize;
		if (!activeTrackChunks.Contains(trackChunk))
		{
			activeTrackChunks.Add(trackChunk);
		}
		trackChunk.RestoreHiddenObstacles();
		yield return StartCoroutine(PerformRecursiveSelection(trackChunk.gameObject));
		Array.Sort(trackChunk.objects, delegate(TrackObject g1, TrackObject g2)
		{
			Vector3 position = g1.transform.position;
			ref float z = ref position.z;
			Vector3 position2 = g2.transform.position;
			return z.CompareTo(position2.z);
		});
		int j = 0;
		int cnt = trackChunk.objects.Length;
		for (int i = 0; i < cnt; i++)
		{
			TrackObject o = trackChunk.objects[i];
			if (o.gameObject.activeInHierarchy)
			{
				j++;
				o.Activate();
			}
			if (j == 1)
			{
				j = 0;
				yield return null;
			}
		}
	}

	private IEnumerator PerformRecursiveSelection(GameObject parent, bool sortSpawnPoints = true)
	{
		List<GameObjectWrapper> objectsToActivate = new List<GameObjectWrapper>();
		List<GameObject> objectsToVisit = new List<GameObject>();
		List<GameObjectWrapper> spawnPoints = new List<GameObjectWrapper>();
		objectsToVisit.Add(parent);
		while (objectsToVisit.Count > 0)
		{
			GameObject gameObject = objectsToVisit[0];
			objectsToVisit.RemoveAt(0);
			Selector component = gameObject.GetComponent<Selector>();
			if (sortSpawnPoints)
			{
				SpawnPoint x2 = null;
				try
				{
					x2 = (SpawnPoint)component;
				}
				catch
				{
				}
				if (x2 != null)
				{
					spawnPoints.Add(new GameObjectWrapper(gameObject));
					continue;
				}
				SpawnPointEgg x3 = null;
				try
				{
					x3 = (SpawnPointEgg)component;
				}
				catch
				{
				}
				if (x3 != null)
				{
					spawnPoints.Add(new GameObjectWrapper(gameObject));
					continue;
				}
			}
			RandomizeOffset component2 = gameObject.GetComponent<RandomizeOffset>();
			if (component2 != null)
			{
				component2.ChooseRandomOffset();
			}
			objectsToActivate.Add(new GameObjectWrapper(gameObject));
			if (component != null)
			{
				Transform thisTransform2 = component.thisTransform;
				component.PerformSelection(objectsToVisit);
				continue;
			}
			Transform transform = gameObject.transform;
			int childCount = transform.childCount;
			for (int k = 0; k < childCount; k++)
			{
				GameObject gameObject2 = transform.GetChild(k).gameObject;
				objectsToVisit.Add(gameObject2);
			}
		}
		List<GameObjectWrapper> lowPriority = (from x in objectsToActivate
			where IsLowPriority(x.GameObject)
			select x).ToList();
		objectsToActivate = (from x in objectsToActivate
			where !IsLowPriority(x.GameObject)
			select x).ToList();
		objectsToActivate.Sort((GameObjectWrapper x, GameObjectWrapper y) => x.Z.CompareTo(y.Z));
		lowPriority.Sort((GameObjectWrapper x, GameObjectWrapper y) => x.Z.CompareTo(y.Z));
		objectsToActivate.AddRange(lowPriority);
		int j = 0;
		int cnt = objectsToActivate.Count;
		for (int i = 0; i < cnt; i++)
		{
			GameObjectWrapper gameObjectWrapper = objectsToActivate[i];
			if (gameObjectWrapper.GameObject != null)
			{
				GameObjectWrapper gameObjectWrapper2 = objectsToActivate[i];
				gameObjectWrapper2.GameObject.SetActive(value: true);
			}
			j++;
			if (j == 4)
			{
				yield return null;
				j = 0;
			}
		}
		if (spawnPoints.Count > 0)
		{
			spawnPoints.Sort((GameObjectWrapper x, GameObjectWrapper y) => x.Z.CompareTo(y.Z));
			foreach (GameObjectWrapper item in spawnPoints)
			{
				GameObjectWrapper spawnPoint = item;
				yield return StartCoroutine(PerformRecursiveSelection(spawnPoint.GameObject, sortSpawnPoints: false));
			}
		}
	}

	private bool IsLowPriority(GameObject g)
	{
		return g.layer != 16;
	}

	public void CleanupTrackChunks(float characterZ)
	{
		float num = characterZ - cleanUpDistance;
		foreach (TrackChunk activeTrackChunk in activeTrackChunks)
		{
			Vector3 position = activeTrackChunk.transform.position;
			if (position.z + activeTrackChunk.zSize < num)
			{
				trackChunksForDeactivation.Add(activeTrackChunk);
			}
		}
		foreach (TrackChunk item in trackChunksForDeactivation)
		{
			if (!item.isTutorial)
			{
				item.Deactivate();
			}
			activeTrackChunks.Remove(item);
		}
		trackChunksForDeactivation.Clear();
	}

	public void DeactivateTrackChunks()
	{
		StopAllCoroutines();
		foreach (TrackChunk activeTrackChunk in activeTrackChunks)
		{
			activeTrackChunk.Deactivate();
		}
	}

	public void Restart()
	{
		RandomizerHold.Initialize();
		TrackChunk[] array = trackChunks.TrackChunks;
		foreach (TrackChunk trackChunk in array)
		{
			Vector3 position = trackChunk.transform.position;
			position.y = -1000f;
			trackChunk.transform.position = position;
		}
		trackChunkZ = 0f;
		trackChunks.Initialize(0f);
		foreach (TrackChunk activeTrackChunk in activeTrackChunks)
		{
			activeTrackChunk.Deactivate();
		}
		activeTrackChunks.Clear();
		firstTrackChunk = true;
	}

	private void TrackPositionGizmos()
	{
		for (int i = 0; i < numberOfTracks; i++)
		{
			Vector3 vector = Vector3.Lerp(trackLeft.position, trackRight.position, (float)i / (float)(numberOfTracks - 1));
			Gizmos.DrawLine(vector, vector + Vector3.forward * 5f);
		}
	}

	public void OnDrawGizmos()
	{
		TrackPositionGizmos();
	}

	public float GetLastCheckPoint(float characterZ)
	{
		foreach (TrackChunk activeTrackChunk in activeTrackChunks)
		{
			if (IsRunningOnTutorialTrack && activeTrackChunk == tutorialTrackChunk)
			{
				return activeTrackChunk.GetLastCheckPoint(characterZ);
			}
		}
		return 0f;
	}
}
