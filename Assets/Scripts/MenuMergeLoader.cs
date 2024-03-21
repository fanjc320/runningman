using Lean;
using SerializableClass;
using Soomla.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Wenee;

public class MenuMergeLoader : MonoBehaviour
{
	public Animator DirectorRoot;

	public Image CIImage;

	public Image LightImage;

	public Button TouchToStartBtn;

	public Image LoadingImageText;

	public Slider LoadingSlider;

	public AudioClip AudGet;

	public AudioClip AudRoll;

	public GameObject LoadingIndicator;

	public GameObject splash;

	private AsyncOperation asyncMerge;

	private GameObject menuGO;

	private bool touchToStart;

	private void Awake()
	{
		FirebasePlugin.Init();
		GoogleAnalyticsV4.s_BundleVersion = PlayerInfo.Instance.AppVersion;
		LeanLocalization.Instance.SetLanguage(DataContainer.LocaleIdentifier[PlayerInfo.Instance.LocaleIndex]);
		FBManager.Instance.IsLoadedCompleted = false;
		splash.SetActive(value: true);
		float num = Screen.height;
		float num2 = (float)Screen.width / (float)Screen.height;
		float num3 = 1024f;
		if (Application.platform == RuntimePlatform.Android)
		{
			num = ((!((float)Screen.height > num3)) ? ((float)Screen.height) : num3);
		}
		Screen.SetResolution((int)(num * num2), (int)num, fullscreen: true);
		PlayerInfo.Instance.IsMenuSceneByTitleScene = true;
		PlayerInfo.Instance.IsSenseBackBtn = false;
		UnityEngine.Random.seed = (int)DateTime.Now.Ticks;
		Screen.sleepTimeout = -1;
		if (PlayerInfo.Instance.SoundOn)
		{
			GetComponent<AudioSource>().volume = 1f;
		}
		else
		{
			GetComponent<AudioSource>().volume = 0f;
		}
		Wenee.AdManager.Instance.Init();
	}

	private void OnDestroy()
	{
		FBManager.Instance.OnReady -= OnReady_FBManager;
	}

	private IEnumerator Start()
	{
		StoreEvents.OnSoomlaStoreInitialized = (StoreEvents.Action)Delegate.Combine(StoreEvents.OnSoomlaStoreInitialized, (StoreEvents.Action)delegate
		{
			SoomlaStore.StartIabServiceInBg();
			StoreEvents.OnMarketItemsRefreshFinished = delegate(List<MarketItem> items)
			{
				foreach (MarketItem item in items)
				{
					MarketInfoData[] dataArray = DataContainer.Instance.MarketTableRaw.dataArray;
					foreach (MarketInfoData marketInfoData in dataArray)
					{
						if (marketInfoData.Marketkey == item.ProductId)
						{
							DataContainer.Instance.ShopTableRaw.dataArray[int.Parse(marketInfoData.Shopinfoid)].costString = item.MarketPriceAndCurrency;
							DataContainer.Instance.ShopTableRaw.dataArray[int.Parse(marketInfoData.Shopinfoid)].currencyCode = item.MarketCurrencyCode;
						}
					}
				}
				StoreEvents.OnMarketItemsRefreshFinished = null;
				StoreEvents.OnMarketItemsRefreshFailed = null;
			};
			StoreEvents.OnMarketItemsRefreshFailed = delegate
			{
				StoreEvents.OnMarketItemsRefreshFinished = null;
				StoreEvents.OnMarketItemsRefreshFailed = null;
			};
			StoreEvents.OnSoomlaStoreInitialized = null;
		});
		SoomlaStore.Initialize(new StoreAsset(DataContainer.Instance.MarketTableRaw));
		yield return 0;
		yield return 0;
		splash.SetActive(value: false);
		GoogleAnalyticsV4.getInstance().StartSession();
		yield return 0;
		GoogleAnalyticsV4.getInstance().LogScreen("TitleScreen");
		yield return 0;
		AdBannerManager.Init();
		yield return 0;
		AdInterstitialManager.Init();
		yield return 0;
		ADRewardManager.Init();
		yield return 0;
		UnityEngine.Object.Destroy(GameObject.Find("EventSystem"));
		CIImage.transform.parent.gameObject.SetActive(value: false);
		DirectorRoot.gameObject.SetActive(value: true);
		GetComponent<AudioSource>().PlayOneShot(AudRoll);
		yield return new WaitForSeconds(0.5f);
		LeanTween.delayedCall(1.3f, (Action)delegate
		{
			Color white = Color.white;
			white.a *= 0.6667f;
			LightImage.color = white;
		});
		LeanTween.delayedCall(0.2f, (Action)delegate
		{
			GetComponent<AudioSource>().PlayOneShot(AudGet);
			LeanTween.delayedCall(1.1f, (Action)delegate
			{
				UnityEngine.Object.Destroy(GetComponent<AudioListener>());
			});
		});
		while (DirectorRoot.GetCurrentAnimatorStateInfo(0).IsName("Title"))
		{
			yield return 0;
		}
		LoadingImageText.gameObject.SetActive(value: true);
		asyncMerge = Application.LoadLevelAdditiveAsync("MenuScene");
		Coroutine loadCrtn = StartCoroutine(loadingProgress());
		yield return loadCrtn;
		StopCoroutine(loadCrtn);
		menuGO = GameObject.Find("MenuCanvas");
		menuGO.gameObject.SetActive(value: false);
		LoadingSlider.normalizedValue = 1f;
		CanvasGroup sliderCG = LoadingSlider.gameObject.AddComponent<CanvasGroup>();
		LeanTween.value(base.gameObject, delegate(float norm)
		{
			sliderCG.alpha = norm;
		}, 1f, 0f, 0.6667f);
		LoadingImageText.gameObject.SetActive(value: false);
		touchToStart = false;
		Button fbButton = base.transform.Find("Title/PlatformBtns/FBBtn").GetComponent<Button>();
		FBManager.Instance.OnReady += OnReady_FBManager;
		if (FBManager.Instance.IsReady)
		{
			fbButton.gameObject.SetActive(value: false);
		}
		else
		{
			fbButton.gameObject.SetActive(value: true);
			fbButton.transform.Find("BonusIcon").gameObject.SetActive(!PlayerInfo.Instance.FBFirstLoginReward);
			fbButton.onClick.AddListener(delegate
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(MenuUIManager.Instance.ClickAud);
				FBManager.Instance.Login();
				LoadingIndicator.SetActive(value: true);
			});
		}
		TouchToStartBtn.gameObject.SetActive(value: true);
		TouchToStartBtn.onClick.AddListener(delegate
		{
			Camera.main.GetComponent<AudioSource>().PlayOneShot(MenuUIManager.Instance.ClickAud);
			menuGO.SetActive(value: true);
			touchToStart = true;
		});
		while (!touchToStart)
		{
			yield return 0;
		}
		PlayerInfo.Instance.IsMenuSceneByTitleScene = false;
		MenuUIManager.Instance.YieldRegistFBReadyHandler();
		MenuUIManager.Instance.YieldCheckFirstLoginBtns();
		LeanTween.delayedCall(0f, (Action)delegate
		{
			AsyncOperation asyncOperation = Resources.UnloadUnusedAssets();
		});
		PlayerInfo.Instance.IsSenseBackBtn = true;
		yield return 0;
		QTest.CheckingPerformance();
		yield return 0;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnReady_FBManager(bool isSuccess)
	{
		if (isSuccess)
		{
			GameObject gameObject = base.transform.Find("Title/FacebookRewardPopup").gameObject;
			if (!PlayerInfo.Instance.FBFirstLoginReward)
			{
				gameObject.gameObject.SetActive(value: true);
				gameObject.transform.Find("MsgText").GetComponent<Text>().text = string.Format(LeanLocalization.GetTranslationText("192"), 3);
				gameObject.transform.Find("OKBtn").GetComponent<Button>().onClick.RemoveAllListeners();
				gameObject.transform.Find("OKBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					LoadingIndicator.SetActive(value: false);
					menuGO.SetActive(value: true);
					touchToStart = true;
				});
				PlayerInfo.Instance.FBFirstLoginReward = true;
				CurrencyTypeMapInt currency;
				(currency = PlayerInfo.Instance.Currency)[CurrencyType.Gem] = currency[CurrencyType.Gem] + 3;
			}
			else
			{
				gameObject.gameObject.SetActive(value: false);
				LoadingIndicator.SetActive(value: false);
				menuGO.SetActive(value: true);
				touchToStart = true;
			}
		}
		else
		{
			LoadingIndicator.SetActive(value: false);
		}
	}

	private IEnumerator loadingProgress()
	{
		while (!asyncMerge.isDone)
		{
			LoadingSlider.normalizedValue = asyncMerge.progress * 0.8f;
			yield return 0;
		}
		GPGSManager.Instance.Authenticate();
		while (GPGSManager.Instance.Authenticating && !FBManager.Instance.IsLoadedCompleted)
		{
			yield return 0;
		}
		LoadingSlider.normalizedValue = 1f;
		yield return 0;
		PlayerInfo.Instance.LoadedDateTime = DateTime.Now;
	}
}
