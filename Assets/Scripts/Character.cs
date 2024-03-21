using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Character : MonoBehaviour
{
	private struct SuperSneakersJump
	{
		public float z_start;

		public float z_length;

		public float z_end;

		public float y_start;
	}

	public enum StumbleHorizontalHit
	{
		Left,
		LeftCorner,
		Center,
		RightCorner,
		Right
	}

	public enum StumbleVerticalHit
	{
		Upper,
		Middle,
		Lower
	}

	public enum StumbleType
	{
		Normal,
		Bush,
		Side
	}

	public delegate void OnStumbleDelegate(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName);

	public enum CriticalHitType
	{
		Train,
		Barrier,
		MovingTrain,
		None
	}

	public delegate void OnCriticalHitDelegate(CriticalHitType type);

	public delegate void OnJumpDelegate();

	public delegate void OnLandingDelegate(Transform characterTransform);

	public delegate void OnHangtimeDelegate();

	public delegate void OnRollDelegate();

	public delegate void OnRollGuardDelegate();

	public delegate void OnRollEnemyNpcDelegate();

	public delegate void OnHitByTrainDelegate();

	public enum OnChangeTrackDirection
	{
		Left,
		Right
	}

	public delegate void OnChangeTrackDelegate(OnChangeTrackDirection direction);

	public delegate void OnTutorialMoveBackToCheckPointDelegate(float duration);

	public delegate void OnTutorialStartFromCheckPointDelegate();

	public delegate void OnPassedObstacleDelegate(ObstacleType type);

	public delegate void OnJumpOverTrainDelegate();

	public enum OnNametagAction
	{
		Success,
		Miss
	}

	public delegate void OnNametagDelegate(OnNametagAction action);

	public enum ObstacleType
	{
		JumpHighBarrier,
		JumpTrain,
		RollBarrier,
		JumpBarrier,
		None
	}

	private enum ImpactX
	{
		Left,
		Middle,
		Right
	}

	private enum ImpactY
	{
		Upper,
		Middle,
		Lower
	}

	private enum ImpactZ
	{
		Before,
		Middle,
		After
	}

	public Transform characterRoot;

	public CapsuleCollider characterCollider;

	public OnTriggerObject coinMagnetCollider;

	public OnTriggerObject coinMagnetLongCollider;

	public float characterAngle = 45f;

	public ParticleSystem hoverboardCrashParticleSystem;

	public CharacterPickupParticles CharacterPickupParticleSystem;

	public float ColliderTrackWidth = 17f;

	public Transform NameTagAnimRoot;

	public Animation[] NameTagAnimation;

	public SkinnedMeshRenderer[] NameTagSMeshRenderer;

	public MeshRenderer[] NameTagSmallMeshRenderer;

	public CharacterController characterController;

	[HideInInspector]
	public OnTriggerObject characterColliderTrigger;

	[HideInInspector]
	public CharacterModel characterModel;

	[HideInInspector]
	public CharacterCamera characterCamera;

	[HideInInspector]
	public Hoverboard hoverboard;

	[HideInInspector]
	public SuperSneakers superSneakers;

	[HideInInspector]
	public Running running;

	[HideInInspector]
	public bool immuneToCriticalHit;

	[HideInInspector]
	public int trackIndex;

	[HideInInspector]
	public float x;

	public float z;

	public float verticalSpeed;

	[HideInInspector]
	public float lastGroundedY;

	private bool isInsideSubway;

	[HideInInspector]
	public float subwayMaxY;

	private Vector3 characterControllerCenter;

	private float characterControllerHeight;

	private Vector3 characterColliderCenter;

	private float characterColliderHeight;

	private int initialTrackIndex = 1;

	private int trackMovement;

	private int trackMovementNext;

	private float characterRotation;

	private int trackIndexTarget;

	private float trackIndexPosition;

	private Game game;

	private FollowingGuard guard;

	private Revive revive;

	[HideInInspector]
	public float jumpHeight;

	public float gravity = 200f;

	public float jumpHeightNormal = 20f;

	public float jumpHeightSuperSneakers = 40f;

	public float verticalFallSpeedLimit = -1f;

	public float stumbleCornerTolerance = 15f;

	public float stumbleDecayTime = 5f;

	public float delayRaceTime = 5f;

	private bool isJumping;

	private bool isRolling;

	private bool isFalling;

	public bool isStumbling;

	private bool inAirJump;

	private bool isJumpingHigher;

	public Variable<bool> IsGrounded = new Variable<bool>(initialValue: false);

	private HashSet<Collider> subwayColliders = new HashSet<Collider>();

	private VariableBool squeezeCollider = new VariableBool();

	private SuperSneakersJump? superSneakersJump;

	public AnimationCurve superSneakersJumpCurve;

	public float superSneakersJumpApexRatio = 0.5f;

	private string lastHitTag;

	public float hitPoistionDistance;

	[HideInInspector]
	public bool stopColliding;

	private bool startedJumpFromGround;

	private float trainJumpSampleZ;

	private float trainJumpSampleLength = 10f;

	private bool trainJump;

	private float verticalSpeed_jumpTolerance = -30f;

	private Layers layers;

	private ObstacleType lastObstacleTriggerType;

	private int lastObstacleTriggerTrackIndex;

	public float sameLaneTimeStamp;

	private bool _dJumpable = true;

	public static Character instance;

	public bool IsTrainJump => trainJump;

	public bool IsStumbling
	{
		get
		{
			return isStumbling;
		}
		set
		{
			isStumbling = value;
		}
	}

	public bool IsFalling
	{
		get
		{
			return isFalling;
		}
		set
		{
			isFalling = value;
		}
	}

	public bool IsJumping
	{
		get
		{
			return isJumping;
		}
		set
		{
			isJumping = value;
		}
	}

	public bool IsRolling => isRolling;

	public bool IsInsideSubway => isInsideSubway;

	public int TrackIndex => trackIndex;

	public bool IsAboveGround
	{
		get
		{
			Vector3 position = base.transform.position;
			return position.y > 20f;
		}
	}

	public bool IsJumpingHigher
	{
		get
		{
			return isJumpingHigher || (hoverboard.isActive && HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.bouncer);
		}
		set
		{
			isJumpingHigher = value;
		}
	}

	public VariableBool SqueezeCollider => squeezeCollider;

	public static Character Instance
	{
		get
		{
			if (instance == null)
			{
				instance = (UnityEngine.Object.FindObjectOfType(typeof(Character)) as Character);
			}
			return instance;
		}
	}

	public event OnStumbleDelegate OnStumble;

	public event OnCriticalHitDelegate OnCriticalHit;

	public event OnJumpDelegate OnJump;

	public event OnLandingDelegate OnLanding;

	public event OnHangtimeDelegate OnHangtime;

	public event OnRollDelegate OnRoll;

	public event OnRollGuardDelegate OnRollGuard;

	public event OnRollEnemyNpcDelegate OnRollEnemyNpc;

	public event OnHitByTrainDelegate OnHitByTrain;

	public event OnChangeTrackDelegate OnChangeTrack;

	public event OnTutorialMoveBackToCheckPointDelegate OnTutorialMoveBackToCheckPoint;

	public event OnTutorialStartFromCheckPointDelegate OnTutorialStartFromCheckPoint;

	public event OnPassedObstacleDelegate OnPassedObstacle;

	public event OnJumpOverTrainDelegate OnJumpOverTrain;

	public event OnNametagDelegate OnNametag;

	public void ResetCharacterSkill()
	{
		for (int j = 0; Enum.GetValues(typeof(CharacterSkillType)).Length > j; j++)
		{
			PlayerInfo.Instance.CharacterSkills[j] = false;
		}
		string selCID = DataContainer.Instance.CharacterTableRaw[PlayerInfo.Instance.SelectedCharID].CID;
		int num = (from s in DataContainer.Instance.CharacterTableRaw.dataArray.Select((CharacterInfoData s, int i) => new
			{
				Data = s,
				Index = i
			})
			where s.Data.CID == selCID
			select s.Index).First();
		PlayerInfo.Instance.CharacterSkills[num] = true;
		if (num == 1)
		{
			PlayerInfo.Instance.StartItems[3] = true;
			PlayerInfo.Instance.StartItemCounts[3] += 3;
		}
	}

	public void Initialize()
	{
		ResetCharacterSkill();
		layers = Layers.Instance;
		game = Game.Instance;
		Variable<bool> isInGame2 = game.IsInGame;
		isInGame2.OnChange = (Variable<bool>.OnChangeDelegate)Delegate.Combine(isInGame2.OnChange, (Variable<bool>.OnChangeDelegate)delegate(bool isInGame)
		{
			if (!isInGame)
			{
				StopAllCoroutines();
				immuneToCriticalHit = false;
				characterController.enabled = true;
				stopColliding = false;
			}
		});
		VariableBool variableBool = squeezeCollider;
		variableBool.OnChange = (VariableBool.OnChangeDelegate)Delegate.Combine(variableBool.OnChange, (VariableBool.OnChangeDelegate)delegate(bool squeeze)
		{
			if (squeeze)
			{
				characterController.height = 4f;
				characterController.center = new Vector3(0f, 2f, characterControllerCenter.z);
				characterCollider.height = 4f;
				characterCollider.center = new Vector3(0f, 4f, characterColliderCenter.z);
			}
			else
			{
				characterController.center = characterControllerCenter;
				characterController.height = characterControllerHeight;
				characterCollider.center = characterColliderCenter;
				characterCollider.height = characterColliderHeight;
			}
		});
		hoverboard = Hoverboard.Instance;
		running = Running.Instance;
		revive = Revive.Instance;
		revive.OnRevive += HandleRevive;
		CharacterRendering component = GetComponent<CharacterRendering>();
		component.Initialize();
		superSneakers = this.FindObject<SuperSneakers>();
		characterModel = GetComponentInChildren<CharacterModel>();
		characterRoot = characterModel.transform;
		characterCamera = CharacterCamera.Instance;
		guard = FollowingGuard.Instance;
		CharacterPickupParticleSystem = GetComponentInChildren<CharacterPickupParticles>();
		characterColliderTrigger = characterCollider.GetComponent<OnTriggerObject>();
		OnTriggerObject onTriggerObject = characterColliderTrigger;
		onTriggerObject.OnEnter = (OnTriggerObject.OnEnterDelegate)Delegate.Combine(onTriggerObject.OnEnter, new OnTriggerObject.OnEnterDelegate(OnCharacterColliderEnter));
		OnTriggerObject onTriggerObject2 = characterColliderTrigger;
		onTriggerObject2.OnExit = (OnTriggerObject.OnExitDelegate)Delegate.Combine(onTriggerObject2.OnExit, new OnTriggerObject.OnExitDelegate(OnCharacterColliderExit));
		characterControllerCenter = characterController.center;
		characterControllerHeight = characterController.height;
		characterColliderCenter = characterCollider.center;
		characterColliderHeight = characterCollider.height;
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
			componentInChildren.NotifyPickup(CharacterPickupParticleSystem);
		}
	}

	private IEnumerator Pull(Coin coin)
	{
		Transform pivot = coin.PivotTransform;
		Vector3 position = pivot.position;
		float distance = (position - characterController.transform.position).magnitude;
		Vector3 offsetCoinHitPosition = new Vector3(0f, -6f, 0f);
		yield return StartCoroutine(pTween.To(distance / (200f * game.NormalizedGameSpeed), delegate(float t)
		{
			pivot.position = Vector3.Lerp(position, transform.position + offsetCoinHitPosition, t * t);
		}));
		Pickup pickup = coin.GetComponent<Pickup>();
		NotifyPickup(pickup);
	}

	public void Restart()
	{
		z = 0f;
		RestartCharRotation();
		verticalSpeed = 0f;
		superSneakersJump = null;
		jumpHeight = jumpHeightNormal;
		inAirJump = false;
		isJumping = false;
		isRolling = false;
		IsGrounded.Value = false;
		lastGroundedY = 0f;
		guard.Restart(closeToCharacter: true);
		StartStumble();
		startedJumpFromGround = false;
		sameLaneTimeStamp = Time.time;
		subwayColliders.Clear();
		isInsideSubway = false;
	}

	public void RestartCharRotation()
	{
		trackIndex = initialTrackIndex;
		trackIndexTarget = initialTrackIndex;
		x = Track.Instance.GetTrackX(trackIndex);
		trackIndexPosition = trackIndex;
		trackMovement = 0;
		trackMovementNext = 0;
		characterRotation = 0f;
		characterRoot.localRotation = Quaternion.Euler(0f, characterRotation, 0f);
		squeezeCollider.Clear();
		characterController.transform.position = Track.Instance.GetPosition(x, z) + Vector3.up * 5f;
		characterController.Move(-5f * Vector3.up);
	}

	public void RestartRevive()
	{
		RestartCharRotation();
		verticalSpeed = 0f;
		superSneakersJump = null;
		jumpHeight = jumpHeightNormal;
		inAirJump = false;
		isJumping = false;
		isRolling = false;
		IsGrounded.Value = false;
		lastGroundedY = 0f;
		guard.Restart(closeToCharacter: true);
		StopStumble();
		startedJumpFromGround = false;
		sameLaneTimeStamp = Time.time;
		subwayColliders.Clear();
		isInsideSubway = false;
	}

	public void ChangeTrack(int movement, float duration)
	{
		sameLaneTimeStamp = Time.time;
		if (trackMovement != movement)
		{
			ForceChangeTrack(movement, duration);
		}
		else
		{
			trackMovementNext = movement;
		}
	}

	public void ForceChangeTrack(int movement, float duration)
	{
		StopAllCoroutines();
		StartCoroutine(ChangeTrackCoroutine(movement, duration));
	}

	private IEnumerator ChangeTrackCoroutine(int move, float duration)
	{
		if ((Game.Instance.isDead && !Game.Instance.Jetpack.isLastFever) || Game.Instance.IsHitDeadByTrain)
		{
			yield break;
		}
		trackMovement = move;
		trackMovementNext = 0;
		int newTrackIndex = trackIndexTarget + move;
		float trackChangeIndexDistance = Mathf.Abs((float)newTrackIndex - trackIndexPosition);
		float trackIndexPositionBegin = trackIndexPosition;
		float startX = x;
		float endX = Track.Instance.GetTrackX(newTrackIndex);
		float dir = Mathf.Sign(newTrackIndex - trackIndexTarget);
		float startRotation = characterRotation;
		if (newTrackIndex < 0)
		{
			HandleStumble(StumbleType.Side, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, "side");
			yield break;
		}
		if (newTrackIndex >= Track.Instance.numberOfTracks)
		{
			HandleStumble(StumbleType.Side, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, "side");
			yield break;
		}
		if (this.OnChangeTrack != null)
		{
			this.OnChangeTrack((move >= 0) ? OnChangeTrackDirection.Right : OnChangeTrackDirection.Left);
		}
		trackIndexTarget = newTrackIndex;
		yield return StartCoroutine(pTween.To(trackChangeIndexDistance * duration, delegate(float t)
		{
			if ((!Game.Instance.isDead || Game.Instance.Jetpack.isLastFever) && !Game.Instance.IsHitDeadByTrain)
			{
				trackIndexPosition = Mathf.Lerp(trackIndexPositionBegin, newTrackIndex, t);
				x = Mathf.Lerp(startX, endX, t);
				characterRotation = pMath.Bell(t) * dir * characterAngle + Mathf.Lerp(startRotation, 0f, t);
				if (float.IsNaN(characterRotation))
				{
					Character character = this;
					Vector3 eulerAngles = characterRoot.localRotation.eulerAngles;
					character.characterRotation = eulerAngles.y;
				}
				characterRoot.localRotation = Quaternion.Euler(0f, characterRotation, 0f);
			}
		}));
		trackIndex = newTrackIndex;
		trackMovement = 0;
		if (trackMovementNext != 0)
		{
			StartCoroutine(ChangeTrackCoroutine(trackMovementNext, duration));
		}
	}

	public void SetBackToCheckPoint(float zoomTime)
	{
		float lastCheckPoint = Track.Instance.GetLastCheckPoint(z);
		trackIndex = TrackIndex;
		trackIndexTarget = TrackIndex;
		float trackX = Track.Instance.GetTrackX(trackIndex);
		trackIndexPosition = trackIndex;
		trackMovementNext = 0;
		trackMovement = TrackIndex;
		StartCoroutine(MoveCharacterToPosition(trackX, lastCheckPoint, zoomTime));
	}

	private IEnumerator MoveCharacterToPosition(float newX, float newZ, float time)
	{
		float oldX = x;
		float oldZ = z;
		game.ChangeState(null);
		immuneToCriticalHit = true;
		stopColliding = true;
		characterController.enabled = false;
		NotifyOnTutorialMoveBackToCheckPoint(time);
		yield return StartCoroutine(pTween.To(time, delegate(float t)
		{
			x = Mathf.SmoothStep(oldX, newX, t);
			z = Mathf.SmoothStep(oldZ, newZ, t);
		}));
		immuneToCriticalHit = false;
		characterController.enabled = true;
		NotifyOnTutorialStartFromCheckPoint();
		stopColliding = false;
		game.ChangeState(game.Running);
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

	public void ForceLeaveSubway()
	{
		subwayColliders.Clear();
		isInsideSubway = false;
	}

	private void OnCharacterColliderExit(Collider collider)
	{
		if (collider.CompareTag("Subway"))
		{
			if (subwayColliders.Contains(collider))
			{
				subwayColliders.Remove(collider);
				isInsideSubway = (subwayColliders.Count > 0);
			}
		}
		else
		{
			if (collider.isTrigger && collider.name == "Collider Npcs")
			{
				return;
			}
			ObstacleType obstacleType = ObstacleTagToType(collider.tag);
			if (obstacleType == lastObstacleTriggerType && lastObstacleTriggerTrackIndex == trackIndex && this.OnPassedObstacle != null)
			{
				this.OnPassedObstacle(obstacleType);
			}
			if (obstacleType == ObstacleType.RollBarrier)
			{
				if (this.OnRollGuard != null)
				{
					this.OnRollGuard();
				}
				if (this.OnRollEnemyNpc != null)
				{
					this.OnRollEnemyNpc();
				}
			}
		}
	}

	private void OnCharacterColliderEnter(Collider collider)
	{
		Vector3 position = base.transform.position;
		hitPoistionDistance = position.z;
		Pickup componentInChildren = collider.GetComponentInChildren<Pickup>();
		if (!game.IsInGame.Value)
		{
			if (Jetpack.Instance.isLastFever && null != componentInChildren)
			{
				NotifyPickup(componentInChildren);
			}
		}
		else
		{
			if (Game.Instance.isDead && !Game.Instance.Jetpack.isLastFever)
			{
				return;
			}
			if (collider.CompareTag("Subway"))
			{
				subwayColliders.Add(collider);
				isInsideSubway = (subwayColliders.Count > 0);
				Vector3 max = collider.bounds.max;
				subwayMaxY = max.y - 3f;
			}
			else
			{
				if (stopColliding || collider.gameObject.layer == layers.KeepOnHoverboard)
				{
					return;
				}
				if (componentInChildren != null)
				{
					NotifyPickup(componentInChildren);
				}
				else
				{
					if (collider.gameObject.layer == layers.Enemy)
					{
						return;
					}
					if (collider.gameObject.layer == layers.Default)
					{
						if (collider.isTrigger && characterController.isGrounded)
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
						return;
					}
					if (collider.isTrigger)
					{
						if (collider.name == "bush")
						{
							HandleStumble(StumbleType.Bush, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
						}
						else if (!(collider.name == "Collider Npcs"))
						{
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name);
						}
						return;
					}
					lastHitTag = collider.tag;
					ImpactX impactX = GetImpactX(collider);
					ImpactY impactY = GetImpactY(collider);
					ImpactZ impactZ = GetImpactZ(collider);
					Vector3 min = collider.bounds.min;
					float num = min.x;
					Vector3 max2 = collider.bounds.max;
					float num2 = (num + max2.x) / 2f;
					Vector3 position2 = base.transform.position;
					float num3 = position2.x;
					int num4 = (num3 < num2) ? 1 : ((num3 > num2) ? (-1) : 0);
					bool flag = num4 == 0 || trackMovement == num4;
					Vector3 center = characterCollider.bounds.center;
					float num5 = center.z;
					Vector3 min2 = collider.bounds.min;
					bool flag2 = num5 < min2.z;
					bool flag3 = impactZ == ImpactZ.Before && !flag2 && flag;
					if (impactZ == ImpactZ.Middle || flag3)
					{
						if (trackMovement != 0)
						{
							float duration = 0.5f;
							if (Track.Instance.IsRunningOnTutorialTrack)
							{
								duration = 0.2f;
							}
							ChangeTrack(-trackMovement, duration);
						}
						switch (impactX)
						{
						case ImpactX.Left:
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Left, StumbleVerticalHit.Middle, collider.name);
							break;
						case ImpactX.Right:
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Right, StumbleVerticalHit.Middle, collider.name);
							break;
						}
						return;
					}
					if (impactX == ImpactX.Middle || trackMovement == 0)
					{
						if (impactZ == ImpactZ.Before)
						{
							if (impactY == ImpactY.Lower)
							{
								verticalSpeed = CalculateJumpVerticalSpeed(8f);
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
							}
							else if (collider.gameObject.CompareTag("HitMovingTrain"))
							{
								HitByTrainSequence();
								NotifyCriticalHit();
							}
							else if (impactY == ImpactY.Middle)
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Middle, collider.name);
								NotifyCriticalHit();
							}
							else
							{
								HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Upper, collider.name);
								NotifyCriticalHit();
							}
						}
						return;
					}
					if (impactZ == ImpactZ.Before && flag)
					{
						if (collider.gameObject.CompareTag("HitMovingTrain"))
						{
							HitByTrainSequence();
							NotifyCriticalHit();
							return;
						}
						if (collider.gameObject.layer == layers.HitBounceOnly)
						{
							HandleStumble(StumbleType.Normal, StumbleHorizontalHit.Center, StumbleVerticalHit.Lower, collider.name);
						}
						else
						{
							ForceChangeTrack(-trackMovement, 0.5f);
						}
					}
					else if (collider.gameObject.layer == layers.HitBounceOnly)
					{
						ForceChangeTrack(-trackMovement, 0.5f);
					}
					else if (impactZ == ImpactZ.Before && collider.gameObject.CompareTag("HitMovingTrain"))
					{
						HitByTrainSequence();
						NotifyCriticalHit();
						return;
					}
					switch (impactX)
					{
					case ImpactX.Left:
						HandleStumble(StumbleType.Normal, StumbleHorizontalHit.LeftCorner, StumbleVerticalHit.Middle, collider.name);
						break;
					case ImpactX.Right:
						HandleStumble(StumbleType.Normal, StumbleHorizontalHit.RightCorner, StumbleVerticalHit.Middle, collider.name);
						break;
					}
				}
			}
		}
	}

	public void HitByNpcSequence2()
	{
		characterCamera.Shake();
		CharacterRendering.Instance.characterAnimation.Play(PlayerInfo.Instance.SelectedCharAnimPrefix + "knock_back");
		if (!Track.Instance.IsRunningOnTutorialTrack)
		{
			this.OnNametag(OnNametagAction.Success);
		}
		game.Die();
	}

	private IEnumerator HitByNpcSequence()
	{
		characterCamera.Shake();
		CharacterRendering.Instance.characterAnimation.Play(PlayerInfo.Instance.SelectedCharAnimPrefix + "knock_back");
		if (!Track.Instance.IsRunningOnTutorialTrack)
		{
			this.OnNametag(OnNametagAction.Success);
		}
		game.Die();
		yield return new WaitForSeconds(2f);
	}

	private void HitByTrainSequence()
	{
		if (hoverboard.isActive)
		{
			NotifyOnJump();
		}
		else
		{
			NotifyOnHitByTrain();
		}
	}

	private ImpactX GetImpactX(Collider collider)
	{
		Bounds bounds = characterCollider.bounds;
		Bounds bounds2 = collider.bounds;
		Vector3 min = bounds.min;
		float a = min.x;
		Vector3 min2 = bounds2.min;
		float num = Mathf.Max(a, min2.x);
		Vector3 max = bounds.max;
		float a2 = max.x;
		Vector3 max2 = bounds2.max;
		float num2 = Mathf.Min(a2, max2.x);
		float num3 = (num + num2) * 0.5f;
		float num4 = num3;
		Vector3 min3 = bounds2.min;
		float num5 = num4 - min3.x;
		double num6 = num5;
		Vector3 size = bounds2.size;
		if (num6 > (double)size.x - (double)ColliderTrackWidth * 0.33)
		{
			return ImpactX.Right;
		}
		if ((double)num5 < (double)ColliderTrackWidth * 0.33)
		{
			return ImpactX.Left;
		}
		return ImpactX.Middle;
	}

	private ImpactZ GetImpactZ(Collider collider)
	{
		Vector3 position = base.transform.position;
		Bounds bounds = collider.bounds;
		float num = position.z;
		Vector3 max = bounds.max;
		float num2 = max.z;
		Vector3 max2 = bounds.max;
		float num3 = max2.z;
		Vector3 min = bounds.min;
		float num4;
		if (num3 - min.z > 30f)
		{
			num4 = stumbleCornerTolerance;
		}
		else
		{
			Vector3 max3 = bounds.max;
			float num5 = max3.z;
			Vector3 min2 = bounds.min;
			num4 = (num5 - min2.z) * 0.5f;
		}
		if (num > num2 - num4)
		{
			return ImpactZ.After;
		}
		float num6 = position.z;
		Vector3 min3 = bounds.min;
		if (num6 < min3.z + stumbleCornerTolerance)
		{
			return ImpactZ.Before;
		}
		return ImpactZ.Middle;
	}

	private ImpactY GetImpactY(Collider collider)
	{
		Bounds bounds = characterCollider.bounds;
		Bounds bounds2 = collider.bounds;
		Vector3 min = bounds.min;
		float y = min.y;
		Vector3 min2 = bounds2.min;
		float num = Mathf.Max(y, min2.y);
		Vector3 max = bounds.max;
		float y2 = max.y;
		Vector3 max2 = bounds2.max;
		float num2 = Mathf.Min(y2, max2.y);
		float num3 = (num + num2) * 0.5f;
		float num4 = num3;
		Vector3 min3 = bounds.min;
		float num5 = num4 - min3.y;
		Vector3 size = bounds.size;
		float num6 = num5 / size.y;
		if (num6 < 0.33f)
		{
			return ImpactY.Lower;
		}
		if (num6 < 0.66f)
		{
			return ImpactY.Middle;
		}
		return ImpactY.Upper;
	}

	public void Update()
	{
		Vector3 position = base.transform.position;
		if (position.y < 0f)
		{
			position.y = 1f;
			base.transform.position = position;
		}
	}

	public float GetTrackX()
	{
		Vector3 position = Track.Instance.GetPosition(Track.Instance.GetTrackX(trackIndex), 0f);
		return position.x;
	}

	public void dJump()
	{
		if (PlayerInfo.Instance.StartItems[3] && 0 < PlayerInfo.Instance.StartItemCounts[3] && _dJumpable)
		{
			PlayerInfo.Instance.StartItemCounts[3]--;
			if (PlayerInfo.Instance.StartItemCounts[3] == 0)
			{
				MainUIManager.Instance.StopBuffIcon(3);
			}
			_dJumpable = false;
			isJumping = true;
			isFalling = false;
			IsGrounded.Value = false;
			NotifyOnJump();
			if (IsJumpingHigher)
			{
				Vector3 position = base.transform.position;
				SuperSneakersJump value = default(SuperSneakersJump);
				value.z_start = position.z;
				value.z_length = JumpLength(game.currentSpeed, jumpHeightSuperSneakers) * superSneakersJumpApexRatio;
				value.z_end = value.z_start + value.z_length;
				value.y_start = position.y;
				superSneakersJump = value;
				verticalSpeed = 0f;
			}
			else
			{
				verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
			}
			if (IsRunningOnGround())
			{
				startedJumpFromGround = true;
				trainJump = false;
				trainJumpSampleZ = z + trainJumpSampleLength;
			}
		}
	}

	public void Jump()
	{
		bool flag = !isJumping && verticalSpeed <= 0f && verticalSpeed > verticalSpeed_jumpTolerance;
		if (characterController.isGrounded)
		{
			_dJumpable = true;
		}
		if (characterController.isGrounded || flag)
		{
			isJumping = true;
			isFalling = false;
			IsGrounded.Value = false;
			PPActionJump nParent = GameObjectPoolMT<PPActionJump>.Instance.GetNParent(Instance.transform, null);
			nParent.transform.SetParent(null);
			NotifyOnJump();
			if (IsJumpingHigher)
			{
				Vector3 position = base.transform.position;
				SuperSneakersJump value = default(SuperSneakersJump);
				value.z_start = position.z;
				value.z_length = JumpLength(game.currentSpeed, jumpHeightSuperSneakers) * superSneakersJumpApexRatio;
				value.z_end = value.z_start + value.z_length;
				value.y_start = position.y;
				superSneakersJump = value;
				verticalSpeed = 0f;
			}
			else
			{
				verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
			}
			if (IsRunningOnGround())
			{
				startedJumpFromGround = true;
				trainJump = false;
				trainJumpSampleZ = z + trainJumpSampleLength;
			}
		}
		else
		{
			if (_dJumpable)
			{
				dJump();
			}
			if (verticalSpeed < 0f)
			{
				inAirJump = true;
			}
		}
	}

	private bool IsRunningFromTrain()
	{
		return running.currentRunPosition == Running.RunPositions.train || running.currentRunPosition == Running.RunPositions.movingTrain;
	}

	private bool IsRunningOnGround()
	{
		return running.currentRunPosition == Running.RunPositions.ground;
	}

	public void CheckInAirJump()
	{
		if (characterController.isGrounded && inAirJump)
		{
			Jump();
			inAirJump = false;
		}
	}

	public void Roll()
	{
		if (!isRolling)
		{
			SuperSneakersJump? superSneakersJump = this.superSneakersJump;
			if (superSneakersJump.HasValue)
			{
				this.superSneakersJump = null;
			}
			squeezeCollider.Add(this);
			verticalSpeed = 0f - CalculateJumpVerticalSpeed(jumpHeight);
			isRolling = true;
			NotifyOnRoll();
		}
	}

	public void ApplyGravity()
	{
		if (verticalSpeed < 0f && characterController.isGrounded)
		{
			if (startedJumpFromGround && trainJump && IsRunningOnGround())
			{
				NotifyOnJumpOverTrain();
			}
			if (running.currentRunPosition != Running.RunPositions.air)
			{
				startedJumpFromGround = false;
			}
			verticalSpeed = 0f;
			IsGrounded.Value = true;
			if (isJumping || isFalling)
			{
				isJumping = false;
				isFalling = false;
				IsGrounded.Value = true;
				_dJumpable = true;
				NotifyOnLanding();
			}
		}
		else if (startedJumpFromGround && trainJumpSampleZ < z)
		{
			if (Physics.Raycast(new Ray(base.transform.position, -Vector3.up), out RaycastHit hitInfo) && (hitInfo.collider.CompareTag("HitMovingTrain") || hitInfo.collider.CompareTag("HitTrain")))
			{
				trainJump = true;
			}
			trainJumpSampleZ += trainJumpSampleLength;
		}
		verticalSpeed -= gravity * Time.deltaTime;
		if (!characterController.isGrounded && !isFalling && verticalSpeed < verticalFallSpeedLimit && !isRolling)
		{
			isFalling = true;
			NotifyOnHangtime();
			IsGrounded.Value = false;
		}
	}

	public void MoveWithGravity()
	{
		if (characterController.enabled)
		{
			verticalSpeed -= gravity * Time.deltaTime;
			if (verticalSpeed > 0f)
			{
				verticalSpeed = 0f;
			}
			Vector3 motion = verticalSpeed * Time.deltaTime * Vector3.up;
			characterController.Move(motion);
		}
	}

	public void MoveForward()
	{
		Vector3 position = base.transform.position;
		float num = z + game.currentSpeed * Time.deltaTime;
		Vector3 a = verticalSpeed * Time.deltaTime * Vector3.up;
		Vector3 position2 = Track.Instance.GetPosition(x, num);
		Vector3 b = new Vector3(position.x, 0f, position.z);
		if (superSneakersJump.HasValue)
		{
			SuperSneakersJump value = superSneakersJump.Value;
			if (z < value.z_end)
			{
				float num2 = superSneakersJumpCurve.Evaluate((num - value.z_start) / value.z_length) * jumpHeightSuperSneakers + value.y_start;
				float d = num2 - position.y;
				a = Vector3.up * d;
			}
			else
			{
				superSneakersJump = null;
				verticalSpeed = 0f;
				a = Vector3.zero;
			}
		}
		Vector3 b2 = position2 - b;
		if (characterController.enabled)
		{
			characterController.Move(a + b2);
		}
		else
		{
			characterController.transform.position = characterController.transform.position + b2;
		}
		Vector3 position3 = base.transform.position;
		z = position3.z;
		if (characterController.isGrounded)
		{
			lastGroundedY = position.y;
		}
	}

	public void EndRoll()
	{
		if (characterController.enabled)
		{
			characterController.Move(Vector3.up * 2f);
		}
		squeezeCollider.Remove(this);
		if (characterController.enabled)
		{
			characterController.Move(Vector3.down * 2f);
		}
		isRolling = false;
	}

	public float CalculateJumpVerticalSpeed(float jumpHeight)
	{
		return Mathf.Sqrt(2f * jumpHeight * gravity);
	}

	public float CalculateJumpVerticalSpeed()
	{
		return CalculateJumpVerticalSpeed(jumpHeight);
	}

	public float JumpLength(float speed, float jumpHeight)
	{
		return speed * 2f * CalculateJumpVerticalSpeed(jumpHeight) / gravity;
	}

	public void HighJump()
	{
		dJump();
	}

	public void StartRace()
	{
	}

	private IEnumerator FSRace()
	{
		yield return new WaitForSeconds(delayRaceTime);
		EndRace();
	}

	public void EndRace()
	{
	}

	private void StartStumble()
	{
		isStumbling = true;
		if (!Track.Instance.IsRunningOnTutorialTrack)
		{
			guard.CatchUp();
		}
		guard.StartCoroutine(StumbleDecay());
	}

	private IEnumerator StumbleDecay()
	{
		yield return new WaitForSeconds(stumbleDecayTime);
		StopStumble();
	}

	public void StopStumble()
	{
		guard.ResetCatchUp();
		isStumbling = false;
	}

	private void HandleStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
	{
		if (!game.IsInJetpackMode)
		{
			NotifyOnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
			StartStumble();
		}
	}

	private void HandleRevive()
	{
		StopAllCoroutines();
		StopStumble();
	}

	private void NotifyCriticalHit()
	{
		if (this.OnCriticalHit != null)
		{
			CriticalHitType type = (!("HitTrain" == lastHitTag)) ? (("HitBarrier" == lastHitTag) ? CriticalHitType.Barrier : ((!("HitMovingTrain" == lastHitTag)) ? CriticalHitType.None : CriticalHitType.MovingTrain)) : CriticalHitType.Train;
			this.OnCriticalHit(type);
		}
	}

	public void NotifyPickup(Pickup pickup)
	{
		pickup.NotifyPickup(CharacterPickupParticleSystem);
	}

	private void NotifyOnStumble(StumbleType stumbleType, StumbleHorizontalHit horizontalHit, StumbleVerticalHit verticalHit, string colliderName)
	{
		if (this.OnStumble != null)
		{
			this.OnStumble(stumbleType, horizontalHit, verticalHit, colliderName);
		}
	}

	private void NotifyOnCriticalHit(CriticalHitType type)
	{
		if (this.OnCriticalHit != null)
		{
			this.OnCriticalHit(type);
		}
	}

	private void NotifyOnJump()
	{
		if (this.OnJump != null)
		{
			this.OnJump();
		}
	}

	private void NotifyOnLanding()
	{
		if (this.OnLanding != null)
		{
			this.OnLanding(base.transform);
		}
	}

	private void NotifyOnHangtime()
	{
		if (this.OnHangtime != null)
		{
			this.OnHangtime();
		}
	}

	private void NotifyOnRoll()
	{
		if (this.OnRoll != null)
		{
			this.OnRoll();
		}
	}

	private void NotifyOnHitByTrain()
	{
		if (this.OnHitByTrain != null)
		{
			this.OnHitByTrain();
		}
		characterRotation = 0f;
		characterRoot.localRotation = Quaternion.Euler(0f, characterRotation, 0f);
	}

	private void NotifyOnChangeTrack(OnChangeTrackDirection direction)
	{
		if (this.OnChangeTrack != null)
		{
			this.OnChangeTrack(direction);
		}
	}

	private void NotifyOnTutorialMoveBackToCheckPoint(float duration)
	{
		if (this.OnTutorialMoveBackToCheckPoint != null)
		{
			this.OnTutorialMoveBackToCheckPoint(duration);
		}
	}

	private void NotifyOnTutorialStartFromCheckPoint()
	{
		if (this.OnTutorialStartFromCheckPoint != null)
		{
			this.OnTutorialStartFromCheckPoint();
		}
	}

	private void NotifyOnJumpOverTrain()
	{
		if (this.OnJumpOverTrain != null)
		{
			this.OnJumpOverTrain();
		}
	}

	private void Awake()
	{
		instance = this;
	}
}
