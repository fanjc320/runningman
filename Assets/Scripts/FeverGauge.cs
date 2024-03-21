using System;

public class FeverGauge : IInstantGauge
{
	private float minimum;

	private float maximum;

	private float value;

	public float Minimum => minimum;

	public float Maximum => maximum;

	public float Ratio
	{
		get
		{
			return value / maximum;
		}
		set
		{
			Value = value * maximum;
		}
	}

	public float Value
	{
		get
		{
			return value;
		}
		set
		{
			this.value = value;
			if (this.OnValue != null)
			{
				this.OnValue(Ratio);
			}
		}
	}

	public event Action<float> OnValue;

	private FeverGauge()
	{
	}

	public FeverGauge(float minimum, float maximum)
	{
		this.minimum = minimum;
		this.maximum = maximum;
	}

	public void RepeatValue()
	{
		Value %= Maximum;
	}
}
