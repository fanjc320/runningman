using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lean
{
	[ExecuteInEditMode]
	[AddComponentMenu("Lean/Localization")]
	public class LeanLocalization : MonoBehaviour
	{
		private static LeanLocalization instance = null;

		public static Dictionary<string, LeanTranslation> Translations = new Dictionary<string, LeanTranslation>();

		public static Action OnLocalizationChanged;

		public List<string> Languages = new List<string>();

		public List<LeanPhrase> Phrases = new List<LeanPhrase>();

		[LeanLanguageName]
		public string CurrentLanguage;

		public static LeanLocalization Instance
		{
			get
			{
				if (null == instance)
				{
					_init();
				}
				return instance;
			}
		}

		private static void _init()
		{
			instance = (UnityEngine.Object.Instantiate(Resources.Load("LeanLocalization", typeof(GameObject))) as GameObject).GetComponent<LeanLocalization>();
			instance.gameObject.name = instance.GetType().Name;
			UnityEngine.Object.DontDestroyOnLoad(instance.gameObject);
		}

		private void Awake()
		{
		}

		public void SetLanguage(string newLanguage)
		{
			if (CurrentLanguage != newLanguage)
			{
				CurrentLanguage = newLanguage;
				UpdateTranslations();
			}
		}

		public static LeanTranslation GetTranslation(string phraseName)
		{
			LeanTranslation value = null;
			if (phraseName != null)
			{
				Translations.TryGetValue(phraseName, out value);
			}
			return value;
		}

		public static string GetTranslationText(string phraseName)
		{
			return GetTranslation(phraseName)?.Text;
		}

		public static UnityEngine.Object GetTranslationObject(string phraseName)
		{
			return GetTranslation(phraseName)?.Object;
		}

		public void AddLanguage(string language)
		{
			if (!Languages.Contains(language))
			{
				Languages.Add(language);
			}
		}

		public LeanPhrase AddPhrase(string phraseName)
		{
			LeanPhrase leanPhrase = Phrases.Find((LeanPhrase p) => p.Name == phraseName);
			if (leanPhrase == null)
			{
				leanPhrase = new LeanPhrase();
				leanPhrase.Name = phraseName;
				Phrases.Add(leanPhrase);
			}
			return leanPhrase;
		}

		public LeanTranslation AddTranslation(string language, string phraseName)
		{
			AddLanguage(language);
			return AddPhrase(phraseName).AddTranslation(language);
		}

		public static void UpdateTranslations()
		{
			Translations.Clear();
			if (instance != null)
			{
				for (int num = instance.Phrases.Count - 1; num >= 0; num--)
				{
					LeanPhrase leanPhrase = instance.Phrases[num];
					if (!Translations.ContainsKey(leanPhrase.Name))
					{
						LeanTranslation leanTranslation = leanPhrase.FindTranslation(instance.CurrentLanguage);
						if (leanTranslation != null)
						{
							Translations.Add(leanPhrase.Name, leanTranslation);
						}
					}
				}
			}
			if (OnLocalizationChanged != null)
			{
				OnLocalizationChanged();
			}
		}

		private static void MergeLocalizations(LeanLocalization oldLocalization, LeanLocalization newLocalization)
		{
			for (int num = oldLocalization.Phrases.Count - 1; num >= 0; num--)
			{
				LeanPhrase leanPhrase = oldLocalization.Phrases[num];
				LeanPhrase leanPhrase2 = newLocalization.AddPhrase(leanPhrase.Name);
				for (int num2 = leanPhrase.Translations.Count - 1; num2 >= 0; num2--)
				{
					LeanTranslation leanTranslation = leanPhrase.Translations[num2];
					LeanTranslation leanTranslation2 = leanPhrase2.AddTranslation(leanTranslation.Language);
					leanTranslation2.Text = leanTranslation.Text;
					leanTranslation2.Object = leanTranslation.Object;
				}
			}
		}
	}
}
