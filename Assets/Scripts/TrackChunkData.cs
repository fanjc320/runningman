using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TrackChunkData
{
	public string chunk_name;

	public Vector3 position;

	public Quaternion rotation;

	public Bounds groundBounds;

	public TrackPrefabObject[] grounds;

	public TrackPrefabObject[] objects;

	public TrackPrefabObject[] items;

	public float zSize = 40f;

	public int probability = 1;

	public float zMinimum;

	public bool zMaximumActive;

	public float zMaximum;

	public List<TrackChunk.TrackCheckPoint> CheckPoints;

	public bool wasDisabledDueToHoverBoard;

	public bool isTutorial;
}
