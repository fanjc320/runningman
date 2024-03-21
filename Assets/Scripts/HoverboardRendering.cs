using System;
using System.Collections.Generic;
using UnityEngine;

public class HoverboardRendering : MonoBehaviour
{
	[Serializable]
	public class AnimationStringPair
	{
		public string characterAnimation = string.Empty;

		public string hoverboardAnimation = string.Empty;
	}

	[Serializable]
	public class AnimationPair
	{
		public AnimationClip characterAnimationClip;

		public AnimationClip hoverboardAnimationClip;
	}

	public AnimationClip defaultHoverboardAnimation;

	public AnimationPair[] runAnimations;

	public AnimationPair[] landAnimations;

	public AnimationPair[] jumpAnimations;

	public AnimationPair[] hangtimeAnimations;

	public AnimationPair[] rollAnimations;

	public AnimationPair[] dodgeLeftAnimations;

	public AnimationPair[] dodgeRightAnimations;

	public AnimationPair[] grindAnimations;

	public AnimationPair[] grindLandAnimations;

	public AnimationPair[] getOnBoardAnimations;

	private bool hasInitializedAnimationSounds;

	public void Initialize(Animation avatarAnimation, Animation hoverboardAnimation, List<AnimationClip> addedClipsList)
	{
		CharacterRendering instance = CharacterRendering.Instance;
		if (jumpAnimations.Length != hangtimeAnimations.Length)
		{
		}
		instance.animations.RUN = GetNamesAddAnimationClips(runAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.LAND = GetNamesAddAnimationClips(landAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.JUMP = GetNamesAddAnimationClips(jumpAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.HANGTIME = GetNamesAddAnimationClips(hangtimeAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.ROLL = GetNamesAddAnimationClips(rollAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.DODGE_LEFT = GetNamesAddAnimationClips(dodgeLeftAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.DODGE_RIGHT = GetNamesAddAnimationClips(dodgeRightAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.GRIND = GetNamesAddAnimationClips(grindAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		GetNamesAddAnimationClips(grindLandAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		instance.animations.GET_ON_BOARD = GetNamesAddAnimationClips(getOnBoardAnimations, avatarAnimation, hoverboardAnimation, addedClipsList);
		if (hoverboardAnimation != null)
		{
			hoverboardAnimation.AddClip(defaultHoverboardAnimation, defaultHoverboardAnimation.name);
		}
		if (defaultHoverboardAnimation != null)
		{
			instance.animations.DEFAULT_HOVERBOARD_ANIMATION = defaultHoverboardAnimation.name;
		}
		InitialieAnimationSounds(avatarAnimation.gameObject);
	}

	private void InitialieAnimationSounds(GameObject characterAnimationGameObject)
	{
		if (!hasInitializedAnimationSounds)
		{
			hasInitializedAnimationSounds = true;
		}
	}

	private string[] GetNamesAddAnimationClips(AnimationPair[] pairs, Animation avaterAnimation, Animation hoverboardAnimation, List<AnimationClip> addedClipsList)
	{
		string[] array = new string[pairs.Length];
		for (int i = 0; i < pairs.Length; i++)
		{
			AnimationPair animationPair = pairs[i];
			AnimationClip characterAnimationClip = animationPair.characterAnimationClip;
			if (avaterAnimation != null)
			{
				AddClipsToAnimationComp(avaterAnimation, characterAnimationClip, addedClipsList);
			}
			if (hoverboardAnimation != null && animationPair.hoverboardAnimationClip != null)
			{
				AddClipsToAnimationComp(hoverboardAnimation, animationPair.hoverboardAnimationClip, null);
			}
			if (characterAnimationClip == null)
			{
				throw new Exception("character must be not null.");
			}
			array[i] = characterAnimationClip.name;
		}
		return array;
	}

	private void AddClipsToAnimationComp(Animation animation, AnimationClip clip, List<AnimationClip> addedClipsList)
	{
		if (!(clip != null))
		{
			return;
		}
		if (addedClipsList != null)
		{
			if (!addedClipsList.Contains(clip))
			{
				addedClipsList.Add(clip);
				animation.AddClip(clip, clip.name);
			}
		}
		else
		{
			animation.AddClip(clip, clip.name);
		}
	}
}
