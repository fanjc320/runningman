using System;
using System.Collections;
using UnityEngine;

public class SuperSneakers : CharacterModifier
{
	public delegate void OnSwitchToSuperSneakersDelegate();

	public delegate void SuperSneakerOnStopDelegate();

	[HideInInspector]
	public bool isActive;

	public float pullSpeed = 200f;

	private CharacterController characterController;

	private Character character;

	private OnTriggerObject coinMagnetCollider;

	private CharacterRendering characterRendering;

	private CharacterModel characterModel;

	private Game game;

	public AudioClipInfo powerDownSound;

	public ActivePowerup Powerup;

	private VariableBool superSneakersSuction = new VariableBool();

	public override bool ShouldPauseInJetpack => true;

	public VariableBool SuperSneakersSuction => superSneakersSuction;

	public event OnSwitchToSuperSneakersDelegate OnSwitchToSuperSneakers;

	public event SuperSneakerOnStopDelegate SuperSneakerOnStop;

	public void Awake()
	{
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterModel = characterRendering.CharacterModel;
		coinMagnetCollider = character.coinMagnetLongCollider;
		characterController = character.characterController;
		game = Game.Instance;
		Variable<bool> isInGame = game.IsInGame;
		isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, (Variable<bool>.OnChangeDelegate)delegate(bool value)
		{
			if (value)
			{
				superSneakersSuction.Clear();
			}
		});
		VariableBool variableBool = superSneakersSuction;
		variableBool.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(variableBool.OnChange, (VariableBool.OnChangeDelegate)delegate(bool value)
		{
			if (value)
			{
				coinMagnetCollider.OnEnter = CoinHit;
				coinMagnetCollider.GetComponent<Collider>().enabled = true;
			}
			else
			{
				coinMagnetCollider.GetComponent<Collider>().enabled = false;
				OnTriggerObject onTriggerObject = coinMagnetCollider;
				onTriggerObject.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Remove(onTriggerObject.OnEnter, new OnTriggerObject.OnEnterDelegate(CoinHit));
			}
		});
	}

	public override void Reset()
	{
		Paused = false;
	}

	public override IEnumerator Begin()
	{
		character.IsJumpingHigher = true;
		GameStats.Instance.pickedUpPowerups++;
		Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		isActive = true;
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.supersneakers);
		superSneakersSuction.Add(this);
		if (this.OnSwitchToSuperSneakers != null)
		{
			this.OnSwitchToSuperSneakers();
		}
		stop = StopSignal.DONT_STOP;
		while (Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			yield return null;
		}
		superSneakersSuction.Remove(this);
		isActive = false;
		if (Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
		if (this.SuperSneakerOnStop != null)
		{
			this.SuperSneakerOnStop();
		}
		character.IsJumpingHigher = false;
	}

	public void CoinHit(Collider collider)
	{
		Coin component = collider.GetComponent<Coin>();
		if (component != null)
		{
			component.GetComponent<Collider>().enabled = false;
			StartCoroutine(Pull(component));
			return;
		}
		Pickup componentInChildren = collider.GetComponentInChildren<Pickup>();
		if (componentInChildren != null)
		{
			componentInChildren.NotifyPickup(character.CharacterPickupParticleSystem);
		}
	}

	private IEnumerator Pull(Coin coin)
	{
		Transform pivot = coin.PivotTransform;
		Vector3 position = pivot.position;
		float distance = (position - characterController.transform.position).magnitude;
		new Vector3(0f, -6f, 0f);
		yield return StartCoroutine(pTween.To(distance / (pullSpeed * game.NormalizedGameSpeed), delegate
		{
		}));
		Pickup pickup = coin.GetComponent<Pickup>();
		character.NotifyPickup(pickup);
	}
}
