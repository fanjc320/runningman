using UnityEngine;

public class BackBtnBehaviourAndroid : MonoBehaviour
{
	public enum ScreenChangeType
	{
		PushScreen,
		SwitchScreen,
		BackToPrevious,
		QueuePopup,
		ClosePopup,
		ExitGame,
		SaveMePopup
	}

	private GameObject target;

	private string functionName = string.Empty;

	public ScreenChangeType screenChangeType;

	public GameObject popupLayerAnchor;

	public string ScreenNameToOpen = string.Empty;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			Send();
		}
	}

	private void CheckForFunctionToExecute()
	{
		if (screenChangeType == ScreenChangeType.PushScreen)
		{
			functionName = "PushScreen";
		}
		else if (screenChangeType == ScreenChangeType.SwitchScreen)
		{
			functionName = "SwitchScreen";
		}
		else if (screenChangeType == ScreenChangeType.BackToPrevious)
		{
			functionName = "BackToPrevious";
		}
		else if (screenChangeType == ScreenChangeType.QueuePopup)
		{
			functionName = "QueuePopup";
		}
		else if (screenChangeType == ScreenChangeType.ClosePopup)
		{
			functionName = "ClosePopup";
		}
		else if (screenChangeType == ScreenChangeType.ExitGame)
		{
			functionName = "ExitGame";
		}
		else if (screenChangeType == ScreenChangeType.SaveMePopup)
		{
			functionName = "CloseSaveMe";
		}
	}

	protected void Send()
	{
	}

	private void alertButtonClickedEvent(string buttonString)
	{
		if (buttonString.Equals(Strings.Get(StringID.M01_YES)))
		{
			Application.Quit();
		}
	}
}
