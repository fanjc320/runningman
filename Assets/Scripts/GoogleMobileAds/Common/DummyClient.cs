using GoogleMobileAds.Api;
using System;

namespace GoogleMobileAds.Common
{
	public class DummyClient : IBannerClient, IInterstitialClient, IRewardBasedVideoAdClient, IAdLoaderClient, INativeExpressAdClient
	{
		public string UserId
		{
			get
			{
				return "UserId";
			}
			set
			{
			}
		}

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdStarted;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<Reward> OnAdRewarded;

		public event EventHandler<EventArgs> OnAdLeavingApplication;

		public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

		public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
		{
		}

		public void CreateBannerView(string adUnitId, AdSize adSize, int positionX, int positionY)
		{
		}

		public void LoadAd(AdRequest request)
		{
		}

		public void ShowBannerView()
		{
		}

		public void HideBannerView()
		{
		}

		public void DestroyBannerView()
		{
		}

		public void CreateInterstitialAd(string adUnitId)
		{
		}

		public bool IsLoaded()
		{
			return true;
		}

		public void ShowInterstitial()
		{
		}

		public void DestroyInterstitial()
		{
		}

		public void CreateRewardBasedVideoAd()
		{
		}

		public void SetUserId(string userId)
		{
		}

		public void LoadAd(AdRequest request, string adUnitId)
		{
		}

		public void DestroyRewardBasedVideoAd()
		{
		}

		public void ShowRewardBasedVideoAd()
		{
		}

		public void CreateAdLoader(AdLoader.Builder builder)
		{
		}

		public void Load(AdRequest request)
		{
		}

		public void CreateNativeExpressAdView(string adUnitId, AdSize adSize, AdPosition position)
		{
		}

		public void CreateNativeExpressAdView(string adUnitId, AdSize adSize, int positionX, int positionY)
		{
		}

		public void SetAdSize(AdSize adSize)
		{
		}

		public void ShowNativeExpressAdView()
		{
		}

		public void HideNativeExpressAdView()
		{
		}

		public void DestroyNativeExpressAdView()
		{
		}
	}
}
