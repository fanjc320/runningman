using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class FollowingGuard : MonoBehaviour
{
	[Serializable]
	public class CatchAnimationSet
	{
		public string avatar;

		public string guard;

		public float catchAvatarAnimationPlayOffset;

		public float waitTimeBeforeScreen;
	}

	[Serializable]
	public class DefaultAnimations
	{
		public string[] introGuard;

		public string[] runGuard;

		public string[] jumpGuard;

		public string[] dodgeLeftGuard;

		public string[] dodgeRigthGuard;

		public string[] rollGuard;

		public string[] catchupGuard;
	}

	public delegate void OnCatchPlayerDelegate(string currentChartacterCatch, float catchUpTime, float waitTimeBeforeScreen);

	public DefaultAnimations defaultAnimations;

	public float distanceToCharacterMin = 10f;

	public float distanceToCharacterMax = 50f;

	public float catchUpDuration = 0.7f;

	public float resetCatchUpDuration = 1.5f;

	public float lastGroundedSmoothTime = 0.3f;

	public float xSmoothTime = 0.1f;

	public float gravity = 200f;

	public bool isShowing;

	public Animation guardAnimation;

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

	private CharacterController characterController;

	private Character character;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	public SkinnedMeshRenderer model;

	private SkinnedMeshRenderer[] models;

	private Dictionary<string, SkinnedMeshRenderer> modelLookupTable;

	private string[] modelNames;

	private string[] rivalNames = new string[7]
	{
		"gangsu",
		"jongkuk",
		"haha",
		"jihyo",
		"suckjin",
		"garry",
		"McYou"
	};

	private string previusAvatarStumbleDeath;

	public OnCatchPlayerDelegate OnCatchPlayer;

	private float verticalSpeed;

	public float guardProximityLoopVolume = 0.9f;

	private bool isPaused = true;

	private bool caught;

	public static FollowingGuard instance;

	public static FollowingGuard Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(FollowingGuard)) as FollowingGuard));

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
		game = Game.Instance;
		character = Character.Instance;
		characterController = character.characterController;
		characterRendering = CharacterRendering.Instance;
		characterTransform = character.transform;
		enemyRenderers = base.gameObject.GetComponentsInChildren<Renderer>();
		enemiesStartPos = new Vector3[enemies.Length];
		for (int i = 0; i < enemies.Length; i++)
		{
			enemiesStartPos[i] = enemies[i].position;
		}
		x = new SmoothDampFloat(0f, xSmoothTime);
		GetComponent<AudioSource>().volume = guardProximityLoopVolume;
		Game obj = game;
		obj.OnPauseChange = (Game.OnPauseChangeDelegate)Delegate.Combine(obj.OnPauseChange, new Game.OnPauseChangeDelegate(HandleOnPauseChange));
		CatchAnimationSet[] array = caughtLeft;
		foreach (CatchAnimationSet catchAnimationSet in array)
		{
			SetupAvatarAnimationsStates(characterRendering.characterAnimation, catchAnimationSet.avatar);
			SetupDogGuardAnimationsStates(guardAnimation, catchAnimationSet.guard);
		}
		CatchAnimationSet[] array2 = caughtRight;
		foreach (CatchAnimationSet catchAnimationSet2 in array2)
		{
			SetupAvatarAnimationsStates(characterRendering.characterAnimation, catchAnimationSet2.avatar);
			SetupDogGuardAnimationsStates(guardAnimation, catchAnimationSet2.guard);
		}
		List<AnimationClip> list = new List<AnimationClip>();
		IEnumerator enumerator = guardAnimation.GetEnumerator();
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
		InitializeClips(guardAnimation, list, defaultAnimations.jumpGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.runGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.introGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.dodgeRigthGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.dodgeLeftGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.rollGuard);
		InitializeClips(guardAnimation, list, defaultAnimations.catchupGuard);
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

	private void SetupDogGuardAnimationsStates(Animation animation, string animationClipName)
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
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].localPosition = enemiesStartPos[i] + new Vector3(0f, 0f, z);
			enemies[i].rotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public void ResetModelRootPositionReset()
	{
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].localPosition = enemiesStartPos[i] + new Vector3(0f, 0f, 0f - distanceToCharacterMin);
			enemies[i].rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		distanceToCharacter = 0f - distanceToCharacterMin;
	}

	public void Restart(bool closeToCharacter)
	{
		StopAllCoroutines();
		this.closeToCharacter = closeToCharacter;
		distanceToCharacter = ((!closeToCharacter) ? distanceToCharacterMax : distanceToCharacterMin);
	}

	public void OnEnable()
	{
		lastGroundedSmooth = character.lastGroundedY;
		lastGroundedVelocity = 0f;
		y = character.lastGroundedY;
		SmoothDampFloat smoothDampFloat = x;
		Vector3 position = character.transform.position;
		smoothDampFloat.Value = position.x;
		distanceToCharacter = distanceToCharacterMin;
		closeToCharacter = true;
		verticalSpeed = 0f;
		character.OnJump += OnJump;
		character.OnRollGuard += OnRoll;
		character.OnRoll += OnRollNoAnimation;
		character.OnChangeTrack += OnChangeTrack;
	}

	public void OnDisable()
	{
		character.OnJump -= OnJump;
		character.OnRollGuard -= OnRoll;
		character.OnRoll -= OnRollNoAnimation;
		character.OnChangeTrack += OnChangeTrack;
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
			ShowEnemies(vis: true);
			StopAllCoroutines();
			guardAnimation.Play(ReturnRandomAnimations(defaultAnimations.catchupGuard)[0]);
			guardAnimation.PlayQueued(modelPrefix + "running01");
			GetComponent<AudioSource>().timeSamples = UnityEngine.Random.Range(0, GetComponent<AudioSource>().timeSamples);
			GetComponent<AudioSource>().Play();
			GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.9f, 1.05f);
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMin, t);
			}));
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				GetComponent<AudioSource>().volume = Mathf.SmoothStep(0f, guardProximityLoopVolume, t);
			}));
			closeToCharacter = true;
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
		if (closeToCharacter)
		{
			float distanceFrom = distanceToCharacter;
			closeToCharacter = false;
			StartCoroutine(pTween.To(duration, delegate(float t)
			{
				distanceToCharacter = Mathf.SmoothStep(distanceFrom, distanceToCharacterMax, t);
			}));
			yield return StartCoroutine(pTween.To(duration * 2f, delegate(float t)
			{
				GetComponent<AudioSource>().volume = Mathf.SmoothStep(guardProximityLoopVolume, 0f, t);
			}));
			GetComponent<AudioSource>().Stop();
			if (!game.isDead)
			{
				ShowEnemies(vis: false);
			}
		}
	}

	public void MuteProximityLoop()
	{
		GetComponent<AudioSource>().Stop();
	}

	public void PlayIntro()
	{
		base.gameObject.transform.position = new Vector3(0f, 0f, -10f);
		for (int i = 0; i < enemies.Length; i++)
		{
			enemies[i].position = enemiesStartPos[i];
			enemies[i].rotation = Quaternion.Euler(0f, 0f, 0f);
		}
		string[] array = ReturnRandomAnimations(defaultAnimations.introGuard);
		guardAnimation.Play(array[0]);
		string[] array2 = ReturnRandomAnimations(defaultAnimations.runGuard);
		guardAnimation.CrossFadeQueued(array2[0], 0.2f);
	}

	private string[] ReturnRandomAnimations(string[] guardAnimationNames)
	{
		string[] array = new string[2];
		if (guardAnimationNames != null)
		{
			int num = UnityEngine.Random.Range(0, guardAnimationNames.Length);
			array[0] = guardAnimationNames[num];
		}
		else
		{
			array[0] = string.Empty;
		}
		return array;
	}

	private void OnChangeTrack(Character.OnChangeTrackDirection direction)
	{
		if (!caught)
		{
			if (characterController.isGrounded)
			{
				string[] array = (direction != 0) ? ReturnRandomAnimations(defaultAnimations.dodgeRigthGuard) : ReturnRandomAnimations(defaultAnimations.dodgeLeftGuard);
				guardAnimation[array[0]].speed = game.NormalizedGameSpeed;
				guardAnimation.CrossFade(array[0], 0.1f);
			}
			if (!character.IsJumping)
			{
				string[] array2 = ReturnRandomAnimations(defaultAnimations.runGuard);
				guardAnimation.CrossFadeQueued(array2[0], 0.1f);
			}
		}
	}

	private void OnRoll()
	{
		string[] array = ReturnRandomAnimations(defaultAnimations.rollGuard);
		guardAnimation.CrossFade(array[0], 0.1f);
		string[] array2 = ReturnRandomAnimations(defaultAnimations.runGuard);
		guardAnimation.CrossFadeQueued(array2[0], 0.2f);
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
		if (distanceToCharacter <= distanceToCharacterMin)
		{
		}
		StartCoroutine(JumpCoroutine(delay));
	}

	private IEnumerator JumpCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		JumpAnimation();
		verticalSpeed = character.CalculateJumpVerticalSpeed() * 0.7f;
	}

	private void JumpAnimation()
	{
		string[] array = ReturnRandomAnimations(defaultAnimations.jumpGuard);
		guardAnimation.Play(array[0]);
		string[] array2 = ReturnRandomAnimations(defaultAnimations.runGuard);
		guardAnimation.CrossFadeQueued(array2[0], 0.2f);
	}

	public void CatchPlayer(float pos)
	{
		GetComponent<AudioSource>().Stop();
		StopAllCoroutines();
		caught = true;
		guardAnimation.Play(modelPrefix + "attacking");
		float duration = 1f;
		StartCoroutine(pTween.To(duration, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, character.transform.position, t);
			}
		}));
	}

	public void HitByTrainSequence()
	{
		GetComponent<AudioSource>().Stop();
		StartCoroutine(HitByTrainSequenceCoroutine());
	}

	public IEnumerator HitByTrainSequenceCoroutine()
	{
		GameStats.Instance.guardHitScreen++;
		Vector3 charPos = characterTransform.position;
		float catchUpTime = 0.2f;
		yield return StartCoroutine(pTween.To(catchUpTime, delegate(float t)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				enemies[i].position = Vector3.Lerp(enemies[i].position, new Vector3(charPos.x, charPos.y, charPos.z - 10f), t);
			}
		}));
		yield return new WaitForSeconds(0.4f);
		StartCoroutine(pTween.To(1f, delegate(float t)
		{
			characterTransform.position = Vector3.Lerp(charPos, new Vector3(charPos.x, charPos.y, charPos.z), t);
		}));
		yield return new WaitForSeconds(0.2f);
		guardAnimation.Play(modelPrefix + "attacking");
		yield return new WaitForSeconds(0.2f);
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

	public void LateUpdate()
	{
		SmoothDampFloat smoothDampFloat = x;
		Vector3 position = character.transform.position;
		smoothDampFloat.Target = position.x;
		x.Update();
		lastGroundedSmooth = Mathf.SmoothDamp(lastGroundedSmooth, character.lastGroundedY, ref lastGroundedVelocity, lastGroundedSmoothTime);
		if (y > lastGroundedSmooth)
		{
			verticalSpeed -= gravity * Time.deltaTime;
		}
		y += verticalSpeed * Time.deltaTime;
		y = Mathf.Max(y, lastGroundedSmooth);
		Vector3 position2 = characterTransform.position - Vector3.forward * distanceToCharacter;
		position2.y = y;
		position2.x = x.Value;
		base.transform.position = position2;
	}

	public void ChangeCharacterModel(int customizedVersion)
	{
		string[] array = (from s in DataContainer.Instance.CharacterTableRaw.dataArray
			where s.ID != PlayerInfo.Instance.SelectedCharID
			select s.Modelname).ToArray();
		int num = UnityEngine.Random.Range(0, array.Length);
		string key = modelPrefix = array[num];
		modelPrefix = Regex.Replace(modelPrefix, "[0-9]+$", string.Empty);
		modelPrefix = $"{modelPrefix}01_";
		if (modelLookupTable.TryGetValue(key, out SkinnedMeshRenderer value))
		{
			for (int i = 0; i < models.Length; i++)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = models[i];
				if (PlayerInfo.Instance.ThisGameType == GameType.Multi)
				{
					skinnedMeshRenderer.enabled = false;
				}
				else
				{
					skinnedMeshRenderer.enabled = (skinnedMeshRenderer == value);
				}
			}
		}
		if (value != null)
		{
			model = value;
		}
	}
}
