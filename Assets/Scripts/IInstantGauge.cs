using System;

internal interface IInstantGauge
{
	float Minimum
	{
		get;
	}

	float Maximum
	{
		get;
	}

	float Value
	{
		get;
		set;
	}

	float Ratio
	{
		get;
		set;
	}

	event Action<float> OnValue;
}
