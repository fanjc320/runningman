using System.Collections;
using System.Linq;
using UnityEngine;

public class Confuse : CharacterModifier
{
	public float ratio;

	public ActivePowerup Powerup;

	public AudioClipInfo powerDownSound;

	private float durationFromData;

	private PGOEffMental confusionLoopEff;

	private void Awake()
	{
		durationFromData = (from s in DataContainer.Instance.BasicStatusTableRaw.dataArray
			where s.ID == "5"
			select s).First().Pvalue;
	}

	public override IEnumerator Begin()
	{
		Paused = false;
		stop = StopSignal.DONT_STOP;
		float duration = durationFromData;
		float _timeLeft = duration;
		GameStats.Instance.IsConfuse = true;
		GameObjectPoolMT<PPItemConfusion>.Instance.GetNParent(Character.Instance.transform, null);
		confusionLoopEff = GameObjectPoolMT<PGOEffMental>.Instance.GetNParent(Character.Instance.transform, null);
		Material chMat = Character.Instance.characterModel.model.material;
		Color chAddColor = Color.clear;
		while (_timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			_timeLeft -= Time.deltaTime;
			ratio = _timeLeft / duration;
			float revRatio = 1f - ratio;
			if (0.8f <= revRatio)
			{
				float num = revRatio - 0.8f;
				chAddColor.a = Mathf.PingPong(num / 0.2f * 20f, 1f);
				chMat.SetColor("_AddColor", chAddColor);
			}
			yield return null;
		}
		chMat.SetColor("_AddColor", Color.clear);
		confusionLoopEff.Dispose();
		confusionLoopEff = null;
		GameStats.Instance.IsConfuse = false;
		if (_timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public override void Reset()
	{
		GameStats.Instance.IsConfuse = false;
		if (null != confusionLoopEff)
		{
			confusionLoopEff.Dispose();
			confusionLoopEff = null;
		}
	}
}
