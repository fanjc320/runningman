using System.Collections;
using UnityEngine;

public class AudioStateLoop : MonoBehaviour
{
	public AudioSource musicPlayer;

	private float musicVolume;

	private float otherMusicVolume = 0.3f;

	private bool isOtherAudioPlaying;

	public float menuMusicVolume = 1f;

	public float ingameMusicVolume = 1f;

	public AudioClip jetpackLoop;

	public float jetpackVolume = 0.5f;

	public float jetpackMinPitch = 1f;

	public float jetpackMaxPitch = 1f;

	public AudioClip magnetLoop;

	public float magnetVolume = 1f;

	public float magnetMinPitch = 1f;

	public float magnetMaxPitch = 1f;

	public AudioClip mysteryBoxOpenSound;

	public float mysteryVolume = 1f;

	public AudioClip unlockSound;

	public float unlockSoundVolume = 1f;

	public AudioClip reviveSound;

	public float reviveVolume = 1f;

	public float revivePauseTime = 1f;

	public float reviveFadeUpTime = 2f;

	private AudioSource jetpackSource;

	private AudioSource magnetSource;

	private AudioSource mysterySource;

	private AudioSource reviveSource;

	private AudioSource unlockSource;

	private bool hasPlayedIntro;

	public float fadeDownTime = 0.5f;

	public float pauseTime = 3f;

	public float fadeUpTime = 4f;

	private void Awake()
	{
		jetpackSource = base.gameObject.AddComponent<AudioSource>();
		jetpackSource.clip = jetpackLoop;
		jetpackSource.volume = jetpackVolume;
		jetpackSource.loop = true;
		jetpackSource.playOnAwake = false;
		magnetSource = base.gameObject.AddComponent<AudioSource>();
		magnetSource.clip = magnetLoop;
		magnetSource.volume = magnetVolume;
		magnetSource.loop = true;
		magnetSource.playOnAwake = false;
		mysterySource = base.gameObject.AddComponent<AudioSource>();
		mysterySource.clip = mysteryBoxOpenSound;
		mysterySource.volume = mysteryVolume;
		mysterySource.playOnAwake = false;
		unlockSource = base.gameObject.AddComponent<AudioSource>();
		unlockSource.clip = unlockSound;
		unlockSource.volume = unlockSoundVolume;
		unlockSource.playOnAwake = false;
		reviveSource = base.gameObject.AddComponent<AudioSource>();
		reviveSource.clip = reviveSound;
		reviveSource.volume = reviveVolume;
		reviveSource.playOnAwake = false;
		musicVolume = ingameMusicVolume;
		UpdateMusicPlayer();
		musicPlayer.bypassEffects = true;
		musicPlayer.Play();
		musicPlayer.volume = ((!PlayerInfo.Instance.MusicOn) ? 0f : 0.3f);
		jetpackSource.volume = ((!PlayerInfo.Instance.MusicOn) ? 0f : 0.6f);
	}

	private void UpdateMusicPlayer()
	{
		musicPlayer.volume = ((!PlayerInfo.Instance.MusicOn) ? 0f : ((!isOtherAudioPlaying) ? 0.3f : 0f));
		jetpackSource.volume = ((!PlayerInfo.Instance.MusicOn) ? 0f : ((!isOtherAudioPlaying) ? 0.6f : 0f));
	}

	public void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			isOtherAudioPlaying = false;
			UpdateMusicPlayer();
		}
	}

	public void PlayMysteryBoxOpenSound()
	{
		mysterySource.Play();
		StartCoroutine(MusicFader(fadeUpTime, pauseTime));
	}

	public void PlayUnlockSound()
	{
		unlockSource.Play();
		StartCoroutine(MusicFader(fadeUpTime, pauseTime));
	}

	public void PlayReviveSound()
	{
		reviveSource.Play();
		StartCoroutine(MusicFader(reviveFadeUpTime, revivePauseTime));
	}

	private IEnumerator MusicFader(float timeFadeUp, float timePauseTime)
	{
		float counter2 = 0f;
		float startFade = musicVolume;
		float fadeSpeed2 = 1f / fadeDownTime;
		while (counter2 < 1f)
		{
			musicVolume = Mathf.Lerp(startFade, 0f, counter2);
			UpdateMusicPlayer();
			counter2 += Time.deltaTime * fadeSpeed2;
			yield return 0;
		}
		musicVolume = 0f;
		UpdateMusicPlayer();
		yield return new WaitForSeconds(timePauseTime);
		counter2 = 0f;
		fadeSpeed2 = 1f / timeFadeUp;
		while (counter2 < 1f)
		{
			musicVolume = Mathf.Lerp(0f, menuMusicVolume, counter2);
			UpdateMusicPlayer();
			counter2 += Time.deltaTime * fadeSpeed2;
			yield return null;
		}
		musicVolume = menuMusicVolume;
		UpdateMusicPlayer();
	}

	public void ChangeLoop(AudioState audioState)
	{
		switch (audioState)
		{
		case AudioState.Menu:
			if (hasPlayedIntro)
			{
				musicPlayer.bypassEffects = true;
				musicVolume = menuMusicVolume;
				UpdateMusicPlayer();
			}
			break;
		case AudioState.Ingame:
			if (hasPlayedIntro)
			{
				musicPlayer.timeSamples = 0;
			}
			else
			{
				hasPlayedIntro = true;
			}
			musicPlayer.bypassEffects = true;
			musicVolume = ingameMusicVolume;
			UpdateMusicPlayer();
			break;
		case AudioState.Jetpack:
			PlayLoop(jetpackSource, jetpackMaxPitch, jetpackMaxPitch);
			break;
		case AudioState.JetpackStop:
			StopLoop(jetpackSource);
			break;
		case AudioState.Magnet:
			PlayLoop(magnetSource, magnetMinPitch, magnetMaxPitch);
			break;
		case AudioState.MagnetStop:
			StopLoop(magnetSource);
			break;
		}
	}

	public void PlayLoop(AudioSource audioSource)
	{
		audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.1f);
		audioSource.Play();
	}

	public void PlayLoop(AudioSource audioSource, float minPitch, float maxPitch)
	{
		audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
		audioSource.Play();
	}

	public void StopLoop(AudioSource audioSource)
	{
		audioSource.Stop();
	}
}
