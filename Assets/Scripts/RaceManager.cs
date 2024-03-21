using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : RealTimeMultiplayerListener
{
	public enum RaceState
	{
		SettingUp,
		Playing,
		Start,
		Finished,
		SetupFailed,
		Ready,
		Waiting,
		Aborted
	}

	private const string RaceTrackName = "RaceTrack";

	private string[] PlayerNames = new string[2]
	{
		"Player1",
		"Player2"
	};

	private const int QuickGameOpponents = 1;

	private const int GameVariant = 1;

	private const int MinOpponents = 1;

	private const int MaxOpponents = 1;

	public PlayerInfo.MultiplayOppnent GameOpponent;

	public bool IsRoomSetupProgress;

	private static RaceManager sInstance;

	private const int PointsToFinish = 100;

	public bool isHost;

	private float _lantencyTotal;

	private int _lantency_count;

	private bool showingWaitingRoom;

	private RaceState mRaceState;

	private Dictionary<string, int> mRacerReady = new Dictionary<string, int>();

	private Dictionary<string, int> mRacerCharID = new Dictionary<string, int>();

	private Dictionary<string, int> mRacerScore = new Dictionary<string, int>();

	private HashSet<string> mGotFinalScore = new HashSet<string>();

	private string mMyParticipantId = string.Empty;

	private int mFinishRank;

	private float mRoomSetupProgress;

	private const float FakeProgressSpeed = 1f;

	private const float MaxFakeProgress = 30f;

	private float mRoomSetupStartTime;

	public Action OnCleanUp;

	private List<byte> mPosPacket = new List<byte>();

	private List<byte> mFinalPacket = new List<byte>();

	public float lantency
	{
		get
		{
			if (_lantency_count == 0)
			{
				return 0f;
			}
			return _lantencyTotal / (float)_lantency_count;
		}
		set
		{
			_lantencyTotal += value;
			_lantency_count++;
		}
	}

	public RaceState State => mRaceState;

	public static RaceManager Instance => sInstance;

	public int FinishRank => mFinishRank;

	public float RoomSetupProgress
	{
		get
		{
			float num = (Time.time - mRoomSetupStartTime) * 1f;
			if (num > 30f)
			{
				num = 30f;
			}
			float num2 = mRoomSetupProgress + num;
			return (!(num2 < 99f)) ? 99f : num2;
		}
	}

	private RaceManager()
	{
		mRoomSetupStartTime = Time.time;
	}

	public static void CreateQuickGame()
	{
		if (sInstance == null)
		{
			sInstance = new RaceManager();
		}
		sInstance.mRaceState = RaceState.SettingUp;
		sInstance.mRacerReady.Clear();
		sInstance.mRacerCharID.Clear();
		sInstance.mRacerScore.Clear();
		sInstance.mGotFinalScore.Clear();
		PlayGamesPlatform.Instance.RealTime.CreateQuickGame(1u, 1u, 1u, sInstance);
	}

	public static void CreateWithInvitationScreen()
	{
		if (sInstance == null)
		{
			sInstance = new RaceManager();
		}
		PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(1u, 1u, 1u, sInstance);
	}

	public static void AcceptFromInbox()
	{
		if (sInstance == null)
		{
			sInstance = new RaceManager();
		}
		PlayGamesPlatform.Instance.RealTime.AcceptFromInbox(sInstance);
	}

	public static void AcceptInvitation(string invitationId)
	{
		if (sInstance == null)
		{
			sInstance = new RaceManager();
		}
		PlayGamesPlatform.Instance.RealTime.AcceptInvitation(invitationId, sInstance);
	}

	private void SetupTrack()
	{
		SetupCharID(int.Parse(PlayerInfo.Instance.SelectedCharID));
	}

	private void TearDownTrack()
	{
	}

	public void OnRoomConnected(bool success)
	{
		if (success)
		{
			mRaceState = RaceState.Ready;
			mMyParticipantId = GetSelf().ParticipantId;
			SetupTrack();
		}
		else
		{
			mRaceState = RaceState.SetupFailed;
		}
	}

	public void OnLeftRoom()
	{
		mRaceState = RaceState.Aborted;
		sInstance = null;
	}

	public void OnPeersConnected(string[] peers)
	{
	}

	public void OnParticipantLeft(Participant participant)
	{
		PlayGamesPlatform.Instance.RealTime.LeaveRoom();
	}

	public void OnPeersDisconnected(string[] peers)
	{
		if (mRaceState == RaceState.Ready)
		{
			PlayGamesPlatform.Instance.RealTime.LeaveRoom();
		}
		foreach (string text in peers)
		{
			mGotFinalScore.Add(text);
			mRacerScore[text] = 0;
			RemoveCarFor(text);
			if (text != mMyParticipantId && GameOpponent != null)
			{
				GameOpponent.Dead = true;
			}
		}
		List<Participant> racers = GetRacers();
		if (mRaceState == RaceState.Playing && (racers == null || racers.Count < 2))
		{
			PlayGamesPlatform.Instance.RealTime.LeaveRoom();
		}
	}

	private void RemoveCarFor(string participantId)
	{
	}

	public void OnRoomSetupProgress(float percent)
	{
		mRoomSetupProgress = percent;
		IsRoomSetupProgress = true;
		if (!showingWaitingRoom)
		{
			mRaceState = RaceState.Waiting;
			showingWaitingRoom = true;
			PlayGamesPlatform.Instance.RealTime.ShowWaitingRoomUI();
		}
	}

	public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data)
	{
		if (data[0] == 84)
		{
			GameOpponent.StartTime = BitConverter.ToSingle(data, 1);
			return;
		}
		if (data[0] == 80)
		{
			data[0] = 81;
			PlayGamesPlatform.Instance.RealTime.SendMessage(reliable: false, senderId, data);
			return;
		}
		if (data[0] == 81)
		{
			long num = BitConverter.ToInt64(data, 1);
			lantency = (float)((DateTime.Now.Ticks - num) / 10000) * 0.001f;
			return;
		}
		int num2 = BitConverter.ToInt32(data, 1);
		if (data[0] == 82)
		{
			mRacerCharID[senderId] = num2;
			CheckReady();
		}
		else if (data[0] == 83)
		{
			mRacerReady[senderId] = num2;
			CheckReadyToStart();
		}
		else if (data[0] == 73)
		{
			mRacerScore[senderId] = num2;
			GameOpponent.PosX = BitConverter.ToInt32(data, 5);
			GameOpponent.PosY = BitConverter.ToInt32(data, 9);
			GameOpponent.Meter = num2;
		}
		else if (data[0] == 70 && !mGotFinalScore.Contains(senderId))
		{
			mRacerScore[senderId] = num2;
			mGotFinalScore.Add(senderId);
			UpdateMyRank();
			GameOpponent.Dead = true;
			if (mRaceState != RaceState.Playing)
			{
			}
		}
	}

	public void CleanUp()
	{
		if (OnCleanUp != null)
		{
			OnCleanUp();
		}
		if (sInstance != null)
		{
			TearDownTrack();
		}
	}

	public float GetRacerProgress(string participantId)
	{
		return (float)GetRacerPosition(participantId) / 100f;
	}

	public int GetRacerPosition(string participantId)
	{
		if (mRacerScore.ContainsKey(participantId))
		{
			return mRacerScore[participantId];
		}
		return 0;
	}

	private Participant GetSelf()
	{
		return PlayGamesPlatform.Instance.RealTime.GetSelf();
	}

	private List<Participant> GetRacers()
	{
		return PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
	}

	private Participant GetParticipant(string participantId)
	{
		return PlayGamesPlatform.Instance.RealTime.GetParticipant(participantId);
	}

	public void UpdateSelf(float distance, int posy, int posx)
	{
		int racerPosition = GetRacerPosition(mMyParticipantId);
		int num = (int)distance;
		if (racerPosition != num)
		{
			mRacerScore[mMyParticipantId] = num;
			BroadCastPosition(num, posx, posy);
		}
	}

	public void SetupCharID(int charID)
	{
		mRacerCharID[mMyParticipantId] = charID;
		CheckReady();
		BroadCastCharacterID(charID);
	}

	public void CheckReady()
	{
		if (mRacerCharID.Count == 2)
		{
			Participant participant = GetRacers().Find((Participant p) => p.ParticipantId != mMyParticipantId);
			GameOpponent = new PlayerInfo.MultiplayOppnent
			{
				CharID = mRacerCharID[participant.ParticipantId],
				NickName = participant.DisplayName,
				Meter = 0f
			};
			mRaceState = RaceState.Playing;
		}
	}

	public bool CheckReadyToStart()
	{
		if (mRacerReady.Count == 2)
		{
			mRaceState = RaceState.Start;
			return true;
		}
		return false;
	}

	private void BroadCastPosition(int pos, int posx, int posy)
	{
		mPosPacket.Clear();
		mPosPacket.Add(73);
		mPosPacket.AddRange(BitConverter.GetBytes(pos));
		mPosPacket.AddRange(BitConverter.GetBytes(posx));
		mPosPacket.AddRange(BitConverter.GetBytes(posy));
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable: false, mPosPacket.ToArray());
	}

	private void BroadCastCharacterID(int charID)
	{
		mPosPacket.Clear();
		mPosPacket.Add(82);
		mPosPacket.AddRange(BitConverter.GetBytes(charID));
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable: true, mPosPacket.ToArray());
	}

	private void FinishRace()
	{
		mGotFinalScore.Add(mMyParticipantId);
		mRaceState = RaceState.Finished;
		UpdateMyRank();
		mFinalPacket.Clear();
		mFinalPacket.Add(70);
		mFinalPacket.AddRange(BitConverter.GetBytes(mRacerScore[mMyParticipantId]));
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable: true, mFinalPacket.ToArray());
	}

	public void PlayerDead()
	{
		mGotFinalScore.Add(mMyParticipantId);
		mRaceState = RaceState.Finished;
		UpdateMyRank();
		mFinalPacket.Clear();
		mFinalPacket.Add(70);
		mFinalPacket.AddRange(BitConverter.GetBytes(mRacerScore[mMyParticipantId]));
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable: false, mFinalPacket.ToArray());
	}

	public void PingUpdate()
	{
		mPosPacket.Clear();
		mPosPacket.Add(80);
		mPosPacket.AddRange(BitConverter.GetBytes(DateTime.Now.Ticks));
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable: false, mPosPacket.ToArray());
	}

	public void PlayerReadyToStart()
	{
		mRacerReady[mMyParticipantId] = 1;
		if (CheckReadyToStart())
		{
			isHost = true;
		}
		mPosPacket.Clear();
		mPosPacket.Add(83);
		mPosPacket.AddRange(BitConverter.GetBytes(1));
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable: false, mPosPacket.ToArray());
	}

	public void PlayerStartTime(float time)
	{
		mPosPacket.Clear();
		mPosPacket.Add(84);
		mPosPacket.AddRange(BitConverter.GetBytes(time));
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable: true, mPosPacket.ToArray());
	}

	private void UpdateMyRank()
	{
		int count = GetRacers().Count;
		if (mGotFinalScore.Count < count)
		{
			mFinishRank = 0;
		}
		int num = mRacerScore[mMyParticipantId];
		int num2 = 1;
		foreach (string key in mRacerScore.Keys)
		{
			if (mRacerScore[key] > num)
			{
				num2++;
			}
		}
		mFinishRank = num2;
	}
}
