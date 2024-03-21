using GoogleMobileAds.Api;
using System;

public class AdBannerManager
{
	public static string adUnitId = "ca-app-pub-2853664307815463/4120278233";

	private BannerView bannerView;

	private bool isLoadComplete;

	private bool isLoading;

	private static AdBannerManager instance;

	public static AdBannerManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new AdBannerManager();
			}
			return instance;
		}
	}

	public AdBannerManager()
	{
		isLoadComplete = false;
		isLoading = false;
		RequestBanner();
	}

	public static bool isLoadedAD()
	{
		return instance.isLoadComplete;
	}

	public static void ShowBannerAD()
	{
		if (instance.bannerView != null)
		{
			instance.bannerView.Show();
			if (!instance.isLoading && !instance.isLoadComplete)
			{
				instance.LoadBannerAD();
			}
		}
	}

	public static void HideBannerAD()
	{
		if (instance.bannerView != null)
		{
			instance.bannerView.Hide();
		}
	}

	public static void Init()
	{
		AdBannerManager adBannerManager = Instance;
	}

	private AdRequest createAdRequest()
	{
		return new AdRequest.Builder().AddKeyword("game").Build();
	}

	private void RequestBanner()
	{
		bannerView = new BannerView(adUnitId, AdSize.MediumRectangle, AdPosition.Top);
		bannerView.OnAdLoaded += HandleBannerLoaded;
		bannerView.OnAdFailedToLoad += HandleBannerFailedToLoad;
		bannerView.OnAdOpening += HandleBannerOpened;
		bannerView.OnAdClosed += HandleBannerClosed;
		bannerView.OnAdLeavingApplication += HandleBannerLeftApplication;
		bannerView.Hide();
		LoadBannerAD();
	}

	private void LoadBannerAD()
	{
		isLoading = true;
		bannerView.LoadAd(createAdRequest());
	}

	public void HandleBannerLoaded(object sender, EventArgs args)
	{
		isLoadComplete = true;
		isLoading = false;
	}

	public void HandleBannerFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		isLoadComplete = false;
		isLoading = false;
	}

	public void HandleBannerOpened(object sender, EventArgs args)
	{
	}

	private void HandleInterstitialClosing(object sender, EventArgs args)
	{
	}

	public void HandleBannerClosed(object sender, EventArgs args)
	{
	}

	public void HandleBannerLeftApplication(object sender, EventArgs args)
	{
	}
}
