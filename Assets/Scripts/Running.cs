using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : CharacterState
{
	public enum RunPositions
	{
		ground,
		station,
		train,
		movingTrain,
		air
	}

	public float slowDownDuration = 0.625f;

	public float slowDownRatio = 1f;

	public float tunnelDelta = -20f;

	public Vector3 cameraOffset = new Vector3(0f, 32f, -34f);

	public float cameraOffsetSmoothDuration = 0.5f;

	public float cameraAimOffset = 18f;

	public float cameraFOV = 64f;

	public float smoothCameraXDuration = 0.05f;

	public float ySmoothDuration = 0.1f;

	public float characterChangeTrackLength = 30f;

	public AnimationCurve transitionFromJetpackCurve;

	private float tunnelStartZ;

	private Curve offsetDeltaCurve;

	private Coroutine crtUpdateNameTagAttack;

	private Game game;

	private Character character;

	private CharacterRendering characterRendering;

	private Transform characterTransform;

	private CharacterController characterController;

	private CharacterCamera characterCamera;

	private Transform characterCameraTransform;

	private Animation characterAnimation;

	public RunPositions currentRunPosition;

	private Queue<Collider> GrindedTrains = new Queue<Collider>();

	private int GrindedTrainsBufferSize = 5;

	public static Running instance;

	private RaycastHit groundTagHitInfo = default(RaycastHit);

	private bool isHandleCriticalHit;

	public static Running Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(Running)) as Running));

	public void Awake()
	{
		instance = this;
		game = Game.Instance;
		character = Character.Instance;
		characterRendering = CharacterRendering.Instance;
		characterTransform = character.transform;
		characterController = character.characterController;
		characterCamera = CharacterCamera.Instance;
		characterCameraTransform = characterCamera.transform;
		character.OnStumble += delegate
		{
			character.characterCamera.Shake();
		};
		character.OnLanding += UpdateGroundTag;
	}

	public override IEnumerator Begin()
	{
		Vector3 position2 = characterTransform.position;
		bool transitionFromJetpack = position2.y > 70f;
		character.characterCollider.enabled = true;
		character.characterCamera.enabled = true;
		SmoothDampVector3 currentCameraOffset = new SmoothDampVector3(cameraOffset, cameraOffsetSmoothDuration)
		{
			Target = cameraOffset
		};
		Vector3 position3 = character.transform.position;
		SmoothDampFloat smoothCameraX = new SmoothDampFloat(position3.x, smoothCameraXDuration);
		SmoothDampFloat currentCameraAimOffset = new SmoothDampFloat(cameraAimOffset, cameraOffsetSmoothDuration);
		offsetDeltaCurve = new Curve();
		Character obj = character;
		Vector3 position4 = character.transform.position;
		obj.lastGroundedY = position4.y;
		float transitionTimeMax = 2f;
		float transitionTime = 0f;
		Vector3 cameraPositionStart = characterCamera.position;
		Vector3 cameraTargetStart = characterCamera.target;
		characterController.Move(Vector3.down * 2f);
		ResetCRTUpdateNameTagAttack();
		SmoothDampFloat smoothDampFloat = smoothCameraX;
		Vector3 position5 = character.transform.position;
		float num3 = smoothDampFloat.Value = (smoothCameraX.Target = position5.x * 0.75f);
		Vector3 position6 = characterTransform.position;
		SmoothDampFloat y = new SmoothDampFloat(position6.y, ySmoothDuration);
		while (true)
		{
			game.LayTrackChunks();
			game.currentSpeed = game.currentLevelSpeed;
			if ((!Game.Instance.isDead || Game.Instance.Jetpack.isLastFever) && !Game.Instance.IsHitDeadByTrain)
			{
				game.HandleControls();
				character.ApplyGravity();
				character.MoveForward();
			}
			Vector3 position = character.transform.position;
			if (character.IsJumpingHigher)
			{
				y.Target = 0.5f * (character.lastGroundedY + position.y);
			}
			else
			{
				y.Target = position.y;
			}
			y.Update();
			currentCameraOffset.Update();
			currentCameraAimOffset.Update();
			smoothCameraX.Update();
			Vector3 offsetDelta = offsetDeltaCurve.Evaluate(character.z - tunnelStartZ);
			smoothCameraX.Target = position.x * 0.75f;
			Vector3 cameraPositionEnd = new Vector3(smoothCameraX.Value, y.Value, position.z) + currentCameraOffset.Value + offsetDelta;
			Vector3 cameraTargetEnd = new Vector3(smoothCameraX.Value, y.Value, position.z) + Vector3.up * currentCameraAimOffset.Value + offsetDelta * 0.5f;
			Vector3 offset = cameraTargetEnd - cameraPositionEnd;
			if (character.IsInsideSubway)
			{
				float subwayMaxY = character.subwayMaxY;
				cameraPositionEnd.y = Mathf.Min(subwayMaxY, cameraPositionEnd.y);
				cameraTargetEnd = cameraPositionEnd + offset;
			}
			if (transitionFromJetpack)
			{
				float num4 = Mathf.Clamp01(transitionTime / transitionTimeMax);
				float t = transitionFromJetpackCurve.Evaluate(num4);
				character.characterCamera.position = Vector3.Lerp(cameraPositionStart, cameraPositionEnd, t);
				character.characterCamera.target = Vector3.Lerp(cameraTargetStart, cameraTargetEnd, t);
				if (num4 == 1f)
				{
					transitionFromJetpack = false;
				}
				transitionTime += Time.deltaTime;
			}
			else
			{
				character.characterCamera.position = cameraPositionEnd;
				character.characterCamera.target = cameraTargetEnd;
			}
			character.CheckInAirJump();
			game.UpdateMeters();
			UpdateInAirRunPosition();
			UpdateRunStateMeters();
			yield return null;
		}
	}

	private IEnumerator startItem(GameStats stats)
	{
		yield return new WaitForEndOfFrame();
		if (game._tryFever)
		{
			game._tryFever = false;
			game.StartHeadStart500();
			stats.pickedUpPowerups++;
			stats.TriggerPowerup(PowerupType.headstart500);
		}
		if (game._tryMagnet)
		{
			game._tryMagnet = false;
			game.StartMagnet();
		}
		if (game._tryRStickerFever && stats.saveMeTokenPickup == 5)
		{
			game._tryRStickerFever = false;
			game.PickupJetpack();
		}
		if (1f <= stats.FeverGauge.Ratio && game.stateRunningFever)
		{
			stats.FeverGauge.RepeatValue();
			game.stateRunningFever = false;
			game.PickupJetpack();
		}
	}

	private void UpdateRunStateMeters()
	{
		float num = game.currentSpeed * Time.deltaTime;
		GameStats gameStats = GameStats.Instance;
		if (!Track.Instance.IsRunningOnTutorialTrack && PlayerInfo.Instance.ThisGameType != GameType.MissionSingle)
		{
			float num2 = (!PlayerInfo.Instance.CharacterSkills[2]) ? 0f : 0.5f;
			gameStats.FeverGauge.Value += num + num * num2;
		}
		StartCoroutine(startItem(gameStats));
		if (currentRunPosition != RunPositions.air)
		{
			if (character.trackIndex == 0)
			{
				gameStats.metersRunLeftTrack += num;
			}
			if (character.trackIndex == 1)
			{
				gameStats.metersRunCenterTrack += num;
			}
			if (character.trackIndex == 2)
			{
				gameStats.metersRunRightTrack += num;
			}
		}
		if (currentRunPosition == RunPositions.ground)
		{
			GameStats.Instance.metersRunGround += num;
		}
		if (currentRunPosition == RunPositions.air)
		{
			GameStats.Instance.metersFly += num;
		}
		if (currentRunPosition == RunPositions.station)
		{
			GameStats.Instance.metersRunStation += num;
		}
		if (currentRunPosition == RunPositions.train)
		{
			GameStats.Instance.metersRunTrain += num;
		}
		if (currentRunPosition == RunPositions.movingTrain)
		{
			GameStats.Instance.metersRunTrain += num;
		}
	}

	private float getAddValue(float num)
	{
		int num2 = (GameStats.Instance.npcShow <= 10) ? GameStats.Instance.npcShow : 10;
		for (int i = 0; i < num2; i++)
		{
			num *= 1.1f;
		}
		return num;
	}

	public void resetNpcStartPosition(bool isStopByFever = false)
	{
	}

	public void resetNpcStartPositionZero()
	{
	}

	public void ResetCRTUpdateNameTagAttack()
	{
	}

	private IEnumerator cetUpdateNameTagAttack()
	{
		float elapsedTime = 0f - Time.deltaTime;
		float waitPeriod = 0f;
		Action randWaitPeriod = delegate
		{
			waitPeriod = 11f + 0.5f * UnityEngine.Random.Range(-4f, 4f);
		};
		Func<bool> randShowOpportunity = () => 0.66f > UnityEngine.Random.value;
		randWaitPeriod();
		while (true)
		{
			elapsedTime += Time.deltaTime;
			if (waitPeriod <= elapsedTime && randShowOpportunity())
			{
				break;
			}
			yield return 0;
		}
		float num = elapsedTime - waitPeriod;
		Npc_Show();
	}

	private void Npc_Show()
	{
	}

	private void UpdateInAirRunPosition()
	{
		if (!characterController.isGrounded)
		{
			currentRunPosition = RunPositions.air;
		}
	}

	private void LandedOnTrain(Collider trainCollider)
	{
		if (character.hoverboard.enabled && !GrindedTrains.Contains(trainCollider))
		{
			if (GrindedTrains.Count > GrindedTrainsBufferSize)
			{
				GrindedTrains.Dequeue();
			}
			GrindedTrains.Enqueue(trainCollider);
			GameStats.Instance.grindedTrains++;
		}
	}

	private void UpdateGroundTag(Transform characterTransform)
	{
		Ray ray = new Ray(character.characterRoot.position, -Vector3.up);
		if (!Physics.Raycast(ray, out groundTagHitInfo))
		{
			return;
		}
		string tag = groundTagHitInfo.collider.tag;
		if (tag == null)
		{
			return;
		}
		if (!(tag == "Ground"))
		{
			if (!(tag == "HitTrain"))
			{
				if (!(tag == "HitMovingTrain"))
				{
					if (tag == "Station")
					{
						currentRunPosition = RunPositions.station;
					}
				}
				else
				{
					currentRunPosition = RunPositions.movingTrain;
					LandedOnTrain(groundTagHitInfo.collider);
				}
			}
			else
			{
				currentRunPosition = RunPositions.train;
				LandedOnTrain(groundTagHitInfo.collider);
			}
		}
		else
		{
			currentRunPosition = RunPositions.ground;
		}
	}

	public override void HandleCriticalHit()
	{
		if (!isHandleCriticalHit)
		{
			isHandleCriticalHit = true;
			LateUpdater.Instance.AddAction(delegate
			{
				isHandleCriticalHit = false;
			});
			character.characterCamera.Shake();
			game.Die();
		}
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		if (!Game.Instance.IsHitDeadByTrain)
		{
			switch (swipeDir)
			{
			case SwipeDir.None:
				break;
			case SwipeDir.Left:
				character.ChangeTrack(-1, Mathf.Min(0.22f, characterChangeTrackLength / game.currentSpeed));
				break;
			case SwipeDir.Right:
				character.ChangeTrack(1, Mathf.Min(0.22f, characterChangeTrackLength / game.currentSpeed));
				break;
			case SwipeDir.Up:
				character.Jump();
				break;
			case SwipeDir.Down:
				character.Roll();
				break;
			}
		}
	}

	public override void HandleDoubleTap()
	{
		if (PlayerInfo.Instance.StartItems[2])
		{
			PlayerInfo.Instance.StartItems[2] = false;
			MainUIManager.Instance.StopBuffIcon(2);
			Game.Instance.Modifiers.Add(Game.Instance.Modifiers.Hoverboard);
		}
	}

	public void StartTunnel(float tunnelLength)
	{
		tunnelStartZ = character.z;
		offsetDeltaCurve = new Curve();
		offsetDeltaCurve.AddKey(0f, Vector3.zero, -Vector3.up, -Vector3.up);
		offsetDeltaCurve.AddKey(tunnelLength / 2f, Vector3.up * tunnelDelta);
		offsetDeltaCurve.AddKey(tunnelLength, Vector3.zero, Vector3.up * 0.001f, Vector3.up * 0.001f);
	}

	public void EndTunnel()
	{
	}
}
