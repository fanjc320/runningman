using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GiftBoxDirector : MonoBehaviour
{
	public Image GiftBG;

	public Animation GiftBoxAnim;

	public Animation GiftItemAnim;

	public AudioClip AudGet;

	public ParticleSystem OpenPart;

	public ParticleSystem OpenLoopPart;

	public IEnumerator OnCompleteEtor;

	public string GiftModelName = string.Empty;

	private void OnEnable()
	{
		StartCoroutine(cetUpdateDirector());
	}

	private void OnDisable()
	{
		GiftBG.gameObject.SetActive(value: false);
		GiftBoxAnim.gameObject.SetActive(value: false);
		GiftItemAnim.gameObject.SetActive(value: false);
		StopAllCoroutines();
		OnCompleteEtor = null;
	}

	private IEnumerator cetUpdateDirector()
	{
		yield return 0;
		GiftBG.gameObject.SetActive(value: true);
		Color modifyColor = GiftBG.color;
		modifyColor.r = 0f;
		modifyColor.g = 1f;
		modifyColor.b = 0.5f;
		modifyColor.a = 1f;
		GiftBG.color = modifyColor;
		StartCoroutine(pTween.To(1f, delegate(float norm)
		{
			modifyColor = GiftBG.color;
			modifyColor.a = norm;
			GiftBG.color = modifyColor;
		}));
		LeanTween.delayedCall(1.2f, (Action)delegate
		{
			StartCoroutine(pTween.While(() => true, delegate(float elapsed)
			{
				modifyColor = GiftBG.color;
				modifyColor.r = Mathf.Repeat(elapsed * 0.2f, 1f);
				GiftBG.color = modifyColor;
			}));
		});
		LeanTween.delayedCall(0f, (Action)delegate
		{
			StartCoroutine(pTween.To(3f, delegate(float norm)
			{
				modifyColor = GiftBG.color;
				modifyColor.g = 1f - Math.Max(0f, (norm - 0.25f) / 0.75f);
				modifyColor.b = norm;
				GiftBG.color = modifyColor;
			}));
		});
		Transform giftboxItemTrans = GiftItemAnim.transform.Find("GiftboxItem");
		GameObject modelGO = giftboxItemTrans.transform.Find(GiftModelName).gameObject;
		LeanTween.delayedCall(0.5f, (Action)delegate
		{
			GiftBoxAnim.gameObject.SetActive(value: true);
		});
		LeanTween.delayedCall(1.5f, (Action)delegate
		{
			OpenPart.gameObject.SetActive(value: true);
			OpenLoopPart.gameObject.SetActive(value: true);
			ParticleSystemRenderer component = OpenPart.GetComponent<ParticleSystemRenderer>();
			Mesh mesh = modelGO.GetComponent<MeshFilter>().mesh;
			OpenLoopPart.GetComponent<ParticleSystemRenderer>().mesh = mesh;
			component.mesh = mesh;
		});
		for (int i = 0; i < giftboxItemTrans.childCount; i++)
		{
			giftboxItemTrans.transform.GetChild(i).gameObject.SetActive(value: false);
		}
		modelGO.SetActive(value: true);
		LeanTween.delayedCall(3.5f, (Action)delegate
		{
			GiftItemAnim.gameObject.SetActive(value: true);
			GiftItemAnim.Play("Appearance");
			GiftItemAnim.CrossFadeQueued("Loop");
			LeanTween.delayedCall(0.3f, (Action)delegate
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(AudGet);
			});
		});
		bool isYield = true;
		LeanTween.delayedCall(5f, (Action)delegate
		{
			isYield = false;
		});
		while (isYield)
		{
			yield return 0;
		}
		while (true)
		{
			if (OnCompleteEtor == null)
			{
				yield return 0;
				continue;
			}
			if (!OnCompleteEtor.MoveNext())
			{
				break;
			}
			yield return 0;
		}
		OpenPart.gameObject.SetActive(value: false);
		OpenLoopPart.gameObject.SetActive(value: false);
		GiftBoxAnim.gameObject.SetActive(value: false);
		GiftItemAnim.gameObject.SetActive(value: false);
		StartCoroutine(pTween.To(0.5f, delegate(float norm)
		{
			modifyColor = GiftBG.color;
			modifyColor.g = norm;
			modifyColor.b = 1f - norm;
			modifyColor.a = 1f - norm;
			GiftBG.color = modifyColor;
		}, delegate
		{
			gameObject.SetActive(value: false);
		}));
	}
}
