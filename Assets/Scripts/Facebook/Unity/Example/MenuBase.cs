using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal abstract class MenuBase : ConsoleBase
	{
		private static ShareDialogMode shareDialogMode;

		protected abstract void GetGui();

		protected virtual bool ShowDialogModeSelector()
		{
			return false;
		}

		protected virtual bool ShowBackButton()
		{
			return true;
		}

		protected void HandleResult(IResult result)
		{
			if (result == null)
			{
				base.LastResponse = "Null Response\n";
				LogView.AddLog(base.LastResponse);
				return;
			}
			base.LastResponseTexture = null;
			if (!string.IsNullOrEmpty(result.Error))
			{
				base.Status = "Error - Check log for details";
				base.LastResponse = "Error Response:\n" + result.Error;
			}
			else if (result.Cancelled)
			{
				base.Status = "Cancelled - Check log for details";
				base.LastResponse = "Cancelled Response:\n" + result.RawResult;
			}
			else if (!string.IsNullOrEmpty(result.RawResult))
			{
				base.Status = "Success - Check log for details";
				base.LastResponse = "Success Response:\n" + result.RawResult;
			}
			else
			{
				base.LastResponse = "Empty Response\n";
			}
			LogView.AddLog(result.ToString());
		}

		protected void OnGUI()
		{
			if (IsHorizontalLayout())
			{
				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
			}
			GUILayout.Label(GetType().Name, base.LabelStyle);
			AddStatus();
			if (UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				Vector2 scrollPosition = base.ScrollPosition;
				float y = scrollPosition.y;
				Vector2 deltaPosition = UnityEngine.Input.GetTouch(0).deltaPosition;
				scrollPosition.y = y + deltaPosition.y;
				base.ScrollPosition = scrollPosition;
			}
			base.ScrollPosition = GUILayout.BeginScrollView(base.ScrollPosition, GUILayout.MinWidth(ConsoleBase.MainWindowFullWidth));
			GUILayout.BeginHorizontal();
			if (ShowBackButton())
			{
				AddBackButton();
			}
			AddLogButton();
			if (ShowBackButton())
			{
				GUILayout.Label(GUIContent.none, GUILayout.MinWidth(ConsoleBase.MarginFix));
			}
			GUILayout.EndHorizontal();
			if (ShowDialogModeSelector())
			{
				AddDialogModeButtons();
			}
			GUILayout.BeginVertical();
			GetGui();
			GUILayout.Space(10f);
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}

		private void AddStatus()
		{
			GUILayout.Space(5f);
			GUILayout.Box("Status: " + base.Status, base.TextStyle, GUILayout.MinWidth(ConsoleBase.MainWindowWidth));
		}

		private void AddBackButton()
		{
			GUI.enabled = ConsoleBase.MenuStack.Any();
			if (Button("Back"))
			{
				GoBack();
			}
			GUI.enabled = true;
		}

		private void AddLogButton()
		{
			if (Button("Log"))
			{
				SwitchMenu(typeof(LogView));
			}
		}

		private void AddDialogModeButtons()
		{
			GUILayout.BeginHorizontal();
			IEnumerator enumerator = Enum.GetValues(typeof(ShareDialogMode)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					AddDialogModeButton((ShareDialogMode)current);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			GUILayout.EndHorizontal();
		}

		private void AddDialogModeButton(ShareDialogMode mode)
		{
			bool enabled = GUI.enabled;
			GUI.enabled = (enabled && mode != shareDialogMode);
			if (Button(mode.ToString()))
			{
				shareDialogMode = mode;
				FB.Mobile.ShareDialogMode = mode;
			}
			GUI.enabled = enabled;
		}
	}
}
