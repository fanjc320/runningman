using System;
using UnityEngine;

namespace Lean
{
	[AddComponentMenu("Lean/Localization Loader")]
	public class LeanLocalizationLoader : MonoBehaviour
	{
		[LeanLanguageName]
		public string SourceLanguage;

		public TextAsset Source;

		private static readonly char[] newlineCharacters = new char[2]
		{
			'\r',
			'\n'
		};

		private static readonly string newlineString = "\\n";

		protected virtual void Start()
		{
			LoadFromSource();
		}

		[ContextMenu("Load From Source")]
		public void LoadFromSource()
		{
			if (!(Source != null) || string.IsNullOrEmpty(SourceLanguage))
			{
				return;
			}
			LeanLocalization instance = LeanLocalization.Instance;
			string[] array = Source.text.Split(newlineCharacters, StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = array;
			foreach (string text in array2)
			{
				int num = text.IndexOf('=');
				if (num != -1)
				{
					string phraseName = text.Substring(0, num).Trim();
					string text2 = text.Substring(num + 1).Trim();
					text2 = text2.Replace(newlineString, Environment.NewLine);
					int num2 = text2.IndexOf("//");
					if (num2 != -1)
					{
						text2 = text2.Substring(0, num2).Trim();
					}
					LeanTranslation leanTranslation = instance.AddTranslation(SourceLanguage, phraseName);
					leanTranslation.Text = text2;
				}
			}
			if (instance.CurrentLanguage == SourceLanguage)
			{
				LeanLocalization.UpdateTranslations();
			}
		}
	}
}
