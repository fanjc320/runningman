using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStats
{
	public delegate void CoinsChangedIngame();

	public FeverGauge FeverGauge = new FeverGauge(0f, 15000f);

	public bool IsConfuse;

	public bool IsDoubleCoin;

	public float duration;

	private const int SCORE_FOR_PICKUPS = 30;

	public Action OnCoinsChanged;

	private int _coins;

	private int _coinsInAir;

	private int _coinCollectedOnLeftTrack;

	private int _coinsCollectedOnCenterTrack;

	private int _coinsCollectedOnRightTrack;

	private int _coinsNotTouchingGround;

	private int _coinsCoinMagnet;

	private bool _scoreBoosterActivated;

	private int _score;

	private float _metersLastUsedForScore;

	private float _meterScore;

	public bool isWin;

	private const int TAG_SCORE = 300;

	private int _nametagScore;

	private List<ActivePowerup> _listOfActivePowerups = new List<ActivePowerup>();

	public int npcShow;

	public Action OnMetersChanged;

	public float _meters;

	public bool isLevelup;

	public bool isStartItemExpUp;

	public float metersRunLeftTrack;

	public float metersRunCenterTrack;

	public float metersRunRightTrack;

	public float metersFly;

	public float metersRunGround;

	public float metersRunTrain;

	public float metersRunStation;

	private int _grindedTrains;

	private int _jumps;

	private int _allCoinsInJetpack;

	private int _jumpsOverTrains;

	private int _rolls;

	private int _rollsLeftTrack;

	private int _rollsCenterTrack;

	private int _rollsRightTrack;

	public int trackChanges;

	private int _dodgeBarrier;

	private int _jumpBarrier;

	private int _jumpHighBarrier;

	private int _trainHit;

	private int _movingTrainHit;

	private int _guardHitScreen;

	private int _barrierHit;

	private int _jetpackPickups;

	private int _superSneakerPickups;

	private int _letterPickups;

	private int _coinMagnetsPickups;

	private int _mysteryBoxPickups;

	private int _doubleMultiplierPickups;

	private int _doubleCoinPickups;

	private int _pickedUpPowerups;

	private int _saveMeTokenPickup;

	public CoinsChangedIngame OnCoinsChangedIngame;

	public List<KeyValuePair<int, int>> coinsSummerized = new List<KeyValuePair<int, int>>();

	public static GameStats instance;

	public int coins
	{
		get
		{
			return _coins;
		}
		set
		{
			_coins = value;
			OnCoinsChanged?.Invoke();
			if (value == 0)
			{
			}
		}
	}

	public int coinsWithJetpack
	{
		get
		{
			return _allCoinsInJetpack;
		}
		set
		{
			_allCoinsInJetpack = value;
			if (value == 0)
			{
			}
		}
	}

	public int coinsInAir
	{
		get
		{
			return _coinsInAir;
		}
		set
		{
			_coinsInAir = value;
			if (value == 0)
			{
			}
		}
	}

	public int coinsCollectedOnLeftTrack
	{
		get
		{
			return _coinCollectedOnLeftTrack;
		}
		set
		{
			_coinCollectedOnLeftTrack = value;
			if (value == 0)
			{
			}
		}
	}

	public int coinsCollectedOnCenterTrack
	{
		get
		{
			return _coinsCollectedOnCenterTrack;
		}
		set
		{
			_coinsCollectedOnCenterTrack = value;
			if (value == 0)
			{
			}
		}
	}

	public int coinsCollectedOnRightTrack
	{
		get
		{
			return _coinsCollectedOnRightTrack;
		}
		set
		{
			_coinsCollectedOnRightTrack = value;
			if (value == 0)
			{
			}
		}
	}

	public int coinsNotTouchingGround
	{
		get
		{
			return _coinsNotTouchingGround;
		}
		set
		{
			_coinsNotTouchingGround = value;
		}
	}

	public int coinsCoinMagnet
	{
		get
		{
			return _coinsCoinMagnet;
		}
		set
		{
			_coinsCoinMagnet = value;
			if (value == 0)
			{
			}
		}
	}

	public bool scoreBoosterActivated
	{
		get
		{
			return _scoreBoosterActivated;
		}
		set
		{
			_scoreBoosterActivated = value;
			PlayerInfo.Instance.TriggerOnScoreMultiplierChanged();
		}
	}

	public int score => _score;

	public int nameTagScore
	{
		get
		{
			return _nametagScore;
		}
		set
		{
			_nametagScore = value;
		}
	}

	public float meters
	{
		get
		{
			return _meters;
		}
		set
		{
			_meters = value;
			OnMetersChanged?.Invoke();
		}
	}

	public int grindedTrains
	{
		get
		{
			return _grindedTrains;
		}
		set
		{
			_grindedTrains = value;
		}
	}

	public int jumps
	{
		get
		{
			return _jumps;
		}
		set
		{
			_jumps = value;
			if (value == 0)
			{
			}
		}
	}

	public int allCoinsInJetpack
	{
		get
		{
			return _allCoinsInJetpack;
		}
		set
		{
			_allCoinsInJetpack = value;
		}
	}

	public int jumpsOverTrains
	{
		get
		{
			return _jumpsOverTrains;
		}
		set
		{
			_jumpsOverTrains = value;
			if (value == 0)
			{
			}
		}
	}

	public int rolls
	{
		get
		{
			return _rolls;
		}
		set
		{
			_rolls = value;
			if (value == 0)
			{
			}
		}
	}

	public int rollsLeftTrack
	{
		get
		{
			return _rollsLeftTrack;
		}
		set
		{
			_rollsLeftTrack = value;
		}
	}

	public int rollsCenterTrack
	{
		get
		{
			return _rollsCenterTrack;
		}
		set
		{
			_rollsCenterTrack = value;
		}
	}

	public int rollsRightTrack
	{
		get
		{
			return _rollsRightTrack;
		}
		set
		{
			_rollsRightTrack = value;
		}
	}

	public int dodgeBarrier
	{
		get
		{
			return _dodgeBarrier;
		}
		set
		{
			_dodgeBarrier = value;
			if (value == 0)
			{
			}
		}
	}

	public int jumpBarrier
	{
		get
		{
			return _jumpBarrier;
		}
		set
		{
			_jumpBarrier = value;
			if (value == 0)
			{
			}
		}
	}

	public int jumpHighBarrier
	{
		get
		{
			return _jumpHighBarrier;
		}
		set
		{
			_jumpHighBarrier = value;
			if (value == 0)
			{
			}
		}
	}

	public int trainHit
	{
		get
		{
			return _trainHit;
		}
		set
		{
			_trainHit = value;
		}
	}

	public int movingTrainHit
	{
		get
		{
			return _movingTrainHit;
		}
		set
		{
			_movingTrainHit = value;
		}
	}

	public int guardHitScreen
	{
		get
		{
			return _guardHitScreen;
		}
		set
		{
			_guardHitScreen = value;
		}
	}

	public int barrierHit
	{
		get
		{
			return _barrierHit;
		}
		set
		{
			_barrierHit = value;
		}
	}

	public int jetpackPickups
	{
		get
		{
			return _jetpackPickups;
		}
		set
		{
			_jetpackPickups = value;
		}
	}

	public int superSneakerPickups
	{
		get
		{
			return _superSneakerPickups;
		}
		set
		{
			_superSneakerPickups = value;
		}
	}

	public int letterPickups
	{
		get
		{
			return _letterPickups;
		}
		set
		{
			_letterPickups = value;
		}
	}

	public int coinMagnetsPickups
	{
		get
		{
			return _coinMagnetsPickups;
		}
		set
		{
			_coinMagnetsPickups = value;
		}
	}

	public int mysteryBoxPickups
	{
		get
		{
			return _mysteryBoxPickups;
		}
		set
		{
			_mysteryBoxPickups = value;
		}
	}

	public int doubleMultiplierPickups
	{
		get
		{
			return _doubleMultiplierPickups;
		}
		set
		{
			_doubleMultiplierPickups = value;
		}
	}

	public int doubleCoinPickups
	{
		get
		{
			return _doubleCoinPickups;
		}
		set
		{
			_doubleCoinPickups = value;
		}
	}

	public int pickedUpPowerups
	{
		get
		{
			return _pickedUpPowerups;
		}
		set
		{
			_pickedUpPowerups = value;
			if (value == 0)
			{
			}
		}
	}

	public int saveMeTokenPickup
	{
		get
		{
			return _saveMeTokenPickup;
		}
		set
		{
			_saveMeTokenPickup = value;
			if (value == 0)
			{
			}
		}
	}

	public static GameStats Instance => instance ?? (instance = new GameStats());

	public static int CoinToScoreConversion(int coins)
	{
		if (instance.IsDoubleCoin)
		{
			return coins * 3;
		}
		return coins * 2;
	}

	public void CalculateScore()
	{
		if (_metersLastUsedForScore < meters)
		{
			_meterScore = meters - _metersLastUsedForScore;
			_metersLastUsedForScore = meters;
			int num = (int)(_meterScore * 1f);
			_score += num;
			if (_listOfActivePowerups.Count == 0 || _listOfActivePowerups.Count != 1 || _listOfActivePowerups[0].type == PowerupType.hoverboard)
			{
			}
			if (!scoreBoosterActivated)
			{
			}
		}
	}

	public void addTagScore()
	{
		_nametagScore += 300;
	}

	public void AddScoreForPickup(PowerupType type)
	{
		switch (type)
		{
		case PowerupType.supermysterybox:
			break;
		case PowerupType.jetpack:
		case PowerupType.supersneakers:
		case PowerupType.coinmagnet:
		case PowerupType.doubleMultiplier:
			_AddScoreForPickup(wasPowerup: true);
			break;
		case PowerupType.mysterybox:
		case PowerupType.letters:
			_AddScoreForPickup(wasPowerup: false);
			break;
		}
	}

	private void _AddScoreForPickup(bool wasPowerup)
	{
		int num = 30;
		_score += num;
		if (wasPowerup)
		{
		}
	}

	public void ResetScore()
	{
		_score = 0;
		_metersLastUsedForScore = 0f;
		_meterScore = 0f;
	}

	public void Reset()
	{
		FeverGauge.Value = 0f;
		MainUIManager.Instance.StopAllBuffIcon();
		if (PlayerInfo.Instance.TutorialCompleted)
		{
			int num = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][0];
			float pvalue = DataContainer.Instance.PlayerParamLevelTableRawByLevel[0].PPLevelRaws[num].Pvalue;
			FeverGauge.Ratio = pvalue;
			if (PlayerInfo.Instance.StartItems[1])
			{
				MainUIManager.Instance.StartBuffIcon(1, 0f);
			}
			if (PlayerInfo.Instance.StartItems[2])
			{
				MainUIManager.Instance.StartBuffIcon(2, 0f);
			}
			if (PlayerInfo.Instance.StartItems[3])
			{
				MainUIManager.Instance.StartBuffIcon(3, 0f);
			}
			if (PlayerInfo.Instance.StartItems[4])
			{
				MainUIManager.Instance.StartBuffIcon(4, 0f);
			}
			if (PlayerInfo.Instance.StartItems[5])
			{
				MainUIManager.Instance.StartBuffIcon(5, 0f);
			}
			if (PlayerInfo.Instance.StartItems[6])
			{
				MainUIManager.Instance.StartBuffIcon(6, 0f);
			}
			if (PlayerInfo.Instance.StartItems[7])
			{
				MainUIManager.Instance.StartBuffIcon(7, 0f);
			}
			if (PlayerInfo.Instance.StartItems[0])
			{
				PlayerInfo.Instance.StartItems[0] = false;
				float pvalue2 = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
					where s.ID == "1"
					select s).First().Pvalue;
				int num2 = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][1];
				float pvalue3 = DataContainer.Instance.PlayerParamLevelTableRawByLevel[1].PPLevelRaws[num2].Pvalue;
				MainUIManager.Instance.StartBuffIcon(0, pvalue2 + pvalue3);
				MainUIManager.Instance.StartStartItemIconDirector(StartItemType.StartFever);
				FeverGauge.Ratio += 1f;
			}
		}
		isLevelup = false;
		nameTagScore = 0;
		duration = 0f;
		npcShow = 1;
		ResetScore();
		coins = 0;
		coinsCoinMagnet = 0;
		coinsWithJetpack = 0;
		allCoinsInJetpack = 0;
		scoreBoosterActivated = false;
		meters = 0f;
		metersRunLeftTrack = 0f;
		metersRunCenterTrack = 0f;
		metersRunRightTrack = 0f;
		metersRunGround = 0f;
		metersRunTrain = 0f;
		metersRunStation = 0f;
		metersFly = 0f;
		grindedTrains = 0;
		jumps = 0;
		jumpsOverTrains = 0;
		rolls = 0;
		rollsLeftTrack = 0;
		rollsCenterTrack = 0;
		rollsRightTrack = 0;
		trackChanges = 0;
		dodgeBarrier = 0;
		jumpBarrier = 0;
		jumpHighBarrier = 0;
		trainHit = 0;
		guardHitScreen = 0;
		barrierHit = 0;
		jetpackPickups = 0;
		superSneakerPickups = 0;
		letterPickups = 0;
		coinMagnetsPickups = 0;
		mysteryBoxPickups = 0;
		pickedUpPowerups = 0;
		doubleMultiplierPickups = 0;
		doubleCoinPickups = 0;
		coinsSummerized = new List<KeyValuePair<int, int>>();
		saveMeTokenPickup = 0;
	}

	public ActivePowerup TriggerPowerup(PowerupType type)
	{
		ActivePowerup activePowerup = new ActivePowerup();
		activePowerup.type = type;
		activePowerup.timeActivated = Time.time;
		float timeLeft = 10f;
		switch (type)
		{
		case PowerupType.headstart500:
		case PowerupType.headstart2000:
		case PowerupType.jetpack:
		{
			float pvalue2 = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
				where s.ID == "1"
				select s).First().Pvalue;
			int num2 = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][1];
			timeLeft = pvalue2 + DataContainer.Instance.PlayerParamLevelTableRawByLevel[1].PPLevelRaws[num2].Pvalue;
			break;
		}
		case PowerupType.coinmagnet:
		{
			float pvalue3 = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
				where s.ID == "3"
				select s).First().Pvalue;
			int num3 = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][2];
			timeLeft = pvalue3 + DataContainer.Instance.PlayerParamLevelTableRawByLevel[2].PPLevelRaws[num3].Pvalue;
			break;
		}
		case PowerupType.doubleCoin:
		{
			float pvalue = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
				where s.ID == "4"
				select s).First().Pvalue;
			int num = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][3];
			timeLeft = pvalue + DataContainer.Instance.PlayerParamLevelTableRawByLevel[3].PPLevelRaws[num].Pvalue;
			break;
		}
		}
		activePowerup.timeLeft = timeLeft;
		for (int num4 = _listOfActivePowerups.Count - 1; num4 >= 0; num4--)
		{
			if (_listOfActivePowerups[num4].type == activePowerup.type)
			{
				_listOfActivePowerups.RemoveAt(num4);
			}
		}
		AddScoreForPickup(type);
		_listOfActivePowerups.Add(activePowerup);
		return activePowerup;
	}

	public List<ActivePowerup> GetActivePowerups()
	{
		return _listOfActivePowerups;
	}

	public void UpdatePowerupTimes(float deltaTime)
	{
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if ((Game.Instance.IsInJetpackMode && (_listOfActivePowerups[num].type == PowerupType.hoverboard || _listOfActivePowerups[num].type == PowerupType.supersneakers)) || (_listOfActivePowerups[num].type == PowerupType.supersneakers && Game.Instance.Modifiers.Hoverboard.isActive && HoverboardManager.Instance.Hoverboard == Hoverboards.BoardType.bouncer))
			{
				continue;
			}
			_listOfActivePowerups[num].timeLeft -= deltaTime;
			if (!(_listOfActivePowerups[num].timeLeft < 0f) || (Game.Instance.IsInJetpackMode && _listOfActivePowerups[num].type == PowerupType.jetpack))
			{
				continue;
			}
			if (_listOfActivePowerups[num].type == PowerupType.hoverboard)
			{
				float num2 = Hoverboard.Instance.WaitForParticlesDelay + PlayerInfo.Instance.GetHoverBoardCoolDown();
				if (_listOfActivePowerups[num].timeLeft > 0f - num2)
				{
					continue;
				}
			}
			_listOfActivePowerups.RemoveAt(num);
		}
	}

	public void ClearPowerups()
	{
		_listOfActivePowerups.Clear();
	}

	public void RemoveHoverBoardPowerup()
	{
		for (int num = _listOfActivePowerups.Count - 1; num >= 0; num--)
		{
			if (_listOfActivePowerups[num].type == PowerupType.hoverboard)
			{
				_listOfActivePowerups[num].timeLeft = 0f;
			}
		}
	}
}
