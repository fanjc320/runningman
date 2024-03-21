using System.Collections;
using UnityEngine;

public class LevelAnimTrackObject : MonoBehaviour
{
	private Animation anim;

	private void Awake()
	{
		anim = GetComponent<Animation>();
		anim.Stop(anim.clip.name);
		anim.Rewind(anim.clip.name);
		anim.Sample();
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.layer == 11 && collider.gameObject.name.Contains("Character"))
		{
			StartCoroutine(animate(anim));
		}
	}

	private IEnumerator animate(Animation anim)
	{
		yield return new WaitForEndOfFrame();
		anim[anim.clip.name].speed = 0.8f;
		anim.CrossFade(anim.clip.name, 0.1f, PlayMode.StopAll);
		yield return new WaitForSeconds(2f);
		anim.Stop(anim.clip.name);
		anim.Rewind(anim.clip.name);
		anim.Sample();
	}

	public void OnEnable()
	{
		anim.Rewind();
		anim.Play();
		anim.Sample();
		anim.Stop();
	}
}
