using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NameTagAttackState : CharacterState
{
	public static NameTagAttackState instance;

	public bool IsAttackState = true;

	public int LastDragDir;

	public AudioClip ClickAud;

	public AudioClip SliderAud;

	public AudioClip SuccessAud;

	public AudioClip CountAud;

	public Renderer FilterRenderer;

	public Sprite[] CountTextSprites;

	public string[] SlideDirTextSpriteNames;

	public string[] ResultTextSpriteNames;

	public Camera BGCamera;

	public Renderer BGRenderer;

	public Camera CHCamera;

	public RectTransform UIFilter;

	public RectTransform Phase1Ready;

	public RectTransform Phase2Touch;

	public RectTransform Phase3Slide;

	public RectTransform Phase4Result;

	public GameObject PrefTouchSeqPoint;

	private Vector3[] rectanglePath;

	private Vector3[] rectanglePathRelative;

	private int tpAnimStateNameHash;

	private SwipeDir curSwipeDir;

	private float[] dragDirEulerZ;

	private SwipeDir[] eulerMappingSwipeDir;

	private bool tpAnyAnimationEnded;

	public static NameTagAttackState Instance => instance ?? (instance = (UnityEngine.Object.FindObjectOfType(typeof(NameTagAttackState)) as NameTagAttackState));

	public override bool PauseActiveModifiers => true;

	private void Awake()
	{
		instance = this;
		InitVars();
	}

	private void InitVars()
	{
		rectanglePath = new Vector3[4]
		{
			128f * (-Vector3.right * 1.3f + -Vector3.up * 0.7f),
			128f * (Vector3.right * 1.3f + -Vector3.up * 0.7f),
			128f * (Vector3.right * 1.3f + Vector3.up * 0.7f),
			128f * (-Vector3.right * 1.3f + Vector3.up * 0.7f)
		};
		rectanglePathRelative = new Vector3[4]
		{
			rectanglePath[1] - rectanglePath[0],
			rectanglePath[2] - rectanglePath[1],
			rectanglePath[3] - rectanglePath[2],
			rectanglePath[0] - rectanglePath[3]
		};
		tpAnimStateNameHash = Animator.StringToHash("NameTagDirectorPoint");
		dragDirEulerZ = new float[4]
		{
			0f,
			270f,
			180f,
			90f
		};
		eulerMappingSwipeDir = new SwipeDir[4]
		{
			SwipeDir.Up,
			SwipeDir.Right,
			SwipeDir.Down,
			SwipeDir.Left
		};
	}

	public override void HandleSwipe(SwipeDir swipeDir)
	{
		curSwipeDir = swipeDir;
	}

	private IEnumerator CheckTPAnimEnd(Animator animator)
	{
		while (true)
		{
			if (null == animator)
			{
				yield break;
			}
			if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != tpAnimStateNameHash)
			{
				break;
			}
			yield return 0;
		}
		tpAnyAnimationEnded = true;
	}

	public override IEnumerator Begin()
	{
		Camera mainCamera = Camera.main;
		FilterRenderer.enabled = true;
		Material filterOrigMat = FilterRenderer.material;
		Color newColor = Color.black;
		newColor.a = 0f;
		FilterRenderer.material.color = newColor;
		LeanTween.value(base.gameObject, delegate(float norm)
		{
			newColor.a = norm;
			FilterRenderer.material.color = newColor;
		}, 0f, 1f, 0.5f).setOnComplete((Action)delegate
		{
			FilterRenderer.material = filterOrigMat;
		});
		yield return pTween.To(0.5f, delegate
		{
		});
		CHCamera.enabled = true;
		CHCamera.transform.parent.position = NpcEnemiesNew.Instance.HoldPosition();
		Animation CHCameraAnim = CHCamera.GetComponent<Animation>();
		CHCameraAnim.enabled = true;
		AnimationState CHCameraAnimState = CHCameraAnim.PlayQueued("camere_anim");
		float camAninLength = CHCameraAnimState.length;
		mainCamera.enabled = false;
		BGCamera.enabled = true;
		BGCamera.clearFlags = CameraClearFlags.Color;
		BGRenderer.enabled = true;
		Material BGOrigMat = BGRenderer.material;
		BGRenderer.material.SetFloat("_Alpha", 0f);
		Phase1Ready.gameObject.SetActive(value: true);
		Image countTextImage = Phase1Ready.transform.Find("CountTextImage").GetComponent<Image>();
		float num6 = 1f / (float)CountTextSprites.Length;
		StartCoroutine(pTween.To(camAninLength, 0f, CountTextSprites.Length, delegate(float val)
		{
			int num5 = Mathf.FloorToInt(Mathf.Clamp(val, 0f, CountTextSprites.Length - 1));
			bool flag = countTextImage.sprite != CountTextSprites[num5];
			countTextImage.sprite = CountTextSprites[num5];
			if (flag)
			{
				mainCamera.GetComponent<AudioSource>().PlayOneShot(CountAud);
				countTextImage.SetNativeSize();
			}
		}));
		LeanTween.value(base.gameObject, delegate(float norm)
		{
			BGRenderer.material.SetFloat("_Alpha", norm);
		}, 0f, 1f, 0.5f).setOnComplete((Action)delegate
		{
			BGRenderer.material = BGOrigMat;
		});
		yield return pTween.To(0.5f, delegate
		{
		});
		while (null != CHCameraAnimState && 1f > CHCameraAnimState.normalizedTime)
		{
			yield return 0;
		}
		Phase1Ready.gameObject.SetActive(value: false);
		Phase2Touch.gameObject.SetActive(value: true);
		RectTransform touchPointerSeqRoot = Phase2Touch.Find("PointRoot").GetComponent<RectTransform>();
		bool isMiniGameSuccess = false;
		int totalNumOfTouchPoint = 4;
		float ptOffsetRange = 1f / (float)totalNumOfTouchPoint - 0.001f;
		List<float> rndPTOffsets = new List<float>();
		for (int i = 0; totalNumOfTouchPoint > i; i++)
		{
			rndPTOffsets.Add((float)i * ptOffsetRange);
		}
		System.Random random = new System.Random();
		int num = rndPTOffsets.Count;
		while (num > 1)
		{
			num--;
			int index = random.Next(num + 1);
			float value = rndPTOffsets[index];
			rndPTOffsets[index] = rndPTOffsets[num];
			rndPTOffsets[num] = value;
		}
		LastDragDir = UnityEngine.Random.Range(0, 3);
		int currentTPSeq = 0;
		int triggerTPCount = 0;
		float lastPtOffset = 0f;
		float touchDuration = 3f;
		float slideDuration = 3f;
		float totalDuration = touchDuration + slideDuration;
		tpAnyAnimationEnded = false;
		float elapsedTouchWait = 0f - Time.deltaTime;
		Image durationTouchImage = Phase2Touch.transform.Find("Progress").GetComponent<Image>();
		Coroutine crtCheckTotalDuration = StartCoroutine(pTween.To(totalDuration, delegate(float norm)
		{
			durationTouchImage.fillAmount = 1f - norm;
		}, delegate
		{
			tpAnyAnimationEnded = true;
		}));
		while (totalNumOfTouchPoint > triggerTPCount && !tpAnyAnimationEnded)
		{
			elapsedTouchWait += Time.deltaTime;
			if (totalNumOfTouchPoint <= currentTPSeq)
			{
				yield return 0;
				continue;
			}
			Action anony = delegate
			{
				_003CBegin_003Ec__Iterator1 _003CBegin_003Ec__Iterator = this;
				GameObject tPtGO = UnityEngine.Object.Instantiate(PrefTouchSeqPoint);
				RectTransform component = tPtGO.GetComponent<RectTransform>();
				component.SetParent(touchPointerSeqRoot);
				component.localScale = Vector3.one;
				lastPtOffset = rndPTOffsets[currentTPSeq];
				float num3 = lastPtOffset * 4f;
				int num4 = Mathf.FloorToInt(num3);
				float d = num3 - (float)num4;
				component.anchoredPosition3D = rectanglePath[num4] + rectanglePathRelative[num4] * d;
				Transform transform3 = component.Find("Image");
				EventTrigger eventTrigger = transform3.GetComponent<EventTrigger>() ?? transform3.gameObject.AddComponent<EventTrigger>();
				EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
				int evtTPSeq = currentTPSeq;
				Action activeNextAnim = delegate
				{
					if (totalNumOfTouchPoint > triggerTPCount)
					{
						Animator component2 = touchPointerSeqRoot.GetChild(triggerTPCount).GetComponent<Animator>();
						component2.Play("TouchToPointSeq");
					}
				};
				if (triggerTPCount == 0)
				{
					activeNextAnim();
				}
				triggerEvent.AddListener(delegate
				{
					mainCamera.GetComponent<AudioSource>().PlayOneShot(_003CBegin_003Ec__Iterator._0024this.ClickAud);
					if (evtTPSeq == triggerTPCount)
					{
						tPtGO.SetActive(value: false);
						triggerTPCount++;
						activeNextAnim();
					}
					else
					{
						_003CBegin_003Ec__Iterator._0024this.tpAnyAnimationEnded = true;
					}
				});
				EventTrigger.Entry item = new EventTrigger.Entry
				{
					eventID = EventTriggerType.PointerDown,
					callback = triggerEvent
				};
				eventTrigger.triggers.Add(item);
				component.Find("SeqText").GetComponent<Text>().text = (currentTPSeq + 1).ToString();
				currentTPSeq++;
			};
			anony();
		}
		int num2 = touchPointerSeqRoot.childCount - 1;
		while (0 <= num2)
		{
			UnityEngine.Object.Destroy(touchPointerSeqRoot.GetChild(num2).gameObject);
			num2--;
		}
		if (crtCheckTotalDuration != null)
		{
			StopCoroutine(crtCheckTotalDuration);
			crtCheckTotalDuration = null;
		}
		Phase2Touch.gameObject.SetActive(value: false);
		if (totalNumOfTouchPoint <= triggerTPCount)
		{
			Phase3Slide.gameObject.SetActive(value: true);
			Image durationSlideImage = Phase3Slide.Find("Progress").GetComponent<Image>();
			RectTransform animRotRoot = Phase3Slide.Find("SlideRoot").GetComponent<RectTransform>();
			int dragDir = LastDragDir;
			animRotRoot.rotation = Quaternion.Euler(0f, 0f, dragDirEulerZ[dragDir]);
			Phase3Slide.Find("SlideRoot/AnimRoot/Point").rotation = Quaternion.identity;
			Image dirTextImage = Phase3Slide.Find("DirTextImage").GetComponent<Image>();
			dirTextImage.GetComponent<LLocImage>().SetPhraseName(SlideDirTextSpriteNames[dragDir]);
			dirTextImage.SetNativeSize();
			SwipeDir targetSwipeDir = eulerMappingSwipeDir[dragDir];
			float currentDur = durationTouchImage.fillAmount;
			crtCheckTotalDuration = StartCoroutine(pTween.To(totalDuration * currentDur, 1f - currentDur, 1f, delegate(float norm)
			{
				durationSlideImage.fillAmount = 1f - norm;
			}));
			curSwipeDir = SwipeDir.None;
			tpAnyAnimationEnded = false;
			while (!tpAnyAnimationEnded)
			{
				Game.Instance.HandleControls();
				if (curSwipeDir == SwipeDir.None)
				{
					yield return 0;
					continue;
				}
				if (targetSwipeDir == curSwipeDir)
				{
					mainCamera.GetComponent<AudioSource>().PlayOneShot(SliderAud);
					isMiniGameSuccess = true;
				}
				else
				{
					mainCamera.GetComponent<AudioSource>().PlayOneShot(SliderAud);
					isMiniGameSuccess = false;
				}
				break;
			}
		}
		else if (tpAnyAnimationEnded)
		{
			isMiniGameSuccess = false;
		}
		if (crtCheckTotalDuration != null)
		{
			StopCoroutine(crtCheckTotalDuration);
		}
		Phase3Slide.gameObject.SetActive(value: false);
		Phase4Result.gameObject.SetActive(value: true);
		CharacterCamera.Instance.enabled = false;
		float fromFOV = CHCamera.fieldOfView;
		Vector3 fromMainCameraPos = CHCamera.transform.position;
		Quaternion fromMainCameraRot = CHCamera.transform.rotation;
		float origFOV = mainCamera.fieldOfView;
		Vector3 origMainCameraPos = mainCamera.transform.position;
		Quaternion origMainCameraRot = mainCamera.transform.rotation;
		mainCamera.fieldOfView = CHCamera.fieldOfView;
		mainCamera.transform.position = CHCamera.transform.position;
		mainCamera.transform.rotation = CHCamera.transform.rotation;
		mainCamera.enabled = true;
		BGCamera.clearFlags = CameraClearFlags.Depth;
		BGRenderer.material.SetFloat("_Alpha", 1f);
		newColor = Color.black;
		newColor.a = 1f;
		FilterRenderer.material.color = newColor;
		Transform fixSuccessBG = Phase4Result.Find("FixSuccessBG");
		Transform fixFailBG = Phase4Result.Find("FixFailBG");
		Transform successBG = Phase4Result.Find("SuccessBG");
		Transform failBG = Phase4Result.Find("FailBG");
		Image topResultTextImage = Phase4Result.Find("TopTextImage").GetComponent<Image>();
		Image bottomResultTextImage = Phase4Result.Find("BottomTextImage").GetComponent<Image>();
		if (isMiniGameSuccess)
		{
			fixSuccessBG.gameObject.SetActive(value: true);
			successBG.gameObject.SetActive(value: true);
			fixFailBG.gameObject.SetActive(value: false);
			failBG.gameObject.SetActive(value: false);
			bottomResultTextImage.GetComponent<LLocImage>().SetPhraseName(ResultTextSpriteNames[0]);
			bottomResultTextImage.SetNativeSize();
			topResultTextImage.GetComponent<LLocImage>().SetPhraseName(ResultTextSpriteNames[0]);
			topResultTextImage.SetNativeSize();
		}
		else
		{
			fixSuccessBG.gameObject.SetActive(value: false);
			successBG.gameObject.SetActive(value: false);
			fixFailBG.gameObject.SetActive(value: true);
			failBG.gameObject.SetActive(value: true);
			bottomResultTextImage.GetComponent<LLocImage>().SetPhraseName(ResultTextSpriteNames[1]);
			bottomResultTextImage.SetNativeSize();
			topResultTextImage.GetComponent<LLocImage>().SetPhraseName(ResultTextSpriteNames[1]);
			topResultTextImage.SetNativeSize();
		}
		NpcEnemiesNew.Instance.Attack(isMiniGameSuccess);
		if (isMiniGameSuccess)
		{
			mainCamera.GetComponent<AudioSource>().PlayOneShot(SuccessAud);
		}
		IEnumerator etorDelay2 = pTween.To(1f, delegate
		{
		});
		while (etorDelay2.MoveNext())
		{
			yield return 0;
		}
		Phase4Result.gameObject.SetActive(value: false);
		LeanTween.value(base.gameObject, delegate(float norm)
		{
			newColor.a = 1f - norm;
			FilterRenderer.material.color = newColor;
			BGRenderer.material.SetFloat("_Alpha", 1f - norm);
			Camera cHCamera = CHCamera;
			float fieldOfView = Mathf.Lerp(fromFOV, origFOV, norm);
			mainCamera.fieldOfView = fieldOfView;
			cHCamera.fieldOfView = fieldOfView;
			Transform transform = CHCamera.transform;
			Vector3 position = Vector3.Lerp(fromMainCameraPos, origMainCameraPos, norm);
			mainCamera.transform.position = position;
			transform.position = position;
			Transform transform2 = CHCamera.transform;
			Quaternion rotation = Quaternion.Lerp(fromMainCameraRot, origMainCameraRot, norm);
			mainCamera.transform.rotation = rotation;
			transform2.rotation = rotation;
		}, 0f, 1f, 1f).setOnComplete((Action)delegate
		{
			FilterRenderer.material = filterOrigMat;
			BGRenderer.material = BGOrigMat;
			CharacterCamera.Instance.enabled = true;
			CHCamera.enabled = false;
			UIFilter.gameObject.SetActive(value: false);
		});
		etorDelay2 = pTween.To(1f, delegate
		{
		});
		while (etorDelay2.MoveNext())
		{
			yield return 0;
		}
		FilterRenderer.enabled = false;
		BGCamera.enabled = false;
		BGRenderer.enabled = false;
	}
}
