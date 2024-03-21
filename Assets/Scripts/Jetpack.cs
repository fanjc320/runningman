using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Jetpack : CharacterState
{
	[Serializable]
	public class FlyAheadInfo
	{
		public AnimationCurve cameraMovement;
	}

	public delegate void OnStartDelegate(bool isHeadStart);

	public delegate void OnStopDelegate();

	public delegate void OnFlyAheadStartDelegate();

	public delegate void OnFlyAheadUpdateDelegate(float ratio);

	public bool isActive;

	public bool isLastFever;

	public AudioClipInfo powerDownSound;

	public Vector3 cameraOffset = new Vector3(0f, 32f, -34f);

	public float cameraOffsetSmoothDuration = 0.8f;

	public float cameraAimOffset = 18f;

	public float cameraFOV = 64f;

	public float ySmoothDuration = 0.5f;

	public float speedup = 2f;

	public float flyHeight = 1f;

	public float hitCeilingZPosition = 10f;

	public ParticleSystem ceilingBrickExpolsion;

	public float coinOffset = 200f;

	public float flyAheadDuration = 1.5f;

	private float flyingDuration;

	public float calmDownDuration = 2f;

	public float stopBeforeLandingChunkDistance = 50f;

	public float characterAngle = 45f;

	public float characterChangeTrackLength = 60f;

	public bool headStart;

	public float headStartDistance;

	public float headStartSpeed = 100f;

	public PowerupType powerType;

	public ActivePowerup Powerup;

	[SerializeField]
	public ParticleSystem feverParticle;

	public FlyAheadInfo flyAhead;

	public AudioClip AudFeverFinish;

	private Game game;

	private Character character;

	private CharacterController characterController;

	private Transform characterTransform;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	private Animation characterAnimation;

	public InAirCoinsManager coinsManager;

	private float landingZ;

	private float landingTime;

	public AudioStateLoop audioStateLoop;

	public AnimationCurve fisso;

	public static Jetpack instance;

	public OnStartDelegate OnStart;

	public OnStopDelegate OnStop;

	public OnFlyAheadStartDelegate OnFlyAheadStart;

	public OnFlyAheadUpdateDelegate OnFlyAheadUpdate;

	private Vector3 jetpackParticlesInitialRotation;

	private Vector3 jetpackParticlesInitialScale;

	public override bool PauseActiveModifiers => true;

	public float LandingZ => landingZ;

	public float LandingTime => landingTime;

	public static Jetpack Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(Jetpack)) as Jetpack));

	public event Action<bool> OnEvtFeverBeginEnd;

	public void Awake()
	{
		instance = this;
		game = Game.Instance;
		character = Character.Instance;
		characterController = character.characterController;
		characterTransform = characterController.transform;
		characterCamera = CharacterCamera.Instance;
		characterCameraTransform = characterCamera.transform;
		coinsManager = this.FindObject<InAirCoinsManager>();
	}

	public void SFX_Reset()
	{
		feverParticle.loop = false;
		feverParticle.Pause();
		feverParticle.gameObject.SetActive(value: false);
	}

	public override IEnumerator Begin()
	{
		isActive = true;
		if (this.OnEvtFeverBeginEnd != null)
		{
			this.OnEvtFeverBeginEnd(obj: true);
		}
		float time = Time.time;
		PlayerInfo.Instance.AccMissionByCondTypeID("dofever", "-1", 1.ToString());
		List<PickupDefault> clonePickupDefault = new List<PickupDefault>(PickupDefault.ActivatedPickups);
		clonePickupDefault.All(delegate(PickupDefault s)
		{
			s.OnDeactivate();
			return true;
		});
		Character.Instance.characterModel.shadow.enabled = true;
		Collider smashObjCollider = character.transform.Find("SmashTrackObject").GetComponent<Collider>();
		smashObjCollider.enabled = true;
		CoinPool.Instance.InvisibleAllCoins();
		PPActionRun runActionEff = GameObjectPoolMT<PPActionRun>.Instance.GetNParent(character.transform, null);
		Material chMat = Character.Instance.characterModel.model.material;
		if (powerType != PowerupType.headstart2000 && powerType != PowerupType.headstart500)
		{
			GameStats.Instance.pickedUpPowerups++;
		}
		Powerup = GameStats.Instance.TriggerPowerup(powerType);
		audioStateLoop.ChangeLoop(AudioState.Jetpack);
		if (character.IsStumbling)
		{
			character.StopStumble();
		}
		NotifyOnStart(headStart);
		Vector3 startCameraOffset = characterCameraTransform.position - characterTransform.position;
		float startCameraAimOffset = game.Running.cameraAimOffset;
		Vector3 position = characterTransform.position;
		float startY = position.y;
		characterController.detectCollisions = false;
		character.characterCollider.enabled = false;
		Vector3 position2 = characterTransform.position;
		SmoothDampFloat y = new SmoothDampFloat(position2.y, ySmoothDuration)
		{
			Target = flyHeight
		};
		float currentLevelSpeed = Game.Instance.Speed(Time.time - Game.Instance.startTime, Game.Instance.currentSpeedInfo);
		float jetpackSpeed = (!headStart) ? (currentLevelSpeed * speedup) : headStartSpeed;
		float feverBaseDur = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
			where s.ID == "1"
			select s).First().Pvalue;
		int feverParamLevel = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][1];
		float bonusValue = DataContainer.Instance.PlayerParamLevelTableRawByLevel[1].PPLevelRaws[feverParamLevel].Pvalue;
		float flyingDuration = feverBaseDur + bonusValue;
		flyAheadDuration = 0f;
		float flyDistance2 = jetpackSpeed * flyingDuration;
		float flyAheadDistance = jetpackSpeed * flyAheadDuration;
		float jetpackDistance2 = flyAheadDistance + flyDistance2;
		jetpackDistance2 = Track.Instance.LayJetpackChunks(character.z, jetpackDistance2) - stopBeforeLandingChunkDistance * Game.Instance.NormalizedGameSpeed;
		float extendedJetpackDuration = flyDistance2 / jetpackSpeed;
		float extendedFlyDuration = flyingDuration;
		flyDistance2 = extendedFlyDuration * jetpackSpeed;
		float length = flyDistance2 - coinOffset;
		float startZ = character.z + flyAheadDistance + coinOffset;
		coinsManager.Spawn(startZ, length, flyHeight);
		landingTime = Time.time + extendedJetpackDuration;
		landingZ = character.z + jetpackDistance2;
		float cameraZ = character.z;
		float startTime2 = Time.time;
		float ratio2 = 1f;
		game.currentSpeed = jetpackSpeed;
		Vector3 cameraPositionStart = characterCamera.position;
		Vector3 cameraTargetStart = characterCamera.target;
		if (OnFlyAheadStart != null)
		{
			OnFlyAheadStart();
		}
		bool hasExploded = false;
		feverParticle.gameObject.SetActive(value: true);
		feverParticle.loop = true;
		feverParticle.Play();
		while (ratio2 < 1f)
		{
			game.HandleControls();
			Vector3 currentCameraOffset = Vector3.Lerp(startCameraOffset, cameraOffset, Mathf.SmoothStep(0f, 1f, ratio2));
			float characterAimOffset = Mathf.Lerp(startCameraAimOffset, cameraAimOffset, Mathf.SmoothStep(0f, 1f, ratio2));
			character.z += jetpackSpeed * Time.deltaTime;
			Vector3 pivot = Track.Instance.GetPosition(character.x, character.z) + Vector3.up * (startY + (flyHeight - startY) * Mathf.SmoothStep(0f, 1f, ratio2));
			characterTransform.position = pivot;
			cameraZ += ((!headStart) ? currentLevelSpeed : jetpackSpeed) * Time.deltaTime;
			Vector3 characterPosition = Track.Instance.GetPosition(character.x, character.z);
			Vector3 cameraPositionEnd = new Vector3(characterPosition.x, pivot.y, character.z) + currentCameraOffset;
			Vector3 cameraTargetEnd = pivot + Vector3.up * characterAimOffset;
			float warpedRatio = flyAhead.cameraMovement.Evaluate(ratio2);
			Vector3 cameraPositionNew = Vector3.Lerp(cameraPositionStart, cameraPositionEnd, warpedRatio);
			Vector3 cameraTargetNew = Vector3.Lerp(cameraTargetStart, cameraTargetEnd, warpedRatio);
			characterCamera.position = cameraPositionNew;
			characterCamera.target = cameraTargetNew;
			if (OnFlyAheadUpdate != null)
			{
				OnFlyAheadUpdate(ratio2);
			}
			if (!hasExploded && pivot.y > hitCeilingZPosition && character.IsInsideSubway)
			{
				hasExploded = true;
				character.ForceLeaveSubway();
			}
			game.UpdateMeters();
			game.LayTrackChunks();
			yield return null;
			ratio2 = (Time.time - startTime2) / flyAheadDuration;
		}
		character.characterCollider.enabled = true;
		startTime2 = Time.time;
		ratio2 = (Time.time - startTime2) / extendedFlyDuration;
		Color chAddColor = Color.clear;
		bool isFinishEffecting = false;
		Camera mainCam = Camera.main;
		while (ratio2 < 1f)
		{
			game.HandleControls();
			character.z += jetpackSpeed * Time.deltaTime;
			character.transform.position = Track.Instance.GetPosition(character.x, character.z) + Vector3.up * flyHeight;
			Vector3 characterPosition2 = character.transform.position;
			characterCamera.position = characterPosition2 + cameraOffset;
			characterCamera.target = characterPosition2 + Vector3.up * cameraAimOffset;
			game.UpdateMeters();
			game.LayTrackChunks();
			yield return null;
			if (0.8f <= ratio2)
			{
				float num = ratio2 - 0.8f;
				chAddColor.a = Mathf.PingPong(num / 0.2f * 20f, 1f);
				chMat.SetColor("_AddColor", chAddColor);
			}
			if (!isFinishEffecting && 0.05f > extendedFlyDuration - (Time.time - startTime2))
			{
				isFinishEffecting = true;
			}
			ratio2 = (Time.time - startTime2) / extendedFlyDuration;
		}
		Camera.main.GetComponent<AudioSource>().PlayOneShot(AudFeverFinish);
		mainCam.transform.Find("FeverEndBoom").gameObject.SetActive(value: true);
		feverParticle.loop = false;
		feverParticle.Pause();
		feverParticle.gameObject.SetActive(value: false);
		audioStateLoop.ChangeLoop(AudioState.JetpackStop);
		isActive = false;
		NotifyOnStop();
		characterController.detectCollisions = true;
		coinsManager.ReleaseCoins();
		game._tryRStickerFever = true;
		game.stateRunningFever = true;
		GameStats.Instance.saveMeTokenPickup = 0;
		if (this.OnEvtFeverBeginEnd != null)
		{
			this.OnEvtFeverBeginEnd(obj: false);
		}
		runActionEff.Dispose();
		chMat.SetColor("_AddColor", Color.clear);
		StartCoroutine(endFeverSafely((BoxCollider)smashObjCollider));
		if (isLastFever)
		{
			CharacterRendering.Instance.characterAnimation.Play(PlayerInfo.Instance.SelectedCharAnimPrefix + "knock_back");
			PlayerInfo.Instance.StartItems[1] = false;
		}
		else
		{
			CharacterRendering.Instance.characterAnimation.CrossFade(CharacterRendering.Instance.animations.RUN[0], 0.1f);
			game.Modifiers.Resume();
			game.ChangeState(game.Running);
		}
	}

	private IEnumerator endFeverSafely(BoxCollider smashObjCollider)
	{
		Vector3 orgSize = smashObjCollider.size;
		smashObjCollider.size = orgSize * 5f;
		yield return new WaitForFixedUpdate();
		smashObjCollider.size = orgSize;
		smashObjCollider.enabled = false;
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		switch (swipeDir)
		{
		case SwipeDir.Left:
			character.ChangeTrack(-1, Mathf.Min(0.22f, characterChangeTrackLength / game.currentSpeed));
			break;
		case SwipeDir.Right:
			character.ChangeTrack(1, Mathf.Min(0.22f, characterChangeTrackLength / game.currentSpeed));
			break;
		}
	}

	private void NotifyOnStart(bool isHeadstart)
	{
		if (OnStart != null)
		{
			OnStart(isHeadstart);
		}
	}

	private void NotifyOnStop()
	{
		Running.Instance.resetNpcStartPosition(isStopByFever: true);
		if (OnStop != null)
		{
			OnStop();
		}
	}
}
