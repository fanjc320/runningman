using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class EvtDelgManager
{
	private Dictionary<object, List<KeyValuePair<string, Delegate>>> events = new Dictionary<object, List<KeyValuePair<string, Delegate>>>();

	public Delegate this[object owner, string evtName]
	{
		set
		{
			AddEvent(owner, evtName, value);
		}
	}

	public void AddEvent(object owner, string eventName, Delegate handler)
	{
		if (!events.ContainsKey(owner))
		{
			events[owner] = new List<KeyValuePair<string, Delegate>>();
		}
		EventInfo @event = owner.GetType().GetEvent(eventName);
		@event.AddEventHandler(owner, handler);
		events[owner].Add(new KeyValuePair<string, Delegate>(eventName, handler));
	}

	public void RemoveAllEvent()
	{
		events.All(delegate(KeyValuePair<object, List<KeyValuePair<string, Delegate>>> keyPair)
		{
			keyPair.Value.All(delegate(KeyValuePair<string, Delegate> handlerPair)
			{
				EventInfo @event = keyPair.Key.GetType().GetEvent(handlerPair.Key);
				@event.RemoveEventHandler(keyPair.Key, handlerPair.Value);
				return true;
			});
			return true;
		});
	}
}
