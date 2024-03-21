using EnhancedUI.EnhancedScroller;
using GooglePlayGames;
using Lean;
using SerializableClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Wenee;

public class MenuUIManager : MonoBehaviour, IEnhancedScrollerDelegate
{
	private static MenuUIManager instance;

	public EnhancedScrollerCellView CVHeaderPref;

	public EnhancedScrollerCellView CVContentGenericPref;

	public EnhancedScrollerCellView CVContentCHCoinsPref;

	public GameObject PreviewCharacterPref;

	public Sprite[] CurrencyMidIcons;

	public Sprite[] CurrencySmallIcons;

	public Transform PreviewScrollRoot;

	public Image Filter;

	public Button FilterTop;

	public GameObject FilterTop2;

	public Transform FilterCommonPopup;

	public AudioClip ClickAud;

	public AnimationCurve RouletteCurve;

	public AudioSource RouletteAudSrc;

	public AudioClip RouletteAud;

	public AudioClip RouletteLongAud;

	public ParticleSystem[] ParticleEffects;

	public GiftBoxDirector ThisGiftBoxDirector;

	public int backBtnStackDepth;

	private Transform exitPopup;

	private Button exitPopupOK;

	private Button exitPopupCancel;

	private Image chLockIcon;

	private LLocImage chNameLocImageText;

	private CharacterPurchasePopup chPurchasePopup;

	private Button chPurchasePopupBtn;

	private Image chSkillDescIconImage;

	private LeanLocalizedText chSkillDescTextLoc;

	private Button giftBoxBtn;

	private Button upgradeBoxBtn;

	private Animator upgradeBoxBtnImageTextAnim;

	private TextMeshProUGUI upgradeBoxTimeLeftText;

	private Button upgradeBoxPopupViewADsBtn;

	private TextMeshProUGUI upgradeBoxPopupTimeLeftText;

	public AudioClip AudLevelUp;

	public AudioClip AudGiftBox;

	public Texture ToonRamp;

	public float RimPower;

	public float RimStrength;

	public Color RimColor;

	public float Outline;

	public Color OutlineColor;

	public int kNextSlotSetMin = 4;

	public int kNextSlotSetMax = 6;

	public float kSlotTotalPeriod = 5f;

	public float kScalePeriod = 0.1f;

	public GameObject[] upgradeBoxPart;

	private Button[] upgradeBtns;

	private Button upgradeDescriptionBtn;

	private bool isPVRotStarted;

	private float PVRotRestoreDamp = 5f;

	private float PVRotSensi = 1f;

	private Coroutine crtPVRotDragProc;

	public Text goldText;

	public Text gemText;

	public Text nameTagText;

	private TextMeshProUGUI nameTagTimeLeftText;

	public AudioClip AudBuy;

	public AudioClip AudCancer;

	private Transform commonPopup;

	private RectTransform commonPopupRT;

	private Animator commonPopupAnim;

	private Toggle[] shopCurrencyToggles;

	private Transform[] shopCurrencyPopups;

	public AnimationCurve PopupLerpAccCurve;

	private Coroutine crtRevPopupCommon;

	private Dictionary<string, Action<Dictionary<string, object>>> popupDispatcher;

	public EnhancedScroller MissionScroller;

	private bool cvMissionDataDirtyAll;

	private List<CVMissionData> cvMissionDatas = new List<CVMissionData>();

	private Transform missionPopup;

	public Sprite[] LocaleSprites;

	private Transform settingPopup;

	private Button settingLanguageBtn;

	private Toggle settingSoundToggle;

	private Toggle settingMusicToggle;

	private Toggle settingGPSignToggle;

	private Toggle settingFBSignToggle;

	public Animation[] pvAnims;

	public int pvIndicIdx;

	public float pvPoolingOffset;

	private float pvEasePeriod = 0.3334f;

	private float pvDist = 300f;

	private Vector3 pvPos;

	private Button pvPrevBtn;

	private Button pvNextBtn;

	public GameObject PrefFlopItem;

	private Button inviteFriendBtn;

	private Button requestFriendBtn;

	private Button requestFriendLoginBtn;

	private Button messageBoxBtn;

	private Button facebookLoginBtn;

	private GameObject loadingIndicatorGO;

	public InviteFriendsPopup popInviteFriend;

	public RequestFriendsPopup popRequestFriend;

	private MessageBoxPopup popMessageBox;

	private Transform popReady;

	private Button[] popStartBtns;

	private Button[] popStartDisableBtn;

	private Button popReadyCloseBtn;

	private Button popReadyRaceStartBtn;

	private Button popReadyMultiRaceStartBtn;

	private Toggle[] popReadyStartItems;

	private Sprite[] popReadyStartItemIconSprite = new Sprite[Enum.GetValues(typeof(StartItemType)).Length];

	private Transform popReadyStartItemDescRoot;

	private Image popReadyStartItemIconImage;

	private Text popReadyStartItemCoinText;

	private Image popReadyStartItemTextImage;

	private Text popReadyStartItemDescText;

	private int popReadyStartItemLastIdx;

	private int popReadyStartReqGold;

	private Coroutine crtCheckMultiPlayer;

	private bool showMultiWaitCancelPopup;

	private int MultiGameType = 1;

	public static MenuUIManager Instance => instance;

	private void senseBackBtn()
	{
		if (showMultiWaitCancelPopup && UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			cancelWaitMultiplay();
		}
		else
		{
			if (!PlayerInfo.Instance.IsSenseBackBtn || !Input.GetKeyDown(KeyCode.Escape))
			{
				return;
			}
			switch (backBtnStackDepth)
			{
			case 0:
				if (exitPopup.gameObject.activeInHierarchy)
				{
					closeExitPopup();
				}
				else
				{
					doExitPopup();
				}
				break;
			case 1:
				backBtnStackDepth = 0;
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				closeExitPopup();
				SetActivateFilter(activate: false);
				popReady.gameObject.SetActive(value: false);
				settingPopup.gameObject.SetActive(value: false);
				missionPopup.gameObject.SetActive(value: false);
				popInviteFriend.Close();
				popRequestFriend.Close();
				popMessageBox.Close();
				chPurchasePopup.Close();
				Enumerable.Range(0, Enum.GetValues(typeof(MenuCurrencyType)).Length).All(delegate(int s)
				{
					shopCurrencyPopups[s].gameObject.SetActive(value: false);
					return true;
				});
				break;
			case 2:
				commonPopup.Find("CloseBtn").GetComponent<Button>().onClick.Invoke();
				break;
			}
		}
	}

	private void OnValue_currency(CurrencyType cType, int oldValue, int value)
	{
		switch (cType)
		{
		case CurrencyType.Gold:
			goldText.text = value.ToString();
			break;
		case CurrencyType.Gem:
			gemText.text = value.ToString();
			break;
		}
	}

	private void OnEvtNameTagCount_playerinfo(int value, int origin)
	{
		nameTagText.text = $"{value.ToString():D}";
		int num = value - origin;
		if (0 > num && origin == 10)
		{
			PlayerInfo.Instance.TimeRelative = DateTime.Now.Ticks;
		}
		PlayerInfo.Instance.DirtyAll();
	}

	private void OnValue_CharParamLevels(string chID, int[] oldValue, int[] value)
	{
		PlayerInfo.Instance.DirtyAll();
	}

	private void OnValue_CharUnlocks(string charID, bool oldValue, bool value)
	{
		PlayerInfo.Instance.DirtyAll();
	}

	private void OnValue_CharOwnedTokens(string tokenID, int oldValue, int value)
	{
		PlayerInfo.Instance.DirtyAll();
	}

	private void OnValue_CharParamLevels2(string chID, int[] oldValue, int[] value)
	{
		int length = Enum.GetValues(typeof(PlayerParameterType)).Length;
		bool flag = false;
		int i;
		for (i = 0; i < length; i++)
		{
			int maxlevel = DataContainer.Instance.PlayerParamTableRaw.dataArray[i].Maxlevel;
			int level = value[i];
			if (maxlevel - 1 < level)
			{
				flag = true;
				level = maxlevel - 1;
				((Func<bool>)delegate
				{
					int idx = i;
					LateUpdater.Instance.AddAction(delegate
					{
						value[idx] = level;
					});
					return true;
				})();
			}
			upgradeBtns[i].interactable = (maxlevel - 1 > level);
			upgradeBtns[i].transform.Find("LevelText").GetComponent<Text>().text = $"{level + 1}";
			int requiregold = DataContainer.Instance.PlayerParamLevelTableRawByLevel[i].PPLevelRaws[level].Requiregold;
			upgradeBtns[i].transform.Find("CoinText").GetComponent<TextMeshProUGUI>().text = $"{requiregold}";
		}
		if (flag)
		{
			LateUpdater.Instance.AddAction(delegate
			{
				PlayerInfo.Instance.CharParamLevels[chID] = value;
			});
		}
	}

	private void OnReady_FBManager(bool isSuccess)
	{
		if (FBManager.Instance.IsReady)
		{
			inviteFriendBtn.gameObject.SetActive(value: true);
			facebookLoginBtn.gameObject.SetActive(value: false);
			requestFriendBtn.transform.parent.gameObject.SetActive(value: true);
			requestFriendLoginBtn.transform.parent.gameObject.SetActive(value: false);
			messageBoxBtn.gameObject.SetActive(value: true);
			NeedCheckMessageBoxNew();
			if (!PlayerInfo.Instance.FBFirstLoginReward)
			{
				Action value = delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				};
				ShowPopupCommon(new Dictionary<string, object>
				{
					{
						"type",
						"Notify"
					},
					{
						"msg",
						string.Format(LeanLocalization.GetTranslationText("192"), 3)
					},
					{
						"CloseHandler",
						value
					}
				});
				PlayerInfo.Instance.FBFirstLoginReward = true;
				CurrencyTypeMapInt currency;
				(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] + 3;
				settingPopup.Find("LogoutElements/FacebookBtn/BonusIcon").gameObject.SetActive(!PlayerInfo.Instance.FBFirstLoginReward);
				facebookLoginBtn.transform.Find("BonusIcon").gameObject.SetActive(!PlayerInfo.Instance.FBFirstLoginReward);
				requestFriendLoginBtn.transform.Find("BonusIcon").gameObject.SetActive(!PlayerInfo.Instance.FBFirstLoginReward);
			}
		}
		ActivateLoadingScreenFilter(isActivate: false);
	}

	public void RegistEvents()
	{
		PlayerInfo.Instance.Currency.OnValue += OnValue_currency;
		PlayerInfo.Instance.OnEvtNameTagCount += OnEvtNameTagCount_playerinfo;
		PlayerInfo.Instance.CharParamLevels.OnValue += OnValue_CharParamLevels;
		PlayerInfo.Instance.CharUnlocks.OnValue += OnValue_CharUnlocks;
		PlayerInfo.Instance.CharOwnedTokens.OnValue += OnValue_CharOwnedTokens;
		Action onCompleteAction = LeanTween.value(base.gameObject, (Action<float>)delegate
		{
		}, 0f, 1f, 1f).setOnCompleteOnRepeat(isOn: true).setOnComplete((Action)delegate
		{
			if (10 <= PlayerInfo.Instance.NameTagCount)
			{
				nameTagTimeLeftText.transform.parent.gameObject.SetActive(value: false);
			}
			else
			{
				long ticks = DateTime.Now.Ticks;
				int num = (int)((ticks - PlayerInfo.Instance.TimeRelative) / 10000000);
				if (600 <= num)
				{
					int num2 = num / 600;
					PlayerInfo.Instance.TimeRelative += (long)(600 * num2) * 10000000L;
					PlayerInfo.Instance.NameTagCount = Mathf.Clamp(PlayerInfo.Instance.NameTagCount + num2, 0, 10);
				}
				nameTagTimeLeftText.transform.parent.gameObject.SetActive(value: true);
				int num3 = 600 - num % 600;
				if (10 <= PlayerInfo.Instance.NameTagCount)
				{
					nameTagTimeLeftText.transform.parent.gameObject.SetActive(value: false);
				}
				else
				{
					nameTagTimeLeftText.text = $"{num3 / 60:D2}:{num3 % 60:D2}";
				}
			}
		})
			.setLoopClamp()
			.onComplete;
			LeanTween.delayedCall(0f, (Action)delegate
			{
				onCompleteAction();
			});
			PlayerInfo.Instance.CharParamLevels.OnValue += OnValue_CharParamLevels2;
			if (!PlayerInfo.Instance.IsMenuSceneByTitleScene)
			{
				YieldRegistFBReadyHandler();
			}
		}

		public void YieldRegistFBReadyHandler()
		{
			FBManager.Instance.OnReady += OnReady_FBManager;
		}

		private void initEventVars()
		{
			PlayerInfo.Instance.NameTagCount = PlayerInfo.Instance.NameTagCount;
			PlayerInfo.Instance.Currency.Keys.ToArray().All(delegate(CurrencyType key)
			{
				PlayerInfo.Instance.Currency[key] = PlayerInfo.Instance.Currency[key];
				return true;
			});
			PlayerInfo.Instance.CharParamLevels.Keys.ToArray().All(delegate(string key)
			{
				PlayerInfo.Instance.CharParamLevels[key] = PlayerInfo.Instance.CharParamLevels[key];
				return true;
			});
		}

		private void initUIs()
		{
			Time.timeScale = 1f;
			updatePreviewAnim();
			updateCharacterSelectRelatedByUnlock(forceDisable: false);
			if (PlayerInfo.Instance.IsRetryGame)
			{
				OnBtnClick_RaceReady();
			}
			commonPopup.Find("LanguageSelect").GetChild(PlayerInfo.Instance.LocaleIndex).GetComponent<Toggle>()
				.isOn = true;
				if (!PlayerInfo.Instance.WelcomeUpdateReward)
				{
					PlayerInfo.Instance.WelcomeUpdateReward = true;
					CurrencyTypeMapInt currency;
					(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] + 30;
					commonPopup.Find("SeasonReward/Text").GetComponent<Text>().text = LeanLocalization.GetTranslationText("166");
					commonPopup.Find("SeasonReward/RewardText").GetComponent<Text>().text = "X " + 30.ToString();
					Action value = delegate
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
						if (!PlayerInfo.Instance.TutorialCompleted)
						{
							ClosePopupCommon();
							LeanTween.delayedCall(0f, (Action)delegate
							{
								Action value3 = delegate
								{
									Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
									GoogleAnalyticsV4.getInstance().LogEvent("Tutorial", "Begin", "Tutorial Start", 1L);
									PlayerInfo.Instance.DirtyAll();
									setupHighMeters();
									StartCoroutine(LoadGame());
								};
								ShowPopupCommon(new Dictionary<string, object>
								{
									{
										"type",
										"Notify"
									},
									{
										"msg",
										LeanLocalization.GetTranslationText("170")
									},
									{
										"CloseHandler",
										value3
									}
								});
							});
						}
					};
					Action value2 = delegate
					{
					};
					ShowPopupCommon(new Dictionary<string, object>
					{
						{
							"type",
							"BtnPop"
						},
						{
							"BtnPopType",
							"SeasonReward"
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
							new Vector2(480f, 550f)
						},
						{
							"CloseHandler",
							value
						},
						{
							"RewardBtnHandler",
							value2
						}
					});
				}
				RewardSelectableAds(new HashSet<string>
				{
					"tapjoy",
					"nas"
				}, needShowPopup: true);
				updateChSkillDesc();
			}

			private void Awake()
			{
				instance = this;
				LeanLocalization.Instance.SetLanguage(DataContainer.LocaleIdentifier[PlayerInfo.Instance.LocaleIndex]);
				LiteGameStarter.startScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
				RegistEvents();
				createCurrency();
				createShop();
				createUpgrades();
				createPreview();
				createReady();
				createPreviewRotation();
				createPurchaseCharacter();
				createGiftUpgradeBox();
				createSettingPopup();
				createExitPopup();
				createGoogleRanking();
				createMission();
				createPopupCommon();
				createInviteFriendsPopup();
				createSelectableRewardAds();
				createCharacterPurchasePopup();
				initEventVars();
				initUIs();
				initTheme();
			}

			public void ResetUI()
			{
				PlayerInfo.Instance.Currency.OnValue -= OnValue_currency;
				PlayerInfo.Instance.OnEvtNameTagCount -= OnEvtNameTagCount_playerinfo;
				PlayerInfo.Instance.CharParamLevels.OnValue -= OnValue_CharParamLevels;
				PlayerInfo.Instance.CharOwnedTokens.OnValue -= OnValue_CharOwnedTokens;
				PlayerInfo.Instance.CharParamLevels.OnValue -= OnValue_CharParamLevels2;
				FBManager.Instance.OnReady -= OnReady_FBManager;
				PlayerInfo.Instance.MsnGoalValues.OnValue -= OnValue_MsnGoalValues;
				PlayerInfo.Instance.MsnCompleted.OnValue -= OnVale_MsnCompleted;
				PlayerInfo.Instance.MsnRewarded.OnValue -= OnValue_MsnRewarded;
				RegistEvents();
				initEventVars();
				initUIs();
				PlayerInfo.Instance.CheckMissionDailyTick();
			}

			private IEnumerator Start()
			{
				yield return 0;
				PlayerInfo.Instance.AccMissionByCondTypeID("launchgame", "-1", 1.ToString());
				GoogleAnalyticsV4.getInstance().LogScreen("MainScreen");
			}

			private void Update()
			{
				senseBackBtn();
			}

			private void OnDestroy()
			{
				LeanTween.cancelAll(callComplete: false);
				PlayerInfo.Instance.Currency.OnValue -= OnValue_currency;
				PlayerInfo.Instance.OnEvtNameTagCount -= OnEvtNameTagCount_playerinfo;
				PlayerInfo.Instance.CharParamLevels.OnValue -= OnValue_CharParamLevels;
				PlayerInfo.Instance.CharOwnedTokens.OnValue -= OnValue_CharOwnedTokens;
				PlayerInfo.Instance.CharParamLevels.OnValue -= OnValue_CharParamLevels2;
				FBManager.Instance.OnReady -= OnReady_FBManager;
				PlayerInfo.Instance.MsnGoalValues.OnValue -= OnValue_MsnGoalValues;
				PlayerInfo.Instance.MsnCompleted.OnValue -= OnVale_MsnCompleted;
				PlayerInfo.Instance.MsnRewarded.OnValue -= OnValue_MsnRewarded;
				instance = null;
			}

			private void registPopupOpenBtn(Transform popup, Button btn, Action afterAction = null)
			{
				btn.onClick.AddListener(delegate
				{
					SetActivateFilter(activate: true);
					popup.gameObject.SetActive(value: true);
					if (afterAction != null)
					{
						afterAction();
					}
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				});
			}

			private void registPopupCloseBtn(Transform popup, Button btn, Action afterAction = null)
			{
				btn.onClick.AddListener(delegate
				{
					SetActivateFilter(activate: false);
					popup.gameObject.SetActive(value: false);
					if (afterAction != null)
					{
						afterAction();
					}
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				});
			}

			public void SetActivateFilter(bool activate, bool bothPreview = true)
			{
				Filter.gameObject.SetActive(activate);
				if (!activate)
				{
					updatePreviewAnim();
				}
			}

			private void updateCharacterSelectRelatedByUnlock(bool forceDisable)
			{
				if (forceDisable)
				{
					popStartBtns.All(delegate(Button s)
					{
						s.interactable = false;
						return true;
					});
					popStartDisableBtn.All(delegate(Button s)
					{
						s.gameObject.SetActive(value: true);
						return true;
					});
					return;
				}
				popStartBtns.All(delegate(Button s)
				{
					s.interactable = true;
					return true;
				});
				string iD = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx].ID;
				bool isUnlock = PlayerInfo.Instance.CharUnlocks[iD];
				CharacterInfoData characterInfoData = DataContainer.Instance.CharacterTableRaw[iD];
				chNameLocImageText.SetPhraseName(characterInfoData.Nameimagepath);
				if (isUnlock)
				{
					upgradeBtns.All(delegate(Button s)
					{
						s.interactable = isUnlock;
						return true;
					});
					chNameLocImageText.gameObject.SetActive(value: true);
					chLockIcon.gameObject.SetActive(value: false);
					popStartDisableBtn.All(delegate(Button s)
					{
						s.gameObject.SetActive(value: false);
						return true;
					});
					PlayerInfo.Instance.CharParamLevels[iD] = PlayerInfo.Instance.CharParamLevels[iD];
					return;
				}
				chNameLocImageText.gameObject.SetActive(value: false);
				chLockIcon.gameObject.SetActive(value: true);
				popStartDisableBtn.All(delegate(Button s)
				{
					s.gameObject.SetActive(value: true);
					return true;
				});
				int[] array = DataContainer.Instance.CharacterUnlockCounts[iD];
				int num = 0;
				for (num = 0; num < array.Length && (num == 2 || 0 >= array[num]); num++)
				{
				}
				Sprite sprite = null;
				string empty = string.Empty;
				switch (num)
				{
				default:
					return;
				case 2:
					return;
				case 0:
					sprite = CurrencyMidIcons[0];
					empty = $"X {array[0]:D}";
					break;
				case 1:
					sprite = CurrencyMidIcons[1];
					empty = $"X{array[1]:D}";
					break;
				case 3:
				{
					string iconpath = DataContainer.Instance.TokenTableRaw[array[2].ToString()].Iconpath;
					sprite = DataContainer.Instance.GetAssetResources<Sprite>(iconpath);
					empty = $"{PlayerInfo.Instance.CharOwnedTokens[array[2].ToString()]:D}/{array[3]:D}";
					break;
				}
				}
				popStartBtns.All(delegate(Button s)
				{
					s.interactable = isUnlock;
					return true;
				});
				upgradeBtns.All(delegate(Button s)
				{
					s.interactable = isUnlock;
					return true;
				});
			}

			public void PlayClickAud()
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
			}

			public void ActivateLoadingScreenFilter(bool isActivate)
			{
				loadingIndicatorGO.SetActive(isActivate);
				PlayerInfo.Instance.IsSenseBackBtn = !isActivate;
			}

			private void doExitPopup()
			{
				AdBannerManager.ShowBannerAD();
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				backBtnStackDepth = 1;
				exitPopup.gameObject.SetActive(value: true);
			}

			private void closeExitPopup()
			{
				AdBannerManager.HideBannerAD();
				backBtnStackDepth = 0;
				exitPopup.gameObject.SetActive(value: false);
			}

			private void createExitPopup()
			{
				exitPopup = base.transform.parent.Find("ExitPop");
				exitPopupOK = exitPopup.Find("ButtonBG/OKBtn").GetComponent<Button>();
				exitPopupCancel = exitPopup.Find("ButtonBG/CancelBtn").GetComponent<Button>();
				exitPopupOK.onClick.AddListener(delegate
				{
					GoogleAnalyticsV4.getInstance().StopSession();
					PlayerInfo.Instance.IsSenseBackBtn = false;
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					LoadingMergeLoader.Instance.AllElements.gameObject.SetActive(value: true);
					LoadingMergeLoader.Instance.AllElements.transform.Find("Canvas/RawImage").gameObject.SetActive(value: false);
					ProcessThreadCollection threads = Process.GetCurrentProcess().Threads;
					IEnumerator enumerator = threads.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ProcessThread processThread = (ProcessThread)enumerator.Current;
							processThread.Dispose();
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
					Process.GetCurrentProcess().Kill();
				});
				exitPopupCancel.onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					closeExitPopup();
				});
			}

			public void OnBtnClick_PurchaseCharacter()
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				string iD = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx].ID;
				CharacterInfoData characterInfoData = DataContainer.Instance.CharacterTableRaw[iD];
				int[] array = DataContainer.Instance.CharacterUnlockCounts[iD];
				int num = 0;
				for (num = 0; num < array.Length && (num == 2 || 0 >= array[num]); num++)
				{
				}
				switch (num)
				{
				default:
					return;
				case 2:
					return;
				case 0:
					if (array[0] <= PlayerInfo.Instance.Currency[CurrencyType.Gem])
					{
						CurrencyTypeMapInt currency;
						(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] - array[0];
						PlayerInfo.Instance.CharUnlocks[iD] = true;
						string value5 = string.Format(LeanLocalization.GetTranslationText("70"), LeanLocalization.GetTranslationText(characterInfoData.Name1loc));
						Action value6 = delegate
						{
						};
						ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"Notify"
							},
							{
								"okHandler",
								value6
							},
							{
								"msg",
								value5
							}
						});
						string modelname = DataContainer.Instance.CharacterTableRaw[iD].Modelname;
						modelname = Regex.Replace(modelname, "[0-9]+$", string.Empty);
						modelname = $"{modelname}01";
						pvAnims[pvIndicIdx].CrossFadeQueued(modelname + "_attacking", 0.1f, QueueMode.PlayNow);
						pvAnims[pvIndicIdx].CrossFadeQueued(modelname + "_idling01", 0.1f, QueueMode.CompleteOthers);
					}
					else
					{
						showCurrencyPop(MenuCurrencyType.Gem, MenuCurrencyType.Gem, delegate
						{
						}, delegate
						{
						});
					}
					break;
				case 1:
					if (array[1] <= PlayerInfo.Instance.Currency[CurrencyType.Gold])
					{
						CurrencyTypeMapInt currency;
						(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] - array[1];
						PlayerInfo.Instance.CharUnlocks[iD] = true;
						string value7 = string.Format(LeanLocalization.GetTranslationText("70"), LeanLocalization.GetTranslationText(characterInfoData.Name1loc));
						Action value8 = delegate
						{
						};
						ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"Notify"
							},
							{
								"okHandler",
								value8
							},
							{
								"msg",
								value7
							}
						});
					}
					else
					{
						showCurrencyPop(MenuCurrencyType.Gold, MenuCurrencyType.Gold, delegate
						{
						}, delegate
						{
						});
					}
					break;
				case 3:
					if (array[3] <= PlayerInfo.Instance.CharOwnedTokens[array[2].ToString()])
					{
						PlayerInfo.Instance.CharUnlocks[iD] = true;
						string value = string.Format(LeanLocalization.GetTranslationText("70"), LeanLocalization.GetTranslationText(characterInfoData.Name1loc));
						Action value2 = delegate
						{
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
								value
							}
						});
					}
					else
					{
						string value3 = string.Format(LeanLocalization.GetTranslationText("68"), LeanLocalization.GetTranslationText(DataContainer.Instance.TokenTableRaw[array[2].ToString()].Name1loc));
						Action value4 = delegate
						{
						};
						ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"Notify"
							},
							{
								"okHandler",
								value4
							},
							{
								"msg",
								value3
							}
						});
					}
					break;
				}
				updateCharacterSelectRelatedByUnlock(forceDisable: false);
			}

			private void createPurchaseCharacter()
			{
				chNameLocImageText = base.transform.Find("TopCenter/CHName/Image").GetComponent<LLocImage>();
				chLockIcon = base.transform.Find("TopCenter/LockIcon").GetComponent<Image>();
			}

			private void updateChSkillDesc()
			{
				CharacterInfoData characterInfoData = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx];
				chSkillDescIconImage.sprite = DataContainer.Instance.GetAssetResources<Sprite>(characterInfoData.Skillimagepath);
				chSkillDescTextLoc.SetPhraseName(characterInfoData.Skilldesc1loc);
			}

			private void createCharacterPurchasePopup()
			{
				chPurchasePopupBtn = base.transform.Find("BottomCenter/CharacterPurchaseBtn").GetComponent<Button>();
				chPurchasePopupBtn.onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					chPurchasePopup.Open();
				});
				chPurchasePopup = base.transform.parent.Find("Popup/CharacterPurchase").GetComponent<CharacterPurchasePopup>();
				chSkillDescIconImage = base.transform.Find("BottomCenter/CharacterSkillDesc/SkillIcon").GetComponent<Image>();
				chSkillDescTextLoc = base.transform.Find("BottomCenter/CharacterSkillDesc/SkillDescText").GetComponent<LeanLocalizedText>();
			}

			private void initTheme()
			{
				Shader.SetGlobalVector("_Distort", Vector4.zero);
				Shader.SetGlobalFloat("_RimPower", RimPower);
				Shader.SetGlobalFloat("_RimStrength", RimStrength);
				Shader.SetGlobalColor("_RimColor", RimColor);
				Shader.SetGlobalTexture("_ToonTex", ToonRamp);
				Shader.SetGlobalFloat("_Outline", Outline);
				Shader.SetGlobalColor("_OutlineColor", OutlineColor);
			}

			private void createGiftUpgradeBox()
			{
				giftBoxBtn = base.transform.Find("BottomCenter/GiftGachaBtn").GetComponent<Button>();
				bool active = false;
				if (!PlayerInfo.Instance.TimeRelativeAttribute.ContainsKey("GiftBox"))
				{
					active = true;
				}
				else
				{
					long num = long.Parse(PlayerInfo.Instance.TimeRelativeAttribute["GiftBox"]);
					if (2400000000u < DateTime.Now.Ticks - num)
					{
						active = true;
					}
				}
				giftBoxBtn.gameObject.SetActive(active);
				giftBoxBtn.onClick.AddListener(delegate
				{
					PlayerInfo.Instance.IsSenseBackBtn = false;
					Action closeHandler;
					if (ADRewardManager.isLoadedAD())
					{
						ADRewardManager.ShowRewardAD(delegate
						{
							GoogleAnalyticsV4.getInstance().LogScreen("UNITYAD_VIEW_COMPLETE_GIFTBOX");
							PlayerInfo.Instance.AccMissionByCondTypeID("dogiftbox", "-1", 1.ToString());
							GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQDA");
							PlayerInfo.Instance.TimeRelativeAttribute["GiftBox"] = DateTime.Now.Ticks.ToString();
							PlayerInfo.Instance.DirtyAll();
							giftBoxBtn.gameObject.SetActive(value: false);
							RouletteAudSrc.PlayOneShot(AudGiftBox);
							int probTotal = DataContainer.Instance.BonusboxTableRaw.ProbTotal;
							BonusboxData bonusboxData = DataContainer.Instance.BonusboxTableRaw.Dice(UnityEngine.Random.value);
							string rewardName = string.Empty;
							int rewardCount = 0;
							switch (bonusboxData.Type)
							{
							case "token":
							{
								rewardName = LeanLocalization.GetTranslationText(DataContainer.Instance.TokenTableRaw[bonusboxData.Pvalue].Name1loc);
								AttributeMapInt charOwnedTokens;
								string iD2;
								(charOwnedTokens = PlayerInfo.Instance.CharOwnedTokens)[iD2 = bonusboxData.ID] = charOwnedTokens[iD2] + 1;
								break;
							}
							case "jewel":
							{
								rewardName = LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[1]);
								rewardCount = int.Parse(bonusboxData.Pvalue);
								int num8 = PlayerInfo.Instance.Currency[CurrencyType.Gem];
								CurrencyTypeMapInt currency2;
								(currency2 = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency2[CurrencyType.Gem] + rewardCount;
								gemText.text = num8.ToString();
								break;
							}
							case "gold":
							{
								rewardName = LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[0]);
								rewardCount = int.Parse(bonusboxData.Pvalue);
								int num7 = PlayerInfo.Instance.Currency[CurrencyType.Gold];
								CurrencyTypeMapInt currency2;
								(currency2 = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency2[CurrencyType.Gold] + rewardCount;
								goldText.text = num7.ToString();
								break;
							}
							case "ticket":
							{
								rewardName = LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[2]);
								rewardCount = int.Parse(bonusboxData.Pvalue);
								int nameTagCount = PlayerInfo.Instance.NameTagCount;
								PlayerInfo.Instance.NameTagCount += rewardCount;
								nameTagText.text = $"{nameTagCount.ToString():D}";
								break;
							}
							}
							SetActivateFilter(activate: true);
							Image filterCommonPopupImage = FilterCommonPopup.GetComponent<Image>();
							Color orgColor = filterCommonPopupImage.color;
							filterCommonPopupImage.color = Color.clear;
							Toggle[] array3 = shopCurrencyToggles;
							foreach (Toggle toggle3 in array3)
							{
								toggle3.interactable = false;
							}
							upgradeBoxBtn.interactable = false;
							ParticleEffects.All(delegate(ParticleSystem s)
							{
								s.gameObject.SetActive(value: false);
								return true;
							});
							ThisGiftBoxDirector.gameObject.SetActive(value: true);
							ThisGiftBoxDirector.GiftModelName = bonusboxData.Modelname;
							Action action = delegate
							{
								bool isPopOKWait = true;
								bool isExcuted = false;
								ThisGiftBoxDirector.OnCompleteEtor = pTween.While(delegate
								{
									if (!isExcuted)
									{
										isExcuted = true;
										PlayerInfo.Instance.Currency[CurrencyType.Gem] = PlayerInfo.Instance.Currency[CurrencyType.Gem];
										PlayerInfo.Instance.Currency[CurrencyType.Gold] = PlayerInfo.Instance.Currency[CurrencyType.Gold];
										PlayerInfo.Instance.NameTagCount = PlayerInfo.Instance.NameTagCount;
										PlayerInfo.Instance.IsSenseBackBtn = true;
										string value3 = string.Format(LeanLocalization.GetTranslationText("156"), rewardName, (rewardCount != 0) ? rewardCount.ToString() : string.Empty);
										closeHandler = delegate
										{
											ClosePopupCommon();
											commonPopup.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
											filterCommonPopupImage.color = orgColor;
											isPopOKWait = false;
											LeanTween.delayedCall(0.5f, (Action)delegate
											{
												SetActivateFilter(activate: false);
												Toggle[] array4 = shopCurrencyToggles;
												foreach (Toggle toggle4 in array4)
												{
													toggle4.interactable = true;
												}
												upgradeBoxBtn.interactable = true;
											});
											ParticleEffects.All(delegate(ParticleSystem s)
											{
												s.gameObject.SetActive(value: true);
												return true;
											});
										};
										ShowPopupCommon(new Dictionary<string, object>
										{
											{
												"type",
												"Notify"
											},
											{
												"CloseHandler",
												closeHandler
											},
											{
												"msg",
												value3
											},
											{
												"yOffset",
												-200f
											}
										});
									}
									return isPopOKWait;
								});
							};
							action();
						});
					}
					else
					{
						closeHandler = delegate
						{
						};
						ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"Notify"
							},
							{
								"CloseHandler",
								closeHandler
							},
							{
								"msg",
								LeanLocalization.GetTranslationText("154")
							}
						});
					}
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				});
				upgradeBoxBtn = base.transform.Find("TopCenter/UpgradeGachaBtn").GetComponent<Button>();
				upgradeBoxBtnImageTextAnim = upgradeBoxBtn.transform.Find("TextImage").GetComponent<Animator>();
				upgradeBoxTimeLeftText = upgradeBoxBtn.transform.Find("Timer/Text").GetComponent<TextMeshProUGUI>();
				upgradeBoxPopupViewADsBtn = commonPopup.transform.Find("UpgradeSelect/ViewADsBtn").GetComponent<Button>();
				upgradeBoxPopupTimeLeftText = commonPopup.transform.Find("UpgradeSelect/Timer/Text").GetComponent<TextMeshProUGUI>();
				LeanTween.value(base.gameObject, (Action<float>)delegate
				{
				}, 0f, 1f, 1f).setOnCompleteOnRepeat(isOn: true).setOnComplete((Action)delegate
				{
					if (!PlayerInfo.Instance.TimeRelativeAttribute.ContainsKey("Upgrade"))
					{
						PlayerInfo.Instance.TimeRelativeAttribute["Upgrade"] = (DateTime.Now.Ticks - 4800000000L).ToString();
					}
					long ticks = DateTime.Now.Ticks;
					long num4 = long.Parse(PlayerInfo.Instance.TimeRelativeAttribute["Upgrade"]);
					long num5 = ticks - num4;
					if (4800000000L <= num5)
					{
						bool activeSelf = upgradeBoxTimeLeftText.transform.parent.gameObject.activeSelf;
						upgradeBoxPopupViewADsBtn.interactable = true;
						upgradeBoxTimeLeftText.transform.parent.gameObject.SetActive(value: false);
						upgradeBoxPopupTimeLeftText.transform.parent.gameObject.SetActive(value: false);
						if (activeSelf)
						{
							upgradeBoxBtnImageTextAnim.enabled = true;
							upgradeBoxBtnImageTextAnim.gameObject.SetActive(value: false);
							upgradeBoxBtnImageTextAnim.gameObject.SetActive(value: true);
						}
					}
					else
					{
						upgradeBoxPopupViewADsBtn.interactable = false;
						upgradeBoxTimeLeftText.transform.parent.gameObject.SetActive(value: true);
						int num6 = 480 - (int)(num5 / 10000000);
						DateTime dateTime = new DateTime(num5);
						upgradeBoxTimeLeftText.text = $"{num6 / 60:D2}:{num6 % 60:D2}";
						upgradeBoxPopupTimeLeftText.transform.parent.gameObject.SetActive(value: true);
						upgradeBoxPopupTimeLeftText.text = $"{num6 / 60:D2}:{num6 % 60:D2}";
					}
				})
					.setLoopClamp()
					.onComplete();
				Action commonHandler = delegate
				{
					PlayerInfo.Instance.IsSenseBackBtn = false;
					PlayerInfo.Instance.AccMissionByCondTypeID("dorandomlvlup", "-1", 1.ToString());
					GPGSManager.Instance.UnlockAchievement("CgkI9qTupKYJEAIQDQ");
					string chID = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx].ID;
					int num2 = PlayerInfo.Instance.DiceCHParamLvl(chID, UnityEngine.Random.value);
					if (0 <= num2)
					{
						PlayerParameterType playerParameterType = (PlayerParameterType)num2;
						int num3 = PlayerInfo.Instance.CharParamLevels[chID][(int)playerParameterType];
						PlayerInfo.Instance.AccMissionByCondTypeID("upgradeparam", DataContainer.Instance.PlayerParamTableRaw.dataArray[(int)playerParameterType].ID, 1.ToString());
						PlayerInfo.Instance.CharParamLevels[chID][(int)playerParameterType]++;
						PlayerInfo.Instance.CharParamLevels[chID] = PlayerInfo.Instance.CharParamLevels[chID];
						int requiregold = DataContainer.Instance.PlayerParamLevelTableRawByLevel[num2].PPLevelRaws[num3].Requiregold;
						upgradeBtns[num2].transform.Find("LevelText").GetComponent<Text>().text = $"{num3 + 1}";
						upgradeBtns[num2].transform.Find("CoinText").GetComponent<TextMeshProUGUI>().text = $"{requiregold}";
						SetActivateFilter(activate: true, bothPreview: false);
						Toggle[] array = shopCurrencyToggles;
						foreach (Toggle toggle in array)
						{
							toggle.interactable = false;
						}
						upgradeBoxBtn.interactable = false;
						StartCoroutine(cetUpgradeDirector(num2, delegate
						{
							PlayerInfo.Instance.CharParamLevels[chID] = PlayerInfo.Instance.CharParamLevels[chID];
							SetActivateFilter(activate: false, bothPreview: false);
							Toggle[] array2 = shopCurrencyToggles;
							foreach (Toggle toggle2 in array2)
							{
								toggle2.interactable = true;
							}
							upgradeBoxBtn.interactable = true;
							PlayerInfo.Instance.IsSenseBackBtn = true;
						}));
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					}
				};
				Action useGemHandler = delegate
				{
					if (2 > PlayerInfo.Instance.Currency[CurrencyType.Gem])
					{
						commonPopup.Find("CloseBtn").GetComponent<Button>().onClick.Invoke();
						showCurrencyPop(MenuCurrencyType.Gem, MenuCurrencyType.Gem, delegate
						{
						}, delegate
						{
						});
					}
					else
					{
						CurrencyTypeMapInt currency;
						(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] - 2;
						commonHandler();
					}
				};
				Action viewADsBtnHandler = delegate
				{
					if (!ADRewardManager.isLoadedAD())
					{
						Action value2 = delegate
						{
						};
						ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"Notify"
							},
							{
								"CloseHandler",
								value2
							},
							{
								"msg",
								LeanLocalization.GetTranslationText("154")
							}
						});
					}
					else
					{
						ADRewardManager.ShowRewardAD(delegate
						{
							GoogleAnalyticsV4.getInstance().LogScreen("UNITYAD_VIEW_COMPLETE_UPGRADE");
							PlayerInfo.Instance.TimeRelativeAttribute["Upgrade"] = DateTime.Now.Ticks.ToString();
							PlayerInfo.Instance.DirtyAll();
							upgradeBoxPopupViewADsBtn.interactable = false;
							upgradeBoxBtnImageTextAnim.gameObject.SetActive(value: false);
							upgradeBoxBtnImageTextAnim.gameObject.SetActive(value: true);
							LateUpdater.Instance.AddAction(delegate
							{
								upgradeBoxBtnImageTextAnim.enabled = false;
							});
							commonHandler();
						});
					}
				};
				upgradeBoxBtn.onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					string iD = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx].ID;
					if (!PlayerInfo.Instance.CharUnlocks[iD])
					{
						commonPopup.Find("CloseBtn").GetComponent<Button>().onClick.Invoke();
						Action value = delegate
						{
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
								LeanLocalization.GetTranslationText("157")
							}
						});
					}
					else
					{
						ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"BtnPop"
							},
							{
								"BtnPopType",
								"UpgradeSelect"
							},
							{
								"msg",
								string.Empty
							},
							{
								"yOffset",
								-66f
							},
							{
								"sizeDelta",
								new Vector2(480f, 435f)
							},
							{
								"UseGemBtnHandler",
								useGemHandler
							},
							{
								"ViewADsBtnHandler",
								viewADsBtnHandler
							}
						});
					}
				});
				commonPopup.Find("UpgradeSelect/UseGemBtn/Text").GetComponent<Text>().text = $"X {2:D2}";
			}

			private IEnumerator cetUpgradeDirectorScale(float duration, int btnIndex)
			{
				RouletteAudSrc.PlayOneShot(RouletteAud);
				GameObject activator = upgradeBtns[btnIndex].transform.Find("ActiveSelect").gameObject;
				activator.SetActive(value: true);
				float elapsed = 0f - Time.deltaTime;
				while (elapsed < duration)
				{
					elapsed += Time.deltaTime;
					upgradeBtns[btnIndex].transform.localScale = Vector3.one * (1f + 0.1f * (elapsed / duration));
					yield return 0;
				}
				upgradeBtns[btnIndex].transform.localScale = Vector3.one;
				activator.SetActive(value: false);
			}

			private IEnumerator cetUpgradeDirector(int chParamIntKey, Action onComplete)
			{
				yield return 0;
				string chID = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx].ID;
				string animPrefix3 = DataContainer.Instance.CharacterTableRaw[chID].Modelname;
				animPrefix3 = Regex.Replace(animPrefix3, "[0-9]+$", string.Empty);
				animPrefix3 = $"{animPrefix3}01";
				pvAnims[pvIndicIdx].CrossFade(animPrefix3 + "_running02");
				Color orgColor = Filter.color;
				Filter.color = Color.clear;
				int paramLength = Enum.GetValues(typeof(PlayerParameterType)).Length;
				for (int i = 0; i < paramLength; i++)
				{
					upgradeBtns[i].GetComponent<Animator>().enabled = false;
				}
				int totalLength = UnityEngine.Random.Range(paramLength * kNextSlotSetMin, paramLength * kNextSlotSetMax);
				int revTotalLength = totalLength + (chParamIntKey - totalLength % paramLength);
				float time = RouletteCurve.keys.Last().time;
				float elapsedSlot = 0f - Time.deltaTime;
				float[] slotFireTiming = new float[revTotalLength + 1];
				for (int j = 0; j < revTotalLength; j++)
				{
					slotFireTiming[j] = RouletteCurve.Evaluate((float)j / (float)revTotalLength) * kSlotTotalPeriod;
				}
				slotFireTiming[revTotalLength] = RouletteCurve.Evaluate(1f) * kSlotTotalPeriod;
				int curveIdx = 0;
				while (curveIdx < revTotalLength)
				{
					elapsedSlot += Time.deltaTime;
					if (elapsedSlot >= slotFireTiming[curveIdx])
					{
						int btnIndex = curveIdx % paramLength;
						float num = slotFireTiming[curveIdx + 1];
						float num2 = slotFireTiming[curveIdx];
						StartCoroutine(cetUpgradeDirectorScale(num - num2, btnIndex));
						curveIdx++;
					}
					yield return 0;
				}
				yield return new WaitForSeconds(1f);
				pvAnims[pvIndicIdx].Play(animPrefix3 + "_jump_hang3");
				pvAnims[pvIndicIdx].CrossFadeQueued(animPrefix3 + "_idling02", 0.03334f, QueueMode.CompleteOthers);
				upgradeBoxPart.All(delegate(GameObject s)
				{
					s.SetActive(value: true);
					return true;
				});
				RouletteAudSrc.PlayOneShot(RouletteLongAud);
				LeanTween.delayedCall(0.5f, (Action)delegate
				{
					RouletteAudSrc.PlayOneShot(AudLevelUp);
				});
				GameObject activator = upgradeBtns[chParamIntKey].transform.Find("ActiveSelect").gameObject;
				Image highlite = upgradeBtns[chParamIntKey].transform.Find("AlphaHighLight").GetComponent<Image>();
				Color orgHighliteColor = highlite.color;
				IEnumerator etorDecision = pTween.To(1f, delegate(float norm)
				{
					float num3 = Mathf.Pow(norm, 2.2f);
					upgradeBtns[chParamIntKey].transform.localScale = Vector3.one * (1f + 0.2f * num3);
					activator.SetActive(value: true);
					highlite.color = Color.Lerp(Color.clear, Color.white, num3);
				});
				while (etorDecision.MoveNext())
				{
					yield return 0;
				}
				upgradeBtns[chParamIntKey].transform.localScale = Vector3.one;
				highlite.color = orgHighliteColor;
				activator.SetActive(value: false);
				for (int k = 0; k < paramLength; k++)
				{
					upgradeBtns[k].GetComponent<Animator>().enabled = true;
				}
				Filter.color = orgColor;
				onComplete();
			}

			public void OnBtnClick_ShowUpgradeDesc()
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				upgradeBtns.All(delegate(Button btn)
				{
					btn.transform.Find("Description").gameObject.SetActive(value: true);
					return true;
				});
				EventSystem evtSys = EventSystem.current;
				evtSys.enabled = false;
				bool isBegan = false;
				StartCoroutine(pTween.While(delegate
				{
					if (!isBegan && 0 < UnityEngine.Input.touchCount && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
					{
						isBegan = true;
					}
					else if (isBegan && 0 < UnityEngine.Input.touchCount && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Ended)
					{
						return false;
					}
					return true;
				}, null, delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					evtSys.enabled = true;
					upgradeBtns.All(delegate(Button btn)
					{
						btn.transform.Find("Description").gameObject.SetActive(value: false);
						return true;
					});
				}));
			}

			private void createUpgrades()
			{
				RectTransform topRightParams = base.transform.Find("TopRight/Params").GetComponent<RectTransform>();
				upgradeDescriptionBtn = topRightParams.parent.Find("DescriptionBtn").GetComponent<Button>();
				upgradeBtns = Enumerable.Range(0, Enum.GetValues(typeof(PlayerParameterType)).Length).Select(delegate(int index)
				{
					Button btn = topRightParams.GetChild(index).GetComponent<Button>();
					btn.transform.Find("Description/DescText").GetComponent<Text>().text = DataContainer.Instance.PlayerParamTableRaw.dataArray[index].Desc1loc;
					btn.onClick.AddListener(delegate
					{
						string chID = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx].ID;
						int num = PlayerInfo.Instance.Currency[CurrencyType.Gold];
						int num2 = PlayerInfo.Instance.CharParamLevels[chID][index];
						int requiregold = DataContainer.Instance.PlayerParamLevelTableRawByLevel[index].PPLevelRaws[num2].Requiregold;
						if (requiregold > num)
						{
							showCurrencyPop(MenuCurrencyType.Gold);
						}
						else
						{
							int maxlevel = DataContainer.Instance.PlayerParamTableRaw.dataArray[index].Maxlevel;
							Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
							((Func<bool>)delegate
							{
								string modelname = DataContainer.Instance.CharacterTableRaw[chID].Modelname;
								modelname = Regex.Replace(modelname, "[0-9]+$", string.Empty);
								modelname = $"{modelname}01";
								pvAnims[pvIndicIdx].Play(modelname + "_jump_hang3");
								pvAnims[pvIndicIdx].CrossFadeQueued(modelname + "_idling02", 125f, QueueMode.CompleteOthers);
								upgradeBoxPart.All(delegate(GameObject s)
								{
									s.SetActive(value: false);
									s.SetActive(value: true);
									return true;
								});
								return true;
							})();
							PlayerInfo.Instance.AccMissionByCondTypeID("upgradeparam", DataContainer.Instance.PlayerParamTableRaw.dataArray[index].ID, 1.ToString());
							PlayerInfo.Instance.CharParamLevels[chID][index]++;
							PlayerInfo.Instance.CharParamLevels[chID] = PlayerInfo.Instance.CharParamLevels[chID];
							CurrencyTypeMapInt currency;
							(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] - requiregold;
							if (maxlevel - 1 <= num2)
							{
								btn.interactable = false;
							}
						}
					});
					return btn;
				}).ToArray();
			}

			public void OnDown_DragStart(BaseEventData evt)
			{
				isPVRotStarted = true;
				if (crtPVRotDragProc == null)
				{
					crtPVRotDragProc = StartCoroutine(crtDragProc());
				}
			}

			private IEnumerator crtDragProc()
			{
				WaitForEndOfFrame yieldOP = new WaitForEndOfFrame();
				while (true)
				{
					isPVRotStarted = (isPVRotStarted && 0 < UnityEngine.Input.touchCount);
					float num;
					if (isPVRotStarted)
					{
						Vector2 deltaPosition = UnityEngine.Input.GetTouch(0).deltaPosition;
						num = 0f - deltaPosition.x;
					}
					else
					{
						num = 0f;
					}
					float deltaPosX = num;
					Vector3 eulerAngles = pvAnims[pvIndicIdx].transform.localRotation.eulerAngles;
					float curOffset3 = eulerAngles.y;
					curOffset3 += PVRotSensi * deltaPosX;
					if (!isPVRotStarted)
					{
						curOffset3 = pMath.Repeat(curOffset3, 360f);
						curOffset3 = Mathf.Lerp(curOffset3, 180f, Time.smoothDeltaTime * PVRotRestoreDamp);
					}
					pvAnims[pvIndicIdx].transform.localRotation = Quaternion.Euler(0f, curOffset3, 0f);
					yield return yieldOP;
				}
			}

			private void createPreviewRotation()
			{
				EventTrigger eventTrigger = base.transform.Find("TopCenter/PreviewRotation").gameObject.AddComponent<EventTrigger>();
				EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
				triggerEvent.AddListener(OnDown_DragStart);
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				entry.callback = triggerEvent;
				EventTrigger.Entry item = entry;
				eventTrigger.triggers.Add(item);
			}

			private void createCurrency()
			{
				goldText = base.transform.Find("TopCenter_Deco/Coins/Gold/Text").GetComponent<Text>();
				gemText = base.transform.Find("TopCenter_Deco/Coins/Gems/Text").GetComponent<Text>();
				nameTagText = base.transform.Find("TopCenter_Deco/Coins/Tag/Text").GetComponent<Text>();
				nameTagTimeLeftText = base.transform.Find("TopCenter_Deco/Coins/Tag/Timer/Text").GetComponent<TextMeshProUGUI>();
			}

			private void createShop()
			{
				commonPopup = base.transform.parent.Find("Popup/Common");
				commonPopupRT = commonPopup.GetComponent<RectTransform>();
				commonPopupAnim = commonPopup.GetComponent<Animator>();
				Transform popupRoot = base.transform.parent.Find("Popup/Coins");
				shopCurrencyPopups = (from index in Enumerable.Range(0, Enum.GetValues(typeof(MenuCurrencyType)).Length)
					select popupRoot.GetChild(index)).ToArray();
				shopCurrencyPopups.Select(delegate(Transform popupTrans, int index)
				{
					popupTrans.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(delegate
					{
						shopCurrencyPopups[index].gameObject.SetActive(value: false);
						SetActivateFilter(activate: false);
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					});
					return popupTrans;
				}).All((Transform s) => true);
				string[] shopTypeName = new string[3]
				{
					"gold",
					"jewel",
					"ticket"
				};
				string[] shopTransName = new string[3]
				{
					"Gold",
					"Gem",
					"NameTag"
				};
				Transform coinsRoot = base.transform.Find("TopCenter_Deco/Coins");
				shopCurrencyToggles = Enumerable.Range(0, Enum.GetValues(typeof(MenuCurrencyType)).Length).Select(delegate(int index)
				{
					Toggle component6 = coinsRoot.GetChild(index).GetComponent<Toggle>();
					component6.onValueChanged.AddListener(delegate(bool isOn)
					{
						shopCurrencyPopups[index].gameObject.SetActive(isOn);
						if (isOn)
						{
							backBtnStackDepth = 1;
							if (index == 1)
							{
								int count2 = DataContainer.Instance.ShopIDByMenuCurrencyType[index].Count;
								int k;
								for (k = 0; k < count2; k++)
								{
									string name6 = $"Popup/Coins/Gem/Items/Item{k + 1}/CoinText";
									string name7 = $"Popup/Coins/Gem/Items/Item{k + 1}/IconCoinText";
									string name8 = $"Popup/Coins/Gem/Items/Item{k + 1}/CostIconText";
									string name9 = $"Popup/Coins/Gem/Items/Item{k + 1}/CurrencyCode";
									Text component7 = base.transform.parent.Find(name6).GetComponent<Text>();
									Text component8 = base.transform.parent.Find(name7).GetComponent<Text>();
									Text component9 = base.transform.parent.Find(name8).GetComponent<Text>();
									Text component10 = base.transform.parent.Find(name9).GetComponent<Text>();
									ShopInfoData s2 = (from s in (from s in DataContainer.Instance.ShopTableRaw.dataArray
											where s.Type == "jewel"
											select s).Select((ShopInfoData s, int _index) => new
										{
											s,
											_index
										})
										where s._index == k
										select s).First().s;
									component7.text = $"{s2.Reward.ToString()}  {LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[1])}";
									component8.text = s2.Reward.ToString();
									if (string.IsNullOrEmpty(s2.costString))
									{
										component9.text = s2.Cost.ToString();
										component10.text = "KRW";
									}
									else
									{
										component9.text = s2.costString;
										component10.text = s2.currencyCode;
									}
								}
							}
							else
							{
								int count3 = DataContainer.Instance.ShopIDByMenuCurrencyType[index].Count;
								int l;
								for (l = 0; l < count3; l++)
								{
									string name10 = $"Popup/Coins/{shopTransName[index]}/Items/Item{l + 1}/CoinText";
									Text component11 = base.transform.parent.Find(name10).GetComponent<Text>();
									ShopInfoData s3 = (from s in (from s in DataContainer.Instance.ShopTableRaw.dataArray
											where s.Type == shopTypeName[index]
											select s).Select((ShopInfoData s, int idx) => new
										{
											s,
											idx
										})
										where s.idx == l
										select s).First().s;
									component11.text = $"{s3.Reward.ToString()}  {LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[index])}";
								}
								if (index == 2)
								{
									Text component12 = base.transform.parent.Find("Popup/Coins/NameTag/NoItems/ViewAd/CoinText").GetComponent<Text>();
									Text component13 = base.transform.parent.Find("Popup/Coins/NameTag/NoItems/ViewAd/IconCoinText").GetComponent<Text>();
									component12.text = $"{1}  {LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[2])}";
									component13.text = 1.ToString();
								}
							}
							popReady.gameObject.SetActive(value: false);
							settingPopup.gameObject.SetActive(value: false);
							missionPopup.gameObject.SetActive(value: false);
							popInviteFriend.Close();
							popRequestFriend.Close();
							popMessageBox.Close();
							chPurchasePopup.Close();
							SetActivateFilter(activate: true);
							Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
						}
					});
					return component6;
				}).ToArray();
				int i;
				for (i = 0; i < Enum.GetValues(typeof(MenuCurrencyType)).Length; i++)
				{
					int count = DataContainer.Instance.ShopIDByMenuCurrencyType[i].Count;
					int j;
					for (j = 0; j < count; j++)
					{
						Action action = delegate
						{
							int typeIdx = i;
							int itemIdx = j;
							string name = $"Popup/Coins/{shopTransName[typeIdx]}/Items/Item{itemIdx + 1}/CoinText";
							string name2 = $"Popup/Coins/{shopTransName[typeIdx]}/Items/Item{itemIdx + 1}/IconCoinText";
							string name3 = $"Popup/Coins/{shopTransName[typeIdx]}/Items/Item{itemIdx + 1}/CostIconText";
							Text component = base.transform.parent.Find(name).GetComponent<Text>();
							Text component2 = base.transform.parent.Find(name2).GetComponent<Text>();
							Text component3 = base.transform.parent.Find(name3).GetComponent<Text>();
							ShopInfoData shopItem = (from s in (from s in DataContainer.Instance.ShopTableRaw.dataArray
									where s.Type == shopTypeName[typeIdx]
									select s).Select((ShopInfoData s, int index) => new
								{
									s,
									index
								})
								where s.index == itemIdx
								select s).First().s;
							if ("jewel".Equals(shopTypeName[typeIdx]))
							{
								component.text = $"{shopItem.Reward.ToString()}  {LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[1])}";
								component2.text = shopItem.Reward.ToString();
								if (string.IsNullOrEmpty(shopItem.costString))
								{
									component3.text = shopItem.Cost.ToString();
								}
								else
								{
									component3.text = shopItem.costString;
									string name4 = $"Popup/Coins/{shopTransName[typeIdx]}/Items/Item{itemIdx + 1}/CurrencyCode";
									Text component4 = base.transform.parent.Find(name4).GetComponent<Text>();
									component4.text = shopItem.currencyCode;
								}
							}
							else
							{
								component.text = $"{shopItem.Reward.ToString()}  {LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[typeIdx])}";
								component2.text = shopItem.Reward.ToString();
								component3.text = shopItem.Cost.ToString();
							}
							string name5 = $"Popup/Coins/{shopTransName[typeIdx]}/Items/Item{itemIdx + 1}/PurchaseBtn";
							Button component5 = base.transform.parent.Find(name5).GetComponent<Button>();
							component5.onClick.AddListener(delegate
							{
								if ("jewel".Equals(shopTypeName[typeIdx]))
								{
									MarketManager.BuyProduct(shopItem.ID, delegate(bool success)
									{
										string empty = string.Empty;
										if (success)
										{
											CurrencyTypeMapInt currency2;
											(currency2 = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency2[CurrencyType.Gem] + shopItem.Reward;
											empty = string.Format(LeanLocalization.GetTranslationText("71"), LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[1]));
											ShowPopupCommon(new Dictionary<string, object>
											{
												{
													"type",
													"Notify"
												},
												{
													"msg",
													empty
												}
											});
											RouletteAudSrc.PlayOneShot(AudBuy);
										}
										else
										{
											empty = LeanLocalization.GetTranslationText("158");
											ShowPopupCommon(new Dictionary<string, object>
											{
												{
													"type",
													"Notify"
												},
												{
													"msg",
													empty
												}
											});
										}
									});
								}
								else if (shopItem.Cost <= PlayerInfo.Instance.Currency[CurrencyType.Gem])
								{
									string value = string.Empty;
									if ("gold".Equals(shopTypeName[typeIdx]))
									{
										CurrencyTypeMapInt currency;
										(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] - shopItem.Cost;
										(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] + shopItem.Reward;
										value = string.Format(LeanLocalization.GetTranslationText("71"), LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[0]));
									}
									else if ("ticket".Equals(shopTypeName[typeIdx]))
									{
										CurrencyTypeMapInt currency;
										(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] - shopItem.Cost;
										PlayerInfo.Instance.NameTagCount += shopItem.Reward;
										value = string.Format(LeanLocalization.GetTranslationText("71"), LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[2]));
									}
									RouletteAudSrc.PlayOneShot(AudBuy);
									ShowPopupCommon(new Dictionary<string, object>
									{
										{
											"type",
											"Notify"
										},
										{
											"msg",
											value
										}
									});
								}
								else
								{
									showCurrencyPop(MenuCurrencyType.Gem);
								}
							});
						};
						action();
					}
				}
			}

			public void showCurrencyPop(MenuCurrencyType lackCType, Action closeHandler = null, Action okHandler = null)
			{
				switch (lackCType)
				{
				case MenuCurrencyType.Gold:
					showCurrencyPop(MenuCurrencyType.Gold, MenuCurrencyType.Gold, closeHandler, okHandler);
					break;
				case MenuCurrencyType.Nametag:
					showCurrencyPop(MenuCurrencyType.Nametag, MenuCurrencyType.Nametag, closeHandler, okHandler);
					break;
				case MenuCurrencyType.Gem:
					showCurrencyPop(MenuCurrencyType.Gem, MenuCurrencyType.Gem, closeHandler, okHandler);
					break;
				}
			}

			public void showCurrencyPop(MenuCurrencyType lackCType, MenuCurrencyType gotoCType, Action closeHandler = null, Action okHandler = null)
			{
				string popupMsg = string.Format(LeanLocalization.GetTranslationText("67"), LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[(int)lackCType]));
				((Func<bool>)delegate
				{
					Action innerOkHandler = okHandler;
					Action value = delegate
					{
						shopCurrencyToggles[(int)gotoCType].isOn = false;
						shopCurrencyToggles[(int)gotoCType].isOn = true;
						if (innerOkHandler != null)
						{
							innerOkHandler();
						}
					};
					ShowPopupCommon(new Dictionary<string, object>
					{
						{
							"type",
							"Select"
						},
						{
							"okHandler",
							value
						},
						{
							"msg",
							popupMsg
						},
						{
							"CloseHandler",
							closeHandler
						}
					});
					return true;
				})();
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
				backBtnStackDepth = 2;
				string key = attribute["type"] as string;
				if (popupDispatcher != null)
				{
					popupDispatcher[key](attribute);
				}
				else
				{
					popupDispatcher[key](attribute);
				}
			}

			public void ClosePopupCommon()
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				FilterCommonPopup.gameObject.SetActive(value: false);
				FilterCommonPopup.parent.SetSiblingIndex(Filter.transform.GetSiblingIndex() + 1);
				commonPopup.gameObject.SetActive(value: false);
				bool isActivePopMenus = false;
				isActivePopMenus |= popReady.gameObject.activeInHierarchy;
				isActivePopMenus |= settingPopup.gameObject.activeInHierarchy;
				isActivePopMenus |= missionPopup.gameObject.activeInHierarchy;
				isActivePopMenus |= popInviteFriend.gameObject.activeInHierarchy;
				isActivePopMenus |= popRequestFriend.gameObject.activeInHierarchy;
				isActivePopMenus |= popMessageBox.gameObject.activeInHierarchy;
				isActivePopMenus |= chPurchasePopup.gameObject.activeInHierarchy;
				Enumerable.Range(0, Enum.GetValues(typeof(MenuCurrencyType)).Length).All(delegate(int s)
				{
					isActivePopMenus |= shopCurrencyPopups[s].gameObject.activeInHierarchy;
					return true;
				});
				if (isActivePopMenus)
				{
					backBtnStackDepth = 1;
				}
				else
				{
					backBtnStackDepth = 0;
				}
			}

			private void createPopupCommon()
			{
				Action<Dictionary<string, object>> commonAction = delegate(Dictionary<string, object> attr)
				{
					string text2 = attr["msg"] as string;
					FilterCommonPopup.gameObject.SetActive(value: true);
					FilterCommonPopup.parent.SetSiblingIndex(FilterTop.transform.GetSiblingIndex());
					commonPopup.gameObject.SetActive(value: true);
					commonPopup.Find("MsgText").GetComponent<Text>().text = text2;
					RectTransform component7 = commonPopup.GetComponent<RectTransform>();
					Vector2 anchoredPosition = component7.anchoredPosition;
					if (attr.ContainsKey("yOffset"))
					{
						anchoredPosition.y = (float)attr["yOffset"];
					}
					else
					{
						anchoredPosition.y = 0f;
					}
					component7.anchoredPosition = anchoredPosition;
					if (crtRevPopupCommon != null)
					{
						StopCoroutine(crtRevPopupCommon);
					}
					crtRevPopupCommon = StartCoroutine(cetPopupCommon(anchoredPosition.y));
					Vector2 sizeDelta = component7.sizeDelta;
					if (attr.ContainsKey("sizeDelta"))
					{
						sizeDelta = (Vector2)attr["sizeDelta"];
					}
					else
					{
						sizeDelta.x = 432f;
						sizeDelta.y = 360f;
					}
					component7.sizeDelta = sizeDelta;
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
								updatePreviewAnim();
								closeBtn.onClick.RemoveListener(btnPopHideHandler);
							};
							closeBtn.onClick.AddListener(btnPopHideHandler);
							string text = attr["BtnPopType"] as string;
							if (text != null)
							{
								if (!(text == "CoinLackSelect"))
								{
									if (!(text == "UpgradeSelect"))
									{
										if (!(text == "LanguageSelect"))
										{
											if (!(text == "SeasonReward"))
											{
												if (!(text == "SelectableRewardAds"))
												{
													if (text == "MultiplayWaitCancel")
													{
														transBtnPopRoot = commonPopup.Find("MultiplayWaitCancel");
														transBtnPopRoot.gameObject.SetActive(value: true);
														Button component = transBtnPopRoot.Find("CancelBtn").GetComponent<Button>();
														component.transform.Find("MsgText").GetComponent<Text>().text = LeanLocalization.GetTranslationText("232");
														component.onClick.RemoveAllListeners();
														component.onClick.AddListener(delegate
														{
															closeBtn.onClick.Invoke();
															(attr["CancelBtnHandler"] as Action)();
														});
													}
												}
												else
												{
													transBtnPopRoot = commonPopup.Find("SelectableRewardAds");
													transBtnPopRoot.gameObject.SetActive(value: true);
													Button component2 = transBtnPopRoot.Find("Ads02Btn").GetComponent<Button>();
													component2.onClick.RemoveAllListeners();
													component2.onClick.AddListener(delegate
													{
														closeBtn.onClick.Invoke();
														(attr["Ads02BtnHandler"] as Action)();
													});
												}
											}
											else
											{
												transBtnPopRoot = commonPopup.Find("SeasonReward");
												transBtnPopRoot.gameObject.SetActive(value: true);
												Button component3 = transBtnPopRoot.Find("RewardBtn").GetComponent<Button>();
												component3.onClick.RemoveAllListeners();
												component3.onClick.AddListener(delegate
												{
													closeBtn.onClick.Invoke();
													(attr["RewardBtnHandler"] as Action)();
												});
											}
										}
										else
										{
											transBtnPopRoot = commonPopup.Find("LanguageSelect");
											transBtnPopRoot.gameObject.SetActive(value: true);
											int i;
											for (i = 0; i < transBtnPopRoot.childCount; i++)
											{
												((Func<bool>)delegate
												{
													int index = i;
													Toggle languageToggle = transBtnPopRoot.GetChild(index).GetComponent<Toggle>();
													UnityAction<bool> action = null;
													action = delegate
													{
														closeBtn.onClick.Invoke();
														languageToggle.onValueChanged.RemoveListener(action);
													};
													languageToggle.onValueChanged.AddListener(action);
													return true;
												})();
											}
										}
									}
									else
									{
										transBtnPopRoot = commonPopup.Find("UpgradeSelect");
										transBtnPopRoot.gameObject.SetActive(value: true);
										Button component4 = transBtnPopRoot.Find("UseGemBtn").GetComponent<Button>();
										component4.onClick.RemoveAllListeners();
										component4.onClick.AddListener(delegate
										{
											closeBtn.onClick.Invoke();
											(attr["UseGemBtnHandler"] as Action)();
										});
										Button component5 = transBtnPopRoot.Find("ViewADsBtn").GetComponent<Button>();
										component5.onClick.RemoveAllListeners();
										component5.onClick.AddListener(delegate
										{
											closeBtn.onClick.Invoke();
											(attr["ViewADsBtnHandler"] as Action)();
										});
									}
								}
								else
								{
									transBtnPopRoot = commonPopup.Find("CoinLackSelect");
									transBtnPopRoot.gameObject.SetActive(value: true);
									Button component6 = transBtnPopRoot.Find("ShopMoveBtn").GetComponent<Button>();
									component6.onClick.RemoveAllListeners();
									component6.onClick.AddListener(delegate
									{
										closeBtn.onClick.Invoke();
										(attr["ShopMoveBtnHandler"] as Action)();
									});
								}
							}
						}
					}
				};
			}

			public int GetNumberOfCells(EnhancedScroller scroller)
			{
				return cvMissionDatas.Count;
			}

			public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
			{
				if (cvMissionDatas[dataIndex] is CVMissionDataHeader)
				{
					return 70f;
				}
				if (cvMissionDatas[dataIndex] is CVMissionDataContentCHCoin)
				{
					return 210.5f;
				}
				return 104f;
			}

			public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
			{
				Vector3 localPosition = Vector3.zero;
				CVMission cVMission = null;
				if (cvMissionDatas[dataIndex] is CVMissionDataHeader)
				{
					cVMission = (scroller.GetCellView(CVHeaderPref) as CVMissionHeader);
					localPosition = cVMission.transform.localPosition;
					Vector3 localPosition2 = CVHeaderPref.transform.localPosition;
					localPosition.z = localPosition2.z;
				}
				else if (cvMissionDatas[dataIndex] is CVMissionDataContentCHCoin)
				{
					cVMission = (scroller.GetCellView(CVContentCHCoinsPref) as CVMissionContentCHCoin);
					localPosition = cVMission.transform.localPosition;
					Vector3 localPosition3 = CVContentCHCoinsPref.transform.localPosition;
					localPosition.z = localPosition3.z;
				}
				else
				{
					cVMission = (scroller.GetCellView(CVContentGenericPref) as CVMissionContentGeneric);
					localPosition = cVMission.transform.localPosition;
					Vector3 localPosition4 = CVContentGenericPref.transform.localPosition;
					localPosition.z = localPosition4.z;
				}
				cVMission.SetData(cvMissionDatas[dataIndex]);
				cVMission.transform.localPosition = localPosition;
				return cVMission;
			}

			private void dirtyMissionCVData()
			{
				if (!cvMissionDataDirtyAll)
				{
					cvMissionDataDirtyAll = true;
					LateUpdater.Instance.AddAction(delegate
					{
						MenuUIManager menuUIManager = this;
						updateMissionCVData();
						cvMissionDataDirtyAll = false;
						if (missionPopup.gameObject.activeInHierarchy)
						{
							try
							{
								MissionScroller.ScrollPosition = 0f;
							}
							catch (Exception)
							{
								LeanTween.delayedCall(0f, (Action)delegate
								{
									menuUIManager.MissionScroller.ScrollPosition = 0f;
								});
							}
							MissionScroller.ReloadData();
						}
						bool isAnyContainLimitedList = false;
						PlayerInfo.Instance.SortedMissionUI.All(delegate(List<string> s1)
						{
							s1.All(delegate(string s2)
							{
								if (PlayerInfo.Instance.MsnCompleted[s2])
								{
									isAnyContainLimitedList = true;
									return false;
								}
								return true;
							});
							return true;
						});
						base.transform.Find("BottomCenter/Mission_ETC/MissionBtn/New").gameObject.SetActive(isAnyContainLimitedList);
					});
				}
			}

			private void updateMissionCVData()
			{
				cvMissionDatas.Clear();
				for (int i = 0; Enum.GetValues(typeof(MenuMissionType)).Length > i; i++)
				{
					List<string> list = PlayerInfo.Instance.SortedMissionUI[i];
					MissionInfoData missionInfoData = DataContainer.Instance.MissionTableRaw[list[0]];
					cvMissionDatas.Add(new CVMissionDataHeader
					{
						DataKey = missionInfoData.ID,
						LocImageText = missionInfoData.Missionnameimagepath
					});
					cvMissionDatas.AddRange(list.Select(delegate(string mID, int dataIdx)
					{
						string goaltype = DataContainer.Instance.MissionTableRaw[mID].Goaltype;
						return (goaltype != null && goaltype == "chcoins") ? ((CVMissionData)new CVMissionDataContentCHCoin
						{
							Index = dataIdx,
							DataKey = mID
						}) : ((CVMissionData)new CVMissionDataContentGeneric
						{
							Index = dataIdx,
							DataKey = mID
						});
					}).ToList());
				}
				string dbgTemp = string.Empty;
				cvMissionDatas.All(delegate(CVMissionData s)
				{
					dbgTemp = $"{dbgTemp}\n{DataContainer.Instance.MissionTableRaw[s.DataKey].ID}";
					return true;
				});
			}

			private void OnValue_MsnGoalValues(string key, string oldValue, string value)
			{
				dirtyMissionCVData();
			}

			private void OnVale_MsnCompleted(string key, bool oldValue, bool value)
			{
				dirtyMissionCVData();
				if (value && !PlayerInfo.Instance.MsnRewarded[key])
				{
					bool isAnyContainLimitedList = false;
					PlayerInfo.Instance.SortedMissionUI.All(delegate(List<string> s1)
					{
						isAnyContainLimitedList |= (0 < (from s2 in s1
							where s2.Equals(key)
							select s2).Count());
						return true;
					});
					base.transform.Find("BottomCenter/Mission_ETC/MissionBtn/New").gameObject.SetActive(isAnyContainLimitedList);
				}
			}

			private void OnValue_MsnRewarded(string key, bool oldValue, bool value)
			{
				dirtyMissionCVData();
			}

			private void createMission()
			{
				PlayerInfo.Instance.CheckMissionDailyTick();
				MissionScroller.Delegate = this;
				missionPopup = base.transform.parent.Find("Popup/Mission");
				base.transform.Find("BottomCenter/Mission_ETC/MissionBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					backBtnStackDepth = 1;
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					SetActivateFilter(activate: true);
					missionPopup.gameObject.SetActive(value: true);
					dirtyMissionCVData();
				});
				missionPopup.Find("CloseBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					SetActivateFilter(activate: false);
					missionPopup.gameObject.SetActive(value: false);
				});
				PlayerInfo.Instance.MsnGoalValues.OnValue += OnValue_MsnGoalValues;
				PlayerInfo.Instance.MsnCompleted.OnValue += OnVale_MsnCompleted;
				PlayerInfo.Instance.MsnRewarded.OnValue += OnValue_MsnRewarded;
				DataContainer.Instance.MissionTableRaw.dataArray.All(delegate(MissionInfoData s)
				{
					PlayerInfo.Instance.MsnCompleted[s.ID] = PlayerInfo.Instance.MsnCompleted[s.ID];
					return true;
				});
				LeanTween.delayedCall(0f, (Action)delegate
				{
					missionPopup.gameObject.SetActive(value: false);
				});
			}

			private void createGoogleRanking()
			{
				base.transform.Find("BottomCenter/Mission_ETC/RankingBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					if (GPGSManager.Instance.Authenticated)
					{
						GPGSManager.Instance.ShowLeaderboardUI();
					}
					else
					{
						GPGSManager.Instance.Authenticate(delegate(bool success)
						{
							if (success)
							{
								GoogleAnalyticsV4.getInstance().LogScreen("ShowLeaderboardUI");
								GPGSManager.Instance.ShowLeaderboardUI();
							}
						});
					}
				});
				base.transform.Find("BottomCenter/Mission_ETC/ArchivementBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					if (GPGSManager.Instance.Authenticated)
					{
						GPGSManager.Instance.ShowAchievementsUI();
					}
					else
					{
						GPGSManager.Instance.Authenticate(delegate(bool success)
						{
							if (success)
							{
								GoogleAnalyticsV4.getInstance().LogScreen("ShowAchievementsUI");
								GPGSManager.Instance.ShowAchievementsUI();
							}
						});
					}
				});
			}

			public void onBtnClick_OpenSettingPopup()
			{
				backBtnStackDepth = 1;
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				SetActivateFilter(activate: true);
				settingPopup.gameObject.SetActive(value: true);
				settingPopup.Find("Elements/Language/Btn/Image").GetComponent<Image>().sprite = LocaleSprites[PlayerInfo.Instance.LocaleIndex];
				settingGPSignToggle.isOn = !GPGSManager.Instance.Authenticated;
				settingFBSignToggle.isOn = !FBManager.Instance.IsReady;
				settingGPSignToggle.isOn = GPGSManager.Instance.Authenticated;
				settingFBSignToggle.isOn = FBManager.Instance.IsReady;
			}

			private void createSettingPopup()
			{
				settingPopup = base.transform.parent.Find("Popup/Setting");
				Button settingPopupCloseBtn = base.transform.parent.Find("Popup/Setting/CloseBtn").GetComponent<Button>();
				settingPopupCloseBtn.onClick.AddListener(delegate
				{
					if (EventSystem.current.currentSelectedGameObject == settingPopupCloseBtn.gameObject)
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					}
					SetActivateFilter(activate: false);
					settingPopup.gameObject.SetActive(value: false);
				});
				settingPopup.Find("Text").GetComponent<Text>().text = $"V{PlayerInfo.Instance.AppVersion}";
				settingLanguageBtn = settingPopup.Find("Elements/Language/Btn").GetComponent<Button>();
				settingLanguageBtn.onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					ShowPopupCommon(new Dictionary<string, object>
					{
						{
							"type",
							"BtnPop"
						},
						{
							"BtnPopType",
							"LanguageSelect"
						},
						{
							"msg",
							string.Empty
						},
						{
							"sizeDelta",
							new Vector2(480f, 300f)
						}
					});
				});
				settingSoundToggle = settingPopup.Find("Elements/SFX/Btn").GetComponent<Toggle>();
				settingSoundToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					if (EventSystem.current.currentSelectedGameObject == settingSoundToggle.gameObject)
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					}
					PlayerInfo.Instance.SoundOn = isOn;
					PlayerInfo.Instance.DirtyAll();
					if (PlayerInfo.Instance.SoundOn)
					{
						Camera.main.GetComponent<AudioSource>().volume = 1f;
					}
					else
					{
						Camera.main.GetComponent<AudioSource>().volume = 0f;
					}
				});
				settingSoundToggle.isOn = PlayerInfo.Instance.SoundOn;
				settingMusicToggle = settingPopup.Find("Elements/BGM/Btn").GetComponent<Toggle>();
				settingMusicToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					if (EventSystem.current.currentSelectedGameObject == settingMusicToggle.gameObject)
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					}
					PlayerInfo.Instance.MusicOn = isOn;
					PlayerInfo.Instance.DirtyAll();
					if (PlayerInfo.Instance.MusicOn)
					{
						GameObject.Find("PreviewCamera").GetComponent<AudioSource>().volume = 1f;
					}
					else
					{
						GameObject.Find("PreviewCamera").GetComponent<AudioSource>().volume = 0f;
					}
				});
				settingMusicToggle.isOn = PlayerInfo.Instance.MusicOn;
				settingGPSignToggle = settingPopup.Find("LogoutElements/GooglePlayBtn").GetComponent<Toggle>();
				settingGPSignToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					bool flag2 = EventSystem.current.currentSelectedGameObject == settingGPSignToggle.gameObject;
					if (flag2)
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					}
					if (isOn)
					{
						settingPopup.Find("LogoutElements/GooglePlayBtn/Text").GetComponent<LeanLocalizedText>().SetPhraseName("102");
						if (flag2)
						{
							GPGSManager.Instance.Authenticate(delegate(bool success)
							{
								if (!success)
								{
									EventSystem.current.SetSelectedGameObject(null);
									settingGPSignToggle.isOn = !isOn;
								}
							});
						}
					}
					else
					{
						settingPopup.Find("LogoutElements/GooglePlayBtn/Text").GetComponent<LeanLocalizedText>().SetPhraseName("101");
						if (flag2)
						{
							((Func<bool>)delegate
							{
								bool isOK2 = false;
								Action value3 = delegate
								{
									isOK2 = true;
								};
								Action value4 = delegate
								{
									if (isOK2)
									{
										GPGSManager.Instance.SignOut();
									}
									else
									{
										EventSystem.current.SetSelectedGameObject(null);
										settingGPSignToggle.isOn = !isOn;
									}
								};
								ShowPopupCommon(new Dictionary<string, object>
								{
									{
										"type",
										"Select"
									},
									{
										"okHandler",
										value3
									},
									{
										"CloseHandler",
										value4
									},
									{
										"msg",
										LeanLocalization.GetTranslationText("164")
									}
								});
								return true;
							})();
						}
					}
				});
				settingFBSignToggle = settingPopup.Find("LogoutElements/FacebookBtn").GetComponent<Toggle>();
				settingFBSignToggle.onValueChanged.AddListener(delegate(bool isOn)
				{
					bool flag = EventSystem.current.currentSelectedGameObject == settingFBSignToggle.gameObject;
					if (flag)
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					}
					if (isOn)
					{
						if (flag)
						{
							Instance.ActivateLoadingScreenFilter(isActivate: true);
							FBManager.Instance.Login();
						}
						settingPopup.Find("LogoutElements/FacebookBtn/Text").GetComponent<LeanLocalizedText>().SetPhraseName("102");
					}
					else
					{
						if (flag)
						{
							((Func<bool>)delegate
							{
								bool isOK = false;
								Action value = delegate
								{
									isOK = true;
								};
								Action value2 = delegate
								{
									if (isOK)
									{
										FBManager.Instance.Logout();
										inviteFriendBtn.gameObject.SetActive(value: false);
										facebookLoginBtn.gameObject.SetActive(value: true);
										requestFriendBtn.transform.parent.gameObject.SetActive(value: false);
										requestFriendLoginBtn.transform.parent.gameObject.SetActive(value: true);
										messageBoxBtn.gameObject.SetActive(value: false);
									}
									else
									{
										EventSystem.current.SetSelectedGameObject(null);
										settingFBSignToggle.isOn = !isOn;
									}
								};
								ShowPopupCommon(new Dictionary<string, object>
								{
									{
										"type",
										"Select"
									},
									{
										"okHandler",
										value
									},
									{
										"CloseHandler",
										value2
									},
									{
										"msg",
										LeanLocalization.GetTranslationText("164")
									}
								});
								return true;
							})();
						}
						settingPopup.Find("LogoutElements/FacebookBtn/Text").GetComponent<LeanLocalizedText>().SetPhraseName("101");
						settingPopup.Find("LogoutElements/FacebookBtn/BonusIcon").gameObject.SetActive(!PlayerInfo.Instance.FBFirstLoginReward);
					}
				});
				Transform languageSelectRoot = commonPopup.Find("LanguageSelect");
				int i;
				for (i = 0; i < languageSelectRoot.childCount; i++)
				{
					((Func<bool>)delegate
					{
						int btnIdx = i;
						Toggle component = languageSelectRoot.GetChild(btnIdx).GetComponent<Toggle>();
						component.onValueChanged.RemoveAllListeners();
						component.onValueChanged.AddListener(delegate(bool isOn)
						{
							if (isOn)
							{
								PlayerInfo.Instance.LocaleIndex = btnIdx;
								LeanLocalization.Instance.SetLanguage(DataContainer.LocaleIdentifier[PlayerInfo.Instance.LocaleIndex]);
								settingPopup.Find("Elements/Language/Btn/Image").GetComponent<Image>().sprite = LocaleSprites[PlayerInfo.Instance.LocaleIndex];
							}
						});
						return true;
					})();
				}
			}

			private int findUnlockChIdx(bool positive)
			{
				int num = positive ? 1 : (-1);
				int result = pvIndicIdx;
				for (int i = 0; DataContainer.Instance.CharacterTableRaw.dataArray.Length > i; i++)
				{
					int num2 = (pvIndicIdx + (i + 1) * num + DataContainer.Instance.CharacterTableRaw.dataArray.Length * 2) % DataContainer.Instance.CharacterTableRaw.dataArray.Length;
					if (PlayerInfo.Instance.CharUnlocks[DataContainer.Instance.CharacterTableRaw.dataArray[num2].ID])
					{
						result = num2;
						break;
					}
				}
				return result;
			}

			public void OnBtnClick_PreviewCharPrev()
			{
				int newIdx = findUnlockChIdx(positive: false);
				if (newIdx != pvIndicIdx)
				{
					pvPrevBtn.interactable = false;
					pvNextBtn.interactable = false;
					updateCharacterSelectRelatedByUnlock(forceDisable: true);
					chPurchasePopupBtn.interactable = false;
					for (int i = 0; pvAnims.Length > i; i++)
					{
						float from = (float)i - pvPoolingOffset;
						float to = (float)i - (float)newIdx;
						doPreviewRotAnimLTProc(i, from, to, pvEasePeriod);
					}
					LeanTween.delayedCall(pvEasePeriod, (Action)delegate
					{
						pvPoolingOffset = newIdx;
						pvIndicIdx = newIdx;
						for (int j = 0; pvAnims.Length > j; j++)
						{
							updatePreviewPos(j, Mathf.FloorToInt(pMath.Repeat((float)j - pvPoolingOffset, DataContainer.Instance.CharacterTableRaw.dataArray.Length)));
						}
						pvPrevBtn.interactable = true;
						pvNextBtn.interactable = true;
						updateCharacterSelectRelatedByUnlock(forceDisable: false);
						chPurchasePopupBtn.interactable = true;
						updatePreviewAnim();
						updateChSkillDesc();
					});
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				}
			}

			public void OnBtnClick_PreviewCharNext()
			{
				int newIdx = findUnlockChIdx(positive: true);
				if (newIdx != pvIndicIdx)
				{
					pvPrevBtn.interactable = false;
					pvNextBtn.interactable = false;
					updateCharacterSelectRelatedByUnlock(forceDisable: true);
					chPurchasePopupBtn.interactable = false;
					for (int i = 0; pvAnims.Length > i; i++)
					{
						float from = (float)i - pvPoolingOffset;
						float to = (float)i - (float)newIdx;
						doPreviewRotAnimLTProc(i, from, to, pvEasePeriod);
					}
					LeanTween.delayedCall(pvEasePeriod, (Action)delegate
					{
						pvPoolingOffset = newIdx;
						pvIndicIdx = newIdx;
						for (int j = 0; pvAnims.Length > j; j++)
						{
							updatePreviewPos(j, Mathf.FloorToInt(pMath.Repeat((float)j - pvPoolingOffset, DataContainer.Instance.CharacterTableRaw.dataArray.Length)));
						}
						pvPrevBtn.interactable = true;
						pvNextBtn.interactable = true;
						updateCharacterSelectRelatedByUnlock(forceDisable: false);
						chPurchasePopupBtn.interactable = true;
						updatePreviewAnim();
						updateChSkillDesc();
					});
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				}
			}

			public void ForcePreviewCharIndex(int index)
			{
				index = Mathf.FloorToInt(Mathf.Repeat(index, DataContainer.Instance.CharacterTableRaw.dataArray.Length));
				pvPoolingOffset = index;
				pvIndicIdx = Mathf.FloorToInt(pMath.Repeat(pvPoolingOffset, DataContainer.Instance.CharacterTableRaw.dataArray.Length));
				for (int i = 0; pvAnims.Length > i; i++)
				{
					updatePreviewPos(i, Mathf.FloorToInt(pMath.Repeat((float)i - pvPoolingOffset, DataContainer.Instance.CharacterTableRaw.dataArray.Length)));
				}
				pvPrevBtn.interactable = true;
				pvNextBtn.interactable = true;
				updateCharacterSelectRelatedByUnlock(forceDisable: false);
				updatePreviewAnim();
			}

			private void updatePreviewAnim()
			{
				string iD = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx].ID;
				string modelname = DataContainer.Instance.CharacterTableRaw[iD].Modelname;
				modelname = Regex.Replace(modelname, "[0-9]+$", string.Empty);
				modelname = $"{modelname}01";
				int max = 1;
				for (int i = 2; 7 > i; i++)
				{
					string name = modelname + "_idling0" + i.ToString();
					if (null == pvAnims[pvIndicIdx].GetClip(name))
					{
						max = i;
						break;
					}
				}
				pvAnims[pvIndicIdx].CrossFade(modelname + "_idling0" + UnityEngine.Random.Range(2, max));
				Renderer[] componentsInChildren = pvAnims[pvIndicIdx].GetComponentsInChildren<Renderer>(includeInactive: true);
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].gameObject.SetActive(value: false);
				}
				pvAnims[pvIndicIdx].transform.Find(DataContainer.Instance.CharacterTableRaw[iD].Modelname).gameObject.SetActive(value: true);
				updatePlayerParams(iD);
				updateCharacterSelectRelatedByUnlock(forceDisable: false);
			}

			private void updatePlayerParams(string chID)
			{
				PlayerInfo.Instance.CharParamLevels[chID] = PlayerInfo.Instance.CharParamLevels[chID];
			}

			private void doPreviewRotAnimLTProc(int index, float from, float to, float period)
			{
				LeanTween.value(base.gameObject, delegate(float norm)
				{
					float offset = Mathf.Lerp(from, to, norm);
					updatePreviewPos(index, offset);
				}, 0f, 1f, period).setEase(LeanTweenType.easeOutExpo).setOnComplete((Action)delegate
				{
				});
			}

			private void updatePreviewPos(int index, float offset)
			{
				pvAnims[index].transform.localPosition = getPreviewPos(offset);
			}

			private Vector3 getPreviewPos(float offset)
			{
				pvPos.x = Mathf.Sin((float)Math.PI * 2f * (offset / (float)DataContainer.Instance.CharacterTableRaw.dataArray.Length)) * pvDist;
				pvPos.y = 0f;
				pvPos.z = Mathf.Cos((float)Math.PI * 2f * (offset / (float)DataContainer.Instance.CharacterTableRaw.dataArray.Length)) * pvDist;
				return pvPos;
			}

			private void createPreview()
			{
				pvPoolingOffset = (pvIndicIdx = (from s in DataContainer.Instance.CharacterTableRaw.dataArray.Select((CharacterInfoData s, int i) => new
					{
						s,
						i
					})
					where s.s.ID == PlayerInfo.Instance.SelectedCharID
					select s.i).First());
				int length = DataContainer.Instance.CharacterTableRaw.dataArray.Length;
				pvAnims = Enumerable.Range(0, length).Select(delegate(int index)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(PreviewCharacterPref);
					gameObject.transform.parent = PreviewScrollRoot;
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
					gameObject.transform.localPosition = getPreviewPos(Mathf.CeilToInt(pMath.Repeat(pvIndicIdx - index, length)));
					Transform transform = gameObject.transform.Find(DataContainer.Instance.CharacterTableRaw.dataArray[index].Modelname);
					transform.gameObject.SetActive(value: true);
					return gameObject.GetComponent<Animation>();
				}).ToArray();
				pvPrevBtn = base.transform.Find("TopCenter/PrevNextBtn/Prev").GetComponent<Button>();
				pvNextBtn = base.transform.Find("TopCenter/PrevNextBtn/Next").GetComponent<Button>();
			}

			private void showPopupRewardSelectableAds(int quantity)
			{
				ClosePopupCommon();
				LeanTween.delayedCall(0f, (Action)delegate
				{
					Action value = delegate
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					};
					string value2 = string.Format(LeanLocalization.GetTranslationText("156"), LeanLocalization.GetTranslationText(DataContainer.Instance.NameByCurrency[1]), quantity.ToString());
					ShowPopupCommon(new Dictionary<string, object>
					{
						{
							"type",
							"Notify"
						},
						{
							"msg",
							value2
						},
						{
							"CloseHandler",
							value
						}
					});
				});
			}

			public void RewardSelectableAds(HashSet<string> adTypes, bool needShowPopup)
			{
				((Func<bool>)delegate
				{
					int handlerCount = (adTypes.Contains("tapjoy") ? 1 : 0) + (adTypes.Contains("nas") ? 1 : 0);
					int totalQuantity = 0;
					if (adTypes.Contains("tapjoy"))
					{
						Wenee.AdManager.Instance.GetTapjoyCurrencyBalance(delegate(bool currencySuccess, int quantity)
						{
							if (currencySuccess && 0 < quantity)
							{
								Wenee.AdManager.Instance.SpendTapjoyCurrency(quantity, delegate(bool spendSuccess)
								{
									if (spendSuccess)
									{
										handlerCount--;
										CurrencyTypeMapInt currency;
										(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] + quantity;
										totalQuantity += quantity;
										if (needShowPopup && 0 >= handlerCount)
										{
											showPopupRewardSelectableAds(totalQuantity);
										}
									}
									else
									{
										handlerCount--;
									}
								});
							}
							else
							{
								handlerCount--;
							}
						});
					}
					return true;
				})();
			}

			private void createSelectableRewardAds()
			{
				Func<bool> selectableRewardAdsHandler = delegate
				{
					Action value = delegate
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
						loadingIndicatorGO.SetActive(value: true);
						Wenee.AdManager.Instance.ShowTapjoyOfferwall(delegate(bool isSuccess)
						{
							loadingIndicatorGO.SetActive(value: false);
							if (isSuccess)
							{
								LeanTween.delayedCall(1f, (Action)delegate
								{
									RewardSelectableAds(new HashSet<string>
									{
										"tapjoy"
									}, needShowPopup: true);
								});
							}
						});
					};
					ShowPopupCommon(new Dictionary<string, object>
					{
						{
							"type",
							"BtnPop"
						},
						{
							"BtnPopType",
							"SelectableRewardAds"
						},
						{
							"msg",
							string.Empty
						},
						{
							"yOffset",
							-32f
						},
						{
							"sizeDelta",
							new Vector2(539f, 488f)
						},
						{
							"Ads02BtnHandler",
							value
						}
					});
					return true;
				};
				base.transform.Find("BottomCenter/SelectableRewardAdsBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					selectableRewardAdsHandler();
				});
				base.transform.parent.Find("Popup/Coins/Gem/NoItems/Item1/PurchaseBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					selectableRewardAdsHandler();
				});
			}

			public void OpenInviteFriendPopup()
			{
				popInviteFriend.Open();
			}

			public void YieldCheckFirstLoginBtns()
			{
				if (FBManager.Instance.IsReady)
				{
					inviteFriendBtn.gameObject.SetActive(value: true);
					facebookLoginBtn.gameObject.SetActive(value: false);
					requestFriendBtn.transform.parent.gameObject.SetActive(value: true);
					requestFriendLoginBtn.transform.parent.gameObject.SetActive(value: false);
					messageBoxBtn.gameObject.SetActive(value: true);
					NeedCheckMessageBoxNew();
				}
				else
				{
					inviteFriendBtn.gameObject.SetActive(value: false);
					facebookLoginBtn.gameObject.SetActive(value: true);
					requestFriendBtn.transform.parent.gameObject.SetActive(value: false);
					requestFriendLoginBtn.transform.parent.gameObject.SetActive(value: true);
					messageBoxBtn.gameObject.SetActive(value: false);
				}
				facebookLoginBtn.transform.Find("BonusIcon").gameObject.SetActive(!PlayerInfo.Instance.FBFirstLoginReward);
				requestFriendLoginBtn.transform.Find("BonusIcon").gameObject.SetActive(!PlayerInfo.Instance.FBFirstLoginReward);
			}

			public void NeedCheckMessageBoxNew()
			{
				if (0 < FBManager.Instance.MessageBoxItems.Count)
				{
					messageBoxBtn.transform.Find("New").gameObject.SetActive(value: true);
				}
				else
				{
					messageBoxBtn.transform.Find("New").gameObject.SetActive(value: false);
				}
			}

			private void createInviteFriendsPopup()
			{
				inviteFriendBtn = base.transform.Find("BottomCenter/Mission_ETC/InviteFriend").GetComponent<Button>();
				inviteFriendBtn.onClick.RemoveAllListeners();
				inviteFriendBtn.onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					popInviteFriend.Open();
				});
				facebookLoginBtn = base.transform.Find("BottomCenter/Mission_ETC/LoginFacebook").GetComponent<Button>();
				facebookLoginBtn.transform.Find("BonusIcon").gameObject.SetActive(!PlayerInfo.Instance.FBFirstLoginReward);
				facebookLoginBtn.onClick.RemoveAllListeners();
				facebookLoginBtn.onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					FBManager.Instance.Login();
					ActivateLoadingScreenFilter(isActivate: true);
				});
				popInviteFriend = base.transform.parent.Find("Popup/InviteFriend").GetComponent<InviteFriendsPopup>();
				if (FBManager.Instance.IsReady)
				{
					inviteFriendBtn.gameObject.SetActive(value: true);
					facebookLoginBtn.gameObject.SetActive(value: false);
				}
				else
				{
					inviteFriendBtn.gameObject.SetActive(value: false);
					facebookLoginBtn.gameObject.SetActive(value: true);
				}
				requestFriendBtn = base.transform.parent.Find("Popup/Coins/NameTag/NoItems/RequestFriend/Login/RequestBtn").GetComponent<Button>();
				requestFriendBtn.onClick.RemoveAllListeners();
				requestFriendBtn.onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					shopCurrencyPopups[2].gameObject.SetActive(value: false);
					popRequestFriend.Open();
				});
				requestFriendLoginBtn = base.transform.parent.Find("Popup/Coins/NameTag/NoItems/RequestFriend/Logout/FacebookBtn").GetComponent<Button>();
				requestFriendLoginBtn.transform.Find("BonusIcon").gameObject.SetActive(!PlayerInfo.Instance.FBFirstLoginReward);
				requestFriendLoginBtn.onClick.RemoveAllListeners();
				requestFriendLoginBtn.onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					FBManager.Instance.Login();
					ActivateLoadingScreenFilter(isActivate: true);
				});
				popRequestFriend = base.transform.parent.Find("Popup/RequestFriend").GetComponent<RequestFriendsPopup>();
				if (FBManager.Instance.IsReady)
				{
					requestFriendBtn.transform.parent.gameObject.SetActive(value: true);
					requestFriendLoginBtn.transform.parent.gameObject.SetActive(value: false);
				}
				else
				{
					requestFriendBtn.transform.parent.gameObject.SetActive(value: false);
					requestFriendLoginBtn.transform.parent.gameObject.SetActive(value: true);
				}
				messageBoxBtn = base.transform.Find("TopCenter_Deco/MessageBox").GetComponent<Button>();
				messageBoxBtn.onClick.RemoveAllListeners();
				messageBoxBtn.onClick.AddListener(delegate
				{
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
					for (int i = 0; shopCurrencyPopups.Length > i; i++)
					{
						shopCurrencyPopups[i].gameObject.SetActive(value: false);
					}
					popReady.gameObject.SetActive(value: false);
					settingPopup.gameObject.SetActive(value: false);
					missionPopup.gameObject.SetActive(value: false);
					popInviteFriend.Close();
					popRequestFriend.Close();
					popMessageBox.Open();
				});
				popMessageBox = base.transform.parent.Find("Popup/MessageBox").GetComponent<MessageBoxPopup>();
				if (FBManager.Instance.IsReady)
				{
					messageBoxBtn.gameObject.SetActive(value: true);
					NeedCheckMessageBoxNew();
				}
				else
				{
					messageBoxBtn.gameObject.SetActive(value: false);
				}
				loadingIndicatorGO = base.transform.Find("UILoadingIndicator").gameObject;
			}

			public void OnBtnClick_RaceReady()
			{
				if (!PlayerInfo.Instance.TutorialCompleted)
				{
					Action value = delegate
					{
						Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
						GoogleAnalyticsV4.getInstance().LogEvent("Tutorial", "Begin", "Tutorial Start", 1L);
						PlayerInfo.Instance.DirtyAll();
						setupHighMeters();
						StartCoroutine(LoadGame());
					};
					ShowPopupCommon(new Dictionary<string, object>
					{
						{
							"type",
							"Notify"
						},
						{
							"msg",
							LeanLocalization.GetTranslationText("170")
						},
						{
							"CloseHandler",
							value
						}
					});
					return;
				}
				GameType thisGameType = GameType.NormalSingle;
				if (PlayerInfo.Instance.IsRetryGame)
				{
					PlayerInfo.Instance.IsRetryGame = false;
					thisGameType = PlayerInfo.Instance.ThisGameType;
				}
				else
				{
					switch (EventSystem.current.currentSelectedGameObject.name)
					{
					case "StartBtn":
						thisGameType = GameType.NormalSingle;
						break;
					case "StartBtnMission":
						thisGameType = GameType.MissionSingle;
						break;
					case "StartBtnMulti":
						thisGameType = GameType.Multi;
						break;
					}
				}
				PlayerInfo.Instance.ThisGameType = thisGameType;
				if (PlayerInfo.Instance.ThisGameType == GameType.Multi)
				{
					popReadyMultiRaceStartBtn.gameObject.SetActive(value: false);
					Vector2 anchoredPosition = popReadyRaceStartBtn.GetComponent<RectTransform>().anchoredPosition;
					anchoredPosition.x = 0f;
					popReadyRaceStartBtn.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
				}
				else
				{
					popReadyMultiRaceStartBtn.gameObject.SetActive(value: false);
					Vector2 anchoredPosition2 = popReadyRaceStartBtn.GetComponent<RectTransform>().anchoredPosition;
					anchoredPosition2.x = 0f;
					popReadyRaceStartBtn.GetComponent<RectTransform>().anchoredPosition = anchoredPosition2;
				}
				SetActivateFilter(activate: true);
				popReady.gameObject.SetActive(value: true);
				backBtnStackDepth = 1;
				popReadyStartItems.All(delegate(Toggle toggle)
				{
					toggle.isOn = false;
					return true;
				});
				updatePopReady();
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
			}

			public void OnBtnClick_RaceStart()
			{
				if (0 >= PlayerInfo.Instance.NameTagCount)
				{
					Action value = delegate
					{
						shopCurrencyToggles[2].isOn = false;
						shopCurrencyToggles[2].isOn = true;
					};
					ShowPopupCommon(new Dictionary<string, object>
					{
						{
							"type",
							"BtnPop"
						},
						{
							"BtnPopType",
							"CoinLackSelect"
						},
						{
							"msg",
							string.Empty
						},
						{
							"sizeDelta",
							new Vector2(480f, 340f)
						},
						{
							"ShopMoveBtnHandler",
							value
						}
					});
				}
				else if (popReadyStartReqGold > PlayerInfo.Instance.Currency[CurrencyType.Gold])
				{
					showCurrencyPop(MenuCurrencyType.Gold, null, delegate
					{
						popReady.gameObject.SetActive(value: false);
					});
				}
				else if (PlayerInfo.Instance.ThisGameType == GameType.Multi)
				{
					onGameStartMulti();
				}
				else
				{
					onGameStart();
				}
			}

			private void updatePopReady()
			{
				HashSet<StartItemType> hashSet = PopReadyAvailable.AttributeMap[PlayerInfo.Instance.ThisGameType];
				for (int i = 0; popReadyStartItems.Length > i; i++)
				{
					if (hashSet.Contains((StartItemType)i) && DataContainer.Instance.StartItemTableRaw[i].Enabled)
					{
						popReadyStartItems[i].interactable = true;
					}
					else
					{
						popReadyStartItems[i].interactable = false;
					}
					popReadyStartItems[i].isOn = PlayerInfo.Instance.StoredStartItemsWithGameTypes[i][(int)PlayerInfo.Instance.ThisGameType];
					popReadyStartItems[i].transform.Find("ActiveSelect").gameObject.SetActive(value: false);
				}
				popReadyStartItemLastIdx = 0;
				popReadyStartReqGold = 0;
				popReadyStartItems[popReadyStartItemLastIdx].transform.Find("ActiveSelect").gameObject.SetActive(value: true);
				popReadyStartItems[popReadyStartItemLastIdx].isOn = true;
				popReadyStartItems[popReadyStartItemLastIdx].isOn = false;
				popReadyStartItems[popReadyStartItemLastIdx].isOn = PlayerInfo.Instance.StoredStartItemsWithGameTypes[popReadyStartItemLastIdx][(int)PlayerInfo.Instance.ThisGameType];
				Image component = popReadyRaceStartBtn.transform.Find("BG").GetComponent<Image>();
				Image component2 = popReadyRaceStartBtn.transform.Find("AlphaHighLight").GetComponent<Image>();
				string[] array = new string[3]
				{
					"language_main_btn_ready_01",
					"language_main_btn_ready_02",
					"language_main_btn_ready_03"
				};
				component.GetComponent<LLocImage>().SetPhraseName(array[(int)PlayerInfo.Instance.ThisGameType]);
				component2.GetComponent<LLocImage>().SetPhraseName(array[(int)PlayerInfo.Instance.ThisGameType]);
			}

			private void cancelWaitMultiplay()
			{
				try
				{
					PlayGamesPlatform.Instance.RealTime.LeaveRoom();
					showMultiWaitCancelPopup = false;
				}
				catch (Exception)
				{
				}
			}

			private IEnumerator checkMultiPlayer()
			{
				bool ready2 = false;
				bool isShowWaitPopup = false;
				float checkTime = Time.timeSinceLevelLoad;
				bool ready;
				while (Application.isPlaying)
				{
					if (RaceManager.Instance != null && RaceManager.Instance.IsRoomSetupProgress && !isShowWaitPopup)
					{
						isShowWaitPopup = true;
						RaceManager.Instance.OnCleanUp = delegate
						{
							showMultiWaitCancelPopup = false;
						};
						Action cancelBtnHandler = delegate
						{
							cancelWaitMultiplay();
							ready = false;
						};
						LeanTween.delayedCall(10f, (Action)delegate
						{
							LoadingMergeLoader.Instance.MultiWaitCancelBtnRoot.SetActive(value: true);
							Button component = LoadingMergeLoader.Instance.MultiWaitCancelBtnRoot.transform.Find("CancelBtn").GetComponent<Button>();
							component.onClick.RemoveAllListeners();
							component.onClick.AddListener(delegate
							{
								cancelBtnHandler();
							});
						});
						LeanTween.delayedCall(0f, (Action)delegate
						{
							LateUpdater.Instance.AddAction(delegate
							{
								showMultiWaitCancelPopup = true;
							});
						});
					}
					if (RaceManager.Instance != null && RaceManager.Instance.IsRoomSetupProgress && Time.timeSinceLevelLoad - checkTime > 30f)
					{
						cancelWaitMultiplay();
						ready2 = false;
						yield break;
					}
					Action abortedAction = delegate
					{
						FilterTop.gameObject.SetActive(value: false);
						FilterTop.transform.Find("BG").gameObject.SetActive(value: false);
						PlayerInfo.Instance.IsSenseBackBtn = true;
						ready2 = false;
						LoadingMergeLoader.Instance.MultiWaitCancelBtnRoot.SetActive(value: false);
						LoadingMergeLoader.Instance.ShowIndicater(isShow: false);
						if (RaceManager.Instance != null)
						{
							RaceManager.Instance.IsRoomSetupProgress = false;
							RaceManager.Instance.CleanUp();
						}
					};
					if (RaceManager.Instance == null)
					{
						abortedAction();
						yield break;
					}
					switch (RaceManager.Instance.State)
					{
					case RaceManager.RaceState.SetupFailed:
						PlayGamesPlatform.Instance.RealTime.LeaveRoom();
						break;
					case RaceManager.RaceState.Aborted:
						abortedAction();
						yield break;
					case RaceManager.RaceState.Ready:
					case RaceManager.RaceState.Waiting:
						if (!ready2)
						{
							ready2 = true;
							LoadingMergeLoader.Instance.ShowIndicater(isShow: true);
						}
						break;
					case RaceManager.RaceState.Playing:
						LoadingMergeLoader.Instance.MultiWaitCancelBtnRoot.SetActive(value: false);
						onGameStartMultiProc();
						yield break;
					}
					yield return 0;
				}
				crtCheckMultiPlayer = null;
			}

			private void onGameStartMulti()
			{
				string iD = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx].ID;
				PlayerInfo.Instance.SelectedCharID = iD;
				PlayerInfo.Instance.IsSenseBackBtn = false;
				FilterTop.gameObject.SetActive(value: true);
				FilterTop.transform.Find("BG").gameObject.SetActive(value: true);
				if (!PlayGamesPlatform.Instance.IsAuthenticated())
				{
					PlayGamesPlatform.Instance.Authenticate(delegate(bool success)
					{
						if (success)
						{
							if (MultiGameType == 1)
							{
								RaceManager.CreateQuickGame();
							}
							else
							{
								RaceManager.CreateWithInvitationScreen();
							}
							crtCheckMultiPlayer = StartCoroutine(checkMultiPlayer());
						}
						else
						{
							PlayerInfo.Instance.IsSenseBackBtn = true;
							FilterTop.gameObject.SetActive(value: false);
							FilterTop.transform.Find("BG").gameObject.SetActive(value: false);
						}
					}, silent: false);
					return;
				}
				if (MultiGameType == 1)
				{
					RaceManager.CreateQuickGame();
				}
				else
				{
					RaceManager.CreateWithInvitationScreen();
				}
				crtCheckMultiPlayer = StartCoroutine(checkMultiPlayer());
			}

			private void onGameStartMultiProc()
			{
				FilterTop.gameObject.SetActive(value: true);
				PlayerInfo playerInfo = PlayerInfo.Instance;
				int thisGameType = (int)PlayerInfo.Instance.ThisGameType;
				playerInfo.AccMissionByCondTypeID("playgamemode", thisGameType.ToString(), 1.ToString());
				int reqGold = 0;
				int reqGem = 0;
				PlayerInfo.Instance.StartItems.Select(delegate(bool isOn, int index)
				{
					if (isOn)
					{
						if (DataContainer.Instance.StartItemTableRaw[index].CostTypeToInt == 0)
						{
							reqGold += DataContainer.Instance.StartItemTableRaw[index].Cost;
						}
						else if (DataContainer.Instance.StartItemTableRaw[index].CostTypeToInt == 1)
						{
							reqGem += DataContainer.Instance.StartItemTableRaw[index].Cost;
						}
						PlayerInfo.Instance.AccMissionByCondTypeID("buystartitem", DataContainer.Instance.StartItemTableRaw.dataArray[index].ID, 1.ToString());
						if (index == 3)
						{
							int num = Mathf.RoundToInt((from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
								where s.ID == "6"
								select s).First().Pvalue);
							PlayerInfo.Instance.StartItemCounts[3] += num;
						}
					}
					PlayerInfo.Instance.StoredStartItemsWithGameTypes[index][(int)PlayerInfo.Instance.ThisGameType] = isOn;
					return isOn;
				}).All((bool isOn) => true);
				CurrencyTypeMapInt currency;
				(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] - reqGold;
				(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] - reqGem;
				PlayerInfo.Instance.DirtyAll();
				PlayerInfo.Instance.ThisGameOpponent = RaceManager.Instance.GameOpponent;
				string oppoCharID = PlayerInfo.Instance.ThisGameOpponent.CharID.ToString();
				string modelname = (from s in DataContainer.Instance.CharacterTableRaw.dataArray
					where s.ID == oppoCharID
					select s).First().Modelname;
				PlayerInfo.Instance.ThisGameOpponent.ModelName = modelname;
				modelname = Regex.Replace(modelname, "[0-9]+$", string.Empty);
				modelname = $"{modelname}01_";
				PlayerInfo.Instance.ThisGameOpponent.AnimID = modelname;
				setupHighMeters();
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				StartCoroutine(LoadGame());
			}

			private void onGameStart()
			{
				LoadingMergeLoader.Instance.ShowIndicater(isShow: false);
				PlayerInfo.Instance.NameTagCount--;
				PlayerInfo playerInfo = PlayerInfo.Instance;
				int thisGameType = (int)PlayerInfo.Instance.ThisGameType;
				playerInfo.AccMissionByCondTypeID("playgamemode", thisGameType.ToString(), 1.ToString());
				int reqGold = 0;
				int reqGem = 0;
				PlayerInfo.Instance.StartItems.Select(delegate(bool isOn, int index)
				{
					if (isOn)
					{
						if (DataContainer.Instance.StartItemTableRaw[index].CostTypeToInt == 0)
						{
							reqGold += DataContainer.Instance.StartItemTableRaw[index].Cost;
						}
						else if (DataContainer.Instance.StartItemTableRaw[index].CostTypeToInt == 1)
						{
							reqGem += DataContainer.Instance.StartItemTableRaw[index].Cost;
						}
						PlayerInfo.Instance.AccMissionByCondTypeID("buystartitem", DataContainer.Instance.StartItemTableRaw.dataArray[index].ID, 1.ToString());
						if (index == 3)
						{
							int num = Mathf.RoundToInt((from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
								where s.ID == "6"
								select s).First().Pvalue);
							PlayerInfo.Instance.StartItemCounts[3] += num;
						}
					}
					PlayerInfo.Instance.StoredStartItemsWithGameTypes[index][(int)PlayerInfo.Instance.ThisGameType] = isOn;
					return isOn;
				}).All((bool isOn) => true);
				CurrencyTypeMapInt currency;
				(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gold] = currency[CurrencyType.Gold] - reqGold;
				(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] - reqGem;
				string iD = DataContainer.Instance.CharacterTableRaw.dataArray[pvIndicIdx].ID;
				PlayerInfo.Instance.SelectedCharID = iD;
				PlayerInfo.Instance.SelectedCharIDVolatile = PlayerInfo.Instance.SelectedCharID;
				PlayerInfo.Instance.DirtyAll();
				setupHighMeters();
				Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				StartCoroutine(LoadGame());
			}

			private void setupHighMeters()
			{
				PlayerInfo.Instance.HighMetersTargetList.Clear();
			}

			private void updateRaceStartItems()
			{
				popReadyStartReqGold = 0;
				int reqGem = 0;
				PlayerInfo.Instance.StartItems.Select(delegate(bool isOn, int index)
				{
					if (isOn)
					{
						if (DataContainer.Instance.StartItemTableRaw[index].CostTypeToInt == 0)
						{
							popReadyStartReqGold += DataContainer.Instance.StartItemTableRaw[index].Cost;
						}
						else if (DataContainer.Instance.StartItemTableRaw[index].CostTypeToInt == 1)
						{
							reqGem += DataContainer.Instance.StartItemTableRaw[index].Cost;
						}
					}
					return isOn;
				}).All((bool isOn) => true);
				Text component = popReady.transform.Find("RequireCoins/CoinGoldText").GetComponent<Text>();
				component.text = $"{popReadyStartReqGold:D5}";
				if (popReadyStartReqGold <= PlayerInfo.Instance.Currency[CurrencyType.Gold])
				{
					component.color = Color.white;
				}
				else
				{
					component.color = Color.red;
				}
			}

			private void createReady()
			{
				popReady = base.transform.parent.Find("Popup/RaceReady").transform;
				popStartBtns = (from s in new string[3]
					{
						"BottomCenter/StartBtn",
						"BottomCenter/StartBtnMission",
						"BottomCenter/StartBtnMulti"
					}
					select base.transform.Find(s).transform.GetComponent<Button>()).ToArray();
				popReadyCloseBtn = base.transform.parent.Find("Popup/RaceReady/CloseBtn").transform.GetComponent<Button>();
				popReadyCloseBtn.onClick.AddListener(delegate
				{
					SetActivateFilter(activate: false);
					popReady.gameObject.SetActive(value: false);
					backBtnStackDepth = 0;
					Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
				});
				popStartDisableBtn = (from s in new string[3]
					{
						"BottomCenter/StartBtn/Disable",
						"BottomCenter/StartBtnMission/Disable",
						"BottomCenter/StartBtnMulti/Disable"
					}
					select base.transform.Find(s).transform.GetComponent<Button>()).ToArray();
				popStartDisableBtn.All(delegate(Button s)
				{
					s.onClick.AddListener(delegate
					{
						ForcePreviewCharIndex(0);
						Action value = delegate
						{
							Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
						};
						ShowPopupCommon(new Dictionary<string, object>
						{
							{
								"type",
								"Notify"
							},
							{
								"msg",
								LeanLocalization.GetTranslationText("178")
							},
							{
								"CloseHandler",
								value
							}
						});
					});
					return true;
				});
				popReadyRaceStartBtn = base.transform.parent.Find("Popup/RaceReady/RaceBtn").transform.GetComponent<Button>();
				popReadyRaceStartBtn.onClick.AddListener(delegate
				{
					MultiGameType = 1;
					OnBtnClick_RaceStart();
				});
				popReadyMultiRaceStartBtn = base.transform.parent.Find("Popup/RaceReady/MultiRaceFriendBtn").transform.GetComponent<Button>();
				popReadyMultiRaceStartBtn.onClick.AddListener(delegate
				{
					MultiGameType = 2;
					OnBtnClick_RaceStart();
				});
				popReadyStartItemDescRoot = base.transform.parent.Find("Popup/RaceReady/Description");
				popReadyStartItemDescText = popReadyStartItemDescRoot.Find("DescText").GetComponent<Text>();
				popReadyStartItemIconImage = popReadyStartItemDescRoot.Find("Icon").GetComponent<Image>();
				popReadyStartItemCoinText = popReadyStartItemDescRoot.Find("CoinText").GetComponent<Text>();
				popReadyStartItemTextImage = popReadyStartItemDescRoot.Find("DescImageText").GetComponent<Image>();
				RectTransform itemsRoot = base.transform.parent.Find("Popup/RaceReady/Items").GetComponent<RectTransform>();
				popReadyStartItems = Enumerable.Range(0, Enum.GetValues(typeof(StartItemType)).Length).Select(delegate(int index)
				{
					Toggle toggle2 = itemsRoot.GetChild(index).GetComponent<Toggle>();
					((Func<bool>)delegate
					{
						int innerIdx = index;
						Toggle innerToggle = toggle2;
						innerToggle.onValueChanged.AddListener(delegate(bool isOn)
						{
							Camera.main.GetComponent<AudioSource>().PlayOneShot(ClickAud);
							if (-1 < popReadyStartItemLastIdx)
							{
								popReadyStartItems[popReadyStartItemLastIdx].transform.Find("ActiveSelect").gameObject.SetActive(value: false);
							}
							else
							{
								popReadyStartItemDescRoot.gameObject.SetActive(value: true);
							}
							popReadyStartItemLastIdx = innerIdx;
							innerToggle.transform.Find("ActiveSelect").gameObject.SetActive(value: true);
							popReadyStartItemDescText.GetComponent<LeanLocalizedText>().SetPhraseName(DataContainer.Instance.StartItemTableRaw[innerIdx].Desc1loc);
							popReadyStartItemCoinText.text = DataContainer.Instance.StartItemTableRaw[innerIdx].Cost.ToString();
							popReadyStartItemIconImage.sprite = popReadyStartItemIconSprite[innerIdx];
							popReadyStartItemTextImage.GetComponent<LLocImage>().SetPhraseName(DataContainer.Instance.StartItemTableRaw[innerIdx].Descnameimagepath);
							popReadyStartItemTextImage.SetNativeSize();
							RectTransform component2 = popReadyStartItemTextImage.transform.parent.Find("Underline").GetComponent<RectTransform>();
							Vector2 sizeDelta = component2.sizeDelta;
							Vector2 sizeDelta2 = popReadyStartItemTextImage.GetComponent<RectTransform>().sizeDelta;
							sizeDelta.x = sizeDelta2.x;
							component2.sizeDelta = sizeDelta;
							if (EventSystem.current.currentSelectedGameObject == innerToggle.gameObject)
							{
								RouletteAudSrc.PlayOneShot((!isOn) ? AudCancer : AudBuy);
							}
							PlayerInfo.Instance.StartItems[innerIdx] = isOn;
							updateRaceStartItems();
						});
						return true;
					})();
					return toggle2;
				}).ToArray();
				popReadyStartItems.Select(delegate(Toggle toggle, int index)
				{
					popReadyStartItemIconSprite[index] = toggle.transform.Find("BG").GetComponent<Image>().sprite;
					Text component = toggle.transform.Find("CoinText").GetComponent<Text>();
					component.text = DataContainer.Instance.StartItemTableRaw[index].Cost.ToString();
					return toggle;
				}).All((Toggle toggle) => true);
				for (int i = 0; Enum.GetValues(typeof(StartItemType)).Length > i; i++)
				{
					PlayerInfo.Instance.StartItems[i] = false;
					PlayerInfo.Instance.StartItemCounts[i] = 0;
				}
				for (int j = 0; Enum.GetValues(typeof(CharacterSkillType)).Length > j; j++)
				{
					PlayerInfo.Instance.CharacterSkills[j] = false;
				}
			}

			private IEnumerator LoadGame()
			{
				bool autoDisableLoadingIndicater = true;
				string empty = string.Empty;
				switch (PlayerInfo.Instance.ThisGameType)
				{
				case GameType.NormalSingle:
					autoDisableLoadingIndicater = false;
					break;
				case GameType.MissionSingle:
					autoDisableLoadingIndicater = false;
					break;
				case GameType.Multi:
					autoDisableLoadingIndicater = false;
					break;
				}
				LoadingMergeLoader.Instance.ActiveLoadSceneName = "GameScene";
				LoadingMergeLoader.Instance.StartLoadLevel(autoDisableLoadingIndicater);
				yield break;
			}

			private void OnApplicationQuit()
			{
				GoogleAnalyticsV4.getInstance().StopSession();
			}
		}
