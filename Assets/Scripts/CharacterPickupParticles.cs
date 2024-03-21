using System;
using System.Collections;
using UnityEngine;

public class CharacterPickupParticles : MonoBehaviour
{
	[Serializable]
	private class EffectDetails
	{
		public Transform target;

		public float duration = 0.05f;

		public AnimationCurve scaleCurve;
	}

	public static Vector3 coinEfxOffset = 1.2f * Vector3.forward;

	public GameObject CoinEFX;

	public GameObject PowerUpEFX;

	public Transform master;

	public AudioClipInfo CoinPickup;

	public AudioClipInfo PowerUpPickup;

	public float CoinDistanceForStairway;

	private int coinStairway;

	private int flyWay;

	private int[] pentatonicScale = new int[17]
	{
		12,
		13,
		14,
		15,
		16,
		17,
		18,
		19,
		20,
		21,
		22,
		23,
		24,
		25,
		26,
		27,
		28
	};

	public AnimationCurve compressCurve;

	[SerializeField]
	private EffectDetails coinEffect;

	[SerializeField]
	private EffectDetails powerupEffect;

	[SerializeField]
	private ParticleSystem redCoinParticle;

	[SerializeField]
	private ParticleSystem goldCoinParticle;

	[SerializeField]
	private ParticleSystem itemPickupParticle;

	public void PickedUpCoin(Pickup pickup)
	{
		Vector3 position = pickup.transform.position;
		if (80f < position.y)
		{
			coinStairway = 0;
			CoinPickup.maxPitch = Mathf.Pow(2f, compressCurve.Evaluate((float)flyWay / 48f));
			CoinPickup.minPitch = Mathf.Pow(2f, compressCurve.Evaluate((float)flyWay / 48f));
			flyWay++;
		}
		else
		{
			Vector3 position2 = pickup.transform.position;
			if (position2.y < 0.1f)
			{
				goto IL_019c;
			}
			Vector3 position3 = pickup.transform.position;
			if (8.795f < position3.y)
			{
				Vector3 position4 = pickup.transform.position;
				if (position4.y < 8.805f)
				{
					goto IL_019c;
				}
			}
			Vector3 position5 = pickup.transform.position;
			if (9.95f < position5.y)
			{
				Vector3 position6 = pickup.transform.position;
				if (position6.y < 10.05f)
				{
					goto IL_019c;
				}
			}
			Vector3 position7 = pickup.transform.position;
			if (28.95f < position7.y)
			{
				Vector3 position8 = pickup.transform.position;
				if (position8.y < 29.05f)
				{
					goto IL_019c;
				}
			}
			Vector3 position9 = pickup.transform.position;
			if (34.95f < position9.y)
			{
				Vector3 position10 = pickup.transform.position;
				if (position10.y < 35.05f)
				{
					goto IL_019c;
				}
			}
			flyWay = 0;
			if (coinStairway < pentatonicScale.Length - 1)
			{
				coinStairway++;
			}
			CoinPickup.maxPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
			CoinPickup.minPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
		}
		goto IL_02b9;
		IL_019c:
		flyWay = 0;
		coinStairway = 0;
		CoinPickup.maxPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
		CoinPickup.minPitch = Mathf.Pow(2f, (float)pentatonicScale[coinStairway % pentatonicScale.Length] / 12f) * 0.5f;
		goto IL_02b9;
		IL_02b9:
		So.Instance.playSound(CoinPickup);
		if (!GameStats.Instance.IsDoubleCoin)
		{
			goldCoinParticle.gameObject.SetActive(value: true);
			Transform transform = goldCoinParticle.transform;
			Vector3 position11 = Character.Instance.transform.position;
			float x = position11.x;
			Vector3 position12 = Character.Instance.transform.position;
			float y = position12.y + 5f;
			Vector3 position13 = Character.Instance.transform.position;
			transform.position = new Vector3(x, y, position13.z + 5f);
			goldCoinParticle.Play();
		}
		else
		{
			redCoinParticle.gameObject.SetActive(value: true);
			Transform transform2 = redCoinParticle.transform;
			Vector3 position14 = Character.Instance.transform.position;
			float x2 = position14.x;
			Vector3 position15 = Character.Instance.transform.position;
			float y2 = position15.y + 5f;
			Vector3 position16 = Character.Instance.transform.position;
			transform2.position = new Vector3(x2, y2, position16.z + 5f);
			redCoinParticle.Play();
		}
	}

	private IEnumerator EffectCoroutine(EffectDetails details)
	{
		details.target.Rotate(0f, 0f, UnityEngine.Random.Range(0f, 360f));
		Material material = details.target.GetComponent<Renderer>().sharedMaterial;
		yield return StartCoroutine(pTween.To(details.duration, delegate(float t)
		{
			details.target.GetComponent<Renderer>().enabled = true;
			material.SetColor("_MainColor", Color.Lerp(Color.white, Color.black, t));
			details.target.localScale = Vector3.one * details.scaleCurve.Evaluate(t);
		}));
		details.target.GetComponent<Renderer>().enabled = false;
	}

	public void PickedUpPowerUp()
	{
		itemPickupParticle.gameObject.SetActive(value: true);
		itemPickupParticle.transform.position = coinEffect.target.position;
		itemPickupParticle.Play();
		So.Instance.playSound(PowerUpPickup);
	}

	public void PickedUpDefaultPowerUp()
	{
		StartCoroutine(EffectCoroutine(powerupEffect));
	}
}
