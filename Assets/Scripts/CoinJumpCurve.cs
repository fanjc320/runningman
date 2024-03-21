using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinJumpCurve : MonoBehaviour
{
	public float speed = 100f;

	public float curveOffset;

	public float coinSpacing = 15f;

	public float beginRatio;

	public float endRatio = 1f;

	public bool superSneakers;

	private int previewSteps = 10;

	private List<Transform> coins = new List<Transform>();

	private bool Initialiseret;

	private int activation;

	private float JumpHeight => (!superSneakers) ? Character.Instance.jumpHeightNormal : Character.Instance.jumpHeightSuperSneakers;

	public void Awake()
	{
		TrackObject component = GetComponent<TrackObject>();
		TrackObject trackObject = component;
		trackObject.OnActivate = (TrackObject.OnActivateDelegate)Delegate.Combine(trackObject.OnActivate, new TrackObject.OnActivateDelegate(OnActivate));
		TrackObject trackObject2 = component;
		trackObject2.OnDeactivate = (TrackObject.OnDeactivateDelegate)Delegate.Combine(trackObject2.OnDeactivate, new TrackObject.OnDeactivateDelegate(OnDeactivate));
	}

	public void OnActivate()
	{
		if (!(Game.Instance.CharacterState == Game.Instance.Jetpack))
		{
			if (activation == 1)
			{
			}
			activation++;
			float num = Character.Instance.JumpLength(Game.Instance.currentLevelSpeed, JumpHeight);
			for (float num2 = beginRatio * num; num2 < endRatio * num; num2 += coinSpacing)
			{
				Transform coin = CoinPool.Instance.GetCoin();
				coin.parent = base.transform;
				coin.position = CalcJumpCurve(num2 / num);
				TrackObject component = coin.GetComponent<TrackObject>();
				component.Activate();
				coins.Add(coin);
			}
			Game.Instance.OnSpeedChanged += PositionCoins;
		}
	}

	public void OnDeactivate()
	{
		Game.Instance.OnSpeedChanged -= PositionCoins;
		int count = coins.Count;
		for (int i = 0; count > i; i++)
		{
			coins[i].GetComponent<TrackObject>().OnDeactivate();
		}
		activation--;
		CoinPool.Instance.Put(coins);
		coins.Clear();
	}

	private void PositionCoins(float forSpeed)
	{
		for (int i = 0; i < coins.Count; i++)
		{
			float ratio = (float)i / (float)(coins.Count - 1);
			coins[i].position = CalcJumpCurve(ratio, forSpeed);
		}
	}

	private float NormalizedJumpCurve(float z)
	{
		return 4f * z * (1f - z);
	}

	private float InvertedSpeed(float z)
	{
		return NormalizedJumpCurve(z) / Mathf.Sqrt(1f + Mathf.Pow(-8f * z + 4f, 2f));
	}

	private Vector3 CalcJumpCurve(float ratio)
	{
		return CalcJumpCurve(ratio, Game.Instance.currentLevelSpeed);
	}

	private Vector3 CalcJumpCurve(float ratio, float speed)
	{
		float d = Character.Instance.JumpLength(speed, JumpHeight);
		return base.transform.position + base.transform.forward * d * (ratio - curveOffset) + base.transform.up * NormalizedJumpCurve(ratio) * JumpHeight;
	}

	private void DrawCurve(float speed, Color color)
	{
		Gizmos.color = color;
		Vector3 from = CalcJumpCurve(beginRatio, speed);
		for (int i = 0; i < previewSteps; i++)
		{
			Vector3 vector = CalcJumpCurve((endRatio - beginRatio) * (float)i / (float)(previewSteps - 1) + beginRatio, speed);
			Gizmos.DrawLine(from, vector);
			from = vector;
		}
	}
}
