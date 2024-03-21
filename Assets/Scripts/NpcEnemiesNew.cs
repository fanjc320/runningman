using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class NpcEnemiesNew : MonoBehaviour
{
	public enum State
	{
		AttackAppear,
		AttackCloseTo,
		AttackMiniGame,
		AttackSuccess,
		AttackFail,
		DefenceAppear,
		DefenceCloseTo,
		DefenceRunTo,
		DefenceMiniGame,
		DefenceSuccess,
		DefenceFail,
		Wait,
		Fever,
		None
	}

	[Serializable]
	public class CatchAnimationSet
	{
		public string Avatar;

		public string Enemy;

		public float CatchAvatarAnimationPlayOffset;

		public float WaitTimeBeforeScreen;
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

		public void CopyOverwrite(DefaultAnimations source)
		{
			EnemyRun = (source.EnemyRun.Clone() as string[]);
			EnemyJump = (source.EnemyJump.Clone() as string[]);
			EnemyHang = (source.EnemyHang.Clone() as string[]);
			EnemyLand = (source.EnemyLand.Clone() as string[]);
			EnemyDodgeL = (source.EnemyDodgeL.Clone() as string[]);
			EnemyDodgeR = (source.EnemyDodgeR.Clone() as string[]);
			EnemyRoll = (source.EnemyRoll.Clone() as string[]);
			EnemyCatchup = (source.EnemyCatchup.Clone() as string[]);
			EnemyDie = (source.EnemyDie.Clone() as string[]);
		}
	}

	public static NpcEnemiesNew instance;

	public Animation EnemyAnimation;

	public DefaultAnimations ThisDefaultAnimations;

	public DefaultAnimations OrigDefaultAnimations = new DefaultAnimations();

	public CharacterController ThisCharacterController;

	public Collider ThisCollider;

	public Animation[] NameTagAnimation;

	public SkinnedMeshRenderer[] NameTagSMeshRenderer;

	public MeshRenderer[] NameTagSmallMeshRenderer;

	public MeshRenderer ShadowRenderer;

	public MeshRenderer CautionRenderer;

	private SkinnedMeshRenderer skinMeshRender;

	private bool isAttack = true;

	private bool isJump;

	private float YSpeed;

	private float gravity = -200f;

	private float lastFeverDuring;

	private Vector3 accRelativePos = Vector3.zero;

	private Vector3 positionCache = Vector3.zero;

	private SmoothDampFloat xSmooth = new SmoothDampFloat(0f, 0.1f);

	private List<IEnumerator> etorList = new List<IEnumerator>();

	private string playingAnimName = string.Empty;

	private string[] nameTagAnimName = new string[4]
	{
		"Up",
		"Right",
		"Down",
		"Left"
	};

	private string modelPrefix = string.Empty;

	private string modelChID = string.Empty;

	private bool initialized;

	private State lastState = State.None;

	private State curState = State.None;

	private Dictionary<State, List<Func<IEnumerator>>> stateDispatcher;

	public AnimationCurve AttackCloseToAnimCurveX;

	public AnimationCurve AttackCloseToAnimCurveZ;

	public AnimationCurve AttackFailedAnimCurveZ;

	public AnimationCurve DefenceCloseToAnimCurveZ;

	public AnimationCurve DefenceRunToAnimCurveZ;

	public static NpcEnemiesNew Instance => instance;

	public string PlayingAnimName => playingAnimName;

	private void Awake()
	{
		instance = this;
		initAll();
		EnableComponents(isEnable: false);
		changeState(State.Wait);
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private IEnumerator Start()
	{
		CheckMissionGame();
		yield break;
	}

	private void OnEnable()
	{
		initAll();
		registEvent();
	}

	private void OnDisable()
	{
		unregistEvent();
	}

	private void lateUpdatePre()
	{
	}

	private void lateUpdatePost()
	{
		if (null == Character.Instance)
		{
			return;
		}
		positionCache = Character.Instance.transform.position;
		ref Vector3 reference = ref positionCache;
		Vector3 position = base.transform.position;
		reference.y = position.y;
		positionCache.x = 0f;
		positionCache += accRelativePos;
		YSpeed += gravity * Time.deltaTime;
		float num = YSpeed * Time.deltaTime;
		positionCache.y += num;
		if (0.8f >= positionCache.y)
		{
			positionCache.y = 0.8f;
			YSpeed = 0f;
			if (isJump)
			{
				isJump = false;
				PPActionJump nParent = GameObjectPoolMT<PPActionJump>.Instance.GetNParent(base.transform, null);
				nParent.transform.SetParent(null);
			}
		}
		base.transform.position = positionCache;
	}

	private void LateUpdate()
	{
		lateUpdatePre();
		int count = etorList.Count;
		int num = count - 1;
		while (0 <= num)
		{
			if (etorList[num] != null && !etorList[num].MoveNext())
			{
				etorList[num] = null;
				etorList.RemoveAt(num);
			}
			num--;
		}
		lateUpdatePost();
		if (lastState != State.None)
		{
			procChangeState();
			lastState = State.None;
		}
		if (ShadowRenderer.enabled)
		{
			Vector3 position = ShadowRenderer.transform.position;
			position.y = 0.5f;
			ShadowRenderer.transform.position = position;
		}
		if (CautionRenderer.enabled)
		{
			Vector3 position2 = CautionRenderer.transform.position;
			position2.y = 0.5f;
			CautionRenderer.transform.position = position2;
		}
		if (Game.Instance.isDead && curState != State.Wait)
		{
			Wait(needReset: true);
		}
	}

	public void Reset()
	{
		CheckMissionGame();
		Wait(needReset: true);
	}

	private void initAll()
	{
		if (!initialized)
		{
			initialized = true;
			OrigDefaultAnimations.CopyOverwrite(ThisDefaultAnimations);
			initModel();
			initAnimation();
			initStatefulCoroutine();
			isAttack = true;
			Variable<bool> isInGame = Game.Instance.IsInGame;
			isInGame.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame.OnChange, (Variable<bool>.OnChangeDelegate)delegate(bool isIngame)
			{
				State state = curState;
				if (state == State.Wait || state == State.None)
				{
					isIngame = false;
				}
				EnableComponents(isIngame);
				State state2 = curState;
				if ((state2 == State.AttackMiniGame || state2 == State.DefenceMiniGame) && isIngame)
				{
					EnableCollide(isEnable: false);
				}
			});
			Jetpack.Instance.OnEvtFeverBeginEnd += delegate(bool isBegin)
			{
				if (isBegin)
				{
					EnableCollide(isEnable: false);
					EnableRenderer(isEnable: false);
					changeState(State.Fever);
				}
			};
		}
	}

	private void initAnimation()
	{
		List<AnimationClip> list = new List<AnimationClip>();
		IEnumerator enumerator = EnemyAnimation.GetEnumerator();
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
		Action<Animation, List<AnimationClip>, string[]> action = delegate(Animation animationComponent, List<AnimationClip> addedClips, string[] clipNames)
		{
			for (int i = 0; i < clipNames.Length; i++)
			{
				if (!clipNames[i].Contains(modelPrefix))
				{
					clipNames[i] = modelPrefix + clipNames[i];
				}
			}
		};
		action(EnemyAnimation, list, ThisDefaultAnimations.EnemyRun);
		action(EnemyAnimation, list, ThisDefaultAnimations.EnemyJump);
		action(EnemyAnimation, list, ThisDefaultAnimations.EnemyDodgeL);
		action(EnemyAnimation, list, ThisDefaultAnimations.EnemyDodgeR);
		action(EnemyAnimation, list, ThisDefaultAnimations.EnemyRoll);
		action(EnemyAnimation, list, ThisDefaultAnimations.EnemyCatchup);
		action(EnemyAnimation, list, ThisDefaultAnimations.EnemyDie);
		action(EnemyAnimation, list, ThisDefaultAnimations.EnemyHang);
		action(EnemyAnimation, list, ThisDefaultAnimations.EnemyLand);
	}

	private void initModel()
	{
		SkinnedMeshRenderer[] models = GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive: true);
		models = (from skinRender in models
			where !skinRender.transform.parent.gameObject.name.Contains("name_anim")
			select skinRender).ToArray();
		int[] randModelIndicies = (from s in Enumerable.Range(0, models.Length)
			select (!PlayerInfo.Instance.SelectedCharAnimPrefix.Contains(models[s].gameObject.name)) ? s : ((s + models.Length - 1) % models.Length)).ToArray();
		int rndIdx = UnityEngine.Random.Range(0, models.Length - 1);
		models.Select(delegate(SkinnedMeshRenderer skinRender, int index)
		{
			if (index == randModelIndicies[rndIdx])
			{
				skinMeshRender = skinRender;
				skinRender.enabled = true;
				modelPrefix = skinRender.gameObject.name;
				modelChID = (from s in DataContainer.Instance.CharacterTableRaw.dataArray
					where s.Modelname.Equals(skinRender.gameObject.name)
					select s).First().ID;
			}
			else
			{
				skinRender.enabled = false;
			}
			return index;
		}).All((int index) => true);
		modelPrefix = Regex.Replace(modelPrefix, "[0-9]+$", string.Empty);
		modelPrefix = $"{modelPrefix}01_";
		NameTagAnimation.All(delegate(Animation anim)
		{
			anim.Stop(anim.clip.name);
			anim.Rewind(anim.clip.name);
			anim.Sample();
			anim.gameObject.SetActive(value: false);
			return true;
		});
	}

	private void registEvent()
	{
		OnTriggerObject component = base.transform.Find("Collider Npcs").GetComponent<OnTriggerObject>();
		component.OnEnter = OnTriggerObjectEnter;
		component.OnExit = OnTriggerObjectExit;
	}

	private void unregistEvent()
	{
		if (null != Character.Instance)
		{
			Character.Instance.OnJump -= OnObstacleJump;
			Character.Instance.OnRoll -= OnObstacleRoll;
		}
		Transform transform = base.transform.Find("Collider Npcs");
		if (null != transform)
		{
			OnTriggerObject component = transform.GetComponent<OnTriggerObject>();
			if (null != component)
			{
				component.OnEnter = null;
				component.OnExit = null;
			}
		}
	}

	private void initStatefulCoroutine()
	{
		stateDispatcher = new Dictionary<State, List<Func<IEnumerator>>>
		{
			{
				State.AttackAppear,
				new List<Func<IEnumerator>>
				{
					cetAttackAppear
				}
			},
			{
				State.AttackCloseTo,
				new List<Func<IEnumerator>>
				{
					cetAttackCloseTo
				}
			},
			{
				State.AttackMiniGame,
				new List<Func<IEnumerator>>
				{
					cetAttackMiniGame
				}
			},
			{
				State.AttackSuccess,
				new List<Func<IEnumerator>>
				{
					cetAttackSuccess
				}
			},
			{
				State.AttackFail,
				new List<Func<IEnumerator>>
				{
					cetAttackFail
				}
			},
			{
				State.DefenceAppear,
				new List<Func<IEnumerator>>
				{
					cetDefenceAppear
				}
			},
			{
				State.DefenceCloseTo,
				new List<Func<IEnumerator>>
				{
					cetDefenceCloseTo
				}
			},
			{
				State.DefenceRunTo,
				new List<Func<IEnumerator>>
				{
					cetDefenceRunTo
				}
			},
			{
				State.DefenceMiniGame,
				new List<Func<IEnumerator>>
				{
					cetDefenceMiniGame
				}
			},
			{
				State.DefenceSuccess,
				new List<Func<IEnumerator>>
				{
					cetDefenceSuccess
				}
			},
			{
				State.DefenceFail,
				new List<Func<IEnumerator>>
				{
					cetDefenceFail
				}
			},
			{
				State.Fever,
				new List<Func<IEnumerator>>
				{
					cetFever
				}
			},
			{
				State.Wait,
				new List<Func<IEnumerator>>
				{
					cetWait
				}
			}
		};
	}

	private void changeState(State state)
	{
		switch (state)
		{
		case State.AttackMiniGame:
		case State.DefenceMiniGame:
			lastState = state;
			break;
		case State.None:
			lastState = State.AttackAppear;
			break;
		default:
			lastState = state;
			break;
		}
	}

	private void procChangeState()
	{
		etorList.Clear();
		curState = lastState;
		List<Func<IEnumerator>> source = stateDispatcher[curState];
		source.All(delegate(Func<IEnumerator> func)
		{
			etorList.Add(func());
			return true;
		});
	}

	private IEnumerator cetAttackAppear()
	{
		EnableComponents(isEnable: true);
		CautionRenderer.enabled = false;
		PlayAnimation(ThisDefaultAnimations.EnemyRun);
		accRelativePos.x = AttackCloseToAnimCurveX.Evaluate(0f);
		accRelativePos.z = AttackCloseToAnimCurveZ.Evaluate(0f);
		YSpeed = 0f;
		changeState(State.AttackCloseTo);
		yield break;
	}

	private IEnumerator cetAttackCloseTo()
	{
		float randomXOffset = 0f;
		switch (UnityEngine.Random.Range(0, 3))
		{
		case 0:
			randomXOffset = -20f;
			break;
		case 1:
			randomXOffset = 0f;
			break;
		case 2:
			randomXOffset = 20f;
			break;
		}
		float period = AttackCloseToAnimCurveX.keys[AttackCloseToAnimCurveX.length - 1].time;
		IEnumerator etorCloseTo = pTween.To(period, 0f, period, delegate(float elapsed)
		{
			accRelativePos.x = AttackCloseToAnimCurveX.Evaluate(elapsed) + randomXOffset;
			accRelativePos.z = AttackCloseToAnimCurveZ.Evaluate(elapsed);
		});
		while (etorCloseTo.MoveNext())
		{
			yield return 0;
		}
		isAttack = false;
		Wait();
	}

	private IEnumerator cetAttackMiniGame()
	{
		Character.Instance.verticalSpeed = -2000f;
		Character.Instance.MoveForward();
		positionCache = base.transform.position;
		positionCache.y = 0f;
		base.transform.position = positionCache;
		EnableCollide(isEnable: false);
		Game.Instance.ChangeState(NameTagAttackState.Instance);
		IEnumerator etorDelay = pTween.To(0f, delegate
		{
		});
		while (etorDelay.MoveNext())
		{
			yield return 0;
		}
		Track track = Track.Instance;
		Vector3 position = Character.Instance.transform.position;
		track.LayEmptyChunks(position.z, 200f);
		CrossAnimation(ThisDefaultAnimations.EnemyRun);
		EnemyAnimation[playingAnimName].speed = 0.05f;
		string chRunAnimName = PlayerInfo.Instance.SelectedCharAnimPrefix + "running01";
		CharacterRendering.Instance.characterAnimation.CrossFade(chRunAnimName);
		CharacterRendering.Instance.characterAnimation[chRunAnimName].speed = 0.05f;
	}

	private IEnumerator cetAttackSuccess()
	{
		string missionGoalKey = $"chcoins_{DataContainer.Instance.CharacterTableRaw[modelChID].CID}";
		PlayerInfo.Instance.MsnCollectableGolals[missionGoalKey] = 1;
		IEnumerator etorDelay3 = pTween.To(0.5f, delegate
		{
		});
		while (etorDelay3.MoveNext())
		{
			yield return 0;
		}
		NameTagAnimation[NameTagAttackState.Instance.LastDragDir].gameObject.SetActive(value: true);
		NameTagAnimation[NameTagAttackState.Instance.LastDragDir].Play("Take 001");
		NameTagSMeshRenderer[NameTagAttackState.Instance.LastDragDir].material.SetTexture("_SecondTex", DataContainer.Instance.GetAssetResources<Texture>("Name/ingame_" + DataContainer.Instance.CharacterTableRaw[modelChID].Nameimagepath));
		ResetAnimation();
		etorDelay3 = pTween.To(1.5f, delegate
		{
		});
		while (etorDelay3.MoveNext())
		{
			yield return 0;
		}
		NameTagAnimation.All(delegate(Animation anim)
		{
			anim.Stop(anim.clip.name);
			anim.Rewind(anim.clip.name);
			anim.Sample();
			anim.gameObject.SetActive(value: false);
			return true;
		});
		Game.Instance.ChangeState(Game.Instance.Running);
		EnableCollide(isEnable: false);
		PlayAnimation(ThisDefaultAnimations.EnemyDie);
		float xTargetOffset = accRelativePos.x + 15f;
		etorDelay3 = pTween.To(0.25f, delegate(float norm)
		{
			xSmooth.Value = accRelativePos.x;
			xSmooth.Target = xTargetOffset;
			xSmooth.Update();
			accRelativePos.x = xSmooth.Value;
			accRelativePos.z = norm * -20f;
		});
		while (etorDelay3.MoveNext())
		{
			yield return 0;
		}
		PPCharacterCoin characterCoinGetEff = GameObjectPoolMT<PPCharacterCoin>.Instance.GetNParent(Character.Instance.transform, null);
		characterCoinGetEff.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.mainTexture = DataContainer.Instance.GetAssetResources<Texture>((from s in DataContainer.Instance.CharacterTableRaw.dataArray
			where s.ID.Equals(modelChID)
			select s).First().Chcoinimagepath);
		ThisDefaultAnimations.CopyOverwrite(OrigDefaultAnimations);
		initModel();
		initAnimation();
		Wait(needReset: true);
	}

	private IEnumerator cetAttackFail()
	{
		NameTagAnimation.All(delegate(Animation anim)
		{
			anim.Stop(anim.clip.name);
			anim.Rewind(anim.clip.name);
			anim.Sample();
			anim.gameObject.SetActive(value: false);
			return true;
		});
		ResetAnimation();
		IEnumerator etorDelay = pTween.To(2f, delegate
		{
		});
		while (etorDelay.MoveNext())
		{
			yield return 0;
		}
		Game.Instance.ChangeState(Game.Instance.Running);
		float period = AttackFailedAnimCurveZ.keys[AttackFailedAnimCurveZ.length - 1].time;
		IEnumerator etorAttackFailTo = pTween.To(period, 0f, period, delegate(float elapsed)
		{
			accRelativePos.z = AttackFailedAnimCurveZ.Evaluate(elapsed);
		});
		while (etorAttackFailTo.MoveNext())
		{
			yield return 0;
		}
		isAttack = false;
		Wait();
	}

	private IEnumerator cetDefenceAppear()
	{
		EnableComponents(isEnable: true);
		CautionRenderer.enabled = false;
		PlayAnimation(ThisDefaultAnimations.EnemyRun);
		ref Vector3 reference = ref accRelativePos;
		Vector3 position = Character.Instance.transform.position;
		reference.x = position.x;
		accRelativePos.z = DefenceCloseToAnimCurveZ.Evaluate(0f);
		YSpeed = 0f;
		changeState(State.DefenceCloseTo);
		yield break;
	}

	private IEnumerator cetDefenceCloseTo()
	{
		float fovBegin = 71.14584f;
		float fovEnd = 90f;
		float period = DefenceCloseToAnimCurveZ.keys[DefenceCloseToAnimCurveZ.length - 1].time;
		IEnumerator etorCloseTo = pTween.To(period, 0f, period, delegate(float elapsed)
		{
			xSmooth.Value = accRelativePos.x;
			SmoothDampFloat smoothDampFloat = xSmooth;
			Vector3 position = Character.Instance.transform.position;
			smoothDampFloat.Target = position.x;
			xSmooth.Update();
			accRelativePos.x = xSmooth.Value;
			accRelativePos.z = DefenceCloseToAnimCurveZ.Evaluate(elapsed);
			Camera.main.fieldOfView = Mathf.Lerp(fovBegin, fovEnd, elapsed / period);
		});
		while (etorCloseTo.MoveNext())
		{
			yield return 0;
		}
		Camera.main.fieldOfView = fovEnd;
		changeState(State.DefenceRunTo);
	}

	private IEnumerator cetDefenceRunTo()
	{
		float fovBegin = 90f;
		float fovEnd = 71.14584f;
		Camera mainCam = Camera.main;
		LeanTween.delayedCall(2f, (Action)delegate
		{
			StartCoroutine(pTween.To(1f, delegate(float norm)
			{
				mainCam.fieldOfView = Mathf.Lerp(fovBegin, fovEnd, norm);
			}, delegate
			{
				mainCam.fieldOfView = fovEnd;
			}));
		});
		float randomXOffset = 0f;
		switch (UnityEngine.Random.Range(0, 3))
		{
		case 0:
			randomXOffset = -20f;
			break;
		case 1:
			randomXOffset = 0f;
			break;
		case 2:
			randomXOffset = 20f;
			break;
		}
		float warningPeriod = 2f;
		IEnumerator etorWarningTo = pTween.To(warningPeriod, 0f, warningPeriod, delegate(float elapsed)
		{
			xSmooth.Value = accRelativePos.x;
			xSmooth.Target = randomXOffset;
			xSmooth.Update();
			accRelativePos.x = xSmooth.Value;
			if (0.05f < Mathf.PingPong(elapsed, 0.1f))
			{
				CautionRenderer.enabled = true;
			}
			else
			{
				CautionRenderer.enabled = false;
			}
		});
		while (etorWarningTo.MoveNext())
		{
			yield return 0;
		}
		float period = DefenceRunToAnimCurveZ.keys[DefenceRunToAnimCurveZ.length - 1].time;
		IEnumerator etorRunTo = pTween.To(period, 0f, period, delegate(float elapsed)
		{
			accRelativePos.z = DefenceRunToAnimCurveZ.Evaluate(elapsed);
		});
		while (etorRunTo.MoveNext())
		{
			yield return 0;
		}
		Wait(needReset: true);
	}

	private IEnumerator cetDefenceMiniGame()
	{
		Character.Instance.verticalSpeed = -2000f;
		Character.Instance.MoveForward();
		Camera.main.fieldOfView = 71.14584f;
		EnableCollide(isEnable: false);
		Game.Instance.ChangeState(NameTagAttackState.Instance);
		IEnumerator etorDelay = pTween.To(0f, delegate
		{
		});
		while (etorDelay.MoveNext())
		{
			yield return 0;
		}
		Track track = Track.Instance;
		Vector3 position = Character.Instance.transform.position;
		track.LayEmptyChunks(position.z, 200f);
		CrossAnimation(ThisDefaultAnimations.EnemyRun);
		EnemyAnimation[playingAnimName].speed = 0.05f;
		string chRunAnimName = PlayerInfo.Instance.SelectedCharAnimPrefix + "running01";
		CharacterRendering.Instance.characterAnimation.CrossFade(chRunAnimName);
		CharacterRendering.Instance.characterAnimation[chRunAnimName].speed = 0.05f;
	}

	private IEnumerator cetDefenceSuccess()
	{
		ResetAnimation();
		IEnumerator etorDelay2 = pTween.To(2f, delegate
		{
		});
		while (etorDelay2.MoveNext())
		{
			yield return 0;
		}
		Game.Instance.ChangeState(Game.Instance.Running);
		EnableCollide(isEnable: false);
		PlayAnimation(ThisDefaultAnimations.EnemyDie);
		etorDelay2 = pTween.To(0.25f, delegate(float norm)
		{
			xSmooth.Value = accRelativePos.x;
			xSmooth.Target = 10f;
			xSmooth.Update();
			accRelativePos.x = xSmooth.Value;
			accRelativePos.z = norm * -20f;
		});
		while (etorDelay2.MoveNext())
		{
			yield return 0;
		}
		Wait(needReset: true);
	}

	private IEnumerator cetDefenceFail()
	{
		ResetAnimation();
		Character.Instance.NameTagAnimRoot.gameObject.SetActive(value: true);
		Character.Instance.NameTagAnimation[NameTagAttackState.Instance.LastDragDir].gameObject.SetActive(value: true);
		Character.Instance.NameTagAnimation[NameTagAttackState.Instance.LastDragDir].Play("Take 001");
		Character.Instance.NameTagSMeshRenderer[NameTagAttackState.Instance.LastDragDir].material.SetTexture("_SecondTex", DataContainer.Instance.GetAssetResources<Texture>("Name/ingame_" + DataContainer.Instance.CharacterTableRaw[PlayerInfo.Instance.SelectedCharID].Nameimagepath));
		bool isProtected = PlayerInfo.Instance.CharacterSkills[3] || PlayerInfo.Instance.StartItems[5];
		if (PlayerInfo.Instance.CharacterSkills[3])
		{
			Character.Instance.NameTagSmallMeshRenderer[NameTagAttackState.Instance.LastDragDir].enabled = true;
			Character.Instance.NameTagSmallMeshRenderer[NameTagAttackState.Instance.LastDragDir].material.SetTexture("_SecondTex", DataContainer.Instance.GetAssetResources<Texture>("Name/ingame_" + DataContainer.Instance.CharacterTableRaw[PlayerInfo.Instance.SelectedCharID].Nameimagepath));
		}
		else if (PlayerInfo.Instance.StartItems[5])
		{
			PlayerInfo.Instance.StartItems[5] = false;
			MainUIManager.Instance.StopBuffIcon(5);
			Character.Instance.NameTagSmallMeshRenderer[NameTagAttackState.Instance.LastDragDir].enabled = true;
			Character.Instance.NameTagSmallMeshRenderer[NameTagAttackState.Instance.LastDragDir].material.SetTexture("_SecondTex", DataContainer.Instance.GetAssetResources<Texture>("Name/ingame_" + DataContainer.Instance.CharacterTableRaw[PlayerInfo.Instance.SelectedCharID].Nameimagepath));
		}
		IEnumerator etorDelay = pTween.To(2f, delegate
		{
		});
		while (etorDelay.MoveNext())
		{
			yield return 0;
		}
		Character.Instance.NameTagAnimation.All(delegate(Animation anim)
		{
			anim.Stop(anim.clip.name);
			anim.Rewind(anim.clip.name);
			anim.Sample();
			anim.gameObject.SetActive(value: false);
			return true;
		});
		Character.Instance.NameTagSmallMeshRenderer[NameTagAttackState.Instance.LastDragDir].enabled = false;
		Character.Instance.NameTagAnimRoot.gameObject.gameObject.SetActive(value: false);
		if (isProtected)
		{
			Game.Instance.ChangeState(Game.Instance.Running);
		}
		else
		{
			Character.Instance.HitByNpcSequence2();
		}
		EnableCollide(isEnable: false);
		Wait(needReset: true);
	}

	private IEnumerator cetFever()
	{
		changeState(State.Wait);
		yield break;
	}

	private IEnumerator cetWait()
	{
		EnableComponents(isEnable: false);
		while (Game.Instance.isDead)
		{
			yield return 0;
		}
		float waitPeriod = 11f + 0.5f * UnityEngine.Random.Range(-4f, 4f);
		if (!Mathf.Approximately(lastFeverDuring, 0f))
		{
			waitPeriod = Mathf.Clamp(waitPeriod - lastFeverDuring, 0f, float.MaxValue);
			lastFeverDuring = 0f;
		}
		IEnumerator etorWait = pTween.To(waitPeriod, delegate
		{
		});
		while (etorWait.MoveNext())
		{
			yield return 0;
		}
		if (0.9f <= GameStats.Instance.FeverGauge.Ratio)
		{
			changeState(State.Wait);
			yield break;
		}
		while (Game.Instance.Jetpack.isActive)
		{
			yield return 0;
		}
		if (0.66f > UnityEngine.Random.value)
		{
			if (isAttack)
			{
				changeState(State.AttackAppear);
			}
			else
			{
				changeState(State.DefenceAppear);
			}
		}
		else
		{
			changeState(State.Wait);
		}
	}

	private IEnumerator cetRollNoAnim(float delay)
	{
		yield return pTween.To(delay, delegate
		{
		});
		YSpeed = 0f - Character.Instance.CalculateJumpVerticalSpeed();
	}

	public void OnObstacleJump()
	{
		string animation = ReturnRandomAnimations(ThisDefaultAnimations.EnemyJump);
		EnemyAnimation.Play(animation);
		string animation2 = ReturnRandomAnimations(ThisDefaultAnimations.EnemyRun);
		EnemyAnimation.CrossFadeQueued(animation2, 0.2f);
		YSpeed = Character.Instance.CalculateJumpVerticalSpeed() * 1f;
		PPActionJump nParent = GameObjectPoolMT<PPActionJump>.Instance.GetNParent(base.transform, null);
		nParent.transform.SetParent(null);
		isJump = true;
	}

	public void OnObstacleJump(float delay)
	{
		YSpeed = Character.Instance.CalculateJumpVerticalSpeed() * 1f;
	}

	public void OnObstacleSide(float offsetX)
	{
	}

	public void OnObstacleRollNoAnim()
	{
		YSpeed = 0f - Character.Instance.CalculateJumpVerticalSpeed();
	}

	public void OnObstacleRoll()
	{
		string animation = ReturnRandomAnimations(ThisDefaultAnimations.EnemyRoll);
		EnemyAnimation.Play(animation);
		string animation2 = ReturnRandomAnimations(ThisDefaultAnimations.EnemyRun);
		EnemyAnimation.CrossFadeQueued(animation2, 0.2f);
		YSpeed = 0f - Character.Instance.CalculateJumpVerticalSpeed();
	}

	private void OnTriggerObjectEnter(Collider collider)
	{
		if (Layers.Instance.Hit == collider.gameObject.layer || Layers.Instance.HitBounceOnly == collider.gameObject.layer)
		{
			WayPointerHelper componentInChildren = collider.GetComponentInChildren<WayPointerHelper>();
			if (null != componentInChildren)
			{
				componentInChildren.OnHitProc();
			}
		}
		else if (Layers.Instance.Character == collider.gameObject.layer && collider.gameObject.name.Equals("Collider Character"))
		{
			switch (curState)
			{
			case State.AttackCloseTo:
				changeState(State.AttackMiniGame);
				break;
			case State.DefenceRunTo:
				changeState(State.DefenceMiniGame);
				break;
			}
		}
	}

	private void OnTriggerObjectExit(Collider collider)
	{
		if (Layers.Instance.Character == collider.gameObject.layer)
		{
		}
	}

	public void EnableComponents(bool isEnable)
	{
		EnableCollide(isEnable);
		EnableRenderer(isEnable);
	}

	public void EnableCollide(bool isEnable)
	{
		ThisCollider.enabled = isEnable;
		ThisCharacterController.enabled = isEnable;
	}

	public void EnableRenderer(bool isEnable)
	{
		skinMeshRender.enabled = isEnable;
		EnemyAnimation.enabled = isEnable;
		ShadowRenderer.enabled = isEnable;
		CautionRenderer.enabled = isEnable;
	}

	public void Wait(bool needReset = false)
	{
		if (needReset)
		{
			isAttack = true;
		}
		NameTagAnimation.All(delegate(Animation anim)
		{
			anim.Stop(anim.clip.name);
			anim.Rewind(anim.clip.name);
			anim.Sample();
			anim.gameObject.SetActive(value: false);
			return true;
		});
		EnableComponents(isEnable: false);
		changeState(State.Wait);
	}

	public Vector3 HoldPosition()
	{
		if (isAttack)
		{
			accRelativePos.y = 0f;
			accRelativePos.z = 12f;
			lateUpdatePost();
			return base.transform.position;
		}
		accRelativePos.z = -12f;
		lateUpdatePost();
		return Character.Instance.transform.position;
	}

	public void Attack(bool isSuccess)
	{
		switch (curState)
		{
		case State.AttackMiniGame:
			PlayerInfo.Instance.AccMissionByCondTypeID("doplucksticker", "-1", 1.ToString());
			changeState((!isSuccess) ? State.AttackFail : State.AttackSuccess);
			break;
		case State.DefenceMiniGame:
			changeState((!isSuccess) ? State.DefenceFail : State.DefenceSuccess);
			break;
		}
	}

	private void CheckMissionGame()
	{
		if (PlayerInfo.Instance.ThisGameType == GameType.NormalSingle || PlayerInfo.Instance.ThisGameType == GameType.Multi)
		{
			Instance.EnableComponents(isEnable: false);
			base.enabled = false;
			base.gameObject.SetActive(value: false);
		}
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

	private void PlayAnimation(string[] enemyAnimations)
	{
		string animation = ReturnRandomAnimations(enemyAnimations);
		EnemyAnimation.Play(animation);
		playingAnimName = animation;
	}

	private void CrossAnimation(string[] enemyAnimations)
	{
		string animation = ReturnRandomAnimations(enemyAnimations);
		EnemyAnimation.CrossFade(animation);
		playingAnimName = animation;
	}

	private void ResetAnimation()
	{
		EnemyAnimation[playingAnimName].speed = 1f;
		string name = PlayerInfo.Instance.SelectedCharAnimPrefix + "running01";
		CharacterRendering.Instance.characterAnimation[name].speed = 1f;
	}
}
