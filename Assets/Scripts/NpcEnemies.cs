using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class NpcEnemies : MonoBehaviour
{
	[Serializable]
	public class CatchAnimationSet
	{
		public string avatar;

		public string enemy;

		public float catchAvatarAnimationPlayOffset;

		public float waitTimeBeforeScreen;
	}

	[Serializable]
	public class DefaultAnimations
	{
		public string[] EnemyRun;

		public string[] EnemyJump;

		public string[] EnemyHang;

		public string[] EnemyLand;

		public string[] EnemyDodgeL;

		public string[] EnemyDodgeR;

		public string[] EnemyRoll;

		public string[] EnemyCatchup;

		public string[] EnemyDie;
	}

	public delegate void OnCatchPlayerDelegate(string currentChartacterCatch, float catchUpTime, float waitTimeBeforeScreen);

	public enum ObstacleType
	{
		JumpHighBarrier,
		JumpTrain,
		RollBarrier,
		JumpBarrier,
		None
	}

	public enum NpcActionType
	{
		Following,
		SelectSide,
		ClearObject,
		Speeding,
		Finish
	}

	public DefaultAnimations defaultAnimations;

	public float distanceToCharacterMin = -10f;

	public float distanceToCharacterMax = 50f;

	public float catchUpDuration = 0.7f;

	public float resetCatchUpDuration = 1.5f;

	public float lastGroundedSmoothTime = 0.3f;

	public float xSmoothTime = 0.1f;

	public float gravity = 200f;

	public bool isShowing;

	public Variable<bool> IsGrounded = new Variable<bool>(initialValue: false);

	public bool isDeadNpc;

	public bool isCAStart;

	public Animation enemyAnimation;

	public CatchAnimationSet[] caughtLeft;

	public CatchAnimationSet[] caughtRight;

	private string currentAvatarStumbleDeath;

	private string previusAvatarCaughtAnimLeft;

	private string previusAvatarCaughtAnimRight;

	public int debugCatchAnimationToPlay = -1;

	public string modelPrefix = string.Empty;

	private Renderer[] enemyRenderers;

	public Transform[] enemies;

	private Vector3[] enemiesStartPos;

	private float y;

	private bool closeToCharacter;

	private float distanceToCharacter;

	private float lastGroundedSmooth;

	private float lastGroundedVelocity;

	private SmoothDampFloat x;

	private Game game;

	private Layers layers;

	[HideInInspector]
	public OnTriggerObject ncharacterColliderTrigger;

	public CapsuleCollider ncharacterCollider;

	public CharacterController ncharacterController;

	private CharacterController characterController;

	private Vector3 ncharacterControllerCenter;

	private float ncharacterControllerHeight;

	private Vector3 ncharacterColliderCenter;

	private float ncharacterColliderHeight;

	private Character character;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	public SkinnedMeshRenderer model;

	private SkinnedMeshRenderer[] models;

	private Dictionary<string, SkinnedMeshRenderer> modelLookupTable;

	private string[] modelNames;

	private string[] rivalNames = new string[7]
	{
		"McYou",
		"garry",
		"suckjin",
		"haha",
		"jihyo",
		"jongkuk",
		"gangsu"
	};

	private string previusAvatarStumbleDeath;

	public OnCatchPlayerDelegate OnCatchPlayer;

	private float verticalSpeed;

	public float verticalFallSpeedLimit = -1f;

	public float enemyProximityLoopVolume = 0.9f;

	private bool isPaused = true;

	private bool caught;

	private Vector3 npcSide;

	private ObstacleType lastObstacleTriggerType;

	private int lastObstacleTriggerTrackIndex;

	[HideInInspector]
	public int trackIndex;

	[HideInInspector]
	public float lastGroundedY;

	private NpcActionType NpcActionState;

	public float startTime;

	private float speedCount;

	private bool isJumping;

	private bool isFalling;

	private bool isRolling;

	[SerializeField]
	private ParticleSystem nameTagParticle;

	[SerializeField]
	private ParticleSystem hangOnParticle;

	public static NpcEnemies instance;

	public NpcActionType getNpcAction => NpcActionState;

	public float getDistanceToCharacter => distanceToCharacter;

	public static NpcEnemies Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(NpcEnemies)) as NpcEnemies));

	public float GetNpcSideX()
	{
		Vector3 position = enemies[0].transform.position;
		return position.x;
	}

	public bool IsPaticle()
	{
		if (hangOnParticle.isPlaying)
		{
			hangOnParticle.loop = false;
			hangOnParticle.Pause();
			hangOnParticle.gameObject.SetActive(value: false);
			return true;
		}
		return false;
	}

	public void Reset()
	{
		closeToCharacter = true;
		isDeadNpc = false;
		isCAStart = false;
		IsGrounded.Value = true;
		lastGroundedY = 0f;
		distanceToCharacter = 0f;
		npcSide.x = 0f;
		x.Target = 0f;
		x.Value = 80f;
		x.SmoothTime = 0.8f;
		ShowEnemies(vis: true);
		base.transform.position = new Vector3(80f, 0f, 0f);
		enemies[0].transform.position = new Vector3(80f, 0f, 0f);
		NpcActionState = NpcActionType.Following;
		Restart(closeToCharacter: true);
		string animation = ReturnRandomAnimations(defaultAnimations.EnemyRun);
		enemyAnimation.Play(animation);
	}

	public void Awake()
	{
		instance = this;
		models = GetComponentsInChildren<SkinnedMeshRenderer>();
		modelNames = new string[models.Length];
		modelLookupTable = new Dictionary<string, SkinnedMeshRenderer>();
		for (int i = 0; i < models.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = models[i];
			string name = skinnedMeshRenderer.gameObject.name;
			modelNames[i] = name;
			modelLookupTable.Add(name, skinnedMeshRenderer);
		}
		ChangeCharacterModel(0);
	}

	public void Initialize()
	{
		layers = Layers.Instance;
		game = Game.Instance;
		Variable<bool> isInGame2 = game.IsInGame;
		isInGame2.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame2.OnChange, (Variable<bool>.OnChangeDelegate)delegate(bool isInGame)
		{
			if (!isInGame)
			{
				StopAllCoroutines();
				ncharacterController.enabled = true;
			}
		});
		character = Character.Instance;
		characterController = character.characterController;
		characterRendering = CharacterRendering.Instance;
		characterTransform = character.transform;
		enemyRenderers = base.gameObject.GetComponentsInChildren<Renderer>();
		isDeadNpc = false;
		isCAStart = false;
		npcSide.x = 0f;
		lastGroundedY = 0f;
		enemiesStartPos = new Vector3[enemies.Length];
		for (int i = 0; i < enemies.Length; i++)
		{
			enemiesStartPos[i] = enemies[i].position;
		}
		x = new SmoothDampFloat(0f, xSmoothTime);
		GetComponent<AudioSource>().volume = enemyProximityLoopVolume;
		Game obj = game;
		obj.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(obj.OnPauseChange, new Game.OnPauseChangeDelegate(HandleOnPauseChange));
		ncharacterColliderTrigger = ncharacterCollider.GetComponent<OnTriggerObject>();
		OnTriggerObject onTriggerObject = ncharacterColliderTrigger;
		onTriggerObject.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Combine(onTriggerObject.OnEnter, new OnTriggerObject.OnEnterDelegate(OnNpcColliderEnter));
		OnTriggerObject onTriggerObject2 = ncharacterColliderTrigger;
		onTriggerObject2.OnExit = (OnTriggerObject.OnExitDelegate)Delegate.Combine(onTriggerObject2.OnExit, new OnTriggerObject.OnExitDelegate(OnNpcColliderExit));
		ncharacterControllerCenter = ncharacterController.center;
		ncharacterControllerHeight = ncharacterController.height;
		ncharacterColliderCenter = ncharacterCollider.center;
		ncharacterColliderHeight = ncharacterCollider.height;
		CatchAnimationSet[] array = caughtLeft;
		foreach (CatchAnimationSet catchAnimationSet in array)
		{
			catchAnimationSet.avatar = modelPrefix + catchAnimationSet.avatar;
			catchAnimationSet.enemy = modelPrefix + catchAnimationSet.enemy;
			SetupAvatarAnimationsStates(characterRendering.characterAnimation, catchAnimationSet.avatar);
			SetupEnemyAnimationsStates(enemyAnimation, catchAnimationSet.enemy);
		}
		CatchAnimationSet[] array2 = caughtRight;
		foreach (CatchAnimationSet catchAnimationSet2 in array2)
		{
			catchAnimationSet2.avatar = modelPrefix + catchAnimationSet2.avatar;
			catchAnimationSet2.enemy = modelPrefix + catchAnimationSet2.enemy;
			SetupAvatarAnimationsStates(characterRendering.characterAnimation, catchAnimationSet2.avatar);
			SetupEnemyAnimationsStates(enemyAnimation, catchAnimationSet2.enemy);
		}
		List<AnimationClip> list = new List<AnimationClip>();
		IEnumerator enumerator = enemyAnimation.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				AnimationState animationState = (AnimationState)enumerator.Current;
				list.Add(animationState.clip);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		InitializeClips(enemyAnimation, list, defaultAnimations.EnemyRun);
		InitializeClips(enemyAnimation, list, defaultAnimations.EnemyJump);
		InitializeClips(enemyAnimation, list, defaultAnimations.EnemyDodgeL);
		InitializeClips(enemyAnimation, list, defaultAnimations.EnemyDodgeR);
		InitializeClips(enemyAnimation, list, defaultAnimations.EnemyRoll);
		InitializeClips(enemyAnimation, list, defaultAnimations.EnemyCatchup);
		InitializeClips(enemyAnimation, list, defaultAnimations.EnemyDie);
		InitializeClips(enemyAnimation, list, defaultAnimations.EnemyHang);
		InitializeClips(enemyAnimation, list, defaultAnimations.EnemyLand);
		OnInternalEnable();
	}

	private void InitializeClips(Animation animationComponent, List<AnimationClip> addedClips, string[] clipNames)
	{
		for (int i = 0; i < clipNames.Length; i++)
		{
			if (!clipNames[i].Contains(modelPrefix))
			{
				clipNames[i] = modelPrefix + clipNames[i];
			}
		}
	}

	private void SetupAvatarAnimationsStates(Animation animation, string animationClipName)
	{
		animation[animationClipName].enabled = false;
		animation[animationClipName].layer = 4;
	}

	private void SetupEnemyAnimationsStates(Animation animation, string animationClipName)
	{
	}

	private void HandleOnPauseChange(bool pause)
	{
		if (pause)
		{
			if (GetComponent<AudioSource>().isPlaying)
			{
				GetComponent<AudioSource>().Pause();
			}
			isPaused = true;
		}
		else
		{
			if (isPaused)
			{
				GetComponent<AudioSource>().Play();
			}
			isPaused = false;
		}
	}

	public void ResetModelRootPosition()
	{
		float z = 8f;
		base.transform.position = Vector3.zero;
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].localPosition = enemiesStartPos[i] + new Vector3(0f, 0f, z);
			enemies[i].rotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public void Restart(bool closeToCharacter)
	{
		StopAllCoroutines();
		this.closeToCharacter = closeToCharacter;
		distanceToCharacter = ((!closeToCharacter) ? distanceToCharacterMax : distanceToCharacterMin);
	}

	public void OnInternalEnable()
	{
		lastGroundedSmooth = Character.Instance.lastGroundedY;
		y = Character.Instance.lastGroundedY;
		SmoothDampFloat smoothDampFloat = x;
		Vector3 position = Character.Instance.transform.position;
		smoothDampFloat.Value = position.x;
		Character.Instance.OnChangeTrack += OnChangeTrack;
		lastGroundedVelocity = 0f;
		distanceToCharacter = distanceToCharacterMin;
		closeToCharacter = true;
		verticalSpeed = 0f;
	}

	public void FollowingOn()
	{
		character.OnJump += OnJump;
		character.OnRollGuard += OnRoll;
		character.OnRoll += OnRollNoAnimation;
	}

	public void OnDisable()
	{
		character.OnJump -= OnJump;
		character.OnRollGuard -= OnRoll;
		character.OnRoll -= OnRollNoAnimation;
		character.OnChangeTrack -= OnChangeTrack;
	}

	public void CatchUp()
	{
		CatchUp(catchUpDuration);
	}

	public void CatchUp(float duration)
	{
		if (!closeToCharacter)
		{
			float distanceFrom = distanceToCharacter;
			StopAllCoroutines();
			enemyAnimation.Play(ReturnRandomAnimations(defaultAnimations.EnemyCatchup));
			enemyAnimation.PlayQueued(modelPrefix + "attacking");
			GetComponent<AudioSource>().timeSamples = UnityEngine.Random.Range(0, GetComponent<AudioSource>().timeSamples);
			GetComponent<AudioSource>().Play();
			GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.05f);
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMin, t);
			}));
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				GetComponent<AudioSource>().volume = Mathf.SmoothStep(0f, enemyProximityLoopVolume, t);
			}));
		}
	}

	public void ResetCatchUp()
	{
		ResetCatchUp(resetCatchUpDuration);
	}

	public void ResetCatchUp(float duration)
	{
		StartCoroutine(ResetCatchUpCoroutine(duration));
	}

	public IEnumerator ResetCatchUpCoroutine(float duration)
	{
		if (!closeToCharacter)
		{
			yield break;
		}
		float distanceFrom = distanceToCharacter;
		closeToCharacter = false;
		StartCoroutine(pTween.To(duration, delegate(float t)
		{
			distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMax, t);
		}));
		yield return StartCoroutine(pTween.To(duration * 2f, delegate(float t)
		{
			GetComponent<AudioSource>().volume = Mathf.SmoothStep(enemyProximityLoopVolume, 0f, t);
		}));
		GetComponent<AudioSource>().Stop();
		if (!game.isDead)
		{
			if (isDeadNpc)
			{
				isCAStart = false;
				yield break;
			}
			isCAStart = true;
			startTime = Time.time;
			NpcActionState = NpcActionType.Following;
			game.enemies.ResetCatchUp();
		}
	}

	public void MuteProximityLoop()
	{
		GetComponent<AudioSource>().Stop();
	}

	public void PlayIntro()
	{
		base.gameObject.transform.position = new Vector3(0f, 0f, 250f);
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].position = enemiesStartPos[i];
			enemies[i].rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		string animation = ReturnRandomAnimations(defaultAnimations.EnemyCatchup);
		enemyAnimation.Play(animation);
		string animation2 = ReturnRandomAnimations(defaultAnimations.EnemyRun);
		enemyAnimation.CrossFadeQueued(animation2, 0.2f);
	}

	private string ReturnRandomAnimations(string[] enemyAnimations)
	{
		if (enemyAnimations != null)
		{
			int num = UnityEngine.Random.Range(0, enemyAnimations.Length);
			return enemyAnimations[num];
		}
		return string.Empty;
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (!caught)
		{
			if (characterController.isGrounded && isCAStart)
			{
				string text = (direction != 0) ? ReturnRandomAnimations(defaultAnimations.EnemyDodgeR) : ReturnRandomAnimations(defaultAnimations.EnemyDodgeL);
				enemyAnimation[text].speed = game.NormalizedGameSpeed;
				enemyAnimation.CrossFade(text, 0.1f);
			}
			if (!character.IsJumping)
			{
				string animation = ReturnRandomAnimations(defaultAnimations.EnemyRun);
				enemyAnimation.CrossFadeQueued(animation, 0.1f);
			}
		}
	}

	public void Roll()
	{
		string animation = ReturnRandomAnimations(defaultAnimations.EnemyRoll);
		enemyAnimation.CrossFade(animation, 0.1f);
		string animation2 = ReturnRandomAnimations(defaultAnimations.EnemyRun);
		enemyAnimation.CrossFadeQueued(animation2, 0.2f);
	}

	private void OnRoll()
	{
		string animation = ReturnRandomAnimations(defaultAnimations.EnemyRoll);
		enemyAnimation.CrossFade(animation, 0.1f);
		string animation2 = ReturnRandomAnimations(defaultAnimations.EnemyRun);
		enemyAnimation.CrossFadeQueued(animation2, 0.2f);
	}

	private void OnRollNoAnimation()
	{
		StartCoroutine(RollCoroutine(distanceToCharacter / game.currentSpeed));
	}

	private IEnumerator RollCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		verticalSpeed = 0f - character.CalculateJumpVerticalSpeed();
	}

	private void OnJump()
	{
		Jump(distanceToCharacter / game.currentSpeed);
	}

	public void Jump(float delay)
	{
		if (ncharacterController.isGrounded)
		{
			isJumping = true;
		}
		if (distanceToCharacter <= distanceToCharacterMin)
		{
		}
		StartCoroutine(JumpCoroutine(delay));
	}

	private IEnumerator JumpCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		JumpAnimation();
		verticalSpeed = character.CalculateJumpVerticalSpeed() * 1.35f;
	}

	private void JumpAnimation()
	{
		string animation = ReturnRandomAnimations(defaultAnimations.EnemyJump);
		enemyAnimation.Play(animation);
	}

	public void HighJump(float delay)
	{
		Vector3 position = base.transform.position;
		Jump(delay);
	}

	public void CatchPlayer(float pos)
	{
		GetComponent<AudioSource>().Stop();
		StopAllCoroutines();
		caught = true;
		int num = (debugCatchAnimationToPlay <= -1 || debugCatchAnimationToPlay >= caughtLeft.Length) ? UnityEngine.Random.Range(0, caughtLeft.Length) : debugCatchAnimationToPlay;
		int num2 = (debugCatchAnimationToPlay <= -1 || debugCatchAnimationToPlay >= caughtRight.Length) ? UnityEngine.Random.Range(0, caughtRight.Length) : debugCatchAnimationToPlay;
		float num3;
		if (pos < 20f)
		{
			enemyAnimation.CrossFade(caughtLeft[num].enemy, 0.2f);
			num3 = caughtLeft[num].catchAvatarAnimationPlayOffset / 25f;
			if (OnCatchPlayer != null)
			{
				OnCatchPlayer(caughtLeft[num].avatar, num3, caughtLeft[num].waitTimeBeforeScreen);
			}
		}
		else
		{
			enemyAnimation.CrossFade(caughtRight[num2].enemy, 0.2f);
			num3 = caughtRight[num2].catchAvatarAnimationPlayOffset / 25f;
			if (OnCatchPlayer != null)
			{
				OnCatchPlayer(caughtRight[num2].avatar, num3, caughtRight[num2].waitTimeBeforeScreen);
			}
		}
		StartCoroutine(pTween.To(num3, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.transform.position, t);
			}
		}));
	}

	public void HitByTrainSequence()
	{
	}

	public IEnumerator HitByTrainSequenceCoroutine()
	{
		GameStats.Instance.guardHitScreen++;
		float catchUpTime = 0.2f;
		yield return StartCoroutine(pTween.To(catchUpTime, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.transform.position, t);
			}
		}));
		Vector3 charPos = characterTransform.position;
		StartCoroutine(pTween.To(1f, delegate(float t)
		{
			characterTransform.position = Vector3.Lerp(charPos, new Vector3(charPos.x, -5f, charPos.z), t);
		}));
		yield return new WaitForSeconds(0.2f);
		enemyAnimation.Play(modelPrefix + "idling");
	}

	public void ShowEnemies(bool vis)
	{
		isShowing = vis;
		caught = false;
		Renderer[] array = enemyRenderers;
		foreach (Renderer renderer in array)
		{
			renderer.gameObject.SetActive(vis);
		}
	}

	public void NpcSidePos(float xPos)
	{
		npcSide.x = xPos;
	}

	public void NpcSetPos(float xPos)
	{
		ref Vector3 reference = ref npcSide;
		Vector3 position = character.transform.position;
		reference.x = position.x;
	}

	public void LateUpdate()
	{
		if (isDeadNpc)
		{
			return;
		}
		if (isCAStart && NpcActionState == NpcActionType.Following && GetDuration(startTime) > 5f)
		{
			NpcActionState = NpcActionType.SelectSide;
			x.SmoothTime = xSmoothTime;
		}
		x.Target = npcSide.x;
		verticalSpeed -= gravity * Time.deltaTime;
		if (!ncharacterController.isGrounded && !isFalling && verticalSpeed < verticalFallSpeedLimit && !isRolling)
		{
			isFalling = true;
			if (!game.IsInJetpackMode)
			{
				OnHangtime1();
			}
			IsGrounded.Value = false;
		}
		Vector3 b = verticalSpeed * Time.deltaTime * Vector3.up;
		Vector3 position = ncharacterController.transform.position;
		if (isCAStart && NpcActionState < NpcActionType.SelectSide)
		{
			SmoothDampFloat smoothDampFloat = x;
			Vector3 position2 = characterTransform.position;
			smoothDampFloat.Target = position2.x;
		}
		x.Update();
		Vector3 vector = Vector3.zero;
		vector = characterTransform.position + Vector3.forward * distanceToCharacterMax - Vector3.forward * distanceToCharacter;
		float d = 1f;
		if (isCAStart)
		{
			vector = characterTransform.position - Vector3.forward * 14f;
			if (NpcActionState == NpcActionType.SelectSide)
			{
				x.Target = 0f;
				x.Update();
				d = 1f;
				startTime = Time.time;
				speedCount = 0f;
				NpcActionState = NpcActionType.ClearObject;
			}
			else if (NpcActionState == NpcActionType.ClearObject)
			{
				if (GetDuration(startTime) > 2f)
				{
					NpcActionState = NpcActionType.Speeding;
					startTime = Time.time;
					hangOnParticle.gameObject.SetActive(value: true);
					hangOnParticle.transform.position = ncharacterController.transform.position;
					hangOnParticle.loop = true;
					hangOnParticle.Play();
				}
			}
			else if (NpcActionState == NpcActionType.Speeding)
			{
				speedCount += 1f;
				if (GetDuration(startTime) > 3f && hangOnParticle.isPlaying)
				{
					nameTagParticle.gameObject.SetActive(value: true);
					Transform transform = nameTagParticle.transform;
					Vector3 position3 = character.characterRoot.position;
					float num = position3.x;
					Vector3 position4 = character.characterRoot.position;
					float num2 = position4.y + 5f;
					Vector3 position5 = character.characterRoot.position;
					transform.position = new Vector3(num, num2, position5.z - 1.5f);
					nameTagParticle.Play();
					hangOnParticle.loop = false;
					hangOnParticle.Pause();
					hangOnParticle.gameObject.SetActive(value: false);
				}
				if (GetDuration(startTime) > 5f)
				{
					npcDead();
				}
			}
		}
		else if (NpcActionState == NpcActionType.Following)
		{
			x.Target = 0f;
			x.Update();
		}
		Vector3 vector2 = new Vector3(x.Value, position.y, vector.z);
		if (NpcActionState == NpcActionType.Speeding)
		{
			vector = characterTransform.position + Vector3.forward * speedCount;
			vector2 = new Vector3(x.Value, position.y, vector.z);
		}
		if (isCAStart)
		{
			vector.x = x.Value;
		}
		position = vector2;
		ncharacterController.transform.position = position;
		ncharacterController.Move(Vector3.forward * d + b);
		if (ncharacterController.isGrounded && verticalSpeed < 0f && (isJumping || isFalling))
		{
			isJumping = false;
			isFalling = false;
			IsGrounded.Value = true;
			OnLanding1(base.transform);
		}
		Vector3 position6 = base.transform.position;
		if (position6.y < 0f)
		{
			position6.y = 0.8f;
			base.transform.position = position6;
		}
	}

	public void npcDead()
	{
		NpcActionState = NpcActionType.Finish;
		isDeadNpc = true;
		ShowEnemies(vis: false);
		Running.Instance.resetNpcStartPosition();
	}

	private void OnHangtime1()
	{
		string animation = ReturnRandomAnimations(defaultAnimations.EnemyHang);
		enemyAnimation.CrossFade(animation, 0.2f);
	}

	private void OnLanding1(Transform characterTransform)
	{
		string animation = ReturnRandomAnimations(defaultAnimations.EnemyRun);
		if (!isRolling)
		{
			string animation2 = ReturnRandomAnimations(defaultAnimations.EnemyLand);
			enemyAnimation.CrossFade(animation2, 0.05f);
			enemyAnimation.CrossFadeQueued(animation, 0.1f);
		}
	}

	private void OnNpcColliderEnter(Collider collider)
	{
		if (!game.IsInGame.Value)
		{
			return;
		}
		if (collider.gameObject.layer == layers.Default)
		{
			if (collider.isTrigger && ncharacterController.isGrounded)
			{
				IsGrounded.Value = true;
			}
			if (collider.isTrigger)
			{
				ObstacleType obstacleType = ObstacleTagToType(collider.tag);
				if (obstacleType != ObstacleType.None)
				{
					lastObstacleTriggerType = obstacleType;
					lastObstacleTriggerTrackIndex = trackIndex;
				}
			}
		}
		else if (collider.isTrigger && !(collider.name == "Collider Character"))
		{
		}
	}

	private void OnNpcColliderExit(Collider collider)
	{
	}

	public float GetDuration(float startT)
	{
		return Time.time - startT;
	}

	public void AttackTag()
	{
		ncharacterController.Move(Vector3.right * 5f);
		string animation = ReturnRandomAnimations(defaultAnimations.EnemyDie);
		enemyAnimation.Play(animation);
		DeathSequence();
		isDeadNpc = true;
		isShowing = false;
		GameStats.Instance.addTagScore();
	}

	private IEnumerator DeathSequence()
	{
		yield return new WaitForSeconds(0.5f);
	}

	public bool NpcAttactSuccess()
	{
		if (NpcActionState == NpcActionType.Speeding && !game.IsInJetpackMode)
		{
			return true;
		}
		return false;
	}

	private ObstacleType ObstacleTagToType(string tag)
	{
		if (tag != null)
		{
			if (tag == "JumpTrain")
			{
				return ObstacleType.JumpTrain;
			}
			if (tag == "RollBarrier")
			{
				return ObstacleType.RollBarrier;
			}
			if (tag == "JumpBarrier")
			{
				return ObstacleType.JumpBarrier;
			}
			if (tag == "JumpHighBarrier")
			{
				return ObstacleType.JumpHighBarrier;
			}
		}
		return ObstacleType.None;
	}

	public void ChangeCharacterModel(int customizedVersion)
	{
		int num = UnityEngine.Random.Range(0, DataContainer.Instance.CharacterTableRaw.dataArray.Length);
		modelPrefix = DataContainer.Instance.CharacterTableRaw.dataArray[num].Modelname;
		modelPrefix = Regex.Replace(modelPrefix, "[0-9]+$", string.Empty);
		modelPrefix = $"{modelPrefix}01_";
		string modelname = DataContainer.Instance.CharacterTableRaw.dataArray[num].Modelname;
		if (modelLookupTable.TryGetValue(modelname, out SkinnedMeshRenderer value))
		{
			for (int i = 0; i < models.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = models[i];
				skinnedMeshRenderer.enabled = (skinnedMeshRenderer == value);
			}
		}
		if (value != null)
		{
			model = value;
		}
	}
}
