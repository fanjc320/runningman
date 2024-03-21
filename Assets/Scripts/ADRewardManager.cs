using GoogleMobileAds.Api;
using System;

public class ADRewardManager
{
	public static string adUnitId = "ca-app-pub-2853664307815463/9586559034";

	private bool isLoadComplete;

	private bool isLoading;

	private bool isRewarded;

	private static ADRewardManager instance;

	private RewardBasedVideoAd rewardBasedVideo;

	private float deltaTime;

	private static string outputMessage = string.Empty;

	private Action<Reward> rewardCallBack;

	private Action<EventArgs> closedCallBack;

	public bool IsRewarded => isRewarded;

	public static ADRewardManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ADRewardManager();
			}
			return instance;
		}
	}

	public static string OutputMessage
	{
		set
		{
			outputMessage = value;
		}
	}

	public ADRewardManager()
	{
		rewardBasedVideo = RewardBasedVideoAd.Instance;
		rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
		rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
		rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
		rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
		rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
		rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
		rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
		isLoadComplete = false;
		isLoading = false;
	}

	public static bool isLoadedAD()
	{
		if (instance.rewardBasedVideo.IsLoaded())
		{
			return true;
		}
		if (instance.isLoading)
		{
			return false;
		}
		RequestRewardAD();
		return false;
	}

	public static void RequestRewardAD()
	{
		instance.RequestRewardBasedVideo();
	}

	public static void ShowRewardAD(Action<Reward> callBack, Action<EventArgs> closedCallBack = null)
	{
		if (callBack != null)
		{
			instance.rewardCallBack = callBack;
			instance.closedCallBack = closedCallBack;
			instance.ShowRewardBasedVideo();
		}
	}

	public static void Init()
	{
		ADRewardManager aDRewardManager = Instance;
		RequestRewardAD();
	}

	private AdRequest createAdRequest()
	{
		return new AdRequest.Builder().AddKeyword("game").Build();
	}

	private void RequestRewardBasedVideo()
	{
		isLoading = true;
		rewardBasedVideo.LoadAd(createAdRequest(), adUnitId);
	}

	private void ShowRewardBasedVideo()
	{
		if (rewardBasedVideo.IsLoaded())
		{
			rewardBasedVideo.Show();
		}
		else if (!isLoading)
		{
			RequestRewardBasedVideo();
		}
	}

	public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
	{
		isLoadComplete = true;
		isLoading = false;
		isRewarded = false;
	}

	public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		isLoadComplete = false;
		isLoading = false;
	}

	public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
	{
	}

	public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
	{
	}

	public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
	{
		if (closedCallBack != null)
		{
			LateUpdater.Instance.AddAction(delegate
			{
				closedCallBack(args);
				closedCallBack = null;
			});
		}
		RequestRewardAD();
	}

	public void HandleRewardBasedVideoRewarded(object sender, Reward args)
	{
		string type = args.Type;
		double amount = args.Amount;
		isRewarded = (1.0 <= args.Amount);
		if (rewardCallBack != null)
		{
			LateUpdater.Instance.AddAction(delegate
			{
				rewardCallBack(args);
				rewardCallBack = null;
			});
		}
	}

	public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
	{
	}
}
