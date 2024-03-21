using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarAnimations : MonoBehaviour
{
	public Animation Target;

	public bool PlayIdleAnimations;

	public int MinIdleTimes;

	public int MaxIdleTimes;

	public AnimationClip Breath;

	public List<AnimationClip> Idles;

	public List<AnimationClip> IdlesPopup;

	public List<AnimationClip> UnlockPopup;

	private bool usePopupIdles;

	private bool useUnlockIdles;

	public bool Paused;

	private IEnumerator animationRoutine;

	private float nextAnimationTime;

	private void Start()
	{
		Target = FindAnimationInParent(base.gameObject);
		if (!(Target == null) && PlayIdleAnimations)
		{
			if (usePopupIdles)
			{
				StartIdlePopupAnimations();
			}
			else if (useUnlockIdles)
			{
				StartUnlockAnimations();
			}
			else
			{
				StartIdleAnimations();
			}
		}
	}

	private Animation FindAnimationInParent(GameObject current)
	{
		Animation component = current.GetComponent<Animation>();
		if (component != null)
		{
			return component;
		}
		if (current.transform.parent != null)
		{
			return FindAnimationInParent(current.transform.parent.gameObject);
		}
		return null;
	}

	private void Update()
	{
		if (PlayIdleAnimations && animationRoutine != null && !Paused)
		{
			animationRoutine.MoveNext();
			if (useUnlockIdles && Target != null)
			{
				Target.transform.Rotate(0f, 50f * Time.deltaTime, 0f);
			}
		}
	}

	public void StartIdleAnimations()
	{
		PlayIdleAnimations = true;
		Paused = false;
		usePopupIdles = false;
		useUnlockIdles = false;
		Target.AddClip(Breath, Breath.name);
		foreach (AnimationClip idle in Idles)
		{
			Target.AddClip(idle, idle.name);
		}
		animationRoutine = Play();
		animationRoutine.MoveNext();
	}

	public void StartIdlePopupAnimations()
	{
		PlayIdleAnimations = true;
		Paused = false;
		usePopupIdles = true;
		useUnlockIdles = false;
		Target.AddClip(Breath, Breath.name);
		foreach (AnimationClip item in IdlesPopup)
		{
			Target.AddClip(item, item.name);
		}
		animationRoutine = Play();
		animationRoutine.MoveNext();
	}

	public void StartUnlockAnimations()
	{
		PlayIdleAnimations = true;
		Paused = false;
		usePopupIdles = false;
		useUnlockIdles = true;
		foreach (AnimationClip idle in Idles)
		{
			Target.AddClip(idle, idle.name);
		}
		animationRoutine = Play();
		animationRoutine.MoveNext();
	}

	public void StopIdleAnimations()
	{
		PlayIdleAnimations = false;
		animationRoutine = null;
	}

	public void PauseIdleAnimations()
	{
		Paused = true;
		IEnumerator enumerator = Target.GetComponent<Animation>().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				AnimationState animationState = (AnimationState)enumerator.Current;
				animationState.speed = 0f;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void ResumeIdleAnimations()
	{
		Paused = false;
		IEnumerator enumerator = Target.GetComponent<Animation>().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				AnimationState animationState = (AnimationState)enumerator.Current;
				animationState.speed = 1f;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	private IEnumerator Play()
	{
		AnimationClip selectedClip = null;
		while (PlayIdleAnimations)
		{
			List<AnimationClip> possibleClips = usePopupIdles ? IdlesPopup.FindAll((AnimationClip a) => a != selectedClip) : ((!useUnlockIdles) ? Idles.FindAll((AnimationClip a) => a != selectedClip) : Idles.FindAll((AnimationClip a) => a != selectedClip));
			if (possibleClips.Count > 0)
			{
				selectedClip = possibleClips[UnityEngine.Random.Range(0, possibleClips.Count)];
			}
			Target.Play(selectedClip.name);
			nextAnimationTime = selectedClip.length;
			while (nextAnimationTime > 0f)
			{
				nextAnimationTime -= Time.deltaTime;
				yield return null;
			}
			if (usePopupIdles || useUnlockIdles)
			{
				continue;
			}
			int count = UnityEngine.Random.Range(MinIdleTimes, MaxIdleTimes);
			for (int i = 0; i < count; i++)
			{
				Target.Play(Breath.name);
				nextAnimationTime = Breath.length;
				while (nextAnimationTime > 0f)
				{
					nextAnimationTime -= Time.deltaTime;
					yield return 0;
				}
			}
		}
		animationRoutine = null;
	}
}
