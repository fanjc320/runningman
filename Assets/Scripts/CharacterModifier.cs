using System.Collections;
using UnityEngine;

public abstract class CharacterModifier : MonoBehaviour
{
	public enum StopSignal
	{
		DONT_STOP,
		STOP,
		STOP_NO_ENDING
	}

	public bool Paused;

	protected StopSignal stop;

	private IEnumerator current;

	public StopSignal Stop
	{
		get
		{
			return stop;
		}
		set
		{
			stop = value;
		}
	}

	public virtual bool ShouldPauseInJetpack => false;

	public IEnumerator Current
	{
		get
		{
			return current;
		}
		set
		{
			current = value;
		}
	}

	public virtual IEnumerator Begin()
	{
		yield break;
	}

	public virtual void Pause()
	{
		Paused = true;
	}

	public virtual void Resume()
	{
		Paused = false;
	}

	public virtual void Reset()
	{
	}
}
