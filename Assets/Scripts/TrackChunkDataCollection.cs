using System.Collections.Generic;
using UnityEngine;

public class TrackChunkDataCollection : ScriptableObject
{
	public Vector3 trackLeft;

	public Vector3 trackRight;

	public int numberOfTracks = 3;

	public float cleanUpDistance = 2000f;

	public float trackAheadDistance = 700f;

	public Vector3 levelChunksParent;

	public GameObject jetpackLandingChunkPrefab;

	public Vector3 jetpackLandingChunkPrefabPosition;

	public bool tutorial;

	public string tutorialTrackChunkPrefab;

	public Vector3 tutorialTrackChunkPrefabPosition;

	public string introPrefab;

	public Vector3 introPrefabPosition;

	public GameObject trackChunkBasePrefab;

	public List<TrackChunkData> chunkList = new List<TrackChunkData>();

	public void InstantiateTrack()
	{
		GameObject gameObject = new GameObject("Track");
		Track track = gameObject.AddComponent<Track>();
		GameObject gameObject2 = new GameObject("TrackLeft");
		GameObject gameObject3 = new GameObject("TrackRight");
		track.trackLeft = gameObject2.transform;
		track.trackRight = gameObject3.transform;
		track.trackLeft.parent = gameObject.transform;
		track.trackRight.parent = gameObject.transform;
		track.trackLeft.position = trackLeft;
		track.trackRight.position = trackRight;
		track.numberOfTracks = numberOfTracks;
		track.cleanUpDistance = cleanUpDistance;
		track.trackAheadDistance = trackAheadDistance;
		track.tutorial = tutorial;
		track.Init();
		GameObject gameObject4 = UnityEngine.Object.Instantiate(jetpackLandingChunkPrefab);
		gameObject4.transform.parent = gameObject.transform;
		gameObject4.transform.position = jetpackLandingChunkPrefabPosition;
		track.jetpackLandingChunk = gameObject4.GetComponent<TrackChunk>();
		if (!PlayerInfo.Instance.TutorialCompleted)
		{
			GameObject gameObject5 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(tutorialTrackChunkPrefab));
			gameObject5.transform.SetParent(gameObject.transform, worldPositionStays: false);
			gameObject5.transform.position = tutorialTrackChunkPrefabPosition;
			track.tutorialTrackChunk = gameObject5.GetComponent<TrackChunk>();
		}
		GameObject gameObject6 = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(introPrefab));
		gameObject6.transform.SetParent(gameObject.transform, worldPositionStays: false);
		gameObject6.transform.position = introPrefabPosition;
	}

	public void InstantiateTrackChunk()
	{
		foreach (TrackChunkData chunk in chunkList)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("TrackChunkBase"));
			TrackChunk component = gameObject.GetComponent<TrackChunk>();
			gameObject.transform.parent = Track.instance.transform;
			gameObject.transform.position = chunk.position;
			GameObject gameObject2 = new GameObject("Ground");
			gameObject2.transform.parent = gameObject.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			BoxCollider componentInChildren = gameObject.GetComponentInChildren<BoxCollider>();
			componentInChildren.center = chunk.groundBounds.center;
			componentInChildren.size = chunk.groundBounds.size;
			component.probability = chunk.probability;
			component.zSize = chunk.zSize;
			component.zMaximumActive = chunk.zMaximumActive;
			component.zMaximum = chunk.zMaximum;
			component.zMinimum = chunk.zMinimum;
			component.wasDisabledDueToHoverBoard = chunk.wasDisabledDueToHoverBoard;
			component.isTutorial = chunk.isTutorial;
			component.chunkData = chunk;
			component.Init();
		}
	}
}
