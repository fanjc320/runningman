using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using SerializableClass;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GPGSManager
{
	private static GPGSManager sInstance = new GPGSManager();

	private bool mAuthenticating;

	private Dictionary<string, bool> mUnlockedAchievements = new Dictionary<string, bool>();

	private Dictionary<string, int> mPendingIncrements = new Dictionary<string, int>();

	private int mHighestPostedScore;

	private bool mSaving;

	public bool saveDone;

	public bool isCloudLoading;

	public bool isCloudSaving;

	private string mAutoSaveName;

	private Texture2D mScreenImage;

	public static GPGSManager Instance => sInstance;

	public bool Authenticating => mAuthenticating;

	public bool Authenticated => Social.Active.localUser.authenticated;

	private GPGSManager()
	{
		mAutoSaveName = "Autosaved";
	}

	public void CaptureScreenshot()
	{
		mScreenImage = new Texture2D(Screen.width, Screen.height);
		mScreenImage.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
		mScreenImage.Apply();
	}

	public void SaveProgress()
	{
	}

	public void UnlockAchievement(string achId)
	{
		if (Authenticated)
		{
			Social.ReportProgress(achId, 100.0, delegate
			{
			});
		}
	}

	public void Authenticate()
	{
		if (!Authenticated && !mAuthenticating)
		{
			PlayGamesPlatform.DebugLogEnabled = false;
			PlayGamesClientConfiguration configuration = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
			PlayGamesPlatform.InitializeInstance(configuration);
			PlayGamesPlatform.Activate();
			mAuthenticating = true;
			Social.localUser.Authenticate(delegate(bool success)
			{
				mAuthenticating = false;
				if (success)
				{
					LoadFromCloud();
				}
			});
		}
	}

	public void Authenticate(Action<bool> callback)
	{
		if (!mAuthenticating)
		{
			mAuthenticating = true;
			Social.localUser.Authenticate(delegate(bool success)
			{
				mAuthenticating = false;
				if (callback != null)
				{
					callback(success);
				}
			});
		}
	}

	private void ProcessCloudData(byte[] cloudData)
	{
		if (cloudData == null || cloudData.Length == 0 || cloudData[0] != 86)
		{
			return;
		}
		uint num = 0u;
		try
		{
			num = BitConverter.ToUInt32(cloudData, 1);
		}
		catch
		{
			num = 0u;
		}
		if (num < 10)
		{
			NotSupportedException ex = new NotSupportedException();
			ex.Data.Add("verison", num);
			ex.Data.Add("savedata", cloudData);
			ex.Data.Add("isCloud", true);
			throw ex;
		}
		List<byte> list = new List<byte>(cloudData);
		list.RemoveRange(0, 5);
		cloudData = list.ToArray();
		UserSaveData userSaveData = PlayerInfo.Instance.DecryptData(cloudData);
		bool flag = false;
		if (userSaveData != null)
		{
			flag = (string.IsNullOrEmpty(PlayerInfo.Instance.LastGPGSID) || !(PlayerInfo.Instance.LastGPGSID == userSaveData.Player.GPGSID) || ((PlayerInfo.Instance.PlayedTimeSpanTotalMilliseconds < userSaveData.Player.PlayedTimeSpanTotalMilliseconds) ? true : false));
		}
		if (flag && PlayerInfo.Instance.IsFirstLaunch)
		{
			PlayerInfo.Instance.SaveFromData(cloudData);
			PlayerInfo.Instance.IsFirstLaunch = false;
			PlayerInfo.Instance.Load();
			if (null != MenuUIManager.Instance)
			{
				MenuUIManager.Instance.ResetUI();
			}
		}
	}

	public void LoadFromCloud()
	{
		mSaving = false;
		isCloudLoading = true;
		((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution("savedata", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseUnmerged, SavedGameOpened);
	}

	public void SaveToCloud()
	{
		if (Authenticated && !isCloudLoading && !isCloudSaving)
		{
			saveDone = false;
			mSaving = true;
			isCloudSaving = true;
			((PlayGamesPlatform)Social.Active).SavedGame.OpenWithAutomaticConflictResolution("savedata", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseUnmerged, SavedGameOpened);
		}
	}

	public void SavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status != SavedGameRequestStatus.Success)
		{
			return;
		}
		if (mSaving)
		{
			if (mScreenImage == null)
			{
			}
			List<byte> list = new List<byte>();
			list.Add(86);
			list.AddRange(BitConverter.GetBytes(10u));
			list.AddRange(PlayerInfo.Instance.LastSavedData);
			TimeSpan newPlayedTime = TimeSpan.FromMilliseconds(PlayerInfo.Instance.PlayedTimeSpanTotalMilliseconds);
			SavedGameMetadataUpdate updateForMetadata = default(SavedGameMetadataUpdate.Builder).WithUpdatedPlayedTime(newPlayedTime).WithUpdatedDescription("Saved Game at " + DateTime.Now).Build();
			((PlayGamesPlatform)Social.Active).SavedGame.CommitUpdate(game, updateForMetadata, list.ToArray(), SavedGameWritten);
		}
		else
		{
			mAutoSaveName = game.Filename;
			((PlayGamesPlatform)Social.Active).SavedGame.ReadBinaryData(game, SavedGameLoaded);
		}
	}

	public void SavedGameLoaded(SavedGameRequestStatus status, byte[] data)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			try
			{
				ProcessCloudData(data);
			}
			catch (NotSupportedException)
			{
			}
		}
		isCloudLoading = false;
	}

	public void SavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		saveDone = true;
		if (status == SavedGameRequestStatus.Success)
		{
		}
		isCloudSaving = false;
	}

	public void SignOut()
	{
		((PlayGamesPlatform)Social.Active).SignOut();
	}

	public void ShowLeaderboardUI()
	{
		if (Authenticated)
		{
			Social.ShowLeaderboardUI();
		}
	}

	public void ShowAchievementsUI()
	{
		if (Authenticated)
		{
			Social.ShowAchievementsUI();
		}
	}

	public void PostToLeaderboard(string leaderboardID, long score)
	{
		if (Authenticated)
		{
			Social.ReportScore(score, leaderboardID, delegate
			{
			});
		}
	}
}
