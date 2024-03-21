using System.Collections.Generic;
using UnityEngine;

public class NpcAnim : MonoBehaviour
{
	public Animation Target;

	public bool PlayIdleAnimations;

	public int MinIdleTimes;

	public int MaxIdleTimes;

	public AnimationClip Breath;

	public List<AnimationClip> Idles;

	public List<AnimationClip> IdlesPopup;

	public List<AnimationClip> UnlockPopup;

	private void Start()
	{
	}

	private void Update()
	{
	}
}
