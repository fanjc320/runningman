using System.Collections;
using UnityEngine;

public class Sound
{
	private So _manager;

	private GameObject gameObject;

	public AudioSource audioSource;

	public bool available = true;

	public bool destroyAfterPlay;

	private string _name;

	public string name => _name;

	public bool loop
	{
		set
		{
			audioSource.loop = value;
		}
	}

	public Sound(So manager)
	{
		_manager = manager;
		gameObject = new GameObject();
		_name = "empty";
		gameObject.name = _name;
		gameObject.transform.parent = manager.transform;
		audioSource = gameObject.AddComponent<AudioSource>();
	}

	public void destroySelf()
	{
		_manager.removeSound(this);
		UnityEngine.Object.Destroy(gameObject);
	}

	public void stop()
	{
		audioSource.Stop();
		destroySelf();
	}

	public IEnumerator fadeOutAndStop(float duration)
	{
		return audioSource.fadeOut(duration, delegate
		{
			stop();
		});
	}

	public IEnumerator playAudioClip(AudioClip audioClip, AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position)
	{
		_name = audioClip.name;
		gameObject.name = _name;
		audioSource.clip = audioClip;
		audioSource.volume = Random.Range(minVolume, maxVolume);
		audioSource.pitch = Random.Range(minPitch, maxPitch);
		return play(rolloff, minVolume, maxVolume, minPitch, maxPitch, position);
	}

	public IEnumerator play(AudioRolloffMode rolloff, float minVolume, float maxVolume, float minPitch, float maxPitch, Vector3 position)
	{
		available = false;
		gameObject.transform.position = position;
		audioSource.rolloffMode = rolloff;
		audioSource.volume = Random.Range(minVolume, maxVolume);
		audioSource.pitch = Random.Range(minPitch, maxPitch);
		audioSource.GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(audioSource.clip.length + 0.1f);
		audioSource.Stop();
		if (destroyAfterPlay)
		{
			destroySelf();
		}
		available = true;
	}
}
