using GoogleMobileAds.Api;
using System;

public class AdInterstitialManager
{
	public static string adUnitId = "ca-app-pub-2853664307815463/2063292232";

	private Action<bool> OnAdClosed;

	private InterstitialAd interstitial;

	private static AdInterstitialManager instance;

	public static AdInterstitialManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AdInterstitialManager();
			}
			return instance;
		}
	}

	public AdInterstitialManager()
	{
		RequestInterstitial();
	}

	public static bool isLoadedAD()
	{
		if (instance.interstitial != null)
		{
			return instance.interstitial.IsLoaded();
		}
		return false;
	}

	public static void RequestInterstitialAD()
	{
		if (!isLoadedAD())
		{
			instance.RequestInterstitial();
		}
	}

	public static void ShowInterstitialAD(Action<bool> callback = null)
	{
		instance.ShowInterstitial(callback);
	}

	public static void Init()
	{
		AdInterstitialManager adInterstitialManager = Instance;
	}

	private AdRequest createAdRequest()
	{
		return new AdRequest.Builder().AddKeyword("game").Build();
	}

	private void RequestInterstitial()
	{
		interstitial = new InterstitialAd(adUnitId);
		interstitial.OnAdLoaded += HandleInterstitialLoaded;
		interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
		interstitial.OnAdOpening += HandleInterstitialOpened;
		interstitial.OnAdClosed += HandleInterstitialClosed;
		interstitial.OnAdLeavingApplication += HandleInterstitialLeftApplication;
		interstitial.LoadAd(createAdRequest());
	}

	private void ShowInterstitial(Action<bool> callback)
	{
		OnAdClosed = callback;
		if (interstitial.IsLoaded())
		{
			interstitial.Show();
			return;
		}
		if (OnAdClosed != null)
		{
			OnAdClosed(obj: false);
		}
		OnAdClosed = null;
		RequestInterstitial();
	}

	public void HandleInterstitialLoaded(object sender, EventArgs args)
	{
	}

	public void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
	}

	public void HandleInterstitialOpened(object sender, EventArgs args)
	{
	}

	private void HandleInterstitialClosing(object sender, EventArgs args)
	{
	}

	public void HandleInterstitialClosed(object sender, EventArgs args)
	{
		if (OnAdClosed != null)
		{
			OnAdClosed(obj: true);
		}
	}

	public void HandleInterstitialLeftApplication(object sender, EventArgs args)
	{
	}
}
