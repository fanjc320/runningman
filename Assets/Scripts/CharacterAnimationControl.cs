using UnityEngine;

public class CharacterAnimationControl : MonoBehaviour
{
	private Animation animationComponent;

	private void Awake()
	{
		animationComponent = base.gameObject.GetComponent<Animation>();
	}

	public void Play(string name)
	{
		animationComponent.Play(name);
	}

	public void Speed(string name, float speed)
	{
		animationComponent[name].speed = speed;
	}
}
