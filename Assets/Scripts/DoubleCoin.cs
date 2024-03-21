using System.Collections;
using System.Linq;
using UnityEngine;

public class DoubleCoin : CharacterModifier
{
	public ActivePowerup Powerup;

	public AudioClipInfo powerDownSound;

	public override IEnumerator Begin()
	{
		GameStats.Instance.pickedUpPowerups++;
		Paused = false;
		stop = StopSignal.DONT_STOP;
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.doubleCoin);
		float baseDuration = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
			where s.ID == "4"
			select s).First().Pvalue;
		int paramLevel = PlayerInfo.Instance.CharParamLevels[PlayerInfo.Instance.SelectedCharID][3];
		float bonusValue = DataContainer.Instance.PlayerParamLevelTableRawByLevel[3].PPLevelRaws[paramLevel].Pvalue;
		float duration = baseDuration + bonusValue;
		GameStats.Instance.IsDoubleCoin = true;
		CoinPool.Instance.NotifyDoubleCoinAttr(isRedCoin: true);
		GameObjectPoolMT<PPItemDoubleCoin>.Instance.GetNParent(Character.Instance.transform, null);
		while (duration > 0f && stop == StopSignal.DONT_STOP)
		{
			duration -= Time.deltaTime;
			yield return null;
		}
		GameStats.Instance.IsDoubleCoin = false;
		CoinPool.Instance.NotifyDoubleCoinAttr(isRedCoin: false);
		if (duration <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public override void Reset()
	{
		GameStats.Instance.IsDoubleCoin = false;
		CoinPool.Instance.NotifyDoubleCoinAttr(isRedCoin: false);
	}
}
