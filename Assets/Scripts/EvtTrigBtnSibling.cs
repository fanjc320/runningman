using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class EvtTrigBtnSibling : MonoBehaviour
{
	public RectTransform Target;

	private int origSiblingIndex;

	private void Awake()
	{
		origSiblingIndex = Target.GetSiblingIndex();
		EventTrigger eventTrigger = base.transform.gameObject.GetComponent<EventTrigger>() ?? base.transform.gameObject.AddComponent<EventTrigger>();
		EventTrigger.TriggerEvent triggerEvent = new EventTrigger.TriggerEvent();
		triggerEvent.AddListener(OnDown_BtnPointerDown);
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerDown;
		entry.callback = triggerEvent;
		EventTrigger.Entry item = entry;
		eventTrigger.triggers.Add(item);
		triggerEvent = new EventTrigger.TriggerEvent();
		triggerEvent.AddListener(OnDown_BtnPointerUp);
		entry = new EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerUp;
		entry.callback = triggerEvent;
		item = entry;
		eventTrigger.triggers.Add(item);
	}

	private void OnDown_BtnPointerDown(BaseEventData evt)
	{
		Target.SetAsLastSibling();
	}

	private void OnDown_BtnPointerUp(BaseEventData evt)
	{
		Target.SetSiblingIndex(origSiblingIndex);
	}
}
