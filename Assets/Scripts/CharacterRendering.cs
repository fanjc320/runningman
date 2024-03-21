using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CharacterRendering : MonoBehaviour
{
	public delegate void CharacterModelInitializedDelegate(GameObject hoverboardRoot);

	public class Animations
	{
		public string[] TOP_MENU = new string[1]
		{
			"idling"
		};

		public string[] RUN = new string[1]
		{
			"running01"
		};

		public string[] LAND = new string[1]
		{
			"jump_land"
		};

		public string[] JUMP = new string[1]
		{
			"jump_start"
		};

		public string[] JUMPSTART = new string[1]
		{
			"jump_start"
		};

		public string[] HANGTIME = new string[1]
		{
			"jump_hang"
		};

		public string[] ROLL = new string[1]
		{
			"rolling"
		};

		public string[] DODGE_LEFT = new string[1]
		{
			"running"
		};

		public string[] DODGE_RIGHT = new string[1]
		{
			"running"
		};

		public string[] GRIND = new string[1]
		{
			"grind"
		};

		public string[] GET_ON_BOARD = new string[1]
		{
			"jump_land"
		};

		public string[] HIT_MID = new string[1]
		{
			"hitMid"
		};

		public string[] HIT_UPPER = new string[1]
		{
			"hitUpper"
		};

		public string[] HIT_LOWER = new string[1]
		{
			"hitLower"
		};

		public string[] HIT_MOVING = new string[1]
		{
			"hitMoving"
		};

		public string[] STUMBLE = new string[1]
		{
			"stumble_low"
		};

		public string[] STUMBLE_MIX = new string[1]
		{
			"stumble"
		};

		public string[] STUMBLE_LEFT_SIDE = new string[1]
		{
			"stumbleLeftSide"
		};

		public string[] STUMBLE_RIGHT_SIDE = new string[1]
		{
			"stumbleRightSide"
		};

		public string[] STUMBLE_LEFT_CORNER = new string[1]
		{
			"stumbleLeftCorner"
		};

		public string[] STUMBLE_RIGHT_CORNER = new string[1]
		{
			"stumbleRightCorner"
		};

		public string DEFAULT_HOVERBOARD_ANIMATION;

		private static int[] runExcept = new int[2]
		{
			0,
			1
		};

		public string TopMenu => GetRandomAnimationName(TOP_MENU);

		public string Run => GetRandomAnimationName(RUN, runExcept);

		public string Land => GetRandomAnimationName(LAND);

		public string JumpStart => GetRandomAnimationName(JUMPSTART);

		public string Jump => GetRandomAnimationName(JUMP);

		public string Hangtime => GetRandomAnimationName(HANGTIME);

		public string[] HoverboardJump => GetRandomHoverJumps(JUMP, HANGTIME);

		public string Roll => GetRandomAnimationName(ROLL);

		public string DodgeLeft => GetRandomAnimationName(DODGE_LEFT);

		public string DodgeRight => GetRandomAnimationName(DODGE_RIGHT);

		public string Grind => GetRandomAnimationName(GRIND);

		public string GetOnBoard => GetRandomAnimationName(GET_ON_BOARD);

		public string HitMid => GetRandomAnimationName(HIT_MID);

		public string HitUpper => GetRandomAnimationName(HIT_UPPER);

		public string HitLower => GetRandomAnimationName(HIT_LOWER);

		public string HitMoving => GetRandomAnimationName(HIT_MOVING);

		public string Stumble => GetRandomAnimationName(STUMBLE);

		public string StumbleMix => GetRandomAnimationName(STUMBLE_MIX);

		public string StumbleLeftSide => GetRandomAnimationName(STUMBLE_LEFT_SIDE);

		public string StumbleRightSide => GetRandomAnimationName(STUMBLE_RIGHT_SIDE);

		public string StumbleLeftCorner => GetRandomAnimationName(STUMBLE_LEFT_CORNER);

		public string StumbleRightCorner => GetRandomAnimationName(STUMBLE_RIGHT_CORNER);

		private string GetRandomAnimationName(string[] animationsNames, int[] exceptIndecies = null)
		{
			int num = 0;
			if (exceptIndecies == null)
			{
				num = UnityEngine.Random.Range(0, animationsNames.Length);
			}
			else
			{
				int[] array = Enumerable.Range(0, animationsNames.Length).ToArray().Except(exceptIndecies)
					.ToArray();
				num = array[UnityEngine.Random.Range(0, array.Length)];
			}
			return animationsNames[num];
		}

		private string[] GetRandomHoverJumps(string[] hoverboardJump, string[] hoverboardHangtime)
		{
			if (hoverboardJump.Length != hoverboardHangtime.Length)
			{
			}
			int num = UnityEngine.Random.Range(0, Mathf.Min(hoverboardJump.Length, hoverboardHangtime.Length));
			return new string[2]
			{
				hoverboardJump[num],
				hoverboardHangtime[num]
			};
		}
	}

	[Serializable]
	public class AnimationClipLists
	{
		public string[] topMenu;

		public string[] run;

		public string[] jumpstart;

		public string[] jump;

		public string[] hangtime;

		public string[] landing;

		public string[] dodgeLeft;

		public string[] dodgeRight;

		public string[] roll;

		public string[] hitMid;

		public string[] hitUpper;

		public string[] hitLower;

		public string[] hitMoving;

		public string[] stumble;

		public string[] stumbleMix;

		public string[] stumbleDeath;

		public string[] stumbleLeftSide;

		public string[] stumbleRightSide;

		public string[] stumbleLeftCorner;

		public string[] stumbleRightCorner;
	}

	[Serializable]
	public class JetpackClips
	{
		public string[] run;

		public string[] dodgeLeft;

		public string[] dodgeRight;
	}

	[Serializable]
	public class SuperSneaksClips
	{
		public string[] run;
	}

	[SerializeField]
	private AnimationClipLists defaultAnimations;

	[SerializeField]
	private JetpackClips jetpackAnimations;

	[SerializeField]
	private SuperSneaksClips SuperSneaksAnimations;

	public Animation characterAnimation;

	private List<AnimationClip> addedAnimClipsNames = new List<AnimationClip>();

	[SerializeField]
	private AnimationCurve jetpackParticleOffsetCurve;

	public Animations animations;

	[SerializeField]
	private GameObject characterModelPrefab;

	private GameObject currentHoverboard;

	[SerializeField]
	private GameObject characterRenderingEffectsPrefab;

	private Vector3 initRot;

	private Vector3 initScale;

	private ParticleSystem[] particleToKill;

	private AnimationState caught;

	private Game game;

	private Character character;

	private Hoverboard hoverboard;

	private SuperSneakers superSneakers;

	private Jetpack jetpack;

	private FollowingGuard followingGuard;

	private CharacterController characterController;

	private MeshRenderer shadow;

	private CharacterModel characterModel;

	private CharacterRenderingEffects characterRenderingEffects;

	private Revive revive;

	[SerializeField]
	private ParticleSystem landingParticle;

	[SerializeField]
	private ParticleSystem hitBlockerParticle;

	private Dictionary<int, string[]> clipsnameDict = new Dictionary<int, string[]>();

	private string jumpAnimation;

	private string hangtimeAnimation;

	public static CharacterRendering instance;

	public CharacterModel CharacterModel => characterModel;

	public static CharacterRendering Instance => instance ?? (instance = UtilRMan.FindObject<CharacterRendering>());

	public event CharacterModelInitializedDelegate CharacterModelInitialized;

	public void Initialize()
	{
		InitializeCharacterModel();
		InitializeCharacterRenderingEffects();
		InitializeAnimations();
		game = Game.Instance;
		character = Character.Instance;
		characterController = character.characterController;
		hoverboard = Hoverboard.Instance;
		superSneakers = this.FindObject<SuperSneakers>();
		jetpack = Jetpack.Instance;
		followingGuard = FollowingGuard.Instance;
		revive = Revive.Instance;
		Variable<bool> isInGame = game.IsInGame;
		isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, new Variable<bool>.OnChangeDelegate(IsInGame_OnChange));
		character.OnChangeTrack += OnChangeTrack;
		character.OnStumble += OnStumble;
		character.OnTutorialMoveBackToCheckPoint += OnTutorialMoveBackToCheckPoint;
		character.OnTutorialStartFromCheckPoint += OnTutorialStartFromCheckPoint;
		character.OnHitByTrain += OnHitByTrain;
		character.OnJump += OnJump;
		character.OnRoll += OnRoll;
		character.OnLanding += OnLanding;
		character.OnHangtime += OnHangtime;
		Variable<bool> isGrounded = character.IsGrounded;
		isGrounded.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isGrounded.OnChange, new Variable<bool>.OnChangeDelegate(OnChangeIsGrounded));
		Game obj = game;
		obj.OnStageMenuSequence = (Game.OnStageMenuSequenceDelegate)Delegate.Combine(obj.OnStageMenuSequence, new Game.OnStageMenuSequenceDelegate(OnStageMenuSequence));
		Game obj2 = game;
		obj2.OnIntroRun = (Game.OnIntroRunDelegate)Delegate.Combine(obj2.OnIntroRun, new Game.OnIntroRunDelegate(OnIntroRun));
		hoverboard.OnSwitchToHoverboard += OnSwitchToHoverboard;
		hoverboard.OnSwitchToRunning += OnSwitchToRunning;
		hoverboard.OnJump += OnJump;
		hoverboard.OnRun += OnRun;
		Jetpack obj3 = jetpack;
		obj3.OnStart = (Jetpack.OnStartDelegate)Delegate.Combine(obj3.OnStart, new Jetpack.OnStartDelegate(OnSwitchToJetpack));
		Jetpack obj4 = jetpack;
		obj4.OnStop = (Jetpack.OnStopDelegate)Delegate.Combine(obj4.OnStop, new Jetpack.OnStopDelegate(JetpackOnStop));
		Jetpack obj5 = jetpack;
		obj5.OnFlyAheadStart = (Jetpack.OnFlyAheadStartDelegate)Delegate.Combine(obj5.OnFlyAheadStart, new Jetpack.OnFlyAheadStartDelegate(JetpackOnFlyAheadStart));
		Jetpack obj6 = jetpack;
		obj6.OnFlyAheadUpdate = (Jetpack.OnFlyAheadUpdateDelegate)Delegate.Combine(obj6.OnFlyAheadUpdate, new Jetpack.OnFlyAheadUpdateDelegate(JetpackOnFlyAheadUpdate));
		superSneakers.OnSwitchToSuperSneakers += OnSwitchToSuperSneakers;
		superSneakers.SuperSneakerOnStop += SuperSneakersOnStop;
		FollowingGuard obj7 = followingGuard;
		obj7.OnCatchPlayer = (FollowingGuard.OnCatchPlayerDelegate)Delegate.Combine(obj7.OnCatchPlayer, new FollowingGuard.OnCatchPlayerDelegate(OnCatchPlayer));
		revive.OnRevive += OnRevive;
		revive.OnSwitchToRunning += OnSwitchToRunning;
	}

	private void Start()
	{
		if (this.CharacterModelInitialized != null)
		{
			this.CharacterModelInitialized(characterModel.meshHoverboard.gameObject);
		}
		OnSwitchToRunning();
	}

	private void InitializeCharacterModel()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(characterModelPrefab);
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		particleToKill = gameObject.GetComponentsInChildren<ParticleSystem>();
		characterModel = gameObject.GetComponent<CharacterModel>();
		shadow = characterModel.shadow;
		characterAnimation = characterModel.characterAnimation;
	}

	private void InitializeCharacterRenderingEffects()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(characterRenderingEffectsPrefab);
		characterRenderingEffects = gameObject.GetComponent<CharacterRenderingEffects>();
		characterRenderingEffects.Initialize(characterModel);
	}

	public void InitializeAnimations()
	{
		animations = new Animations();
		animations.TOP_MENU = InitializeClips(defaultAnimations.topMenu);
		animations.HIT_MID = InitializeClips(defaultAnimations.hitMid);
		animations.HIT_UPPER = InitializeClips(defaultAnimations.hitUpper);
		animations.HIT_LOWER = InitializeClips(defaultAnimations.hitLower);
		animations.HIT_MOVING = InitializeClips(defaultAnimations.hitMoving);
		animations.STUMBLE = InitializeClips(defaultAnimations.stumble);
		animations.STUMBLE_MIX = InitializeClips(defaultAnimations.stumbleMix);
		animations.STUMBLE_LEFT_SIDE = InitializeClips(defaultAnimations.stumbleLeftSide);
		animations.STUMBLE_RIGHT_SIDE = InitializeClips(defaultAnimations.stumbleRightSide);
		animations.STUMBLE_LEFT_CORNER = InitializeClips(defaultAnimations.stumbleLeftCorner);
		animations.STUMBLE_RIGHT_CORNER = InitializeClips(defaultAnimations.stumbleRightCorner);
	}

	public void ClearCacheClipsnameDict()
	{
		clipsnameDict.Clear();
	}

	private string[] InitializeClips(string[] originNames)
	{
		if (clipsnameDict.ContainsKey(originNames.GetHashCode()))
		{
			return clipsnameDict[originNames.GetHashCode()];
		}
		string text = Regex.Replace(originNames[0], "[0-9]+$", string.Empty);
		int num = 8;
		string[] array = null;
		if (text.Contains("running") || text.Contains("idling"))
		{
			int num2 = 0;
			int num3 = num;
			while (0 < num3)
			{
				if (null != characterAnimation.GetClip(PlayerInfo.Instance.SelectedCharAnimPrefix + $"{text}{num3:D2}"))
				{
					num2 = num3;
					break;
				}
				num3--;
			}
			array = new string[num2];
			for (int i = 0; i < num2; i++)
			{
				array[i] = $"{PlayerInfo.Instance.SelectedCharAnimPrefix}{text}{i + 1:D2}";
			}
		}
		else if (text.Contains("hang"))
		{
			array = new string[1]
			{
				$"{PlayerInfo.Instance.SelectedCharAnimPrefix}{text}"
			};
		}
		else
		{
			int num4 = 0;
			int num5 = num;
			while (0 < num5)
			{
				if (null != characterAnimation.GetClip(PlayerInfo.Instance.SelectedCharAnimPrefix + $"{text}{num5:D}"))
				{
					num4 = num5;
					break;
				}
				num5--;
			}
			num4 = ((num4 == 0) ? 1 : num4);
			array = new string[num4];
			for (int j = 0; j < num4; j++)
			{
				array[j] = $"{text}{j + 1:D}";
			}
			array[0] = Regex.Replace(array[0], "[0-9]+$", string.Empty);
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = $"{PlayerInfo.Instance.SelectedCharAnimPrefix}{array[k]}";
			}
		}
		clipsnameDict[originNames.GetHashCode()] = array;
		return array;
	}

	private void OnIntroRun()
	{
		ParticleSystem[] array = particleToKill;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.Stop();
		}
		OnSwitchToRunning();
		string animation = animations.RUN[0];
		characterAnimation.CrossFadeQueued(animation, 0.2f);
	}

	public void OnRun()
	{
		if (!character.IsFalling && !character.IsJumping && characterController.isGrounded)
		{
			string text = animations.RUN[1];
			if (superSneakers.isActive)
			{
				characterAnimation[text].speed = 1f;
			}
			else
			{
				characterAnimation[text].speed = Game.Instance.NormalizedGameSpeed;
			}
			characterAnimation.CrossFade(text);
		}
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (Game.Instance.isDead || Game.Instance.IsHitDeadByTrain)
		{
			return;
		}
		if (characterController.isGrounded)
		{
			string empty = string.Empty;
			empty = ((!jetpack.isActive) ? ((direction != 0) ? animations.DODGE_RIGHT[0] : animations.DODGE_LEFT[0]) : ((direction != 0) ? animations.DODGE_RIGHT[1] : animations.DODGE_LEFT[1]));
			if (!hoverboard.isActive && !jetpack.isActive)
			{
				characterAnimation[empty].speed = Game.Instance.NormalizedGameSpeed;
			}
			else
			{
				characterAnimation[empty].speed = 1f;
			}
			characterAnimation.CrossFade(empty, 0.02f);
			if (!hoverboard.isActive)
			{
			}
		}
		if (character.IsJumping)
		{
			return;
		}
		string run = animations.Run;
		string text = animations.RUN[0];
		string name = animations.RUN[1];
		if (!hoverboard.isActive && !game.IsInJetpackMode)
		{
			characterAnimation[run].speed = Game.Instance.NormalizedGameSpeed;
			characterAnimation[text].speed = Game.Instance.NormalizedGameSpeed;
		}
		else
		{
			characterAnimation[run].speed = 1f;
			characterAnimation[text].speed = Mathf.Clamp(Game.Instance.NormalizedGameSpeed, 0f, 1.2f);
			if (game.IsInJetpackMode)
			{
				characterAnimation[name].speed = Mathf.Clamp(Game.Instance.NormalizedGameSpeed, 0f, 1.2f);
			}
		}
		if (game.IsInJetpackMode)
		{
			characterAnimation.CrossFadeQueued(animations.RUN[1], 0.05f, QueueMode.CompleteOthers);
		}
		else
		{
			if (0.1f > UnityEngine.Random.value)
			{
				characterAnimation.CrossFade(run, (!game.Modifiers.IsActive(game.Modifiers.Hoverboard)) ? 0.02f : 0.4f);
			}
			characterAnimation.CrossFadeQueued(text, 0.05f, QueueMode.CompleteOthers);
		}
		if (!hoverboard.isActive)
		{
		}
	}

	private void OnStumble(Character.StumbleType stumbleType, Character.StumbleHorizontalHit horizontalHit, Character.StumbleVerticalHit verticalHit, string colliderName)
	{
		if (stumbleType == Character.StumbleType.Bush || colliderName == "lightSignal" || colliderName == "powerbox")
		{
			hitBlockerParticle.gameObject.SetActive(value: true);
			hitBlockerParticle.Play();
			characterAnimation.Play(animations.StumbleMix, PlayMode.StopAll);
			characterAnimation.CrossFadeQueued(animations.RUN[0], 0.5f);
			return;
		}
		if (stumbleType == Character.StumbleType.Side)
		{
			if (!game.Modifiers.IsActive(game.Modifiers.Hoverboard) && !game.IsInJetpackMode)
			{
				if (horizontalHit == Character.StumbleHorizontalHit.LeftCorner || horizontalHit == Character.StumbleHorizontalHit.Left)
				{
					characterAnimation.CrossFade(animations.StumbleLeftSide, 0.2f);
				}
				if (horizontalHit == Character.StumbleHorizontalHit.RightCorner || horizontalHit == Character.StumbleHorizontalHit.Right)
				{
					characterAnimation.CrossFade(animations.StumbleRightSide, 0.2f);
				}
			}
			if (!character.IsJumping)
			{
				characterAnimation.CrossFadeQueued(animations.RUN[0], (!game.Modifiers.IsActive(game.Modifiers.Hoverboard)) ? 0.02f : 0.4f);
			}
			return;
		}
		switch (horizontalHit)
		{
		case Character.StumbleHorizontalHit.Center:
			switch (verticalHit)
			{
			case Character.StumbleVerticalHit.Lower:
				if (game.Modifiers.IsActive(game.Modifiers.Hoverboard))
				{
					characterAnimation.CrossFade(animations.StumbleMix, 0.05f);
				}
				else
				{
					characterAnimation.CrossFade(animations.Stumble, 0.05f);
				}
				characterAnimation.CrossFadeQueued(animations.RUN[0], 0.5f);
				break;
			case Character.StumbleVerticalHit.Middle:
				characterAnimation.CrossFade(animations.HitMid, 0.07f);
				break;
			case Character.StumbleVerticalHit.Upper:
				characterAnimation.CrossFade(animations.HitUpper, 0.07f);
				break;
			}
			return;
		case Character.StumbleHorizontalHit.Left:
			characterAnimation.Play((!game.Modifiers.IsActive(game.Modifiers.Hoverboard)) ? animations.StumbleLeftSide : animations.StumbleLeftCorner);
			break;
		case Character.StumbleHorizontalHit.LeftCorner:
			characterAnimation.Play(animations.StumbleLeftCorner);
			break;
		case Character.StumbleHorizontalHit.Right:
			characterAnimation.Play((!game.Modifiers.IsActive(game.Modifiers.Hoverboard)) ? animations.StumbleRightSide : animations.StumbleRightCorner);
			break;
		case Character.StumbleHorizontalHit.RightCorner:
			characterAnimation.Play(animations.StumbleRightCorner);
			break;
		}
		characterAnimation.PlayQueued(animations.RUN[0]);
	}

	private void OnChangeIsGrounded(bool isGrounded)
	{
		shadow.enabled = isGrounded;
	}

	private void OnRoll()
	{
		StartCoroutine(OnRollPlayAnimation());
	}

	private IEnumerator OnRollPlayAnimation()
	{
		string rollAnimation = animations.Roll;
		characterAnimation.CrossFade(rollAnimation, 0.1f);
		if (hoverboard.isActive)
		{
			characterAnimation.CrossFadeQueued(animations.RUN[0], 0.2f);
		}
		else
		{
			characterAnimation.PlayQueued(animations.RUN[0]);
		}
		float endTime = Time.time + characterAnimation[rollAnimation].length;
		while (Time.time < endTime && characterAnimation[rollAnimation].enabled)
		{
			yield return null;
		}
		character.EndRoll();
	}

	private void OnHitByTrain()
	{
		characterAnimation.Play(animations.HitMoving);
		characterAnimation[animations.HitMoving].speed = 1.5f;
		Vector3 currentPos = character.transform.position;
		Vector3 targetPos = Camera.main.transform.TransformPoint(new Vector3(0f, 0f, 7f));
		LateUpdaterLastOrder lateUpdater = character.GetComponent<LateUpdaterLastOrder>();
		StartCoroutine(pTween.To(characterAnimation[animations.HitMoving].length / 1.5f, delegate(float t)
		{
			lateUpdater.AddAction(delegate
			{
				character.transform.position = Vector3.Lerp(currentPos, targetPos, t);
			});
		}));
	}

	private void OnJump()
	{
		jumpAnimation = animations.Jump;
		characterAnimation.CrossFade(animations.JumpStart, 0.05f);
		characterAnimation.CrossFadeQueued(animations.Hangtime, 0.05f, QueueMode.CompleteOthers);
		characterAnimation.CrossFadeQueued(animations.Jump, 0.1f, QueueMode.CompleteOthers);
	}

	private void OnRevive()
	{
		StopAllCoroutines();
		jumpAnimation = animations.Jump;
		characterAnimation.Stop();
		characterAnimation.Play(jumpAnimation);
	}

	private void OnHangtime()
	{
		if (!character.IsRolling)
		{
			if (!hoverboard.isActive || hangtimeAnimation == null)
			{
				hangtimeAnimation = animations.Hangtime;
			}
			characterAnimation.CrossFade(animations.Jump, 0.1f);
		}
	}

	private void OnLanding(Transform characterTransform)
	{
		string run = animations.Run;
		if (!character.IsRolling)
		{
			PPActionJump nParent = GameObjectPoolMT<PPActionJump>.Instance.GetNParent(Character.Instance.transform, null);
			nParent.transform.SetParent(null);
			string land = animations.Land;
			characterAnimation[land].normalizedSpeed = 5f;
			characterAnimation.CrossFade(land, 0.05f);
			if (0.1f > UnityEngine.Random.value)
			{
				characterAnimation.CrossFade(run, 0.05f);
			}
			characterAnimation.CrossFadeQueued(animations.RUN[0], 0.1f, QueueMode.CompleteOthers);
		}
	}

	private void OnTutorialMoveBackToCheckPoint(float duration)
	{
		characterAnimation.CrossFade(animations.RUN[0], duration);
	}

	private void OnTutorialStartFromCheckPoint()
	{
		characterAnimation.Play(animations.RUN[0]);
	}

	private void OnCatchPlayer(string currentCharacterCaught, float catchUpTime, float waitTimeBeforeScreen)
	{
		caught = characterAnimation[currentCharacterCaught];
		caught.weight = 0f;
		caught.normalizedTime = 0f;
		caught.enabled = true;
		StartCoroutine(CatchPlayerAnimStarter(caught, catchUpTime));
	}

	private IEnumerator CatchPlayerAnimStarter(AnimationState caught, float delay)
	{
		yield return new WaitForSeconds(delay);
		StartCoroutine(pTween.To(0.2f, delegate(float t)
		{
			caught.weight = Mathf.Lerp(0f, 1f, t);
		}));
	}

	private void OnStageMenuSequence()
	{
		if (characterAnimation != null)
		{
			if (caught != null)
			{
				caught.enabled = false;
			}
			characterAnimation.transform.rotation = Quaternion.identity;
			characterAnimation.Play(animations.TopMenu);
		}
	}

	public void OnSwitchToRunning()
	{
		if (character.superSneakers.isActive || game.HasSuperSneakers)
		{
			animations.RUN = InitializeClips(SuperSneaksAnimations.run);
		}
		else
		{
			animations.RUN = InitializeClips(defaultAnimations.run);
		}
		animations.LAND = InitializeClips(defaultAnimations.landing);
		animations.JUMP = InitializeClips(defaultAnimations.jump);
		animations.JUMPSTART = InitializeClips(defaultAnimations.jumpstart);
		animations.HANGTIME = InitializeClips(defaultAnimations.hangtime);
		animations.ROLL = InitializeClips(defaultAnimations.roll);
		animations.DODGE_LEFT = InitializeClips(defaultAnimations.dodgeLeft);
		animations.DODGE_RIGHT = InitializeClips(defaultAnimations.dodgeRight);
		ToggleCustomHoverboard(null);
	}

	private void OnSwitchToHoverboard(GameObject hoverBoard)
	{
		ToggleCustomHoverboard(hoverBoard);
		string getOnBoard = animations.GetOnBoard;
		hangtimeAnimation = animations.Hangtime;
		characterAnimation.CrossFade(getOnBoard, 0.1f);
		if (!character.IsFalling && !character.IsJumping)
		{
			string animation = animations.RUN[0];
			characterAnimation.CrossFadeQueued(animation, 0.2f);
		}
		else
		{
			characterAnimation.CrossFade(hangtimeAnimation, 0.2f);
		}
	}

	private void OnSwitchToJetpack(bool isHeadStart)
	{
		characterRenderingEffects.SetRightAndLeftParticlesActive(active: true);
		string animation = animations.RUN[1];
		characterAnimation.CrossFade(animation);
	}

	private void ToggleCustomHoverboard(GameObject newHoverboard)
	{
		if (currentHoverboard != null && currentHoverboard != newHoverboard)
		{
			UnityEngine.Object.Destroy(currentHoverboard);
		}
		currentHoverboard = newHoverboard;
		if (!(newHoverboard != null))
		{
		}
	}

	private void JetpackOnFlyAheadStart()
	{
		initRot = characterRenderingEffects.JetpackParticles.transform.rotation.eulerAngles;
		initScale = characterRenderingEffects.JetpackParticles.transform.localScale;
	}

	private void JetpackOnFlyAheadUpdate(float ratio)
	{
		float num = Mathf.Lerp(0f, 1f, jetpackParticleOffsetCurve.Evaluate(ratio));
		characterRenderingEffects.JetpackParticles.transform.rotation = Quaternion.Euler(initRot - new Vector3(num, 0f, 0f));
		characterRenderingEffects.JetpackParticles.transform.localScale = initScale + new Vector3(0f, 0f, num * 2f);
	}

	private void JetpackOnStop()
	{
		characterRenderingEffects.SetRightAndLeftParticlesActive(active: false);
		OnSwitchToRunning();
	}

	private void IsInGame_OnChange(bool isInGame)
	{
		if (!isInGame)
		{
			JetpackOnStop();
		}
	}

	private void OnSwitchToSuperSneakers()
	{
	}

	private void SuperSneakersOnStop()
	{
	}
}
