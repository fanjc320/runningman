using AudienceNetwork;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RewardedVideoAdTest : MonoBehaviour
{
	private RewardedVideoAd rewardedVideoAd;

	private bool isLoaded;

	public Text statusLabel;

	public void LoadRewardedVideo()
	{
		statusLabel.text = "Loading rewardedVideo ad...";
		RewardedVideoAd rewardedVideoAd = this.rewardedVideoAd = new RewardedVideoAd("YOUR_PLACEMENT_ID");
		this.rewardedVideoAd.Register(base.gameObject);
		this.rewardedVideoAd.RewardedVideoAdDidLoad = delegate
		{
			isLoaded = true;
			statusLabel.text = "Ad loaded. Click show to present!";
		};
		rewardedVideoAd.RewardedVideoAdDidFailWithError = delegate
		{
			statusLabel.text = "RewardedVideo ad failed to load. Check console for details.";
		};
		rewardedVideoAd.RewardedVideoAdWillLogImpression = delegate
		{
		};
		rewardedVideoAd.RewardedVideoAdDidClick = delegate
		{
		};
		this.rewardedVideoAd.LoadAd();
	}

	public void ShowRewardedVideo()
	{
		if (isLoaded)
		{
			rewardedVideoAd.Show();
			isLoaded = false;
			statusLabel.text = string.Empty;
		}
		else
		{
			statusLabel.text = "Ad not loaded. Click load to request an ad.";
		}
	}

	private void OnDestroy()
	{
		if (rewardedVideoAd != null)
		{
			rewardedVideoAd.Dispose();
		}
	}

	public void NextScene()
	{
		SceneManager.LoadScene("InterstitialAdScene");
	}
}
