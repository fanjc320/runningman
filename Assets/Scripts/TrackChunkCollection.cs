using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackChunkCollection
{
	public static List<TrackChunk> trackChunks = new List<TrackChunk>();

	private List<TrackChunk> activeTrackChunks = new List<TrackChunk>();

	private List<TrackChunk> rangedActiveTrackChunks = new List<TrackChunk>();

	private List<int> rangedSeqSpace = new List<int>();

	private int lastAddedIndex = -1;

	private List<int> randomSpace = new List<int>();

	private static System.Random rng = new System.Random((int)DateTime.Now.Ticks);

	public int RandomSpaceCount => randomSpace.Count;

	public TrackChunk[] TrackChunks => trackChunks.ToArray();

	public static void Shuffle(List<int> list)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = rng.Next(num + 1);
			int value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}

	public static void AddToChunks(TrackChunk newTrackChunk)
	{
		int count = trackChunks.Count;
		if (count == 0)
		{
			trackChunks.Add(newTrackChunk);
			return;
		}
		int num = 0;
		while (trackChunks[num].zMinimum < newTrackChunk.zMinimum)
		{
			num++;
			if (num == count)
			{
				break;
			}
		}
		trackChunks.Insert(num, newTrackChunk);
	}

	public void Initialize(float z)
	{
		activeTrackChunks.Clear();
		lastAddedIndex = -1;
		int count = trackChunks.Count;
		for (int i = 0; i < count; i++)
		{
			TrackChunk trackChunk = trackChunks[i];
			if (trackChunk.zMinimum <= z && z < trackChunk.zMaximum)
			{
				activeTrackChunks.Add(trackChunk);
				lastAddedIndex = i;
			}
		}
		Recalculate();
	}

	public void MoveForward(float z)
	{
		int num = 0;
		int count = trackChunks.Count;
		for (int i = lastAddedIndex + 1; i < count; i++)
		{
			TrackChunk trackChunk2 = trackChunks[i];
			if (trackChunk2.zMinimum > z)
			{
				break;
			}
			activeTrackChunks.Add(trackChunk2);
			rangedSeqSpace.Add(i);
			num++;
			lastAddedIndex = i;
		}
		int num2 = activeTrackChunks.RemoveAll((TrackChunk trackChunk) => trackChunk.zMaximum < z);
		rangedSeqSpace.RemoveAll((int idx) => trackChunks[idx].zMaximum < z);
		if (num > 0 || num2 > 0)
		{
			Recalculate();
		}
	}

	public TrackChunk AddRangedActiveTrackChunk()
	{
		TrackChunk trackChunk = null;
		List<int> list = new List<int>();
		list.AddRange(rangedSeqSpace);
		int count = rangedSeqSpace.Count;
		trackChunk = trackChunks[rangedSeqSpace[count - 1]];
		rangedSeqSpace.RemoveAt(count - 1);
		return trackChunk;
	}

	private void Recalculate()
	{
		randomSpace.Clear();
		int count = activeTrackChunks.Count;
		for (int i = 0; i < count; i++)
		{
			TrackChunk trackChunk = activeTrackChunks[i];
			for (int j = 0; j < trackChunk.probability; j++)
			{
				randomSpace.Add(i);
			}
		}
		Shuffle(randomSpace);
	}

	public bool CanDeliver()
	{
		return randomSpace.Count > 0;
	}

	public TrackChunk GetRandomActive()
	{
		int index = UnityEngine.Random.Range(0, randomSpace.Count);
		int index2 = randomSpace[index];
		return activeTrackChunks[index2];
	}

	public TrackChunk GetShuffleActive(int index)
	{
		int index2 = randomSpace[index];
		return activeTrackChunks[index2];
	}

	public TrackChunk GetJetPakChunk(int index)
	{
		TrackChunk trackChunk = trackChunks[trackChunks.Count - 1 - index];
		if (trackChunk.zMaximum > 0f || trackChunk.zMinimum < 1000000f)
		{
		}
		return trackChunk;
	}
}
