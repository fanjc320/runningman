using System;
using System.Collections.Generic;

[Serializable]
public class FBALogEvent
{
	public string EventType;

	public List<FBALogEventParams> Params;

	public FBALogEvent()
	{
		Params = new List<FBALogEventParams>();
	}

	public static FBALogEvent CreateLogEvent(string Type)
	{
		FBALogEvent fBALogEvent = new FBALogEvent();
		fBALogEvent.EventType = Type;
		return fBALogEvent;
	}

	public void AddParams(string Type, string Value)
	{
		Params.Add(new FBALogEventParams
		{
			type = Type,
			value = Value
		});
	}
}
