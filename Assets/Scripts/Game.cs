using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour
{
	[Serializable]
	public struct PoolingPrefab
	{
		public GameObject ActionJump;

		public GameObject ActionRun;

		public GameObject EffectResult;

		public GameObject EffectRevive;

		public GameObject EffectRunningGaugeUp;

		public GameObject EffectMentalStar;

		public GameObject ItemHelmet;

		public GameObject ItemConfusion;

		public GameObject ItemDoubleCoin;

		public GameObject ItemCharacterCoin;

		public GameObject ItemDoubleJump;

		public GameObject ItemMagnet;

		public GameObject ItemRunningCoin;

		public GameObject UIBuffIcon;
	}

	public delegate void OnSpeedChangedDelegate(float speed);

	public delegate void OnNametagDelegate(Character.OnNametagAction action);

	[Serializable]
	public class SwipeInfo
	{
		public float distanceMin = 0.1f;

		public float doubleTapDuration = 0.3f;
	}

	[Serializable]
	public class SpeedInfo
	{
		public float min = 110f;

		public float max = 220f;

		public float rampUpDuration = 200f;
	}

	public delegate void OnGameOverDelegate(GameStats gameStats);

	public delegate void OnPauseChangeDelegate(bool pause);

	public delegate void OnStageMenuSequenceDelegate();

	public delegate void OnTopMenuDelegate();

	public delegate void OnIntroRunDelegate();

	[SerializeField]
	public GameObject HelmetPrefab;

	[SerializeField]
	public PoolingPrefab PoolingPrefabs;

	[HideInInspector]
	public bool isDead;

	public bool ingameTouchDetection = true;

	public bool canRevive;

	public float reviveWaitTime = 3f;

	[HideInInspector]
	public float currentSpeed;

	public float currentLevelSpeed = 30f;

	public float distancePerMeter = 8f;

	public TrackChunkDataCollection normalGameData;

	public TrackChunkDataCollection missionGameData;

	public SwipeInfo swipe;

	[HideInInspector]
	public bool _tryFever;

	[HideInInspector]
	public bool _tryMagnet;

	[HideInInspector]
	public bool _tryRStickerFever;

	[HideInInspector]
	public bool _tryExpUp;

	[HideInInspector]
	public bool stateRunningFever;

	public SpeedInfo speed;

	[SerializeField]
	private SpeedInfo highSpeedInfo;

	[SerializeField]
	private float highSpeedFOV = 10f;

	[SerializeField]
	private SpeedInfo hotrodSpeedInfo;

	[SerializeField]
	private float hotrodSpeedFOV = 5f;

	private SpeedInfo selectedBoardSpeedInfo;

	private float selectedBoardSpeedFOV;

	[SerializeField]
	private float trasitionTime = 1f;

	public float stumbleLerpElapsed = 1f;

	private SpeedInfo stumbleSpeedInfo = new SpeedInfo();

	public SpeedInfo currentSpeedInfo = new SpeedInfo();

	public float backToCheckpointDelayTime = 0.7f;

	public float backToCheckpointZoomTime = 1f;

	private bool goingBackToCheckpoint;

	public Transform introAnimation;

	private IEnumerator currentThread;

	private CharacterState characterState;

	[HideInInspector]
	public CharacterModifierCollection modifiers;

	private Swipe currentSwipe;

	private float lastTapTime = float.MinValue;

	public static bool HasLoaded;

	public static CharacterController characterController;

	public Character character;

	private CharacterRendering characterRendering;

	private Animation characterAnimation;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	public FollowingGuard enemies;

	private Revive revive;

	public Running running;

	private Jetpack jetpack;

	public static Game instance;

	public float startTime;

	private GameStats stats;

	public Action OnGameStarted;

	public Action OnGameEnded;

	public OnGameOverDelegate OnGameOver;

	public OnPauseChangeDelegate OnPauseChange;

	public OnStageMenuSequenceDelegate OnStageMenuSequence;

	public OnTopMenuDelegate OnTopMenu;

	public OnIntroRunDelegate OnIntroRun;

	private float waitTimeBeforeScreen = 3f;

	public Variable<bool> IsInGame;

	public Variable<bool> IsInTopMenu;

	public AudioStateLoop audioStateLoop;

	public AudioClipInfo DieSound;

	public bool awakeDone;

	public static bool isLevelSceneLoadComplete;

	public bool isReadyForSlideinPowerups;

	public bool wasPowerupButtonClicked;

	private bool _paused;

	private float elapsedGameTime;

	private float startIntroTime;

	public float changeIntroAnimationTime = 5f;

	private int introAnimationNum;

	public string[] introAnimationName = new string[3]
	{
		"idling",
		"intro1",
		"gameover"
	};

	public bool IsHitDeadByTrain;

	public float gameStartTime;

	public float startSpeed;

	public bool isReady;

	public bool isReadySecondary;

	public static Game DirectInstance => instance;

	public float ElapsedGameTime => elapsedGameTime;

	public bool isPaused => _paused;

	public Character Character => character;

	public CharacterState CharacterState => characterState;

	public CharacterModifierCollection Modifiers => modifiers;

	public Running Running => running;

	public Jetpack Jetpack => jetpack;

	public bool IsInJetpackMode => characterState == Jetpack;

	public bool HasSuperSneakers => modifiers.SuperSneakes.isActive;

	public float NormalizedGameSpeed => currentSpeed / speed.min;

	public static Game Instance => instance ?? (instance = UtilRMan.FindObject<Game>());

	public static CharacterController Charactercontroller => characterController ?? (characterController = (UnityEngine.Object.FindObjectOfType(typeof(CharacterController)) as CharacterController));

	public event OnSpeedChangedDelegate OnSpeedChanged;

	public event OnNametagDelegate OnNametag;

	public Game()
	{
		IsInGame = new Variable<bool>(initialValue: false);
		IsInTopMenu = new Variable<bool>(initialValue: false);
	}

	public void Awake()
	{
		instance = this;
		GameObjectPoolMT<PPActionJump>.Reset(1, PoolingPrefabs.ActionJump);
		GameObjectPoolMT<PPActionRun>.Reset(1, PoolingPrefabs.ActionRun);
		GameObjectPoolMT<PPEffResult>.Reset(5, PoolingPrefabs.EffectResult);
		GameObjectPoolMT<PPEffRevive>.Reset(1, PoolingPrefabs.EffectRevive);
		GameObjectPoolMT<PPEffRunningGaugeUp>.Reset(1, PoolingPrefabs.EffectRunningGaugeUp);
		GameObjectPoolMT<PPItemHelmet>.Reset(1, PoolingPrefabs.ItemHelmet);
		GameObjectPoolMT<PPItemConfusion>.Reset(1, PoolingPrefabs.ItemConfusion);
		GameObjectPoolMT<PPItemDoubleCoin>.Reset(1, PoolingPrefabs.ItemDoubleCoin);
		GameObjectPoolMT<PPItemDoubleJump>.Reset(1, PoolingPrefabs.ItemDoubleJump);
		GameObjectPoolMT<PPItemMagnet>.Reset(1, PoolingPrefabs.ItemMagnet);
		GameObjectPoolMT<PPItemRunningCoin>.Reset(1, PoolingPrefabs.ItemRunningCoin);
		GameObjectPoolMT<PPCharacterCoin>.Reset(1, PoolingPrefabs.ItemCharacterCoin);
		GameObjectPoolMT<PGOEffMental>.Reset(1, PoolingPrefabs.EffectMentalStar);
		GameObjectPoolMT<PGOBuffIcon>.Reset(Enum.GetValues(typeof(StartItemType)).Length + 1, PoolingPrefabs.UIBuffIcon);
		PlayerInfo.Instance.SelectedCharID = PlayerInfo.Instance.SelectedCharID;
		HasLoaded = true;
		character = Character.Instance;
		character.Initialize();
		character.OnHitByTrain += character_OnHitByTrain;
		characterRendering = CharacterRendering.Instance;
		characterAnimation = characterRendering.characterAnimation;
		characterCamera = CharacterCamera.Instance;
		characterCameraTransform = characterCamera.transform;
		running = Running.Instance;
		jetpack = Jetpack.Instance;
		jetpack.SFX_Reset();
		revive = Revive.Instance;
		enemies = FollowingGuard.Instance;
		FollowingGuard followingGuard = enemies;
		followingGuard.OnCatchPlayer = (FollowingGuard.OnCatchPlayerDelegate)Delegate.Combine(followingGuard.OnCatchPlayer, new FollowingGuard.OnCatchPlayerDelegate(OnCatchPlayer));
		modifiers = new CharacterModifierCollection();
		character.OnStumble += OnStumble;
		character.OnCriticalHit += OnCriticalHit;
		currentLevelSpeed = Speed(0f, currentSpeedInfo);
		stats = GameStats.Instance;
		awakeDone = true;
	}

	private void character_OnHitByTrain()
	{
		IsHitDeadByTrain = true;
	}

	public void OnDestroy()
	{
		instance = null;
	}

	public IEnumerator Start()
	{
		if (PlayerInfo.Instance.ThisGameType == GameType.MissionSingle)
		{
			missionGameData = Resources.Load<TrackChunkDataCollection>("GameLevelMissionScene_trackdata");
			missionGameData.InstantiateTrack();
			missionGameData.InstantiateTrackChunk();
		}
		else
		{
			normalGameData = Resources.Load<TrackChunkDataCollection>("GameLevelNormalScene_trackdata");
			normalGameData.InstantiateTrack();
			normalGameData.InstantiateTrackChunk();
		}
		enemies.Initialize();
		Track.Instance.Restart();
		Track.Instance.LayTrackChunks(0f);
		if (PlayerInfo.Instance.ThisGameType != GameType.Multi)
		{
			isReady = true;
			isReadySecondary = false;
			yield return 0;
			LoadingMergeLoader.Instance.ShowIndicater(isShow: false);
		}
		PlayerInfo.Instance.IsSenseBackBtn = (PlayerInfo.Instance.ThisGameType != GameType.Multi);
		if (PlayerInfo.Instance.ThisGameType == GameType.Multi)
		{
			isReady = true;
			isReadySecondary = false;
			RaceManager.Instance.PingUpdate();
			yield return 0;
			RaceManager.Instance.PingUpdate();
			yield return 0;
			RaceManager.Instance.PingUpdate();
			yield return 0;
			RaceManager.Instance.PingUpdate();
			yield return 0;
			RaceManager.Instance.PingUpdate();
			yield return 0;
			RaceManager.Instance.PingUpdate();
			yield return 0;
			RaceManager.Instance.PlayerReadyToStart();
			yield return 0;
			float checkTime = Time.timeSinceLevelLoad;
			while (RaceManager.Instance.State != RaceManager.RaceState.Start)
			{
				if (RaceManager.Instance.State != RaceManager.RaceState.Playing || Time.timeSinceLevelLoad - checkTime > 30f)
				{
					RaceManager.Instance.CleanUp();
					Resources.UnloadUnusedAssets();
					UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
					yield break;
				}
				yield return 0;
			}
			yield return new WaitForSeconds(1f);
			if (RaceManager.Instance.isHost)
			{
				yield return new WaitForSeconds(RaceManager.Instance.lantency * 0.5f);
			}
			isReady = true;
			isReadySecondary = true;
		}
		else
		{
			isReady = true;
			isReadySecondary = true;
		}
		currentThread = GameIntro();
		currentThread.MoveNext();
	}

	public void StartMagnet()
	{
		modifiers.Add(modifiers.CoinMagnet);
	}

	public void StartNewRun()
	{
		Jetpack.Instance.isLastFever = false;
		Jetpack.Instance.feverParticle.gameObject.SetActive(value: false);
		character.transform.Find("SmashTrackObject").GetComponent<Collider>().enabled = false;
		NpcEnemiesNew.Instance.Reset();
		MainUIManager.Instance.Reset();
		SaveMeManager.IS_PURCHASE_RUNNING_INGAME = false;
		IsInTopMenu.Value = false;
		IsInGame.Value = true;
		_tryRStickerFever = true;
		stateRunningFever = true;
		ChangeState(null, Intro());
		Running.Instance.resetNpcStartPositionZero();
		OnGameStarted?.Invoke();
	}

	public void Update()
	{
		if (!isReadySecondary)
		{
			return;
		}
		float num = 0f;
		if (IsInGame.Value)
		{
			elapsedGameTime += Time.deltaTime;
		}
		if (MainUIManager.Instance.Filter.gameObject.activeInHierarchy)
		{
			startTime += Time.deltaTime;
		}
		else
		{
			num = Time.time - startTime;
			if (1f > stumbleLerpElapsed)
			{
				stumbleLerpElapsed += Time.deltaTime;
				float num2 = Speed(num, currentSpeedInfo);
				currentLevelSpeed = Mathf.Lerp(num2 * 0.5f, num2, stumbleLerpElapsed);
			}
			else
			{
				currentLevelSpeed = Speed(num, currentSpeedInfo);
			}
		}
		currentThread.MoveNext();
		if (characterState != null)
		{
			modifiers.Update();
		}
		GameStats.Instance.UpdatePowerupTimes(Time.deltaTime);
		if (Time.time > startIntroTime + changeIntroAnimationTime)
		{
			startIntroTime = Time.time;
			introAnimationNum++;
			if (introAnimationNum >= introAnimationName.Length)
			{
				introAnimationNum = 0;
			}
		}
	}

	public void LayTrackChunks()
	{
		Track.Instance.LayTrackChunks(character.z);
	}

	private void HandleDebugControls()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.S))
		{
			modifiers.Add(modifiers.SuperSneakes);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.M))
		{
			modifiers.Add(modifiers.CoinMagnet);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.X))
		{
			modifiers.Stop();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
		{
			character.StartRace();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Z))
		{
			Modifiers.Add(Modifiers.Confuse);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0))
		{
			modifiers.Add(modifiers.DoubleCoin);
		}
		if (!(characterState != null))
		{
			return;
		}
		if (GameStats.Instance.IsConfuse)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
			{
				characterState.HandleSwipe(SwipeDir.Down);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
			{
				characterState.HandleSwipe(SwipeDir.Up);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
			{
				characterState.HandleSwipe(SwipeDir.Right);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
			{
				characterState.HandleSwipe(SwipeDir.Left);
			}
		}
		else
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow))
			{
				characterState.HandleSwipe(SwipeDir.Up);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow))
			{
				characterState.HandleSwipe(SwipeDir.Down);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow))
			{
				characterState.HandleSwipe(SwipeDir.Left);
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow))
			{
				characterState.HandleSwipe(SwipeDir.Right);
			}
		}
	}

	public void UpdateMeters()
	{
		int num = Mathf.RoundToInt(character.z / distancePerMeter);
		GameStats.Instance.meters = num;
		if ((float)PlayerInfo.Instance.HighMeters < stats.meters)
		{
			PlayerInfo.Instance.HighMeters = num;
		}
		else
		{
			PlayerInfo.Instance.HighMeters = PlayerInfo.Instance.HighMeters;
		}
	}

	public void ChangeState(CharacterState state)
	{
		characterState = state;
		if (state != null)
		{
			currentThread = state.Begin();
		}
	}

	public void ChangeState(CharacterState state, IEnumerator thread)
	{
		characterState = state;
		currentThread = thread;
	}

	public void ActivateJetpack()
	{
		if (characterState != Jetpack)
		{
			ChangeState(Jetpack);
		}
	}

	public void ActivateHighSpeed(string type)
	{
		if (type == "speedboard")
		{
			selectedBoardSpeedInfo = highSpeedInfo;
			selectedBoardSpeedFOV = highSpeedFOV;
		}
		else if (type == "hotrod")
		{
			selectedBoardSpeedInfo = hotrodSpeedInfo;
			selectedBoardSpeedFOV = hotrodSpeedFOV;
		}
		float fov_start = characterCamera.GetComponent<Camera>().fieldOfView;
		float fov_end = (Running.cameraFOV + selectedBoardSpeedFOV) / characterCamera.GetComponent<Camera>().aspect;
		StartCoroutine(pTween.To(trasitionTime, delegate(float t)
		{
			currentSpeedInfo.min = Mathf.Lerp(speed.min, selectedBoardSpeedInfo.min, t);
			currentSpeedInfo.max = Mathf.Lerp(speed.max, selectedBoardSpeedInfo.max, t);
			if (this.OnSpeedChanged != null)
			{
				this.OnSpeedChanged(Speed(Time.time - startTime, currentSpeedInfo));
			}
			characterCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
		}));
	}

	public void DeactivateHighSpeed()
	{
		float fov_start = characterCamera.GetComponent<Camera>().fieldOfView;
		float fov_end = Running.cameraFOV / characterCamera.GetComponent<Camera>().aspect;
		StartCoroutine(pTween.To(trasitionTime, delegate(float t)
		{
			currentSpeedInfo.min = Mathf.Lerp(selectedBoardSpeedInfo.min, speed.min, t);
			currentSpeedInfo.max = Mathf.Lerp(selectedBoardSpeedInfo.max, speed.max, t);
			if (this.OnSpeedChanged != null)
			{
				this.OnSpeedChanged(Speed(Time.time - startTime, currentSpeedInfo));
			}
			characterCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(fov_start, fov_end, t);
		}));
	}

	public float Speed(float t, SpeedInfo speedInfo)
	{
		if (t < speed.rampUpDuration)
		{
			return t * (speedInfo.max - speedInfo.min) / speed.rampUpDuration + speedInfo.min;
		}
		return speedInfo.max;
	}

	public float GetDuration()
	{
		return Time.time - startTime;
	}

	public void OnCriticalHit(Character.CriticalHitType type)
	{
		if (characterState != null)
		{
			So.Instance.playSound(DieSound);
			characterState.HandleCriticalHit();
			StartCoroutine(NametagSoundPlay());
			enemies.CatchPlayer(character.x - character.GetTrackX());
		}
	}

	private IEnumerator StumbleDeathSequence()
	{
		currentSpeed = speed.min;
		if (characterState != Jetpack && IsInGame.Value)
		{
			characterAnimation.CrossFade(PlayerInfo.Instance.SelectedCharAnimPrefix + "running01", 0.2f);
			if (characterState != null)
			{
				characterState.HandleCriticalHit();
			}
			StartCoroutine(NametagSoundPlay());
			enemies.CatchPlayer(character.x - character.GetTrackX());
		}
		yield break;
	}

	private IEnumerator NametagSoundPlay()
	{
		yield return new WaitForSeconds(1f);
		this.OnNametag(Character.OnNametagAction.Success);
	}

	public void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (character.IsStumbling)
		{
			if (characterState != null)
			{
				StartCoroutine(StumbleDeathSequence());
				return;
			}
			stumbleLerpElapsed = 0f;
			stumbleSpeedInfo.min = currentSpeedInfo.min;
			stumbleSpeedInfo.max = currentSpeedInfo.max;
			stumbleSpeedInfo.rampUpDuration = currentSpeedInfo.rampUpDuration;
		}
		else
		{
			stumbleLerpElapsed = 0f;
			stumbleSpeedInfo.min = currentSpeedInfo.min;
			stumbleSpeedInfo.max = currentSpeedInfo.max;
			stumbleSpeedInfo.rampUpDuration = currentSpeedInfo.rampUpDuration;
		}
	}

	private IEnumerator HitByNpcSequence()
	{
		characterCamera.Shake();
		CharacterRendering.Instance.characterAnimation.Play(PlayerInfo.Instance.SelectedCharAnimPrefix + "knock_back");
		if (!Track.Instance.IsRunningOnTutorialTrack)
		{
			this.OnNametag(Character.OnNametagAction.Success);
		}
		Die();
		yield return new WaitForSeconds(2f);
	}

	public void StartJetpack()
	{
		Jetpack.headStart = false;
		Jetpack.powerType = PowerupType.jetpack;
		ChangeState(Jetpack);
	}

	public void PickupJetpack()
	{
		Instance.StartJetpack();
		GameStats.Instance.jetpackPickups++;
	}

	public void StartTopMenu()
	{
		ChangeState(null, TopMenu());
	}

	public void StartHeadStart2000()
	{
		if (!isDead)
		{
			float powerupDuration = PlayerInfo.Instance.GetPowerupDuration(PowerupType.headstart2000);
			Jetpack.headStart = true;
			Jetpack.powerType = PowerupType.headstart2000;
			Jetpack.headStartDistance = powerupDuration * distancePerMeter;
			Jetpack.headStartSpeed = 1000f;
			ChangeState(Jetpack);
			PlayerInfo.Instance.UseUpgrade(PowerupType.headstart2000);
		}
	}

	public void StartHeadStart500()
	{
		if (!isDead)
		{
			float powerupDuration = PlayerInfo.Instance.GetPowerupDuration(PowerupType.headstart500);
			Jetpack.headStart = true;
			Jetpack.powerType = PowerupType.headstart500;
			Jetpack.headStartDistance = powerupDuration * distancePerMeter;
			Jetpack.headStartSpeed = 500f;
			ChangeState(Jetpack);
			PlayerInfo.Instance.UseUpgrade(PowerupType.headstart500);
		}
	}

	private void OnCatchPlayer(string currentCharacterCaught, float catchUpTime, float waitTime)
	{
		waitTimeBeforeScreen = reviveWaitTime;
	}

	public void Die()
	{
		if (modifiers.IsActive(modifiers.Hoverboard))
		{
			enemies.Restart(closeToCharacter: false);
			enemies.ResetModelRootPosition();
			enemies.ShowEnemies(vis: false);
			enemies.MuteProximityLoop();
			MainUIManager.Instance.StopBuffIcon(2);
			modifiers.Hoverboard.Stop = CharacterModifier.StopSignal.STOP;
			if (character.IsStumbling)
			{
				character.StopStumble();
			}
			GameStats.Instance.RemoveHoverBoardPowerup();
			return;
		}
		if (Track.Instance.IsRunningOnTutorialTrack)
		{
			if (!goingBackToCheckpoint)
			{
				StartCoroutine(BackToCheckPointSequence());
			}
			return;
		}
		bool flag = false;
		if (PlayerInfo.Instance.CharacterSkills[5] && 0.2f >= UnityEngine.Random.value)
		{
			PlayerInfo.Instance.CharacterSkills[5] = false;
			isDead = false;
			Instance.IsHitDeadByTrain = false;
			flag = true;
			Revive.Instance.SendRevive();
			return;
		}
		if (PlayerInfo.Instance.StartItems[6])
		{
			PlayerInfo.Instance.StartItems[6] = false;
			MainUIManager.Instance.StopBuffIcon(6);
			isDead = false;
			Instance.IsHitDeadByTrain = false;
			int num = UnityEngine.Random.Range(0, DataContainer.Instance.CharacterTableRaw.dataArray.Length - 1);
			PlayerInfo.Instance.SelectedCharID = DataContainer.Instance.CharacterTableRaw.dataArray[num].ID;
			Character.characterModel.ChangeModelFromPlayerInfo();
			Character.ResetCharacterSkill();
			characterRendering.ClearCacheClipsnameDict();
			flag = true;
			characterRendering.InitializeAnimations();
			characterRendering.OnSwitchToRunning();
			Revive.Instance.SendRevive();
			return;
		}
		canRevive = true;
		GameStats.Instance.ClearPowerups();
		isDead = true;
		MovingTrain.ActivateAutoPilot();
		MovingCoin.ActivateAutoPilot();
		if (flag)
		{
			if (character.IsStumbling)
			{
				character.isStumbling = false;
				enemies.Restart(closeToCharacter: false);
				enemies.ResetModelRootPosition();
			}
		}
		else if (enemies.isShowing)
		{
			if (characterAnimation[PlayerInfo.Instance.SelectedCharAnimPrefix + "running01"].enabled)
			{
				enemies.HitByTrainSequence();
			}
			else
			{
				enemies.CatchPlayer(character.x - character.GetTrackX());
			}
		}
		stats.duration = GetDuration();
		enemies.enabled = false;
		if (OnGameOver != null)
		{
			OnGameOver(stats);
		}
		OnGameEnded?.Invoke();
		StopAllCoroutines();
		ChangeState(null, SwitchToDieStateWhenGrounded());
	}

	private IEnumerator SwitchToDieStateWhenGrounded()
	{
		if (IsHitDeadByTrain)
		{
			yield return 0;
			yield return 0;
			IsHitDeadByTrain = false;
		}
		else
		{
			while (!character.characterController.isGrounded)
			{
				character.MoveWithGravity();
				yield return null;
			}
		}
		ChangeState(null, DieSequence());
	}

	private IEnumerator DieSequence()
	{
		if (PlayerInfo.Instance.ThisGameType == GameType.Multi)
		{
			if (RaceManager.Instance.State == RaceManager.RaceState.Start)
			{
				if (PlayerInfo.Instance.ThisGameOpponent.Dead)
				{
					GameStats.Instance.isWin = true;
				}
				else
				{
					GameStats.Instance.isWin = false;
				}
				RaceManager.Instance.PlayerDead();
			}
			MainUIManager.Instance.DoResultMultiPopup();
		}
		else
		{
			MainUIManager.Instance.DoResultPopup();
		}
		float wait = Time.time + waitTimeBeforeScreen;
		float skipTime = Time.time + 2.5f;
		while (Time.time < skipTime)
		{
			yield return null;
		}
		while (Time.time < wait && !Input.GetMouseButtonUp(0))
		{
			if (UnityEngine.Input.touchCount > 0)
			{
				Touch touch = Input.touches[0];
				if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					break;
				}
			}
			yield return null;
		}
		ingameTouchDetection = false;
	}

	public void ReviveSet()
	{
		if (Time.time - startTime >= speed.rampUpDuration)
		{
			startTime = Time.time - speed.rampUpDuration / 2f;
			currentLevelSpeed = Speed(Time.time - startTime, currentSpeedInfo);
		}
		else
		{
			startTime = Time.time - (Time.time - startTime) / 2f;
			currentLevelSpeed = Speed(Time.time - startTime, currentSpeedInfo);
		}
		stumbleLerpElapsed = 1f;
		enemies.enabled = true;
		enemies.MuteProximityLoop();
		enemies.ResetCatchUp();
		enemies.ResetModelRootPosition();
		enemies.ShowEnemies(vis: false);
		enemies.isShowing = false;
		character.StopStumble();
		modifiers.StopWithNoEnding();
		modifiers.Confuse.Stop = CharacterModifier.StopSignal.STOP;
		GameStats.Instance.IsConfuse = false;
		IsInGame.Value = true;
		ChangeState(Running);
		canRevive = false;
		isDead = false;
	}

	public IEnumerator SkipRevive()
	{
		ingameTouchDetection = false;
		canRevive = false;
		yield return new WaitForSeconds(0.1f);
		ChangeState(null, TopMenu());
	}

	private void StageMenuSequence()
	{
		enemies.enabled = false;
		enemies.ShowEnemies(vis: false);
		enemies.StopAllCoroutines();
		character.StopAllCoroutines();
		character.transform.position = Vector3.zero + new Vector3(0f, 0f, 0f);
		characterCamera.GetComponent<Camera>().fieldOfView = 76.44444f;
		characterCameraTransform.localPosition = character.transform.position + Running.cameraOffset + Vector3.up * 0.8f;
		characterCameraTransform.localRotation = Quaternion.Euler(21.50143f, 0f, 0f);
		if (OnStageMenuSequence != null)
		{
			OnStageMenuSequence();
		}
	}

	private IEnumerator GameIntro()
	{
		ChangeState(null, TopMenu());
		yield break;
	}

	private IEnumerator TopMenu()
	{
		IsInGame.Value = false;
		IsInTopMenu.Value = true;
		if (audioStateLoop != null)
		{
			audioStateLoop.ChangeLoop(AudioState.Menu);
		}
		enemies.MuteProximityLoop();
		Track.Instance.DeactivateTrackChunks();
		modifiers.StopWithNoEnding();
		modifiers.Update();
		GameStats.Instance.ClearPowerups();
		jetpack.coinsManager.ReleaseCoins();
		enemies.ShowEnemies(vis: false);
		StageMenuSequence();
		if (OnTopMenu != null)
		{
			OnTopMenu();
		}
		StartNewRun();
		yield break;
	}

	private IEnumerator Intro()
	{
		stats.Reset();
		modifiers.Stop();
		modifiers.Update();
		audioStateLoop.ChangeLoop(AudioState.Ingame);
		enemies.MuteProximityLoop();
		isDead = false;
		ingameTouchDetection = true;
		character.CharacterPickupParticleSystem.CoinEFX.transform.localPosition = CharacterPickupParticles.coinEfxOffset;
		StageMenuSequence();
		enemies.ShowEnemies(vis: true);
		enemies.PlayIntro();
		currentSpeedInfo.min = speed.min;
		currentSpeedInfo.max = speed.max;
		currentSpeedInfo.rampUpDuration = speed.rampUpDuration;
		currentLevelSpeed = Speed(0f, currentSpeedInfo);
		startSpeed = currentLevelSpeed;
		startTime = Time.time;
		character.Restart();
		SpawnPointManager.Instance.Restart();
		Track.Instance.Restart();
		Track.Instance.LayTrackChunks(0f);
		characterAnimation.CrossFade(PlayerInfo.Instance.SelectedCharAnimPrefix + "running01", 0.2f);
		if (OnIntroRun != null)
		{
			OnIntroRun();
		}
		IEnumerator cameraMovement = pTween.To(1f, delegate
		{
		});
		float time = Time.time;
		float fov_start = characterCamera.GetComponent<Camera>().fieldOfView;
		float fov_end = Running.cameraFOV / 0.5625f;
		while (cameraMovement.MoveNext())
		{
			characterCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(fov_start, fov_end, (Time.time - time) * 0.75f);
			yield return null;
		}
		characterCamera.GetComponent<Camera>().fieldOfView = fov_end;
		enemies.enabled = true;
		if (Track.Instance.IsRunningOnTutorialTrack)
		{
			enemies.ResetCatchUp();
		}
		if (Track.Instance.IsRunningOnTutorialTrack)
		{
			isReadyForSlideinPowerups = false;
		}
		else
		{
			isReadyForSlideinPowerups = true;
		}
		ChangeState(Running);
		yield return null;
	}

	public void TriggerPause(bool pauseGame)
	{
		_paused = pauseGame;
		if (pauseGame)
		{
			ingameTouchDetection = false;
			Time.timeScale = 0f;
		}
		else
		{
			ingameTouchDetection = true;
			Time.timeScale = 1f;
		}
		IsInGame.Value = !pauseGame;
		if (OnPauseChange != null)
		{
			OnPauseChange(_paused);
		}
	}

	private bool HandleTap()
	{
		bool result = false;
		if (Time.time < lastTapTime + swipe.doubleTapDuration && characterState != null && !wasPowerupButtonClicked)
		{
			characterState.HandleDoubleTap();
			result = true;
		}
		wasPowerupButtonClicked = false;
		lastTapTime = Time.time;
		return result;
	}

	public void HandleControls()
	{
		if (null != EventSystem.current.currentSelectedGameObject || _paused || UnityEngine.Input.touchCount <= 0)
		{
			return;
		}
		Touch touch = Input.touches[0];
		if (touch.phase == TouchPhase.Began)
		{
			currentSwipe = new Swipe();
			currentSwipe.start = touch.position;
			currentSwipe.startTime = Time.time;
		}
		if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && currentSwipe != null)
		{
			currentSwipe.endTime = Time.time;
			currentSwipe.end = touch.position;
			SwipeDir swipeDir = AnalyzeSwipe(currentSwipe);
			if (swipeDir != SwipeDir.None)
			{
				if (characterState != null)
				{
					characterState.HandleSwipe(swipeDir);
				}
				currentSwipe = null;
			}
		}
		if (touch.phase == TouchPhase.Ended && currentSwipe != null)
		{
			currentSwipe.endTime = Time.time;
			currentSwipe.end = touch.position;
			SwipeDir swipeDir2 = AnalyzeSwipe(currentSwipe);
			if (swipeDir2 == SwipeDir.None && characterState != null)
			{
				HandleTap();
			}
		}
	}

	private SwipeDir AnalyzeSwipe(Swipe swipe)
	{
		Camera camera = Camera.main;
		if (null == camera)
		{
			camera = (from cam in Camera.allCameras
				where null != cam && cam.enabled
				select cam).First();
		}
		Vector3 b = camera.ScreenToWorldPoint(new Vector3(swipe.start.x, swipe.start.y, 2f));
		Vector3 a = camera.ScreenToWorldPoint(new Vector3(swipe.end.x, swipe.end.y, 2f));
		float num = Vector3.Distance(a, b);
		if (num < this.swipe.distanceMin)
		{
			return SwipeDir.None;
		}
		Vector3 lhs = swipe.end - swipe.start;
		SwipeDir result = SwipeDir.None;
		float num2 = 0f;
		float num3 = Vector3.Dot(lhs, Vector3.up);
		if (num3 > num2)
		{
			num2 = num3;
			result = (GameStats.Instance.IsConfuse ? SwipeDir.Down : SwipeDir.Up);
		}
		num3 = Vector3.Dot(lhs, Vector3.down);
		if (num3 > num2)
		{
			num2 = num3;
			result = ((!GameStats.Instance.IsConfuse) ? SwipeDir.Down : SwipeDir.Up);
		}
		num3 = Vector3.Dot(lhs, Vector3.left);
		if (num3 > num2)
		{
			num2 = num3;
			result = ((!GameStats.Instance.IsConfuse) ? SwipeDir.Left : SwipeDir.Right);
		}
		num3 = Vector3.Dot(lhs, Vector3.right);
		if (num3 > num2)
		{
			num2 = num3;
			result = ((!GameStats.Instance.IsConfuse) ? SwipeDir.Right : SwipeDir.Left);
		}
		return result;
	}

	private IEnumerator BackToCheckPointSequence()
	{
		goingBackToCheckpoint = true;
		ChangeState(null);
		yield return new WaitForSeconds(backToCheckpointDelayTime);
		if (!IsInGame.Value)
		{
			goingBackToCheckpoint = false;
			yield break;
		}
		character.SetBackToCheckPoint(backToCheckpointZoomTime);
		yield return new WaitForSeconds(backToCheckpointZoomTime);
		goingBackToCheckpoint = false;
	}
}
