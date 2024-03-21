using GooglePlayGames;
using Lean;
using SerializableClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
	public class EnumeratorStopper
	{
		public bool Stop
		{
			set
			{
				if (value && this.OnStop != null)
				{
					this.OnStop();
				}
			}
		}

		public event Action OnStop;
	}

	private static MainUIManager instance;

	public Transform Filter;

	public GameObject FilterTop2;

	public Text GoldText;

	public Slider RGaugeSlider;

	public Text ThisMetersText;

	public Sprite[] CountTextSprites;

	public Image NextHighMetersBest;

	public Text NextHighMetersNameText;

	public RawImage NextHighMetersTargetPic;

	public Text NextHighMetersText;

	public Text NextHighMetersStaticText;

	public GameObject MultiPlayerStatusBar;

	public RawImage MultiPlayerMePic;

	public RawImage MultiPlayerOppoPic;

	public RectTransform MultiPlayerOppoTrans;

	public GameObject MultiplayOppo1st;

	public GameObject MultiplayOppo2nd;

	public GameObject MultiplayOppoOut;

	public Animation MultiplayOppoCharacter;

	public Transform MultiplayOppoCharacterTrans;

	public AudioClip ClickAud;

	public AudioClip BuyAud;

	public Texture[] HighMetersTargetTextures;

	private int HighMetersListCount;

	private int myHighMetersListIndex;

	private int currentHighMetersListIndex;

	private int currentReviveCount;

	private int[] reviveReqGemCount = new int[4]
	{
		1,
		3,
		5,
		10
	};

	private StringBuilder tempSB = new StringBuilder();

	private int backBtnStackDepth;

	public GridLayoutGroup BuffIconRoot;

	private IEnumerator[] buffIconEtors = new IEnumerator[Enum.GetValues(typeof(StartItemType)).Length];

	private EnumeratorStopper[] buffIconStoper = new EnumeratorStopper[Enum.GetValues(typeof(StartItemType)).Length];

	private Coroutine crtBuffLoop;

	private Transform popPause;

	public AudioClip AudBonus;

	public AudioClip AudFire;

	public AudioClip AudCount;

	private Transform popCheckRevive;

	private Button popReviveADView;

	private Button popCheckReviveGemReviveBtn;

	private Text reviveRequireGemCountText;

	private Text revivePurchaseGemCountText;

	private bool isPurchasePopup;

	private Coroutine crtCheckReviveCount;

	public AnimationCurve PopupLerpAccCurve;

	private Coroutine crtRevPopupCommon;

	private Dictionary<string, Action<Dictionary<string, object>>> popupDispatcher;

	private RectTransform commonPopupRT;

	private Animator commonPopupAnim;

	private Transform filterCommonPopup;

	private Transform commonPopup;

	private Text commonPopupMsgText;

	private Transform popResult;

	private Transform popResultMulti;

	public static MainUIManager Instance => instance;

	private void senseBackBtn()
	{
		if (PlayerInfo.Instance.IsSenseBackBtn && UnityEngine.Input.GetKeyDown(KeyCode.Escape) && !popResult.gameObject.activeInHierarchy && !popCheckRevive.gameObject.activeInHierarchy)
		{
			switch (backBtnStackDepth)
			{
			case 0:
				OnBtnClick_PausePopup();
				break;
			case 1:
				OnBtnClick_ClosePausePopup();
				break;
			}
		}
	}

	public void RegistEvents()
	{
		GameStats.Instance.OnCoinsChanged = delegate
		{
			GoldText.text = GameStats.Instance.coins.ToString();
		};
		if (PlayerInfo.Instance.ThisGameType == GameType.Multi)
		{
			SetupMultiMeter();
		}
		else
		{
			SetupHighMeter();
		}
		GameStats.Instance.FeverGauge.OnValue += OnValue_FeverGauge;
	}

	private void OnValue_FeverGauge(float ratio)
	{
		RGaugeSlider.normalizedValue = ratio;
	}

	private void SetupMultiMeter()
	{
		MultiplayOppoCharacterTrans.gameObject.SetActive(value: true);
		MultiplayOppoCharacterTrans.position = new Vector3(0f, 0f, -100f);
		SmoothDampVector3 damp = new SmoothDampVector3(MultiplayOppoCharacterTrans.position, 0.1f);
		NextHighMetersText.enabled = false;
		NextHighMetersStaticText.enabled = false;
		NextHighMetersBest.gameObject.SetActive(value: false);
		MultiplayOppo1st.SetActive(value: true);
		MultiplayOppo2nd.SetActive(value: false);
		MultiplayOppoOut.SetActive(value: false);
		NextHighMetersNameText.gameObject.SetActive(value: true);
		NextHighMetersNameText.text = PlayerInfo.Instance.ThisGameOpponent.NickName;
		SkinnedMeshRenderer[] componentsInChildren = MultiplayOppoCharacter.GetComponentsInChildren<SkinnedMeshRenderer>();
		SkinnedMeshRenderer[] array = componentsInChildren;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			if (skinnedMeshRenderer.gameObject.name != PlayerInfo.Instance.ThisGameOpponent.ModelName)
			{
				skinnedMeshRenderer.gameObject.SetActive(value: false);
				continue;
			}
			skinnedMeshRenderer.gameObject.SetActive(value: true);
			skinnedMeshRenderer.material.shader = Shader.Find("Custom/Distorted/Unlit with overlay, RIM, Outline(ALPHA)");
		}
		MultiplayOppoCharacter.Play(PlayerInfo.Instance.ThisGameOpponent.AnimID + "running01");
		int num = int.Parse(DataContainer.Instance.CharacterTableRaw[PlayerInfo.Instance.ThisGameOpponent.CharID.ToString()].CID) - 1;
		NextHighMetersTargetPic.texture = HighMetersTargetTextures[num];
		MultiPlayerStatusBar.SetActive(value: true);
		if (Social.localUser.image == null)
		{
			int num2 = int.Parse(DataContainer.Instance.CharacterTableRaw[PlayerInfo.Instance.SelectedCharID].CID) - 1;
			MultiPlayerMePic.texture = HighMetersTargetTextures[num2];
		}
		else
		{
			MultiPlayerMePic.texture = HighMetersTargetTextures[num];
		}
		MultiPlayerOppoPic.texture = HighMetersTargetTextures[num];
		PlayerInfo.MultiplayOppnent oopoObj = PlayerInfo.Instance.ThisGameOpponent;
		float UpdateTime = 0f;
		GameStats.Instance.OnMetersChanged = delegate
		{
			UpdateTime += Time.deltaTime;
			int num3 = (int)GameStats.Instance.meters;
			if (UpdateTime >= 0.03333333f)
			{
				RaceManager raceManager = RaceManager.Instance;
				Vector3 position = Character.instance.characterRoot.position;
				float z = position.z;
				Vector3 position2 = Character.instance.characterRoot.position;
				int posy = (int)position2.y;
				Vector3 position3 = Character.instance.characterRoot.position;
				raceManager.UpdateSelf(z, posy, (int)position3.x);
				UpdateTime = 0f;
			}
			tempSB.Remove(0, tempSB.Length);
			ThisMetersText.text = tempSB.AppendFormat("{0}", num3).ToString();
			Vector3 position4 = Character.instance.characterRoot.position;
			if (position4.z >= oopoObj.Meter)
			{
				MultiplayOppo1st.SetActive(value: false);
				MultiplayOppo2nd.SetActive(value: true);
			}
			else
			{
				MultiplayOppo1st.SetActive(value: true);
				MultiplayOppo2nd.SetActive(value: false);
			}
			if (PlayerInfo.Instance.ThisGameOpponent.Dead)
			{
				if (!PlayerInfo.Instance.ThisGameOpponent.Out)
				{
					MultiplayOppoCharacter.Play(oopoObj.AnimID + "knock_front");
					PlayerInfo.Instance.ThisGameOpponent.Out = true;
					MultiplayOppoOut.SetActive(value: true);
					MultiplayOppo1st.SetActive(value: false);
					MultiplayOppo2nd.SetActive(value: true);
				}
				RectTransform multiPlayerOppoTrans = MultiPlayerOppoTrans;
				Vector3 value = damp.Value;
				float z2 = value.z;
				Vector3 position5 = Character.instance.characterRoot.position;
				multiPlayerOppoTrans.anchoredPosition = new Vector2(Mathf.Clamp(z2 - position5.z, -310f, 310f), 0f);
			}
			else
			{
				float meter = oopoObj.Meter;
				float num4;
				if (MultiplayOppo1st.activeInHierarchy)
				{
					float value2 = meter;
					Vector3 position6 = Character.instance.characterRoot.position;
					float z3 = position6.z;
					Vector3 position7 = Character.instance.characterRoot.position;
					num4 = Mathf.Clamp(value2, z3, position7.z + 320f);
				}
				else
				{
					num4 = meter;
				}
				meter = num4;
				damp.Target = new Vector3(oopoObj.PosX, oopoObj.PosY, meter);
				damp.Update();
				MultiplayOppoCharacterTrans.position = damp.Value;
				if ((float)num3 < 40f)
				{
					Vector3 position8 = Character.instance.characterRoot.position;
					float z4 = position8.z;
					Vector3 value3 = damp.Value;
					meter = Mathf.Lerp(z4, value3.z, (float)num3 / 40f);
				}
				else
				{
					Vector3 value4 = damp.Value;
					meter = value4.z;
				}
				RectTransform multiPlayerOppoTrans2 = MultiPlayerOppoTrans;
				float num5 = meter;
				Vector3 position9 = Character.instance.characterRoot.position;
				multiPlayerOppoTrans2.anchoredPosition = new Vector2(Mathf.Clamp(num5 - position9.z, -310f, 310f), 0f);
			}
		};
	}

	private void SetupHighMeter()
	{
		MultiplayOppoCharacter.gameObject.SetActive(value: false);
		if (0 >= PlayerInfo.Instance.HighMetersTargetList.Count)
		{
			PlayerInfo.Instance.HighMetersTargetList = new List<PlayerInfo.HighMetersTarget>
			{
				new PlayerInfo.HighMetersTarget
				{
					Name = LeanLocalization.GetTranslationText("17"),
					Meter = 500,
					TargetPic = HighMetersTargetTextures[0]
				},
				new PlayerInfo.HighMetersTarget
				{
					Name = LeanLocalization.GetTranslationText("18"),
					Meter = 1000,
					TargetPic = HighMetersTargetTextures[1]
				},
				new PlayerInfo.HighMetersTarget
				{
					Name = LeanLocalization.GetTranslationText("19"),
					Meter = 3000,
					TargetPic = HighMetersTargetTextures[2]
				},
				new PlayerInfo.HighMetersTarget
				{
					Name = LeanLocalization.GetTranslationText("20"),
					Meter = 5000,
					TargetPic = HighMetersTargetTextures[3]
				},
				new PlayerInfo.HighMetersTarget
				{
					Name = LeanLocalization.GetTranslationText("21"),
					Meter = 10000,
					TargetPic = HighMetersTargetTextures[4]
				},
				new PlayerInfo.HighMetersTarget
				{
					Name = LeanLocalization.GetTranslationText("22"),
					Meter = 20000,
					TargetPic = HighMetersTargetTextures[5]
				},
				new PlayerInfo.HighMetersTarget
				{
					Name = LeanLocalization.GetTranslationText("23"),
					Meter = 99999,
					TargetPic = HighMetersTargetTextures[6]
				}
			};
			int num = int.Parse(DataContainer.Instance.CharacterTableRaw[PlayerInfo.Instance.SelectedCharID].CID) - 1;
			PlayerInfo.Instance.HighMetersTargetList.Add(new PlayerInfo.HighMetersTarget
			{
				Name = string.Empty,
				Meter = PlayerInfo.Instance.HighMeters,
				TargetPic = HighMetersTargetTextures[num]
			});
			PlayerInfo.Instance.HighMetersTargetList.Sort((PlayerInfo.HighMetersTarget lhs, PlayerInfo.HighMetersTarget rhs) => lhs.Meter.CompareTo(rhs.Meter));
		}
		HighMetersListCount = PlayerInfo.Instance.HighMetersTargetList.Count;
		NextHighMetersText.text = PlayerInfo.Instance.HighMetersTargetList[0].Meter.ToString();
		NextHighMetersBest.gameObject.SetActive(value: false);
		NextHighMetersNameText.gameObject.SetActive(value: true);
		NextHighMetersNameText.text = PlayerInfo.Instance.HighMetersTargetList[0].Name;
		NextHighMetersTargetPic.texture = PlayerInfo.Instance.HighMetersTargetList[0].TargetPic;
		myHighMetersListIndex = (from pInfo in PlayerInfo.Instance.HighMetersTargetList
			where string.IsNullOrEmpty(pInfo.Name)
			select pInfo).Select((PlayerInfo.HighMetersTarget s, int i) => i).First();
		GameStats.Instance.OnMetersChanged = delegate
		{
			MainUIManager mainUIManager = this;
			int meters = (int)GameStats.Instance.meters;
			tempSB.Remove(0, tempSB.Length);
			ThisMetersText.text = tempSB.AppendFormat("{0}", meters).ToString();
			if (HighMetersListCount > currentHighMetersListIndex)
			{
				int currentMeter = PlayerInfo.Instance.HighMetersTargetList[currentHighMetersListIndex].Meter;
				if (currentMeter >= meters)
				{
					tempSB.Remove(0, tempSB.Length);
					NextHighMetersText.text = tempSB.AppendFormat("{0}", currentMeter - meters).ToString();
				}
				else
				{
					NextHighMetersBest.transform.parent.GetComponent<Animator>().SetTrigger("DBLSlide");
					currentHighMetersListIndex++;
					if (HighMetersListCount <= currentHighMetersListIndex)
					{
						LeanTween.delayedCall(1f, (Action)delegate
						{
							mainUIManager.NextHighMetersBest.gameObject.SetActive(value: true);
							mainUIManager.NextHighMetersNameText.gameObject.SetActive(value: false);
							mainUIManager.NextHighMetersTargetPic.texture = PlayerInfo.Instance.HighMetersTargetList[mainUIManager.myHighMetersListIndex].TargetPic;
							mainUIManager.tempSB.Remove(0, mainUIManager.tempSB.Length);
							mainUIManager.NextHighMetersText.text = mainUIManager.tempSB.AppendFormat("{0}", meters).ToString();
						});
					}
					else
					{
						LeanTween.delayedCall(1f, (Action)delegate
						{
							mainUIManager.NextHighMetersBest.gameObject.SetActive(value: false);
							mainUIManager.NextHighMetersNameText.gameObject.SetActive(value: true);
							mainUIManager.NextHighMetersNameText.text = PlayerInfo.Instance.HighMetersTargetList[mainUIManager.currentHighMetersListIndex].Name;
							mainUIManager.NextHighMetersTargetPic.texture = PlayerInfo.Instance.HighMetersTargetList[mainUIManager.currentHighMetersListIndex].TargetPic;
							currentMeter = PlayerInfo.Instance.HighMetersTargetList[mainUIManager.currentHighMetersListIndex].Meter;
							mainUIManager.tempSB.Remove(0, mainUIManager.tempSB.Length);
							mainUIManager.NextHighMetersText.text = mainUIManager.tempSB.AppendFormat("{0}", currentMeter - meters).ToString();
						});
					}
				}
			}
			else
			{
				tempSB.Remove(0, tempSB.Length);
				NextHighMetersText.text = tempSB.AppendFormat("{0}", meters).ToString();
			}
		};
	}

	private void initEventVars()
	{
		PlayerInfo.Instance.HighMeters = PlayerInfo.Instance.HighMeters;
	}

	private void OnEnable()
	{
		LateUpdater.Instance.AddAction(delegate
		{
			initBuffIcon();
		});
	}

	private void Awake()
	{
		instance = this;
		LeanLocalization.Instance.SetLanguage(DataContainer.LocaleIdentifier[PlayerInfo.Instance.LocaleIndex]);
		createPause();
		createCheckRevive();
		createResult();
		createCommonPopup();
		initEventVars();
		RegistEvents();
		initUIs();
		Camera.main.GetComponent<AudioSource>().volume = ((!PlayerInfo.Instance.SoundOn) ? 0f : 1f);
	}

	private IEnumerator Start()
	{
		AdBannerManager.HideBannerAD();
		if (PlayerInfo.Instance.ThisGameType == GameType.Multi)
		{
			GoogleAnalyticsV4.getInstance().LogScreen("MultiGameScreen");
			while (true)
			{
				if (Application.isPlaying)
				{
					if (RaceManager.Instance.State == RaceManager.RaceState.Start)
					{
						break;
					}
					yield return 0;
					continue;
				}
				yield break;
			}
			LoadingMergeLoader.Instance.ShowIndicater(isShow: false);
		}
		else if (PlayerInfo.Instance.ThisGameType == GameType.MissionSingle)
		{
			GoogleAnalyticsV4.getInstance().LogScreen("MissionGameScreen");
		}
		else if (PlayerInfo.Instance.ThisGameType == GameType.NormalSingle)
		{
			GoogleAnalyticsV4.getInstance().LogScreen("NormalGameScreen");
			if (!PlayerInfo.Instance.TutorialCompleted)
			{
				GoogleAnalyticsV4.getInstance().LogScreen("Tutorial Start");
			}
		}
	}

	private void OnDestroy()
	{
		LeanTween.cancelAll(callComplete: false);
		if (!PlayerInfo.Instance.TutorialCompleted)
		{
			PlayerInfo.Instance.TutorialCompleted = true;
		}
		GameStats.Instance.FeverGauge.OnValue -= OnValue_FeverGauge;
		OnDestroyInstances();
		instance = null;
	}

	private void Update()
	{
		senseBackBtn();
	}

	public void Reset()
	{
		currentHighMetersListIndex = 0;
		PlayerInfo.Instance.HighMeters = PlayerInfo.Instance.HighMeters;
	}

	private void initUIs()
	{
		base.transform.Find("CenterLeft/RGauge").gameObject.SetActive(PlayerInfo.Instance.ThisGameType != GameType.MissionSingle);
		if (!PlayerInfo.Instance.TutorialCompleted)
		{
			base.transform.Find("TopLeft/Gold").gameObject.SetActive(value: false);
			base.transform.Find("TopCenter/ThisMeters").gameObject.SetActive(value: false);
			base.transform.Find("CenterLeft/RGauge").gameObject.SetActive(value: false);
			base.transform.Find("CenterRight/SocialScoreRoot").gameObject.SetActive(value: false);
			base.transform.Find("CenterRight/Buffs").gameObject.SetActive(value: false);
			base.transform.Find("Popup/Pause/Buttons/RetryBtn").gameObject.SetActive(value: false);
		}
	}

	private void setActivateFilter(bool activate)
	{
		Filter.gameObject.SetActive(activate);
	}

	public void StartStartItemIconDirector(StartItemType type)
	{
		switch (type)
		{
		case StartItemType.StartFever:
		{
			Animator itemAnim = base.transform.Find("TopCenter/StartItemDirector/StartFever").GetComponent<Animator>();
			itemAnim.gameObject.SetActive(value: true);
			StartCoroutine(pTween.While(() => itemAnim.GetCurrentAnimatorStateInfo(0).IsName("StartItemIcon_StartFever"), delegate
			{
			}, delegate
			{
				itemAnim.gameObject.SetActive(value: false);
			}));
			break;
		}
		case StartItemType.LastFever:
		{
			Animator itemAnim2 = base.transform.Find("TopCenter/StartItemDirector/LastFever").GetComponent<Animator>();
			itemAnim2.gameObject.SetActive(value: true);
			StartCoroutine(pTween.While(() => itemAnim2.GetCurrentAnimatorStateInfo(0).IsName("StartItemIcon_LastFever"), delegate
			{
			}, delegate
			{
				itemAnim2.gameObject.SetActive(value: false);
			}));
			break;
		}
		case StartItemType.IgnoreConfuse:
		{
			Animator itemAnim3 = base.transform.Find("TopCenter/StartItemDirector/IgnoreConfusion").GetComponent<Animator>();
			itemAnim3.gameObject.SetActive(value: true);
			StartCoroutine(pTween.While(() => itemAnim3.GetCurrentAnimatorStateInfo(0).IsName("StartItemIcon_IgnoreConfusion"), delegate
			{
			}, delegate
			{
				itemAnim3.gameObject.SetActive(value: false);
			}));
			break;
		}
		}
	}

	public void StartBuffIcon(int type, float duration)
	{
		StopBuffIcon(type);
		buffIconStoper[type] = new EnumeratorStopper();
		buffIconEtors[type] = cetBuffProgress(type, duration, buffIconStoper[type]);
	}

	public void StopAllBuffIcon()
	{
		int length = Enum.GetValues(typeof(StartItemType)).Length;
		for (int i = 0; length > i; i++)
		{
			StopBuffIcon(i);
		}
	}

	public void StopBuffIcon(int type)
	{
		if (buffIconStoper[type] != null)
		{
			buffIconStoper[type].Stop = true;
			buffIconStoper[type] = null;
		}
		if (buffIconEtors[type] != null)
		{
			buffIconEtors[type] = null;
		}
	}

	public IEnumerator cetBuffProgress(int type, float duration, EnumeratorStopper stopper)
	{
		Color color = Color.white;
		PGOBuffIcon pGO = GameObjectPoolMT<PGOBuffIcon>.Instance.GetNNoParent(null, null);
		GameObject go = pGO.gameObject;
		go.transform.SetParent(BuffIconRoot.transform);
		go.transform.localScale = Vector3.one;
		Image iconImage = go.transform.Find("Icon").GetComponent<Image>();
		iconImage.color = color;
		if (Enum.GetValues(typeof(StartItemType)).Length <= type)
		{
			iconImage.sprite = DataContainer.Instance.GetAssetResources<Sprite>("BuffIcon/ingame_icon_helmatStartItem");
		}
		else
		{
			iconImage.sprite = DataContainer.Instance.GetAssetResources<Sprite>(DataContainer.Instance.StartItemTableRaw[type].Iconpath);
		}
		if (type == 3)
		{
			iconImage.transform.GetChild(0).gameObject.SetActive(value: true);
			iconImage.transform.GetChild(0).GetComponent<Text>().text = PlayerInfo.Instance.StartItemCounts[3].ToString();
		}
		else
		{
			iconImage.transform.GetChild(0).gameObject.SetActive(value: false);
		}
		bool isInfinity = 0f >= duration;
		float blankDuration = duration * 0.2f;
		float noBlankDuration = duration - blankDuration;
		float elapsed2 = 0f - Time.deltaTime;
		bool isYield = true;
		stopper.OnStop += delegate
		{
			pGO.Dispose();
			buffIconStoper[type] = null;
			isYield = false;
		};
		while (isYield && (isInfinity || elapsed2 < noBlankDuration))
		{
			elapsed2 += Time.deltaTime;
			if (type == 3)
			{
				iconImage.transform.GetChild(0).GetComponent<Text>().text = PlayerInfo.Instance.StartItemCounts[3].ToString();
			}
			yield return 0;
		}
		elapsed2 -= noBlankDuration;
		while (isYield && elapsed2 < blankDuration)
		{
			elapsed2 += Time.deltaTime;
			color.a = Mathf.Pow(Mathf.PingPong(elapsed2, 0.25f), 0.5f);
			iconImage.color = color;
			yield return 0;
		}
		if (isYield)
		{
			pGO.Dispose();
			buffIconStoper[type] = null;
		}
	}

	public IEnumerator cetBuffLoop()
	{
		while (true)
		{
			if ((!(Game.Instance.CharacterState == Game.Instance.Running) && !(Game.Instance.CharacterState == Game.Instance.Jetpack)) || Game.Instance.isDead || 0f >= Time.timeScale)
			{
				yield return 0;
				continue;
			}
			for (int i = 0; i < buffIconEtors.Length; i++)
			{
				if (buffIconEtors[i] != null)
				{
					buffIconEtors[i].MoveNext();
				}
			}
			yield return 0;
		}
	}

	private void initBuffIcon()
	{
		if (crtBuffLoop == null)
		{
			crtBuffLoop = StartCoroutine(cetBuffLoop());
		}
	}

	public void DoTutorialEvent(string type)
	{
		Transform transform = base.transform.Find("TutorialRoot/SlideRoot");
		Text component = base.transform.Find("TutorialRoot/SlideRoot/MsgText/Text").GetComponent<Text>();
		Transform transform2 = base.transform.Find("TutorialRoot/DBLSlideRoot");
		Text component2 = base.transform.Find("TutorialRoot/DBLSlideRoot/MsgText/Text").GetComponent<Text>();
		Transform transform3 = base.transform.Find("TutorialRoot/DBLTapRoot");
		Text component3 = base.transform.Find("TutorialRoot/DBLTapRoot/MsgText/Text").GetComponent<Text>();
		if (type == null)
		{
			return;
		}
		if (_003C_003Ef__switch_0024map0 == null)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
			dictionary.Add("Jump", 0);
			dictionary.Add("Roll", 1);
			dictionary.Add("Left", 2);
			dictionary.Add("Right", 3);
			dictionary.Add("DBLJump", 4);
			dictionary.Add("Shield", 5);
			dictionary.Add("End", 6);
			_003C_003Ef__switch_0024map0 = dictionary;
		}
		if (_003C_003Ef__switch_0024map0.TryGetValue(type, out int value))
		{
			switch (value)
			{
			case 0:
				transform.gameObject.SetActive(value: true);
				transform2.gameObject.SetActive(value: false);
				transform3.gameObject.SetActive(value: false);
				transform.Find("AnimRoot").eulerAngles = Vector3.forward * 0f;
				transform.Find("AnimRoot").localScale = new Vector3(1f, 1f, 1f);
				component.text = LeanLocalization.GetTranslationText("171");
				break;
			case 1:
				transform.gameObject.SetActive(value: true);
				transform2.gameObject.SetActive(value: false);
				transform3.gameObject.SetActive(value: false);
				transform.Find("AnimRoot").eulerAngles = Vector3.forward * 0f;
				transform.Find("AnimRoot").localScale = new Vector3(1f, -1f, 1f);
				component.text = LeanLocalization.GetTranslationText("172");
				break;
			case 2:
				transform.gameObject.SetActive(value: true);
				transform2.gameObject.SetActive(value: false);
				transform3.gameObject.SetActive(value: false);
				transform.Find("AnimRoot").eulerAngles = Vector3.forward * 90f;
				transform.Find("AnimRoot").localScale = new Vector3(1f, 1f, 1f);
				component.text = LeanLocalization.GetTranslationText("173");
				break;
			case 3:
				transform.gameObject.SetActive(value: true);
				transform2.gameObject.SetActive(value: false);
				transform3.gameObject.SetActive(value: false);
				transform.Find("AnimRoot").eulerAngles = Vector3.forward * 90f;
				transform.Find("AnimRoot").localScale = new Vector3(1f, -1f, 1f);
				component.text = LeanLocalization.GetTranslationText("174");
				break;
			case 4:
				transform.gameObject.SetActive(value: false);
				transform2.gameObject.SetActive(value: true);
				transform3.gameObject.SetActive(value: false);
				component2.text = LeanLocalization.GetTranslationText("175");
				break;
			case 5:
				transform.gameObject.SetActive(value: false);
				transform2.gameObject.SetActive(value: false);
				transform3.gameObject.SetActive(value: true);
				component3.text = LeanLocalization.GetTranslationText("176");
				break;
			case 6:
				transform.gameObject.SetActive(value: false);
				transform2.gameObject.SetActive(value: false);
				transform3.gameObject.SetActive(value: false);
				break;
			}
		}
	}

	public void OnBtnClick_PausePopup()
	{
		setActivateFilter(activate: true);
		popPause.gameObject.SetActive(value: true);
		Game.instance.TriggerPause(pauseGame: true);
		backBtnStackDepth = 1;
		Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
	}

	public void OnBtnClick_Retry()
	{
		PlayerInfo.Instance.IsRetryGame = true;
		OnBtnClick_GotoMenu();
	}

	public void OnBtnClick_ClosePausePopup()
	{
		setActivateFilter(activate: false);
		popPause.gameObject.SetActive(value: false);
		Game.instance.TriggerPause(pauseGame: false);
		backBtnStackDepth = 0;
		Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
	}

	private void createPause()
	{
		popPause = base.transform.Find("Popup/Pause").transform;
		base.transform.Find("TopRight/PauseBtn").gameObject.SetActive(PlayerInfo.Instance.ThisGameType != GameType.Multi);
	}

	public void OnBtnClick_Result()
	{
		Action action = delegate
		{
			popCheckRevive.gameObject.SetActive(value: false);
			popResult.gameObject.SetActive(value: true);
			Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
			StartCoroutine(cetResultDirector());
		};
		if (PlayerInfo.Instance.StartItems[1])
		{
			StartCoroutine(cetLastFever(action));
		}
		else
		{
			action();
		}
	}

	public void OnBtnClick_ResultMulti()
	{
		Action action = delegate
		{
			popResultMulti.gameObject.SetActive(value: true);
			Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
			StartCoroutine(cetResultMultiDirector());
		};
		action();
		LeanTween.delayedCall(1f, (Action)delegate
		{
			PlayGamesPlatform.Instance.RealTime.LeaveRoom();
		});
	}

	private IEnumerator cetResultMultiDirector()
	{
		GameObject gameObject = base.transform.Find("Popup/Result-Multi/ScoreRoot/Win").gameObject;
		GameObject gameObject2 = base.transform.Find("Popup/Result-Multi/ScoreRoot/Lose").gameObject;
		Text component = base.transform.Find("Popup/Result-Multi/BonusRoot/WinCount").GetComponent<Text>();
		Text component2 = base.transform.Find("Popup/Result-Multi/BonusRoot/LoseCount").GetComponent<Text>();
		Text component3 = base.transform.Find("Popup/Result-Multi/BonusRoot/Ratio").GetComponent<Text>();
		if (GameStats.instance.isWin)
		{
			component.text = (++PlayerInfo.Instance.MultiraceWinCount).ToString();
			component2.text = PlayerInfo.Instance.MultiraceLoseCount.ToString();
			gameObject.SetActive(value: true);
			gameObject2.SetActive(value: false);
		}
		else
		{
			component.text = PlayerInfo.Instance.MultiraceWinCount.ToString();
			component2.text = (++PlayerInfo.Instance.MultiraceLoseCount).ToString();
			gameObject.SetActive(value: false);
			gameObject2.SetActive(value: true);
		}
		int num = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][5];
		float pvalue = DataContainer.Instance.PlayerParamLevelTableRawByLevel[5].PPLevelRaws[num].Pvalue;
		int num2 = Mathf.CeilToInt((float)GameStats.Instance.coins * pvalue);
		num2 += (PlayerInfo.Instance.CharacterSkills[4] ? Mathf.CeilToInt((float)GameStats.Instance.coins * UnityEngine.Random.Range(0.05f, 0.1f)) : 0);
		num2 += (PlayerInfo.Instance.StartItems[7] ? Mathf.CeilToInt((float)GameStats.Instance.coins * 0.5f) : 0);
		num2 += (PlayerInfo.Instance.CharacterSkills[0] ? 100 : 0);
		int num3 = GameStats.Instance.coins + num2;
		CurrencyTypeMapInt currency;
		(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] + num3;
		RaceManager.Instance.CleanUp();
		component3.text = ((float)PlayerInfo.Instance.MultiraceWinCount / (float)(PlayerInfo.Instance.MultiraceWinCount + PlayerInfo.Instance.MultiraceLoseCount) * 100f).ToString("F2") + "%";
		if (0 < PlayerInfo.Instance.MultiraceWinCount)
		{
			GPGSManager.Instance.PostToLeaderboard("CgkI9qTupKYJEAIQAw", PlayerInfo.Instance.MultiraceWinCount);
		}
		Animator wingAnim = base.transform.Find("Popup/Result-Multi/ScoreRoot").GetComponent<Animator>();
		float wingAnimWaitTime = 1.5f;
		StartCoroutine(pTween.While(() => true, delegate(float elapsed)
		{
			if (!wingAnim.GetCurrentAnimatorStateInfo(0).IsName("Result_Wing") && 3.5f < elapsed - wingAnimWaitTime)
			{
				wingAnimWaitTime = elapsed;
				wingAnim.gameObject.SetActive(value: false);
				wingAnim.gameObject.SetActive(value: true);
			}
		}));
		yield break;
	}

	private IEnumerator cetResultDirector()
	{
		Text gradeText = base.transform.Find("Popup/Result/ScoreRoot/ScoreText").GetComponent<Text>();
		Text distanceText = base.transform.Find("Popup/Result/StatRoot/DistText").GetComponent<Text>();
		Text obtainGoldText = base.transform.Find("Popup/Result/StatRoot/GoldText").GetComponent<Text>();
		Text bonusGoldText = base.transform.Find("Popup/Result/BonusRoot/GoldText").GetComponent<Text>();
		Text bonusScoreText = base.transform.Find("Popup/Result/BonusRoot/ScoreText").GetComponent<Text>();
		int score = GameStats.Instance.score + Mathf.CeilToInt(GameStats.Instance.meters);
		int scoreBonusLevel = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][4];
		float scoreBonusValue = DataContainer.Instance.PlayerParamLevelTableRawByLevel[4].PPLevelRaws[scoreBonusLevel].Pvalue;
		int scoreBonus = Mathf.CeilToInt((float)score * scoreBonusValue);
		int goldBonusLevel = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][5];
		float goldBonusValue = DataContainer.Instance.PlayerParamLevelTableRawByLevel[5].PPLevelRaws[goldBonusLevel].Pvalue;
		int goldBonus4 = Mathf.CeilToInt((float)GameStats.Instance.coins * goldBonusValue);
		goldBonus4 += (PlayerInfo.Instance.CharacterSkills[4] ? Mathf.CeilToInt((float)GameStats.Instance.coins * UnityEngine.Random.Range(0.05f, 0.1f)) : 0);
		goldBonus4 += (PlayerInfo.Instance.StartItems[7] ? Mathf.CeilToInt((float)GameStats.Instance.coins * 0.5f) : 0);
		goldBonus4 += (PlayerInfo.Instance.CharacterSkills[0] ? 100 : 0);
		int resultScore = score + scoreBonus;
		if (resultScore > PlayerInfo.Instance.HighScore)
		{
			PlayerInfo.Instance.HighScore = resultScore;
			if (PlayerInfo.Instance.ThisGameType == GameType.MissionSingle && 0 < resultScore)
			{
				GPGSManager.Instance.PostToLeaderboard("CgkI9qTupKYJEAIQAg", resultScore);
			}
			else if (PlayerInfo.Instance.ThisGameType == GameType.NormalSingle && 0 < resultScore)
			{
				GPGSManager.Instance.PostToLeaderboard("CgkI9qTupKYJEAIQAQ", resultScore);
			}
		}
		PlayerInfo.Instance.TempTotalPlayTimes++;
		PlayerInfo.Instance.AccMissionByCondTypeID("runamount", "-1", GameStats.Instance.meters.ToString());
		PlayerInfo.Instance.AccMissionByCondTypeID("dojumpobstacle", "-1", (GameStats.Instance.jumpBarrier + GameStats.Instance.jumpHighBarrier).ToString());
		PlayerInfo.Instance.AccMissionByCondTypeID("dorollobstacle", "-1", GameStats.Instance.dodgeBarrier.ToString());
		if (PlayerInfo.Instance.TempTotalPlayTimes >= 3)
		{
			GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQBA");
		}
		if (PlayerInfo.Instance.TempTotalPlayTimes >= 10)
		{
			GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQBQ");
		}
		if (PlayerInfo.Instance.TempTotalPlayTimes >= 50)
		{
			GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQBg");
		}
		if (PlayerInfo.Instance.TempTotalPlayTimes >= 100)
		{
			GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQBw");
		}
		if (PlayerInfo.Instance.TempTotalPlayTimes >= 300)
		{
			GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQCA");
		}
		if (GameStats.Instance.meters >= 500f)
		{
			GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQCQ");
		}
		if (GameStats.Instance.meters >= 1000f)
		{
			GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQCg");
		}
		if (GameStats.Instance.meters >= 2000f)
		{
			GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQCw");
		}
		int resultGold = GameStats.Instance.coins + goldBonus4;
		CurrencyTypeMapInt currency;
		(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] + resultGold;
		tempSB.Remove(0, tempSB.Length);
		tempSB.AppendFormat("0");
		gradeText.text = tempSB.ToString();
		bonusScoreText.text = tempSB.ToString();
		distanceText.text = tempSB.ToString();
		obtainGoldText.text = tempSB.ToString();
		bonusGoldText.text = tempSB.ToString();
		Animator wingAnim = base.transform.Find("Popup/Result/ScoreRoot").GetComponent<Animator>();
		float wingAnimWaitTime = 1.5f;
		StartCoroutine(pTween.DelayTo(0.8f, 0f, 0f, 0f, delegate
		{
		}, delegate
		{
			if (null != Camera.main)
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(AudBonus);
			}
		}));
		StartCoroutine(pTween.While(() => true, delegate(float elapsed)
		{
			if (!wingAnim.GetCurrentAnimatorStateInfo(0).IsName("Result_Wing") && 3.5f < elapsed - wingAnimWaitTime)
			{
				wingAnimWaitTime = elapsed;
				wingAnim.gameObject.SetActive(value: false);
				wingAnim.gameObject.SetActive(value: true);
			}
		}));
		float fromScore = 0f;
		float fromDistance = 0f;
		float fromGold = 0f;
		float toScore = 0f;
		float toDistance = 0f;
		float toGold = 0f;
		toDistance = GameStats.Instance.meters;
		toGold = resultGold;
		IEnumerator etorBase = pTween.To(1f, delegate(float norm)
		{
			tempSB.Remove(0, tempSB.Length);
			distanceText.text = tempSB.AppendFormat("{0}", Mathf.CeilToInt(Mathf.Lerp(fromDistance, toDistance, norm))).ToString();
			tempSB.Remove(0, tempSB.Length);
			obtainGoldText.text = tempSB.AppendFormat("{0}", Mathf.CeilToInt(Mathf.Lerp(fromGold, toGold, norm))).ToString();
		});
		while (etorBase.MoveNext())
		{
			yield return 0;
		}
		toScore = score;
		IEnumerator etorScorePhase3 = pTween.To(1f, delegate(float norm)
		{
			tempSB.Remove(0, tempSB.Length);
			gradeText.text = tempSB.AppendFormat("{0}", Mathf.CeilToInt(Mathf.Lerp(fromScore, toScore, norm))).ToString();
		});
		while (etorScorePhase3.MoveNext())
		{
			yield return 0;
		}
		StartCoroutine(cetResultEffect());
		toScore = scoreBonus;
		toGold = goldBonus4;
		IEnumerator etorBonus = pTween.To(0.5f, delegate(float norm)
		{
			tempSB.Remove(0, tempSB.Length);
			bonusScoreText.text = tempSB.AppendFormat("{0}", Mathf.CeilToInt(Mathf.Lerp(fromScore, toScore, norm))).ToString();
			tempSB.Remove(0, tempSB.Length);
			bonusGoldText.text = tempSB.AppendFormat("{0}", Mathf.CeilToInt(Mathf.Lerp(fromGold, toGold, norm))).ToString();
		});
		while (etorBonus.MoveNext())
		{
			yield return 0;
		}
		fromScore = score;
		toScore = score + scoreBonus;
		IEnumerator etorScorePhase2 = pTween.To(1f, delegate(float norm)
		{
			tempSB.Remove(0, tempSB.Length);
			gradeText.text = tempSB.AppendFormat("{0}", Mathf.CeilToInt(Mathf.Lerp(fromScore, toScore, norm))).ToString();
		});
		while (etorScorePhase2.MoveNext())
		{
			yield return 0;
		}
	}

	private IEnumerator cetResultEffect()
	{
		float period = 1.5f;
		float maxValue = 5f;
		float elapsed = 0f - Time.deltaTime;
		int floorIdx = -1;
		while (elapsed < period)
		{
			elapsed += Time.deltaTime;
			float val = elapsed / period * maxValue;
			int newFloor = Mathf.FloorToInt(val);
			if (newFloor != floorIdx && 0.9f < UnityEngine.Random.value)
			{
				floorIdx = newFloor;
				PPEffResult nParent = GameObjectPoolMT<PPEffResult>.Instance.GetNParent(null, null);
				Vector3 randomVec = UnityEngine.Random.onUnitSphere;
				randomVec.x *= 0.48f;
				randomVec.y *= 2.69999981f;
				randomVec.z = 0f;
				nParent.transform.localPosition = randomVec;
				if (null != Camera.main)
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(AudFire);
				}
			}
			yield return 0;
		}
	}

	private IEnumerator cetLastFever(Action funcShowResultPopup)
	{
		setActivateFilter(activate: false);
		popCheckRevive.gameObject.SetActive(value: false);
		Character.Instance.RestartCharRotation();
		Jetpack.Instance.isLastFever = true;
		Game.Instance.ChangeState(Jetpack.Instance);
		float baseDur = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
			where s.ID == "1"
			select s).First().Pvalue;
		int paramLevel = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][1];
		Instance.StartBuffIcon(1, baseDur + DataContainer.Instance.PlayerParamLevelTableRawByLevel[1].PPLevelRaws[paramLevel].Pvalue);
		Instance.StartStartItemIconDirector(StartItemType.LastFever);
		while (PlayerInfo.Instance.StartItems[1])
		{
			yield return 0;
		}
		setActivateFilter(activate: true);
		funcShowResultPopup();
	}

	private void OnPurchaseRevive()
	{
		isPurchasePopup = false;
		setActivateFilter(activate: false);
		popCheckRevive.gameObject.SetActive(value: false);
		Game.instance.TriggerPause(pauseGame: false);
		Revive.Instance.SendRevive();
		Game.Instance.IsInGame.Value = true;
		CurrencyTypeMapInt currency;
		(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] - reviveReqGemCount[currentReviveCount];
		currentReviveCount = Mathf.Min(currentReviveCount + 1, reviveReqGemCount.Length - 1);
		PlayerInfo.Instance.DirtyAll();
		if (crtCheckReviveCount != null)
		{
			StopCoroutine(crtCheckReviveCount);
		}
	}

	public void OnBtnClick_PurchaseRevive()
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
		int num = reviveReqGemCount[currentReviveCount];
		if (num <= PlayerInfo.Instance.Currency[CurrencyType.Gem])
		{
			OnPurchaseRevive();
			return;
		}
		Action value = delegate
		{
			LeanTween.delayedCall(0f, (Action)delegate
			{
				MainUIManager mainUIManager = this;
				isPurchasePopup = true;
				ShopInfoData shopItem = DataContainer.Instance.ShopTableRaw["0"];
				string popupMsg = string.Empty;
				filterCommonPopup.gameObject.SetActive(value: true);
				MarketManager.BuyProduct(shopItem.ID, delegate(bool success)
				{
					if (success)
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(mainUIManager.BuyAud);
						CurrencyTypeMapInt currency;
						(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] + shopItem.Reward;
						popupMsg = string.Format(LeanLocalization.GetTranslationText("71"), LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[1]));
						Action value3 = delegate
						{
							mainUIManager.OnPurchaseRevive();
						};
						mainUIManager.ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"Notify"
							},
							{
								"msg",
								popupMsg
							},
							{
								"okHandler",
								value3
							}
						});
					}
					else
					{
						Action value4 = delegate
						{
							mainUIManager.isPurchasePopup = false;
						};
						popupMsg = LeanLocalization.GetTranslationText("158");
						mainUIManager.ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"Notify"
							},
							{
								"msg",
								popupMsg
							},
							{
								"CloseHandler",
								value4
							}
						});
					}
				});
			});
		};
		Action value2 = delegate
		{
			isPurchasePopup = false;
		};
		ShowPopupCommon(new Dictionary<string, object>
		{
			{
				"type",
				"BtnPop"
			},
			{
				"BtnPopType",
				"RevivePurchase"
			},
			{
				"msg",
				string.Empty
			},
			{
				"yOffset",
				0f
			},
			{
				"sizeDelta",
				new Vector2(495f, 400f)
			},
			{
				"okHandler",
				value
			},
			{
				"CloseHandler",
				value2
			}
		});
		isPurchasePopup = true;
		int num2 = (from s in DataContainer.Instance.ShopTableRaw.dataArray
			where s.Type == "jewel"
			select s.Reward).Min();
		revivePurchaseGemCountText.text = num2.ToString();
	}

	public void DoResultPopup()
	{
		setActivateFilter(activate: true);
		popCheckRevive.gameObject.SetActive(value: true);
		popPause.gameObject.SetActive(value: false);
		LeanTween.delayedCall(0f, (Action)delegate
		{
			popPause.gameObject.SetActive(value: false);
		});
		reviveRequireGemCountText.text = reviveReqGemCount[currentReviveCount].ToString();
		Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
		Game.Instance.IsInGame.Value = false;
		if (crtCheckReviveCount != null)
		{
			StopCoroutine(crtCheckReviveCount);
		}
		crtCheckReviveCount = StartCoroutine(cetRevivePopup());
	}

	public void DoResultMultiPopup()
	{
		setActivateFilter(activate: true);
		Game.Instance.IsInGame.Value = false;
		OnBtnClick_ResultMulti();
	}

	private IEnumerator cetRevivePopup()
	{
		Image countImage = base.transform.Find("Popup/CheckRevive/CountTextImage").GetComponent<Image>();
		countImage.sprite = CountTextSprites[0];
		LTDescr countTextLTDescr = null;
		Vector3 fromScale = Vector3.one * 0.5f;
		Vector3 toScale = Vector3.one * 3f;
		Color fromColor = Color.white;
		Color toColor = Color.white;
		toColor.a = 0f;
		Action countTextDirector = delegate
		{
			((Func<bool>)delegate
			{
				if (countTextLTDescr != null)
				{
					LeanTween.cancel(countTextLTDescr.id);
				}
				countTextLTDescr = LeanTween.value(gameObject, delegate(float norm)
				{
					countImage.transform.localScale = Vector3.Lerp(fromScale, toScale, norm);
					if (0.5f > norm)
					{
						countImage.color = Color.Lerp(toColor, fromColor, 0.5f + 0.5f * (norm * 2f));
					}
					else
					{
						countImage.color = Color.Lerp(fromColor, toColor, (norm - 0.5f) * 2f);
					}
				}, 0f, 1f, 0.6667f).setEase(LeanTweenType.easeInOutExpo);
				return true;
			})();
		};
		float period = 3f;
		int spriteCount = CountTextSprites.Length;
		int index = -1;
		float elapsed = 0f - Time.deltaTime;
		while (true)
		{
			if (isPurchasePopup)
			{
				yield return 0;
				continue;
			}
			elapsed += Time.deltaTime;
			GameObject go = EventSystem.current.currentSelectedGameObject;
			if ((null == go || go.name.Contains("Filter") || go.name.Contains("BG")) && 0 < UnityEngine.Input.touchCount && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				elapsed = Mathf.Round(elapsed + 0.5f);
			}
			int newIndex = Mathf.Min(spriteCount - 1, Mathf.FloorToInt(elapsed));
			if (index != newIndex)
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(AudCount);
				countTextDirector();
			}
			index = newIndex;
			countImage.sprite = CountTextSprites[index];
			if (elapsed >= period)
			{
				break;
			}
			yield return 0;
		}
		OnBtnClick_Result();
	}

	private void createCheckRevive()
	{
		popCheckRevive = base.transform.Find("Popup/CheckRevive").transform;
		popCheckReviveGemReviveBtn = base.transform.Find("Popup/CheckRevive/Buttons/ReviveBtn").transform.GetComponent<Button>();
		reviveRequireGemCountText = base.transform.Find("Popup/CheckRevive/Buttons/ReviveBtn/CoinText").GetComponent<Text>();
		revivePurchaseGemCountText = base.transform.Find("Popup/CommonPopup/RevivePurchase/GemCountText").GetComponent<Text>();
		popReviveADView = popCheckRevive.Find("Buttons/AdViewBtn").GetComponent<Button>();
		popReviveADView.onClick.AddListener(delegate
		{
			isPurchasePopup = true;
			if (ADRewardManager.isLoadedAD())
			{
				ADRewardManager.ShowRewardAD(delegate
				{
					isPurchasePopup = false;
					GoogleAnalyticsV4.getInstance().LogScreen("UNITYAD_VIEW_COMPLETE_REVIVE");
					popReviveADView.interactable = false;
					setActivateFilter(activate: false);
					popCheckRevive.gameObject.SetActive(value: false);
					Game.instance.TriggerPause(pauseGame: false);
					Revive.Instance.SendRevive();
					Game.Instance.IsInGame.Value = true;
					if (crtCheckReviveCount != null)
					{
						StopCoroutine(crtCheckReviveCount);
					}
				}, delegate
				{
					if (!ADRewardManager.Instance.IsRewarded)
					{
						Action value2 = delegate
						{
							isPurchasePopup = false;
						};
						ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"Notify"
							},
							{
								"okHandler",
								value2
							},
							{
								"msg",
								LeanLocalization.GetTranslationText("232")
							}
						});
					}
				});
			}
			else
			{
				Action value = delegate
				{
					isPurchasePopup = false;
				};
				ShowPopupCommon(new Dictionary<string, object>
				{
					{
						"type",
						"Notify"
					},
					{
						"okHandler",
						value
					},
					{
						"msg",
						LeanLocalization.GetTranslationText("154")
					}
				});
			}
		});
	}

	private IEnumerator cetPopupCommon(float yPos)
	{
		Vector2 anchoredPosition = commonPopupRT.anchoredPosition;
		Vector2 revPos = commonPopupRT.anchoredPosition;
		revPos.y = yPos;
		while (commonPopupAnim.GetCurrentAnimatorStateInfo(0).IsName("popup_down"))
		{
			float animNormalTime = commonPopupAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
			float curveNorm = PopupLerpAccCurve.Evaluate(animNormalTime);
			LateUpdater.Instance.AddAction(delegate
			{
				commonPopupRT.anchoredPosition = Vector2.Lerp(commonPopupRT.anchoredPosition, revPos, curveNorm);
			});
			yield return 0;
		}
	}

	public void ShowPopupCommon(Dictionary<string, object> attribute)
	{
		string key = attribute["type"] as string;
		if (popupDispatcher != null)
		{
			popupDispatcher[key](attribute);
			return;
		}
		Action<Dictionary<string, object>> commonAction = delegate(Dictionary<string, object> attr)
		{
			string text2 = attr["msg"] as string;
			filterCommonPopup.gameObject.SetActive(value: true);
			commonPopup.gameObject.SetActive(value: true);
			commonPopup.Find("MsgText").GetComponent<Text>().text = text2;
			RectTransform component = commonPopup.GetComponent<RectTransform>();
			Vector2 anchoredPosition = component.anchoredPosition;
			if (attr.ContainsKey("yOffset"))
			{
				anchoredPosition.y = (float)attr["yOffset"];
			}
			else
			{
				anchoredPosition.y = 0f;
			}
			component.anchoredPosition = anchoredPosition;
			if (crtRevPopupCommon != null)
			{
				StopCoroutine(crtRevPopupCommon);
			}
			crtRevPopupCommon = StartCoroutine(cetPopupCommon(anchoredPosition.y));
			Vector2 sizeDelta = component.sizeDelta;
			if (attr.ContainsKey("sizeDelta"))
			{
				sizeDelta = (Vector2)attr["sizeDelta"];
			}
			else
			{
				sizeDelta.x = 495f;
				sizeDelta.y = 400f;
			}
			component.sizeDelta = sizeDelta;
			if (attr.ContainsKey("CloseHandler") && attr["CloseHandler"] != null)
			{
				((Func<bool>)delegate
				{
					Button closeBtn4 = commonPopup.Find("CloseBtn").GetComponent<Button>();
					Action innerHandler = attr["CloseHandler"] as Action;
					UnityAction closeHandler = null;
					closeHandler = delegate
					{
						innerHandler();
						closeBtn4.onClick.RemoveListener(closeHandler);
					};
					closeBtn4.onClick.AddListener(closeHandler);
					return true;
				})();
			}
		};
		Button okBtn = commonPopup.Find("OKBtn").GetComponent<Button>();
		popupDispatcher = new Dictionary<string, Action<Dictionary<string, object>>>
		{
			{
				"Notify",
				delegate(Dictionary<string, object> attr)
				{
					commonAction(attr);
					commonPopup.Find("OKBtn").gameObject.SetActive(value: true);
					Button closeBtn3 = commonPopup.Find("CloseBtn").GetComponent<Button>();
					closeBtn3.gameObject.SetActive(value: false);
					okBtn.onClick.RemoveAllListeners();
					if (attr.ContainsKey("okHandler"))
					{
						okBtn.onClick.AddListener(delegate
						{
							(attr["okHandler"] as Action)();
							closeBtn3.onClick.Invoke();
						});
					}
					else
					{
						okBtn.onClick.AddListener(delegate
						{
							closeBtn3.onClick.Invoke();
						});
					}
				}
			},
			{
				"Select",
				delegate(Dictionary<string, object> attr)
				{
					commonAction(attr);
					commonPopup.Find("OKBtn").gameObject.SetActive(value: true);
					Button closeBtn2 = commonPopup.Find("CloseBtn").GetComponent<Button>();
					closeBtn2.gameObject.SetActive(value: true);
					okBtn.onClick.RemoveAllListeners();
					okBtn.onClick.AddListener(delegate
					{
						(attr["okHandler"] as Action)();
						closeBtn2.onClick.Invoke();
					});
				}
			},
			{
				"BtnPop",
				delegate(Dictionary<string, object> attr)
				{
					commonAction(attr);
					commonPopup.Find("OKBtn").gameObject.SetActive(value: false);
					Button closeBtn = commonPopup.Find("CloseBtn").GetComponent<Button>();
					closeBtn.gameObject.SetActive(value: true);
					Transform transBtnPopRoot = null;
					UnityAction btnPopHideHandler = null;
					btnPopHideHandler = delegate
					{
						transBtnPopRoot.gameObject.SetActive(value: false);
						closeBtn.onClick.RemoveListener(btnPopHideHandler);
					};
					closeBtn.onClick.AddListener(btnPopHideHandler);
					string text = attr["BtnPopType"] as string;
					if (text != null && text == "RevivePurchase")
					{
						commonPopup.Find("OKBtn").gameObject.SetActive(value: true);
						okBtn.onClick.RemoveAllListeners();
						okBtn.onClick.AddListener(delegate
						{
							(attr["okHandler"] as Action)();
							closeBtn.onClick.Invoke();
						});
						transBtnPopRoot = commonPopup.Find("RevivePurchase");
						transBtnPopRoot.gameObject.SetActive(value: true);
						transBtnPopRoot.Find("MsgText").GetComponent<Text>().text = string.Format(LeanLocalization.GetTranslationText("168"), LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[1]));
					}
				}
			}
		};
		popupDispatcher[key](attribute);
	}

	public void ClosePopupCommon()
	{
		filterCommonPopup.gameObject.SetActive(value: false);
		commonPopup.gameObject.SetActive(value: false);
	}

	private void createCommonPopup()
	{
		commonPopup = base.transform.Find("Popup/CommonPopup");
		filterCommonPopup = base.transform.Find("Popup/CommonPopFilter");
		commonPopupMsgText = commonPopup.Find("MsgText").GetComponent<Text>();
		commonPopupRT = commonPopup.GetComponent<RectTransform>();
		commonPopupAnim = commonPopup.GetComponent<Animator>();
	}

	public void OnBtnClick_ResultReplay()
	{
		PlayerInfo.Instance.IsRetryGame = true;
		OnBtnClick_GotoMenu();
	}

	public void OnBtnClick_GotoMenu()
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
		Game.instance.TriggerPause(pauseGame: false);
		doGotoMain();
	}

	private void createResult()
	{
		popResult = base.transform.Find("Popup/Result").transform;
		popResultMulti = base.transform.Find("Popup/Result-Multi").transform;
	}

	private void updateResultStats()
	{
		Text component = base.transform.Find("Popup/Result/ScoreRoot/ScoreText").GetComponent<Text>();
		Text component2 = base.transform.Find("Popup/Result/StatRoot/DistText").GetComponent<Text>();
		Text component3 = base.transform.Find("Popup/Result/StatRoot/GoldText").GetComponent<Text>();
		Text component4 = base.transform.Find("Popup/Result/BonusRoot/GoldText").GetComponent<Text>();
		Text component5 = base.transform.Find("Popup/Result/BonusRoot/ScoreText").GetComponent<Text>();
		int num = GameStats.Instance.score + Mathf.CeilToInt(GameStats.Instance.meters);
		int num2 = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][4];
		float pvalue = DataContainer.Instance.PlayerParamLevelTableRawByLevel[4].PPLevelRaws[num2].Pvalue;
		int num3 = Mathf.CeilToInt((float)num * pvalue);
		component.text = (num + num3).ToString();
		component5.text = num3.ToString();
		tempSB.Remove(0, tempSB.Length);
		component2.text = tempSB.AppendFormat("{0}", GameStats.Instance.meters).ToString();
		component3.text = GameStats.Instance.coins.ToString();
		int num4 = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][5];
		float pvalue2 = DataContainer.Instance.PlayerParamLevelTableRawByLevel[5].PPLevelRaws[num4].Pvalue;
		int num5 = Mathf.CeilToInt((float)GameStats.Instance.coins * pvalue2);
		num5 += (PlayerInfo.Instance.CharacterSkills[4] ? Mathf.CeilToInt((float)GameStats.Instance.coins * UnityEngine.Random.Range(0.05f, 0.1f)) : 0);
		num5 += (PlayerInfo.Instance.StartItems[7] ? Mathf.CeilToInt((float)GameStats.Instance.coins * 0.5f) : 0);
		component4.text = num5.ToString();
		int num6 = num + num3;
		if (num6 > PlayerInfo.Instance.HighScore)
		{
			PlayerInfo.Instance.HighScore = num6;
		}
		int num7 = GameStats.Instance.coins + num5;
		CurrencyTypeMapInt currency;
		(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] + num7;
	}

	private void OnDestroyInstances()
	{
		So.Instance = null;
		Character.instance = null;
		CharacterCamera.instance = null;
		CharacterModelPreviewFactory.instance = null;
		CharacterRendering.instance = null;
		HoverboardManager.instance = null;
		HoverboardModelPreviewFactory.instance = null;
		Hoverboard.instance = null;
		Jetpack.instance = null;
		NameTagAttackState.instance = null;
		Running.instance = null;
		DeviceInfo._instance = null;
		FollowingGuard.instance = null;
		Game.instance = null;
		Game.HasLoaded = false;
		Game.characterController = null;
		AppInfo.instance_ = null;
		GameStats.instance = null;
		GameStats.Instance.OnMetersChanged = null;
		Layers._instance = null;
		RandomizerHold.mapSelect = 0;
		SpawnPointManager.instance = null;
		ThemeAssets.instance = null;
		ThemeManager.instance = null;
		Track.instance = null;
		TrackChunkCollection.trackChunks = new List<TrackChunk>();
		LocalNotificationsManager._instance = null;
		LocalNotificationsManager._notificationCtrl = null;
		NpcEnemies.instance = null;
		NpcEnemiesNew.instance = null;
		CoinPool.instance = null;
		DailyLetterPickupManager.instance = null;
		Revive.instance = null;
		Strings.language = null;
		Strings.values = null;
		MovingTrain.activeTrains = new List<MovingTrain>();
		Globals.addedAnimEvents.Clear();
		PickupDefault.ActivatedPickups.Clear();
	}

	private void doGotoMain()
	{
		int @int = PlayerPrefs.GetInt("adplaycount", 0);
		@int++;
		if (@int >= 3 && AdInterstitialManager.isLoadedAD())
		{
			PlayerPrefs.SetInt("adplaycount", 0);
			AdInterstitialManager.ShowInterstitialAD(delegate
			{
				LateUpdater.Instance.AddAction(delegate
				{
					LoadLevel_Main();
				});
			});
		}
		else
		{
			AdInterstitialManager.RequestInterstitialAD();
			PlayerPrefs.SetInt("adplaycount", @int);
			LoadLevel_Main();
		}
	}

	private static void LoadLevel_Main()
	{
		LoadingMergeLoader.Instance.ActiveLoadSceneName = "MenuScene";
		LoadingMergeLoader.Instance.StartLoadLevel();
		if (!string.IsNullOrEmpty(PlayerInfo.Instance.SelectedCharIDVolatile))
		{
			PlayerInfo.Instance.SelectedCharID = PlayerInfo.Instance.SelectedCharIDVolatile;
		}
		PlayerInfo.Instance.IsSenseBackBtn = true;
	}

	private void OnApplicationQuit()
	{
		GoogleAnalyticsV4.getInstance().StopSession();
	}
}
