using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hoverboard : CharacterModifier
{
	[Serializable]
	public class HoverboardSelection
	{
		public Hoverboards.BoardType boardType;

		public GameObject hoverboardPrefab;
	}

	public delegate void OnSwitchToHoverboardDelegate(GameObject hoverbooard);

	public delegate void OnSwitchToRunningDelegate();

	public delegate void OnHoverboardJumpDelegate();

	public delegate void OnRunDelegate();

	public HoverboardSelection[] hoverboardSelector;

	public AudioClipInfo powerDownSound;

	public float cooldownDstance = 50f;

	public float slowMotionDistance = 90f;

	public float slowDownToScale = 0.3f;

	[HideInInspector]
	public bool isAllowed = true;

	private GameObject hoverboardRoot;

	public float WaitForParticlesDelay;

	public float RemoveObstaclesDistance = 250f;

	private Character character;

	private CharacterRendering characterRendering;

	private Track track;

	private float lastEndActivationTime;

	[HideInInspector]
	public bool isActive;

	public AudioClipInfo CrashSound;

	public AudioClipInfo StartSound;

	public ActivePowerup Powerup;

	public GameObject PartEff;

	public static Hoverboard instance;

	private HoverboardManager hoverboardManager;

	private Dictionary<Hoverboards.BoardType, GameObject> hoverboardThatMatchBoardType = new Dictionary<Hoverboards.BoardType, GameObject>();

	private PPItemHelmet helmetGetEff;

	private GameObject newBoard;

	public override bool ShouldPauseInJetpack => true;

	public static Hoverboard Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(Hoverboard)) as Hoverboard));

	public event OnSwitchToHoverboardDelegate OnSwitchToHoverboard;

	public event OnSwitchToRunningDelegate OnSwitchToRunning;

	public event OnHoverboardJumpDelegate OnJump;

	public event OnRunDelegate OnRun;

	public void Awake()
	{
		instance = this;
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterRendering.CharacterModelInitialized += CharacterModelInitialized;
		track = Track.Instance;
		hoverboardManager = HoverboardManager.Instance;
		HoverboardSelection[] array = hoverboardSelector;
		int num = 0;
		while (true)
		{
			if (num < array.Length)
			{
				HoverboardSelection hoverboardSelection = array[num];
				if (hoverboardThatMatchBoardType.ContainsKey(hoverboardSelection.boardType))
				{
					break;
				}
				hoverboardThatMatchBoardType.Add(hoverboardSelection.boardType, hoverboardSelection.hoverboardPrefab);
				num++;
				continue;
			}
			return;
		}
		throw new Exception("There are more hoverboards assigned to the hoverboard selection");
	}

	public GameObject GetActiveHoverboard()
	{
		return Game.Instance.HelmetPrefab;
	}

	private void CharacterModelInitialized(GameObject root)
	{
		hoverboardRoot = root;
	}

	public override void Reset()
	{
		character.immuneToCriticalHit = false;
		character.characterController.enabled = true;
		character.characterCollider.enabled = true;
		isActive = false;
		Time.timeScale = 1f;
		character.hoverboardCrashParticleSystem.gameObject.SetActive(value: false);
		if (null != helmetGetEff)
		{
			helmetGetEff.Dispose();
			helmetGetEff = null;
		}
		helmetGetEff = GameObjectPoolMT<PPItemHelmet>.Instance.GetNParent(character.transform, null);
		if (null != newBoard)
		{
			newBoard.SetActive(value: false);
			newBoard = null;
		}
	}

	public override IEnumerator Begin()
	{
		float num = Time.time - lastEndActivationTime;
		if (!isAllowed)
		{
			yield break;
		}
		bool bouncerBoard = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.bouncer;
		bool isLowrider = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.lowrider;
		bool isSpeedBoard = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.speedboard;
		bool isHotrod = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.hotrod;
		PlayerInfo.Instance.UseUpgrade(PowerupType.hoverboard);
		Paused = false;
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		isActive = true;
		newBoard = GetActiveHoverboard();
		newBoard.transform.parent = hoverboardRoot.transform;
		newBoard.transform.localScale = GetActiveHoverboard().transform.localScale;
		newBoard.transform.localPosition = GetActiveHoverboard().transform.localPosition;
		newBoard.transform.localRotation = GetActiveHoverboard().transform.localRotation;
		newBoard.SetActive(value: true);
		if (null != helmetGetEff)
		{
			helmetGetEff.Dispose();
			helmetGetEff = null;
		}
		helmetGetEff = GameObjectPoolMT<PPItemHelmet>.Instance.GetNParent(character.transform, null);
		So.Instance.playSound(StartSound);
		character.CharacterPickupParticleSystem.PickedUpDefaultPowerUp();
		character.immuneToCriticalHit = true;
		stop = StopSignal.DONT_STOP;
		float duration = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
			where s.ID == "2"
			select s).First().Pvalue;
		while (duration > 0f && stop == StopSignal.DONT_STOP)
		{
			duration -= Time.deltaTime;
			yield return null;
		}
		if (bouncerBoard)
		{
			character.superSneakers.SuperSneakersSuction.Remove(this);
		}
		if (isLowrider)
		{
			character.SqueezeCollider.Remove(this);
		}
		if (isSpeedBoard || isHotrod)
		{
			Game.Instance.DeactivateHighSpeed();
		}
		if (stop == StopSignal.DONT_STOP)
		{
			So.Instance.playSound(powerDownSound);
			if (this.OnSwitchToRunning != null)
			{
				this.OnSwitchToRunning();
			}
			if (character.IsFalling || character.IsJumping)
			{
				if (this.OnJump != null)
				{
					this.OnJump();
				}
			}
			else if (this.OnRun != null)
			{
				this.OnRun();
			}
			newBoard.SetActive(value: false);
			newBoard = null;
		}
		character.immuneToCriticalHit = false;
		isActive = false;
		lastEndActivationTime = Time.time;
		if (stop == StopSignal.STOP)
		{
			isActive = false;
			character.immuneToCriticalHit = false;
			character.hoverboardCrashParticleSystem.gameObject.SetActive(value: true);
			character.hoverboardCrashParticleSystem.Play();
			PlayCrashSound();
			if (this.OnSwitchToRunning != null)
			{
				this.OnSwitchToRunning();
			}
			if (this.OnJump != null)
			{
				this.OnJump();
			}
			newBoard.SetActive(value: false);
			newBoard = null;
			if (null != helmetGetEff)
			{
				helmetGetEff.Dispose();
				helmetGetEff = null;
			}
			float timeLeft = WaitForParticlesDelay;
			while (timeLeft > 0f)
			{
				timeLeft -= Time.deltaTime;
				yield return null;
			}
			Track.Instance.LayEmptyChunks(character.z, RemoveObstaclesDistance * Game.Instance.NormalizedGameSpeed);
			character.IsJumping = true;
			character.IsFalling = false;
			character.verticalSpeed = character.CalculateJumpVerticalSpeed(10f);
			float newSlowMotionDistance = slowMotionDistance * Game.Instance.NormalizedGameSpeed;
			float newCoolDownDist = cooldownDstance * Game.Instance.NormalizedGameSpeed;
			float distanceLeft = newSlowMotionDistance;
			bool didStopCooldown = false;
			while (distanceLeft > 0f)
			{
				distanceLeft -= Game.Instance.currentLevelSpeed * Time.deltaTime;
				newCoolDownDist -= Game.Instance.currentLevelSpeed * Time.deltaTime;
				if (newCoolDownDist < 0f && !didStopCooldown)
				{
					character.immuneToCriticalHit = false;
					didStopCooldown = true;
				}
				yield return null;
			}
			character.hoverboardCrashParticleSystem.gameObject.SetActive(value: false);
		}
		MainUIManager.Instance.StopBuffIcon(2);
	}

	public void PlayCrashSound()
	{
		So.Instance.playSound(CrashSound);
	}

	public override void Pause()
	{
		hoverboardRoot.SetActive(value: false);
		bool flag = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.speedboard;
		bool flag2 = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.speedboard;
		if (flag)
		{
			Game.Instance.DeactivateHighSpeed();
		}
		if (flag2)
		{
			Game.Instance.ActivateHighSpeed("hotrod");
		}
	}

	public override void Resume()
	{
		hoverboardRoot.SetActive(value: true);
		bool flag = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.speedboard;
		bool flag2 = HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.speedboard;
		if (flag)
		{
			Game.Instance.ActivateHighSpeed("speedboard");
		}
		if (flag2)
		{
			Game.Instance.ActivateHighSpeed("hotrod");
		}
	}
}
