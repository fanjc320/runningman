using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class EvtTrigToggleOffTarget : MonoBehaviour
{
	public Image Target;

	private Toggle toggle;

	private void Awake()
	{
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(delegate(bool isOn)
		{
			if (isOn)
			{
				Target.enabled = false;
			}
			else
			{
				Target.enabled = true;
			}
		});
		toggle.isOn = !toggle.isOn;
		toggle.isOn = !toggle.isOn;
	}
}
