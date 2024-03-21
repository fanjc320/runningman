using System.Collections;

public class DoubleScoreMultiplier : CharacterModifier
{
	public float ratio;

	public ActivePowerup Powerup;

	public AudioClipInfo powerDownSound;

	public override IEnumerator Begin()
	{
		GameStats.Instance.pickedUpPowerups++;
		Paused = false;
		stop = StopSignal.DONT_STOP;
		Powerup = GameStats.Instance.TriggerPowerup(PowerupType.doubleMultiplier);
		float duration = Powerup.timeLeft;
		while (Powerup.timeLeft > 0f && stop == StopSignal.DONT_STOP)
		{
			ratio = Powerup.timeLeft / duration;
			yield return null;
		}
		if (Powerup.timeLeft <= 0f)
		{
			So.Instance.playSound(powerDownSound);
		}
	}

	public override void Reset()
	{
	}
}
