public class RunnerAnimPlayer : AnimationSoundPlayer
{
	private Game game;

	public AudioClipInfo step;

	public AudioClipInfo jump;

	public AudioClipInfo roll;

	public AudioClipInfo landing;

	public AudioStateLoop audioStateLoop;

	public bool playPaintSound = true;

	private void Awake()
	{
		game = Game.Instance;
		string selectedCharAnimPrefix = PlayerInfo.Instance.SelectedCharAnimPrefix;
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 8,
			clip = selectedCharAnimPrefix + "running01",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 0,
			clip = selectedCharAnimPrefix + "running01",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 8,
			clip = selectedCharAnimPrefix + "running02",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 0,
			clip = selectedCharAnimPrefix + "running02",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 8,
			clip = selectedCharAnimPrefix + "running03",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 0,
			clip = selectedCharAnimPrefix + "running03",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 8,
			clip = selectedCharAnimPrefix + "running04",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 0,
			clip = selectedCharAnimPrefix + "running04",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 8,
			clip = selectedCharAnimPrefix + "running05",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 0,
			clip = selectedCharAnimPrefix + "running05",
			Audio = step
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 0,
			clip = selectedCharAnimPrefix + "rolling",
			Audio = roll
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 0,
			clip = selectedCharAnimPrefix + "jump_landing",
			Audio = landing
		});
		AudioClips.Add(new KeyFrameAudio
		{
			KeyFrame = 0,
			clip = selectedCharAnimPrefix + "jump_start",
			Audio = jump,
			Callback = PlayJumpSound
		});
	}

	public void PlayOrMutePaintSound(bool doPlay)
	{
		playPaintSound = doPlay;
	}

	public void PlayIdlePaintSound(KeyFrameAudio info)
	{
		if (playPaintSound)
		{
			So.Instance.playSound(info.Audio);
		}
	}

	public void PlayJumpSound(KeyFrameAudio info)
	{
		So.Instance.playSound(info.Audio);
	}
}
