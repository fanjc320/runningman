using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationSoundPlayer : MonoBehaviour
{
	public Animation TargetAnimation;

	public List<KeyFrameAudio> AudioClips;

	public static List<string> nodesInitialized = new List<string>();

	private int nextAudioClipIndex;

	private List<KeyFrameAudio> AudioClipsInitialized = new List<KeyFrameAudio>();

	private void Start()
	{
		if (nodesInitialized.IndexOf(base.name) == -1)
		{
			nodesInitialized.Add(base.name);
			AudioClips = (from s in AudioClips
				where null != TargetAnimation.GetClip(s.clip)
				select s).ToList();
			foreach (KeyFrameAudio audioClip in AudioClips)
			{
				Add(audioClip);
			}
		}
	}

	private void OnDestroy()
	{
		nodesInitialized.Clear();
	}

	public void Add(KeyFrameAudio key)
	{
		if (key != null)
		{
			AnimationEvent animationEvent = new AnimationEvent();
			animationEvent.messageOptions = SendMessageOptions.RequireReceiver;
			animationEvent.time = (float)key.KeyFrame / TargetAnimation[key.clip].clip.frameRate;
			animationEvent.intParameter = nextAudioClipIndex;
			animationEvent.functionName = "PlayKeyframeAnimation";
			if (Globals.TryAddAnimationEvent(TargetAnimation, key.clip, animationEvent))
			{
				nextAudioClipIndex++;
				AudioClipsInitialized.Add(key);
			}
		}
	}

	public virtual void PlayKeyframeAnimation(int soundIndex)
	{
		if (soundIndex < AudioClipsInitialized.Count)
		{
			KeyFrameAudio keyFrameAudio = AudioClipsInitialized[soundIndex];
			if (keyFrameAudio.Callback != null)
			{
				keyFrameAudio.Callback(keyFrameAudio);
			}
			else
			{
				So.Instance.playSound(keyFrameAudio.Audio);
			}
		}
	}

	public void character()
	{
	}
}
