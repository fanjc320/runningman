using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class CoinMagnet : CharacterModifier
{
	public float pullSpeed = 150f;

	private CharacterController characterController;

	private Animation characterAnimation;

	private Character character;

	private OnTriggerObject coinMagnetCollider;

	private CharacterRendering characterRendering;

	private CharacterModel characterModel;

	private Transform coinEFX;

	private Game game;

	public AudioStateLoop audioStateLoop;

	public AudioClipInfo powerDownSound;

	public ActivePowerup Powerup;

	[SerializeField]
	private ParticleSystem magnetParticle;

	private void Awake()
	{
		LeanTween.delayedCall(0f, (Action)delegate
		{
			character = Character.Instance;
			characterController = character.characterController;
			coinMagnetCollider = character.coinMagnetCollider;
			coinEFX = character.CharacterPickupParticleSystem.CoinEFX.transform;
		});
		characterRendering = CharacterRendering.Instance;
		characterModel = characterRendering.CharacterModel;
		characterAnimation = characterRendering.characterAnimation;
		game = Game.Instance;
	}

	public override void Reset()
	{
		Paused = false;
		magnetParticle.loop = false;
		magnetParticle.Pause();
		magnetParticle.gameObject.SetActive(value: false);
	}

	public override IEnumerator Begin()
	{
		GameStats.Instance.pickedUpPowerups++;
		Paused = false;
		audioStateLoop.ChangeLoop(AudioState.Magnet);
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.coinmagnet);
		coinMagnetCollider.OnEnter = CoinHit;
		coinMagnetCollider.GetComponent<Collider>().enabled = true;
		base.enabled = true;
		stop = StopSignal.DONT_STOP;
		GameObjectPoolMT<PPItemMagnet>.Instance.GetNParent(Character.Instance.transform, null);
		magnetParticle.gameObject.SetActive(value: true);
		magnetParticle.loop = true;
		magnetParticle.Play();
		float baseDuration = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
			where s.ID == "3"
			select s).First().Pvalue;
		int paramLevel = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][2];
		float bonusValue = DataContainer.Instance.PlayerParamLevelTableRawByLevel[2].PPLevelRaws[paramLevel].Pvalue;
		float duration = baseDuration + bonusValue;
		while (duration > 0f && stop == StopSignal.DONT_STOP)
		{
			duration -= Time.deltaTime;
			coinEFX.position = character.transform.position;
			magnetParticle.transform.position = character.transform.position;
			yield return null;
		}
		magnetParticle.loop = false;
		magnetParticle.Pause();
		magnetParticle.gameObject.SetActive(value: false);
		coinMagnetCollider.GetComponent<Collider>().enabled = false;
		base.enabled = false;
		coinEFX.localPosition = CharacterPickupParticles.coinEfxOffset;
		audioStateLoop.ChangeLoop(AudioState.MagnetStop);
		if (duration <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.B))
		{
			MonoBehaviour.print("STOP");
			stop = StopSignal.STOP;
		}
	}

	public void CoinHit(Collider collider)
	{
		Coin component = collider.GetComponent<Coin>();
		Glow componentInChildren = collider.GetComponentInChildren<Glow>();
		if (component != null)
		{
			component.GetComponent<Collider>().enabled = false;
			StartCoroutine(Pull(component, componentInChildren));
		}
	}

	private IEnumerator Pull(Coin coin, Glow glow)
	{
		Transform pivot = coin.PivotTransform;
		Vector3 position = pivot.position;
		float distance = (position - characterController.transform.position).magnitude;
		if (glow != null)
		{
			Transform glowPivot = glow.transform;
			Vector3 glowPosition = glowPivot.position;
			yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.NormalizedGameSpeed), delegate(float t)
			{
				pivot.position = Vector3.Lerp(position, character.transform.position, t * t);
				glowPivot.position = Vector3.Lerp(glowPosition, character.transform.position, t * t);
			}));
		}
		else
		{
			yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.NormalizedGameSpeed), delegate
			{
			}));
		}
		Pickup pickup = coin.GetComponent<Pickup>();
		character.NotifyPickup(pickup);
		GameStats.Instance.coinsCoinMagnet++;
	}
}
