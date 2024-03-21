using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class LogView : ConsoleBase
	{
		private static string datePatt = "M/d/yyyy hh:mm:ss tt";

		private static IList<string> events = new List<string>();

		public static void AddLog(string log)
		{
			events.Insert(0, $"{DateTime.Now.ToString(datePatt)}\n{log}\n");
		}

		protected void OnGUI()
		{
			GUILayout.BeginVertical();
			if (Button("Back"))
			{
				GoBack();
			}
			if (UnityEngine.Input.touchCount > 0 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Moved)
			{
				Vector2 scrollPosition = base.ScrollPosition;
				float y = scrollPosition.y;
				Vector2 deltaPosition = UnityEngine.Input.GetTouch(0).deltaPosition;
				scrollPosition.y = y + deltaPosition.y;
				base.ScrollPosition = scrollPosition;
			}
			base.ScrollPosition = GUILayout.BeginScrollView(base.ScrollPosition, GUILayout.MinWidth(ConsoleBase.MainWindowFullWidth));
			GUILayout.TextArea(string.Join("\n", events.ToArray()), base.TextStyle, GUILayout.ExpandHeight(expand: true), GUILayout.MaxWidth(ConsoleBase.MainWindowWidth));
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
		}
	}
}
