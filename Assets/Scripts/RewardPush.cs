using System;
using UnityEngine;

[Serializable]
public class RewardPush
{
	public string type;

	public string msg;

	public int value;

	public static RewardPush CreateFromJSON(string jsonString)
	{
		return JsonUtility.FromJson<RewardPush>(jsonString);
	}
}
