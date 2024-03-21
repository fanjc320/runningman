using System.Collections;
using UnityEngine;

public class Revive : MonoBehaviour
{
	public delegate void OnSwitchToRunningDelegate();

	public delegate void OnReviveDelegate();

	private Game game;

	private Character character;

	public AudioStateLoop audioStateLoop;

	public float WaitForParticlesDelay = 0.144f;

	public float RemoveObstaclesDistance = 250f;

	[SerializeField]
	private ParticleSystem reviveParticle;

	[SerializeField]
	private GameObject ReviveFX;

	[SerializeField]
	private Animation ReviveAnimation;

	[SerializeField]
	private AnimationClip ReviveAnimationClip;

	public static Revive instance;

	public static Revive Instance => instance ?? (instance = UtilRMan.FindObject<Revive>());

	public event OnSwitchToRunningDelegate OnSwitchToRunning;

	public event OnReviveDelegate OnRevive;

	private void Awake()
	{
		game = Game.Instance;
		character = Character.Instance;
		ReviveFX.SetActive(value: false);
		reviveParticle.gameObject.SetActive(value: false);
	}

	private IEnumerator Start()
	{
		while (!Game.instance.isReady)
		{
			yield return 0;
		}
		reviveParticle.gameObject.SetActive(value: true);
	}

	public void SendRevive()
	{
		Character.Instance.enabled = false;
		Character.Instance.enabled = true;
		Character.Instance.gameObject.SetActive(value: false);
		Character.Instance.gameObject.SetActive(value: true);
		Character.Instance.RestartRevive();
		StartCoroutine(ReviveNow());
	}

	public void SendSkipRevive()
	{
		StartCoroutine(game.SkipRevive());
	}

	public void SFX_Reset()
	{
		reviveParticle.Stop();
		reviveParticle.gameObject.SetActive(value: false);
	}

	private IEnumerator ReviveNow()
	{
		reviveParticle.gameObject.SetActive(value: true);
		reviveParticle.Play();
		audioStateLoop.PlayReviveSound();
		float waitForParticlesDelay = WaitForParticlesDelay;
		Track.Instance.LayEmptyChunks(character.z, RemoveObstaclesDistance * Game.Instance.NormalizedGameSpeed);
		if (this.OnRevive != null)
		{
			this.OnRevive();
		}
		game.ReviveSet();
		if (this.OnSwitchToRunning != null)
		{
			this.OnSwitchToRunning();
		}
		character.IsJumping = true;
		character.IsFalling = false;
		character.verticalSpeed = character.CalculateJumpVerticalSpeed(15f);
		while (reviveParticle.IsAlive(withChildren: false))
		{
			yield return 0;
		}
		reviveParticle.gameObject.SetActive(value: false);
	}
}
